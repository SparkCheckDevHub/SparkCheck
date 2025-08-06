using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SparkCheck.Data;
using SparkCheck.Models;
using System.Net.Http;
using System.Security.Cryptography;

namespace SparkCheck.Services {
	public class UserService {
		private readonly AppDbContext _context;
		private readonly ValidationService _validation;

		public UserService(AppDbContext context, ValidationService validation) {
			_context = context;
			_validation = validation;
		}
		// ===================================================================================
		// Create users/Reactivate users.
		// ===================================================================================
		public async Task<ServiceResult> CreateUserAsync(TUsers objUser, TUserPreferences objPreferences) {
			Console.WriteLine("[CREATE USER] Begin user creation process...");

			var validationResult = await _validation.ValidateUserForCreate(objUser);
			if (!validationResult.blnSuccess) {
				Console.WriteLine($"[VALIDATION FAIL] {validationResult.strErrorMessage}");
				return validationResult;
			}

			try {
				// Check for matching inactive user
				var existingInactiveUser = await _context.TUsers
					.FirstOrDefaultAsync(u =>
						u.strPhone == objUser.strPhone &&
						!u.blnIsActive);

				if (existingInactiveUser != null) {
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
			catch (Exception ex) {
				Console.WriteLine($"[EXCEPTION] CreateUserAsync failed: {ex}");
				return ServiceResult.Fail("Failed to create account. Please try again later.");
			}
		}

		// ===================================================================================
		// Attempt logging a user in.
		// ===================================================================================
		public async Task<ServiceResult> AttemptLoginAsync(string strPhone) {
			Console.WriteLine("[ATTEMPT LOGIN] Attempting login with phone: " + strPhone);

			var validationResult = await _validation.ValidatePhoneLogin(strPhone);
			if (!validationResult.blnSuccess) {
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

			if (objUser == null) {
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

			// Send verification code to the user if configured to use PBX
			if (Environment.GetEnvironmentVariable("SC_USE_ASTERISK") == "True") {
				var httpClient = new HttpClient();
				await httpClient.GetAsync($"http://sparkcheck-verification:9977/sendVerificationCode?strPhone={strPhone}&strCode={strVerificationCode}");
			}

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

		// ===================================================================================
		// Verifying Phone Login
		// ===================================================================================
		public async Task<ServiceResult> VerifyPhoneLoginAsync(string strPhone, string strVerificationCode, int intUserID) {
			Console.WriteLine("[VERIFY CODE] Starting verification...");
			Console.WriteLine($" - Phone: {strPhone}");
			Console.WriteLine($" - Code: {strVerificationCode}");
			Console.WriteLine($" - UserID: {intUserID}");

			return await _validation.ValidateVerificationCodeAttempt(strPhone, strVerificationCode, intUserID);
		}

		// ===================================================================================
		// Getting the user by ID
		// ===================================================================================
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
		// ===================================================================================
		// Updating User Status Async
		// ===================================================================================
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
		// ===================================================================================
		// Deactivating a user account (Delete account logic)
		// ===================================================================================
		public async Task<bool> DeactivateUserAccountAsync(int intUserID, string strPhone) {
			try {
				var user = await _context.TUsers
					.FirstOrDefaultAsync(u => u.intUserID == intUserID && u.strPhone == strPhone);

				if (user == null)
					return false;

				user.blnIsActive = false;
				user.blnIsOnline = false;

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex) {
				Console.WriteLine($"[ERROR] Failed to deactivate account: {ex.Message}");
				return false;
			}
		}

		// ===================================================================================
		// Populating the TGenders Dropdown
		// ===================================================================================
		public async Task<List<TGenders>> GetAllGendersAsync() {
			return await _context.TGenders.OrderBy(g => g.intGenderID).ToListAsync();
		}

		// ===================================================================================
		// Get online user count
		// ===================================================================================
		public async Task<int> GetOnlineUserCountAsync() {
			return await _context.TUsers.CountAsync(u => u.blnIsOnline && u.blnIsActive);
		}

		// ===================================================================================
		// Get all interest categories (for Interests.razor drawers)
		// ===================================================================================
		public async Task<List<InterestSelectionDto>> GetInterestCategoriesAsync() {
			return await _context.TInterestCategories
				.OrderBy(c => c.strInterestCategory)
				.Select(c => new InterestSelectionDto {
					Id = c.intInterestCategoryID,
					Name = c.strInterestCategory
				})
				.ToListAsync();
		}

		// ===================================================================================
		// Get all interests sorted by categories (for Interests.razor drawers)
		// ===================================================================================
		public async Task<List<InterestSelectionDto>> GetInterestCategoriesWithInterestsAsync() {
			var categories = await _context.TInterestCategories
				.OrderBy(c => c.strInterestCategory)
				.Select(c => new InterestSelectionDto {
					Id = c.intInterestCategoryID,
					Name = c.strInterestCategory,
					Interests = _context.TInterests
						.Where(i => i.intInterestCategoryID == c.intInterestCategoryID)
						.OrderBy(i => i.strInterest)
						.Select(i => new InterestDto {
							Id = i.intInterestID,
							Name = i.strInterest
						}).ToList()
				})
				.ToListAsync();

			return categories;
		}

		// ===================================================================================
		// Creates a row in TUserInterests (for Interests.razor drawers)
		// ===================================================================================
		public async Task SaveUserInterestsAsync(int userId, List<int> selectedInterestIds) {
			Console.WriteLine($"[UserService] Saving interests for user ID: {userId}");

			// 1. Remove existing interests for this user
			var existingInterests = await _context.TUserInterests
				.Where(ui => ui.intUserID == userId)
				.ToListAsync();

			if (existingInterests.Any()) {
				Console.WriteLine($"[UserService] Removing {existingInterests.Count} existing interests for user {userId}");
				_context.TUserInterests.RemoveRange(existingInterests);
				await _context.SaveChangesAsync(); // flush deletions before insert
			}

			// 2. Add selected interests
			var newInterests = selectedInterestIds.Select(interestId => new TUserInterests {
				intUserID = userId,
				intInterestID = interestId
			}).ToList();

			Console.WriteLine($"[UserService] Adding {newInterests.Count} new interests for user {userId}");
			_context.TUserInterests.AddRange(newInterests);
			await _context.SaveChangesAsync();

			// 3. Update or insert onboarding progress
			var progress = await _context.TOnboardingProgress
				.FirstOrDefaultAsync(p => p.intUserID == userId);

			if (progress == null) {
				Console.WriteLine($"[UserService] No onboarding progress found. Inserting new row for user {userId}.");

				progress = new TOnboardingProgress {
					intUserID = userId,
					blnPreferencesComplete = true
					// Let blnProfileComplete default to false
				};

				_context.TOnboardingProgress.Add(progress);
			}
			else {
				Console.WriteLine($"[UserService] Updating onboarding progress: setting blnPreferencesComplete = true for user {userId}");
				progress.blnPreferencesComplete = true;

				// DO NOT touch blnProfileComplete
				_context.TOnboardingProgress.Update(progress);
			}

			await _context.SaveChangesAsync();

			Console.WriteLine($"[UserService] Successfully saved onboarding progress for user {userId}");
		}
		

		// ===================================================================================
		// Saves user profile onboarding progress
		// ===================================================================================
		public async Task SaveUserProfileOnboarding(int userId) {
			Console.WriteLine($"[UserService] Saving onboarding for user ID: {userId}");

			// Update or insert onboarding progress
			var progress = await _context.TOnboardingProgress
				.FirstOrDefaultAsync(p => p.intUserID == userId);

			if (progress == null) {
				Console.WriteLine($"[UserService] No onboarding progress found. Inserting new row for user {userId}.");

				progress = new TOnboardingProgress {
					intUserID = userId,
					blnProfileComplete = true
				};

				_context.TOnboardingProgress.Add(progress);
			}
			else {
				Console.WriteLine($"[UserService] Updating onboarding progress: setting blnProfileComplete = true for user {userId}");
				progress.blnProfileComplete = true;
				_context.TOnboardingProgress.Update(progress);
			}

			await _context.SaveChangesAsync();

			Console.WriteLine($"[UserService] Successfully saved onboarding progress for user {userId}");
		}

		// ===================================================================================
		// Grabs the onboarding progress of a user from TOnboardingProgress
		// ===================================================================================
		public async Task<(bool blnProfileComplete, bool blnPreferencesComplete)> GetUserOnboardingProgressAsync(int userId)
		{
			Console.WriteLine($"[GetUserOnboardingProgressAsync] Fetching onboarding progress for UserID {userId}");

			if (userId <= 0)
			{
				Console.WriteLine("[GetUserOnboardingProgressAsync] Invalid UserID supplied.");
				return (false, false);
			}

			var progress = await _context.TOnboardingProgress
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.intUserID == userId);

			if (progress == null)
			{
				Console.WriteLine("[GetUserOnboardingProgressAsync] No onboarding record found.");
				return (false, false);
			}

			Console.WriteLine($"[GetUserOnboardingProgressAsync] Found: ProfileComplete={progress.blnProfileComplete}, PreferencesComplete={progress.blnPreferencesComplete}");
			return (progress.blnProfileComplete, progress.blnPreferencesComplete);
		}

	}
}

