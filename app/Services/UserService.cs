using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SparkCheck.Models;

namespace SparkCheck.Services {
	public class UserService {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<UserService> _logger;

		public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger) {
			_userManager = userManager;
			_logger = logger;
		}

		// Set phone number to be used for login or other purposes
		public async Task SetPhoneNumberAsync(string userId, string phone) {
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null) {
				user.PhoneNumber = phone;
				await _userManager.UpdateAsync(user);
			}
		}

		public async Task<string> GetPhoneNumberAsync(string userId) {
			var user = await _userManager.FindByIdAsync(userId);
			return user?.PhoneNumber;
		}

		// Create a new user after validation
		public async Task<ServiceResult> CreateUserAsync(CreateAccountModel account) {
			try {
				var user = new ApplicationUser {
					UserName = account.strEmail,  // UserName is required for Identity
					Email = account.strEmail,
					strFirstName = account.strFirstName,
					strLastName = account.strLastName,
					dtmDateOfBirth = account.dtmDateOfBirth,
					PhoneNumber = account.strPhone
				};

				var result = await _userManager.CreateAsync(user, "Password123!"); // You can generate a password here or take from input
				if (result.Succeeded) {
					return ServiceResult.Ok();
				}

				foreach (var error in result.Errors) {
					_logger.LogError(error.Description);
				}
				return ServiceResult.Fail("Error creating user.");
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error while creating user.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}

		public async Task<ServiceResult> UpdateUserStatusAsync(string userId, bool isOnline) {
			try {
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null) {
					return ServiceResult.Fail("User not found.");
				}

				user.blnIsOnline = isOnline;
				await _userManager.UpdateAsync(user);
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error while updating user status.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}

		// For login attempt: you should leverage SignInManager or external services.
		public async Task<ServiceResult> AttemptLoginAsync(string phone) {
			try {
				var user = await _userManager.FindByPhoneNumberAsync(phone);  // Use FindByPhoneNumberAsync for Identity users
				if (user == null) {
					return ServiceResult.Fail("Invalid phone number.");
				}

				// Here you can handle the verification code logic (e.g., send an OTP)
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error during login attempt.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}

		// For verification process
		public async Task<ServiceResult> VerifyPhoneAsync(string userId, string verificationCode) {
			try {
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null) {
					return ServiceResult.Fail("User not found.");
				}

				// Add your logic to validate the verification code, maybe via OTP.
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error during phone verification.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}
	}
}
