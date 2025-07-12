using Microsoft.EntityFrameworkCore;
using SparkCheck.Data;
using SparkCheck.Models;
using System.Security.Cryptography;

namespace SparkCheck.Services {
	public class UserService {
		private readonly AppDbContext _context;
		private readonly ValidationService _validation;
		public UserService(AppDbContext context, ValidationService validation) {
			_context = context;
			_validation = validation;
		}

		public async Task<ServiceResult> CreateUserAsync(TUsers objUser) {
			// Centralized validation
			var validationResult = await _validation.ValidateUserForCreate(objUser);
			if (!validationResult.blnSuccess)
				return validationResult;

			Console.WriteLine("Creating new user with the following info:");
			Console.WriteLine($"Username: {objUser.strUsername}");
			Console.WriteLine($"First Name: {objUser.strFirstName}");
			Console.WriteLine($"Last Name: {objUser.strLastName}");
			Console.WriteLine($"Phone: {objUser.strPhone}");
			Console.WriteLine($"Email: {objUser.strEmail}");
			if (objUser.dtmDateOfBirth != null) {
				Console.WriteLine($"DOB: {objUser.dtmDateOfBirth.Value.ToShortDateString()}");
			}
			else {
				Console.WriteLine("DOB: Not provided");
			}
			Console.WriteLine($"GenderID: {objUser.intGenderID}");
			Console.WriteLine($"Created Date: {objUser.dtmCreatedDate}");
			Console.WriteLine($"Is Active: {objUser.blnIsActive}");

			try {
				_context.TUsers.Add(objUser);
				await _context.SaveChangesAsync();
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				Console.WriteLine($"[EXCEPTION] CreateUserAsync: {ex.Message}");
				return ServiceResult.Fail("Failed to create account. Please try again later.");
			}
		}
		public async Task<ServiceResult> AttemptLoginAsync(string strPhone) {
			var validationResult = await _validation.ValidatePhoneLogin(strPhone);
			if (!validationResult.blnSuccess)
				return validationResult;

			strPhone = new string(strPhone.Where(char.IsDigit).ToArray());
			var objUser = await _context.TUsers.FirstAsync(u => u.strPhone == strPhone);

			string strVerificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

			var objLoginAttempt = new TLoginAttempts {
				strPhone = strPhone,
				strVerificationCode = strVerificationCode,
				dtmLoginDate = DateTime.UtcNow,
				strIPAddress = null,
				strUserAgent = null,
				intUserID = objUser.intUserID,
				blnIsActive = true
			};

			Console.WriteLine($"Login attempt created for UserID {objUser.intUserID} - Code: {strVerificationCode}");

			_context.TLoginAttempts.Add(objLoginAttempt);
			await _context.SaveChangesAsync();

			return new ServiceResult {
				blnSuccess = true,
				User = objUser
			};
		}

		public async Task<ServiceResult> VerifyPhoneLoginAsync(string strPhone, string strVerificationCode, int intUserID) {
			Console.WriteLine($"Verifying login for UserID: {intUserID}, Phone: {strPhone}, Code: {strVerificationCode}");
			return await _validation.ValidateVerificationCodeAttempt(strPhone, strVerificationCode, intUserID);
		}

		public async Task<TUsers?> GetUserById(int intUserID) {
			Console.WriteLine($"Fetching user info for UserID: {intUserID}");

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user == null) {
				Console.WriteLine("User not found.");
			}
			else {
				Console.WriteLine($"User found: {user.strUsername} ({user.strPhone})");
			}

			return user;
		}

		public async Task UpdateUserStatusAsync(int intUserID, bool blnIsOnline) {
			Console.WriteLine($"Updating online status for UserID: {intUserID} → {(blnIsOnline ? "Online" : "Offline")}");

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user != null) {
				user.blnIsOnline = blnIsOnline;
				await _context.SaveChangesAsync();
				Console.WriteLine("User status updated.");
			}
			else {
				Console.WriteLine("User not found. Cannot update status.");
			}
		}

	}
}
