//using Microsoft.EntityFrameworkCore;
//using SparkCheck.Data;
//using SparkCheck.Models;
//using System.Security.Cryptography;

//namespace SparkCheck.Services {
//	public class UserService {
//		private readonly AppDbContext _context;
//		private readonly ValidationService _validation;
//		public UserService(AppDbContext context, ValidationService validation) {
//			_context = context;
//			_validation = validation;
//		}

//		public async Task<ServiceResult> CreateUserAsync(TUsers objUser) {
//			// Centralized validation
//			var validationResult = await _validation.ValidateUserForCreate(objUser);
//			if (!validationResult.blnSuccess)
//				return validationResult;

//			Console.WriteLine("Creating new user with the following info:");
//			Console.WriteLine($"Username: {objUser.strUsername}");
//			Console.WriteLine($"First Name: {objUser.strFirstName}");
//			Console.WriteLine($"Last Name: {objUser.strLastName}");
//			Console.WriteLine($"Phone: {objUser.strPhone}");
//			Console.WriteLine($"Email: {objUser.strEmail}");
//			if (objUser.dtmDateOfBirth != null) {
//				Console.WriteLine($"DOB: {objUser.dtmDateOfBirth.Value.ToShortDateString()}");
//			}
//			else {
//				Console.WriteLine("DOB: Not provided");
//			}
//			Console.WriteLine($"GenderID: {objUser.intGenderID}");
//			Console.WriteLine($"Created Date: {objUser.dtmCreatedDate}");
//			Console.WriteLine($"Is Active: {objUser.blnIsActive}");

//			try {
//				_context.TUsers.Add(objUser);
//				await _context.SaveChangesAsync();
//				return ServiceResult.Ok();
//			}
//			catch (Exception ex) {
//				Console.WriteLine($"[EXCEPTION] CreateUserAsync: {ex.ToString()}");
//				return ServiceResult.Fail("Failed to create account. Please try again later.");
//			}
//		}
//		public async Task<ServiceResult> AttemptLoginAsync(string strPhone) {
//			var validationResult = await _validation.ValidatePhoneLogin(strPhone);
//			if (!validationResult.blnSuccess)
//				return validationResult;

//			// Normalize phone number (remove non-digits)
//			strPhone = new string(strPhone.Where(char.IsDigit).ToArray());

//			var objUser = await _context.TUsers
//				.FirstOrDefaultAsync(u => u.strPhone == strPhone);

//			if (objUser == null)
//				return ServiceResult.Fail("No user found with that phone number.");

//			// Deactivate all active login attempts for this user/phone
//			var existingAttempts = await _context.TLoginAttempts
//				.Where(x => x.strPhone == strPhone &&
//							x.intUserID == objUser.intUserID &&
//							x.blnIsActive)
//				.ToListAsync();

//			foreach (var attempt in existingAttempts)
//				attempt.blnIsActive = false;

//			await _context.SaveChangesAsync(); // Save before adding new row

//			// Generate 6-digit code
//			string strVerificationCode = RandomNumberGenerator
//				.GetInt32(100000, 1000000)
//				.ToString();

//			var objLoginAttempt = new TLoginAttempts {
//				strPhone = strPhone,
//				strVerificationCode = strVerificationCode,
//				dtmLoginDate = DateTime.UtcNow,
//				strIPAddress = null,
//				strUserAgent = null,
//				intUserID = objUser.intUserID,
//				blnIsActive = true,
//				intAttempts = 0
//			};

//			Console.WriteLine($"[LOGIN ATTEMPT] Created for UserID {objUser.intUserID} with code {strVerificationCode}");

//			_context.TLoginAttempts.Add(objLoginAttempt);
//			await _context.SaveChangesAsync();

//			return new ServiceResult {
//				blnSuccess = true,
//				User = objUser
//			};
//		}

//		public async Task<ServiceResult> VerifyPhoneLoginAsync(string strPhone, string strVerificationCode, int intUserID) {
//			Console.WriteLine($"Verifying login for UserID: {intUserID}, Phone: {strPhone}, Code: {strVerificationCode}");
//			return await _validation.ValidateVerificationCodeAttempt(strPhone, strVerificationCode, intUserID);
//		}

//		public async Task<TUsers?> GetUserById(int intUserID) {
//			Console.WriteLine($"Fetching user info for UserID: {intUserID}");

//			var user = await _context.TUsers
//				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

//			if (user == null) {
//				Console.WriteLine("User not found.");
//			}
//			else {
//				Console.WriteLine($"User found: {user.strUsername} ({user.strPhone})");
//			}

//			return user;
//		}

//		public async Task UpdateUserStatusAsync(int intUserID, bool blnIsOnline) {
//			Console.WriteLine($"Updating online status for UserID: {intUserID} → {(blnIsOnline ? "Online" : "Offline")}");

//			var user = await _context.TUsers
//				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

//			if (user != null) {
//				user.blnIsOnline = blnIsOnline;
//				await _context.SaveChangesAsync();
//				Console.WriteLine("User status updated.");
//			}
//			else {
//				Console.WriteLine("User not found. Cannot update status.");
//			}
//		}

