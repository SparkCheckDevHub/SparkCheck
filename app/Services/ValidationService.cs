using Microsoft.EntityFrameworkCore;
using SparkCheck.Data;
using SparkCheck.Models;

namespace SparkCheck.Services {
	public class ValidationService {
		private readonly AppDbContext _context;

		public ValidationService(AppDbContext context) {
			_context = context;
		}

		// ===================================================================================
		// Validate user creation fields (username, email, phone, DOB)
		// ===================================================================================
		public async Task<ServiceResult> ValidateUserForCreate(TUsers user) {
			if (string.IsNullOrWhiteSpace(user.strUsername))
				return ServiceResult.Fail("Username is required.");

			if (await _context.TUsers.AnyAsync(u => u.strUsername == user.strUsername))
				return ServiceResult.Fail("Username already exists.");

			if (!string.IsNullOrWhiteSpace(user.strEmail) &&
				await _context.TUsers.AnyAsync(u => u.strEmail == user.strEmail))
				return ServiceResult.Fail("Email already exists.");

			if (!string.IsNullOrWhiteSpace(user.strPhone) &&
				await _context.TUsers.AnyAsync(u => u.strPhone == user.strPhone))
				return ServiceResult.Fail("Phone number already exists.");

			if (user.dtmDateOfBirth == default)
				return ServiceResult.Fail("Date of birth is required.");

			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Validate login by phone input
		// ===================================================================================
		public async Task<ServiceResult> ValidatePhoneLogin(string strPhone) {
			if (string.IsNullOrWhiteSpace(strPhone))
				return ServiceResult.Fail("Phone number is required.");

			strPhone = NormalizePhone(strPhone);

			if (!await _context.TUsers.AnyAsync(u => u.strPhone == strPhone))
				return ServiceResult.Fail("No user found with that phone number.");

			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Validate verification code with retry limit
		// ===================================================================================
		public async Task<ServiceResult> ValidateVerificationCodeAttempt(string strPhone, string strVerificationCode, int intUserID) {
			strPhone = NormalizePhone(strPhone);

			var objLoginAttempt = await _context.TLoginAttempts
				.Where(a => a.intUserID == intUserID &&
							a.strPhone == strPhone &&
							a.blnIsActive)
				.OrderByDescending(a => a.dtmLoginDate)
				.FirstOrDefaultAsync();

			if (objLoginAttempt == null) {
				Console.WriteLine("No active login attempt found.");
				return ServiceResult.Fail("Your login session has expired. Please try again.");
			}

			if (objLoginAttempt.strVerificationCode != strVerificationCode) {
				objLoginAttempt.intAttempts++;

				if (objLoginAttempt.intAttempts >= 3) {
					objLoginAttempt.blnIsActive = false;
					await _context.SaveChangesAsync();

					Console.WriteLine($"3 failed attempts. Login attempt deactivated for UserID: {intUserID}");
					return ServiceResult.Fail("Too many failed attempts. Please return to login and try again.");
				}

				await _context.SaveChangesAsync();
				Console.WriteLine($"Incorrect code. Attempt {objLoginAttempt.intAttempts} of 3.");
				return ServiceResult.Fail("Invalid verification code. Please try again.");
			}

			// Successful login
			objLoginAttempt.blnIsActive = false;
			await _context.SaveChangesAsync();

			Console.WriteLine($"Verification successful for UserID: {intUserID}. Login attempt deactivated.");
			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Internal helper to normalize phone numbers (digits only)
		// ===================================================================================
		private string NormalizePhone(string rawPhone) =>
			new string(rawPhone.Where(char.IsDigit).ToArray());
	}
}
