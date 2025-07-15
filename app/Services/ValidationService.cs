//using Microsoft.EntityFrameworkCore;
//using SparkCheck.Data;
//using SparkCheck.Models;

//namespace SparkCheck.Services {
//	public class ValidationService {
//		private readonly AppDbContext _context;

//		public ValidationService(AppDbContext context) {
//			_context = context;
//		}

//		// ===================================================================================
//		// Validate user creation fields (username, email, phone, DOB)
//		// ===================================================================================
//		public async Task<ServiceResult> ValidateUserForCreate(TUsers user) {
//			if (string.IsNullOrWhiteSpace(user.strUsername))
//				return ServiceResult.Fail("Username is required.");

//			if (await _context.TUsers.AnyAsync(u => u.strUsername == user.strUsername))
//				return ServiceResult.Fail("Username already exists.");

//			if (!string.IsNullOrWhiteSpace(user.strEmail) &&
//				await _context.TUsers.AnyAsync(u => u.strEmail == user.strEmail))
//				return ServiceResult.Fail("Email already exists.");

//			if (!string.IsNullOrWhiteSpace(user.strPhone) &&
//				await _context.TUsers.AnyAsync(u => u.strPhone == user.strPhone))
//				return ServiceResult.Fail("Phone number already exists.");

//			if (user.dtmDateOfBirth == default)
//				return ServiceResult.Fail("Date of birth is required.");

//			return ServiceResult.Ok();
//		}

//		// ===================================================================================
//		// Validate login by phone input
//		// ===================================================================================
//		public async Task<ServiceResult> ValidatePhoneLogin(string strPhone) {
//			if (string.IsNullOrWhiteSpace(strPhone))
//				return ServiceResult.Fail("Phone number is required.");

//			strPhone = NormalizePhone(strPhone);

//			if (!await _context.TUsers.AnyAsync(u => u.strPhone == strPhone))
//				return ServiceResult.Fail("No user found with that phone number.");

//			return ServiceResult.Ok();
//		}

//		// ===================================================================================
//		// Validate verification code with retry limit
//		// ===================================================================================
//		public async Task<ServiceResult> ValidateVerificationCodeAttempt(string strPhone, string strVerificationCode, int intUserID) {
//			strPhone = NormalizePhone(strPhone);

//			var objLoginAttempt = await _context.TLoginAttempts
//				.Where(a => a.intUserID == intUserID &&
//							a.strPhone == strPhone &&
//							a.blnIsActive)
//				.OrderByDescending(a => a.dtmLoginDate)
//				.FirstOrDefaultAsync();

//			if (objLoginAttempt == null) {
//				Console.WriteLine("No active login attempt found.");
//				return ServiceResult.Fail("Your login session has expired. Please try again.");
//			}

//			if (objLoginAttempt.strVerificationCode != strVerificationCode) {
//				objLoginAttempt.intAttempts++;

//				if (objLoginAttempt.intAttempts >= 3) {
//					objLoginAttempt.blnIsActive = false;
//					await _context.SaveChangesAsync();

//					Console.WriteLine($"3 failed attempts. Login attempt deactivated for UserID: {intUserID}");
//					return ServiceResult.Fail("Too many failed attempts. Please return to login and try again.");
//				}

//				await _context.SaveChangesAsync();
//				Console.WriteLine($"Incorrect code. Attempt {objLoginAttempt.intAttempts} of 3.");
//				return ServiceResult.Fail("Invalid verification code. Please try again.");
//			}

//			// Successful login
//			objLoginAttempt.blnIsActive = false;
//			await _context.SaveChangesAsync();

//			Console.WriteLine($"Verification successful for UserID: {intUserID}. Login attempt deactivated.");
//			return ServiceResult.Ok();
//		}