//	}
//}

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
			Console.WriteLine("[CREATE USER] Begin user creation process...");

			var validationResult = await _validation.ValidateUserForCreate(objUser);
			if (!validationResult.blnSuccess) {
				Console.WriteLine($"[VALIDATION FAIL] {validationResult.strErrorMessage}");
				return validationResult;
			}

			Console.WriteLine(" - Username: " + objUser.strUsername);
			Console.WriteLine(" - First Name: " + objUser.strFirstName);
			Console.WriteLine(" - Last Name: " + objUser.strLastName);
			Console.WriteLine(" - Phone: " + objUser.strPhone);
			Console.WriteLine(" - Email: " + objUser.strEmail);
			Console.WriteLine(" - DOB: " + objUser.dtmDateOfBirth?.ToShortDateString());
			Console.WriteLine(" - GenderID: " + objUser.intGenderID);
			Console.WriteLine(" - Created Date: " + objUser.dtmCreatedDate);
			Console.WriteLine(" - Is Active: " + objUser.blnIsActive);

			try {
				_context.TUsers.Add(objUser);
				await _context.SaveChangesAsync();
				Console.WriteLine("[SUCCESS] User saved to database.");
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				Console.WriteLine($"[EXCEPTION] CreateUserAsync failed: {ex}");
				return ServiceResult.Fail("Failed to create account. Please try again later.");
			}
		}

		public async Task<ServiceResult> AttemptLoginAsync(string strPhone) {
			Console.WriteLine("[ATTEMPT LOGIN] Attempting login with phone: " + strPhone);

			var validationResult = await _validation.ValidatePhoneLogin(strPhone);
			if (!validationResult.blnSuccess) {
				Console.WriteLine($"[VALIDATION FAIL] {validationResult.strErrorMessage}");
				return validationResult;
			}

			strPhone = new string(strPhone.Where(char.IsDigit).ToArray());
			Console.WriteLine($"[NORMALIZED] Phone number: {strPhone}");

			var objUser = await _context.TUsers
				.FirstOrDefaultAsync(u => u.strPhone == strPhone);

			if (objUser == null) {
				Console.WriteLine("[FAIL] No user found with that phone.");
				return ServiceResult.Fail("No user found with that phone number.");
			}

			Console.WriteLine($"[FOUND USER] UserID: {objUser.intUserID}, Username: {objUser.strUsername}");

			// Deactivate all active login attempts
			var existingAttempts = await _context.TLoginAttempts
				.Where(x => x.strPhone == strPhone &&
							x.intUserID == objUser.intUserID &&
							x.blnIsActive)
				.ToListAsync();

			if (existingAttempts.Any()) {
				Console.WriteLine($"[CLEANUP] Deactivating {existingAttempts.Count} old login attempts...");
				foreach (var attempt in existingAttempts) {
					attempt.blnIsActive = false;
					Console.WriteLine($" - Deactivated LoginAttemptID: {attempt.intLoginAttemptID}");
				}
				await _context.SaveChangesAsync();
			}
			else {
				Console.WriteLine("[CLEANUP] No active login attempts to deactivate.");
			}

			string strVerificationCode = RandomNumberGenerator
				.GetInt32(100000, 1000000)
				.ToString();

			Console.WriteLine($"[NEW ATTEMPT] Creating new login attempt with code: {strVerificationCode}");

			var objLoginAttempt = new TLoginAttempts {
				strPhone = strPhone,
				strVerificationCode = strVerificationCode,
				dtmLoginDate = DateTime.UtcNow,
				strIPAddress = null,
				strUserAgent = null,
				intUserID = objUser.intUserID,
				blnIsActive = true,
				intAttempts = 0
			};

			_context.TLoginAttempts.Add(objLoginAttempt);
			await _context.SaveChangesAsync();

			Console.WriteLine("[LOGIN ATTEMPT] Successfully saved login attempt.");
			return new ServiceResult {
				blnSuccess = true,
				User = objUser
			};
		}

		public async Task<ServiceResult> VerifyPhoneLoginAsync(string strPhone, string strVerificationCode, int intUserID) {
			Console.WriteLine("[VERIFY CODE] Starting verification...");
			Console.WriteLine($" - Phone: {strPhone}");
			Console.WriteLine($" - Code: {strVerificationCode}");
			Console.WriteLine($" - UserID: {intUserID}");

			return await _validation.ValidateVerificationCodeAttempt(strPhone, strVerificationCode, intUserID);
		}

		public async Task<TUsers?> GetUserById(int intUserID) {
			Console.WriteLine("[GET USER] Fetching user by ID: " + intUserID);

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user == null) {
				Console.WriteLine("[GET USER] No user found.");
			}
			else {
				Console.WriteLine($"[GET USER] Found user: {user.strUsername} ({user.strPhone})");
			}

			return user;
		}

		public async Task UpdateUserStatusAsync(int intUserID, bool blnIsOnline) {
			Console.WriteLine($"[UPDATE STATUS] Setting UserID {intUserID} → {(blnIsOnline ? "Online" : "Offline")}");

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user != null) {
				user.blnIsOnline = blnIsOnline;
				await _context.SaveChangesAsync();
				Console.WriteLine("[STATUS UPDATE] User status updated.");
			}
			else {
				Console.WriteLine("[STATUS UPDATE] User not found. Cannot update.");
			}
		}
	}
}

