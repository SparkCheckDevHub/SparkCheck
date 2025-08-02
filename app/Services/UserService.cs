using SparkCheck.Data;
using SparkCheck.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SparkCheck.Services
{
	public class UserService
	{
		private readonly AppDbContext _context;
		private readonly ValidationService _validation;

		public UserService(AppDbContext context, ValidationService validation)
		{
			_context = context;
			_validation = validation;
		}
		// ===================================================================================
		// Create users/Reactivate users.
		// ===================================================================================
		public async Task<ServiceResult> CreateUserAsync(TUsers objUser, TUserPreferences objPreferences)
		{
			Console.WriteLine("[CREATE USER] Begin user creation process...");

			var validationResult = await _validation.ValidateUserForCreate(objUser);
			if (!validationResult.blnSuccess)
			{
				Console.WriteLine($"[VALIDATION FAIL] {validationResult.strErrorMessage}");
				return validationResult;
			}

			try
			{
				// Check for matching inactive user
				var existingInactiveUser = await _context.TUsers
					.FirstOrDefaultAsync(u =>
						u.strPhone == objUser.strPhone &&
						!u.blnIsActive);

				if (existingInactiveUser != null)
				{
					Console.WriteLine("[REACTIVATION] Inactive user found. Updating existing record.");

					// Reactivate and update fields
					existingInactiveUser.blnIsActive = true;
					existingInactiveUser.blnIsOnline = false;
					existingInactiveUser.strUsername = objUser.strUsername;
					existingInactiveUser.strFirstName = objUser.strFirstName;
					existingInactiveUser.strLastName = objUser.strLastName;
					existingInactiveUser.dtmDateOfBirth = objUser.dtmDateOfBirth;
					existingInactiveUser.strEmail = objUser.strEmail;
					existingInactiveUser.intGenderID = objUser.intGenderID;
					existingInactiveUser.dtmCreatedDate = DateTime.Now;


					await _context.SaveChangesAsync();
					Console.WriteLine("[SUCCESS] Inactive account reactivated.");
					return ServiceResult.Ok();
				}

				// No inactive user, add new
				_context.TUsers.Add(objUser);
				await _context.SaveChangesAsync();

				objPreferences.intUserID = objUser.intUserID; // Ensure preferences link to user
				_context.TUserPreferences.Add(objPreferences);
				await _context.SaveChangesAsync();

				Console.WriteLine("[SUCCESS] New user saved to database.");
				return ServiceResult.Ok();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[EXCEPTION] CreateUserAsync failed: {ex}");
				return ServiceResult.Fail("Failed to create account. Please try again later.");
			}
		}

		// ===================================================================================
		// Attempt logging a user in.
		// ===================================================================================
		public async Task<ServiceResult> AttemptLoginAsync(string strPhone)
		{
			Console.WriteLine("[ATTEMPT LOGIN] Attempting login with phone: " + strPhone);

			var validationResult = await _validation.ValidatePhoneLogin(strPhone);
			if (!validationResult.blnSuccess)
			{
				Console.WriteLine($"[VALIDATION FAIL] {validationResult.strErrorMessage}");
				return validationResult;
			}

			strPhone = new string(strPhone.Where(char.IsDigit).ToArray());
			Console.WriteLine($"[NORMALIZED] Phone number: {strPhone}");

			//var objUser = await _context.TUsers
			//	.FirstOrDefaultAsync(u => u.strPhone == strPhone);

			//if (objUser == null) {
			//	Console.WriteLine("[FAIL] No user found with that phone.");
			//	return ServiceResult.Fail("No user found with that phone number.");
			//}

			var objUser = await _context.TUsers
			.FirstOrDefaultAsync(u => u.strPhone == strPhone && u.blnIsActive);

			if (objUser == null)
			{
				Console.WriteLine("[FAIL] No active user found with that phone.");
				return ServiceResult.Fail("No active account found for that phone number.");
			}

			Console.WriteLine($"[FOUND USER] UserID: {objUser.intUserID}, Username: {objUser.strUsername}");

			// Deactivate all active login attempts
			var existingAttempts = await _context.TLoginAttempts
				.Where(x => x.strPhone == strPhone &&
							x.intUserID == objUser.intUserID &&
							x.blnIsActive)
				.ToListAsync();

			if (existingAttempts.Any())
			{
				Console.WriteLine($"[CLEANUP] Deactivating {existingAttempts.Count} old login attempts...");
				foreach (var attempt in existingAttempts)
				{
					attempt.blnIsActive = false;
					Console.WriteLine($" - Deactivated LoginAttemptID: {attempt.intLoginAttemptID}");
				}
				await _context.SaveChangesAsync();
			}
			else
			{
				Console.WriteLine("[CLEANUP] No active login attempts to deactivate.");
			}

			string strVerificationCode = RandomNumberGenerator
				.GetInt32(100000, 1000000)
				.ToString();

			Console.WriteLine($"[NEW ATTEMPT] Creating new login attempt with code: {strVerificationCode}");

			var objLoginAttempt = new TLoginAttempts
			{
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
			return new ServiceResult
			{
				blnSuccess = true,
				User = objUser
			};
		}

		// ===================================================================================
		// Verifying Phone Login
		// ===================================================================================
		public async Task<ServiceResult> VerifyPhoneLoginAsync(string strPhone, string strVerificationCode, int intUserID)
		{
			Console.WriteLine("[VERIFY CODE] Starting verification...");
			Console.WriteLine($" - Phone: {strPhone}");
			Console.WriteLine($" - Code: {strVerificationCode}");
			Console.WriteLine($" - UserID: {intUserID}");

			return await _validation.ValidateVerificationCodeAttempt(strPhone, strVerificationCode, intUserID);
		}

		// ===================================================================================
		// Getting the user by ID
		// ===================================================================================
		public async Task<TUsers?> GetUserById(int intUserID)
		{
			Console.WriteLine("[GET USER] Fetching user by ID: " + intUserID);

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user == null)
			{
				Console.WriteLine("[GET USER] No user found.");
			}
			else
			{
				Console.WriteLine($"[GET USER] Found user: {user.strUsername} ({user.strPhone})");
			}

			return user;
		}
		// ===================================================================================
		// Updating User Status Async
		// ===================================================================================
		public async Task UpdateUserStatusAsync(int intUserID, bool blnIsOnline)
		{
			Console.WriteLine($"[UPDATE STATUS] Setting UserID {intUserID} → {(blnIsOnline ? "Online" : "Offline")}");

			var user = await _context.TUsers
				.FirstOrDefaultAsync(u => u.intUserID == intUserID);

			if (user != null)
			{
				user.blnIsOnline = blnIsOnline;
				await _context.SaveChangesAsync();
				Console.WriteLine("[STATUS UPDATE] User status updated.");
			}
			else
			{
				Console.WriteLine("[STATUS UPDATE] User not found. Cannot update.");
			}
		}
		// ===================================================================================
		// Deactivating a user account (Delete account logic)
		// ===================================================================================
		public async Task<bool> DeactivateUserAccountAsync(int intUserID, string strPhone)
		{
			try
			{
				var user = await _context.TUsers
					.FirstOrDefaultAsync(u => u.intUserID == intUserID && u.strPhone == strPhone);

				if (user == null)
					return false;

				user.blnIsActive = false;
				user.blnIsOnline = false;

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ERROR] Failed to deactivate account: {ex.Message}");
				return false;
			}
		}
		// ===================================================================================
		// Populating the TGenders Dropdown
		// ===================================================================================
		public async Task<List<TGenders>> GetAllGendersAsync()
		{
			return await _context.TGenders.OrderBy(g => g.intGenderID).ToListAsync();
		}
		// ===================================================================================
		// Get online user count
		// ===================================================================================
		public async Task<int> GetOnlineUserCountAsync()
		{
			return await _context.TUsers.CountAsync(u => u.blnIsOnline && u.blnIsActive);
		}
	}
}