//		// ===================================================================================
//		// Internal helper to normalize phone numbers (digits only)
//		// ===================================================================================
//		private string NormalizePhone(string rawPhone) =>
//			new string(rawPhone.Where(char.IsDigit).ToArray());
//	}
//}

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
			Console.WriteLine("[VALIDATE] Starting user creation validation...");

			if (string.IsNullOrWhiteSpace(user.strUsername)) {
				Console.WriteLine("[FAIL] Username is required.");
				return ServiceResult.Fail("Username is required.");
			}

			if (await _context.TUsers.AnyAsync(u => u.strUsername == user.strUsername)) {
				Console.WriteLine("[FAIL] Username already exists: " + user.strUsername);
				return ServiceResult.Fail("Username already exists.");
			}

			if (!string.IsNullOrWhiteSpace(user.strEmail) &&
				await _context.TUsers.AnyAsync(u => u.strEmail == user.strEmail)) {
				Console.WriteLine("[FAIL] Email already exists: " + user.strEmail);
				return ServiceResult.Fail("Email already exists.");
			}

			if (!string.IsNullOrWhiteSpace(user.strPhone) &&
				await _context.TUsers.AnyAsync(u => u.strPhone == user.strPhone)) {
				Console.WriteLine("[FAIL] Phone already exists: " + user.strPhone);
				return ServiceResult.Fail("Phone number already exists.");
			}

			if (user.dtmDateOfBirth == default) {
				Console.WriteLine("[FAIL] DOB is required.");
				return ServiceResult.Fail("Date of birth is required.");
			}

			Console.WriteLine("[PASS] All user creation fields valid.");
			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Validate login by phone input
		// ===================================================================================
		public async Task<ServiceResult> ValidatePhoneLogin(string strPhone) {
			Console.WriteLine($"[VALIDATE] Starting phone login validation for: {strPhone}");

			if (string.IsNullOrWhiteSpace(strPhone)) {
				Console.WriteLine("[FAIL] Phone number is required.");
				return ServiceResult.Fail("Phone number is required.");
			}

			strPhone = NormalizePhone(strPhone);
			Console.WriteLine($"[NORMALIZED] Phone number: {strPhone}");

			if (!await _context.TUsers.AnyAsync(u => u.strPhone == strPhone)) {
				Console.WriteLine("[FAIL] No user found with that phone.");
				return ServiceResult.Fail("No user found with that phone number.");
			}

			Console.WriteLine("[PASS] Phone login validation passed.");
			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Validate verification code with retry limit
		// ===================================================================================
		public async Task<ServiceResult> ValidateVerificationCodeAttempt(string strPhone, string strVerificationCode, int intUserID) {
			Console.WriteLine($"[VERIFY] Checking code {strVerificationCode} for UserID {intUserID} and phone {strPhone}");

			strPhone = NormalizePhone(strPhone);

			var objLoginAttempt = await _context.TLoginAttempts
				.Where(a => a.intUserID == intUserID &&
							a.strPhone == strPhone &&
							a.blnIsActive)
				.OrderByDescending(a => a.dtmLoginDate)
				.FirstOrDefaultAsync();

			if (objLoginAttempt == null) {
				Console.WriteLine("[FAIL] No active login attempt found.");
				return ServiceResult.Fail("Your login session has expired. Please try again.");
			}

			Console.WriteLine($"[LOGIN ATTEMPT FOUND] AttemptID: {objLoginAttempt.intLoginAttemptID} | Code: {objLoginAttempt.strVerificationCode} | Attempts: {objLoginAttempt.intAttempts}");

			if (objLoginAttempt.strVerificationCode != strVerificationCode) {
				objLoginAttempt.intAttempts++;
				Console.WriteLine($"[INCORRECT] Verification code mismatch. Attempt #{objLoginAttempt.intAttempts}");

				if (objLoginAttempt.intAttempts >= 3) {
					objLoginAttempt.blnIsActive = false;
					await _context.SaveChangesAsync();

					Console.WriteLine($"[LOCKOUT] 3 failed attempts reached. Login attempt deactivated for UserID: {intUserID}");
					return ServiceResult.Fail("Too many failed attempts. Please return to login and try again.");
				}

				await _context.SaveChangesAsync();
				return ServiceResult.Fail("Invalid verification code. Please try again.");
			}

			// ✅ Code matches
			Console.WriteLine("[CORRECT] Code matched successfully.");
			objLoginAttempt.blnIsActive = false;
			await _context.SaveChangesAsync();

			Console.WriteLine($"[SUCCESS] Verification complete. Login attempt deactivated.");
			return ServiceResult.Ok();
		}

		// ===================================================================================
		// Internal helper to normalize phone numbers (digits only)
		// ===================================================================================
		private string NormalizePhone(string rawPhone) {
			string normalized = new string(rawPhone.Where(char.IsDigit).ToArray());
			Console.WriteLine($"[HELPER] Normalized phone: {rawPhone} → {normalized}");
			return normalized;
		}
	}
}

