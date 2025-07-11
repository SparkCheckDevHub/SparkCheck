using Microsoft.EntityFrameworkCore;
using SparkCheck.Models;
using SparkCheck.Data;
namespace SparkCheck.Services {
	public class UserService {
		private readonly AppDbContext _context;
		private readonly ILogger<UserService> _logger;

		public UserService(AppDbContext context, ILogger<UserService> logger) {
			_context = context;
			_logger = logger;
		}

		// Get user by phone number
		public async Task<TUsers?> GetUserByPhoneAsync(string phone)
		{
			return await _context.TUsers.FirstOrDefaultAsync(u => u.strPhone == phone);
		}
		// Set phone number for a user
		public async Task SetPhoneNumberAsync(int userId, string phone) {
			var user = await _context.TUsers.FindAsync(userId);
			if (user != null) {
				user.strPhone = phone;
				await _context.SaveChangesAsync();
			}
		}

		public async Task<string?> GetPhoneNumberAsync(int userId) {
			var user = await _context.TUsers.FindAsync(userId);
			return user?.strPhone;
		}


		// Create a new user after validation
		public async Task<ServiceResult> CreateUserAsync(TUsers user) {
			try {
				_context.TUsers.Add(user);
				await _context.SaveChangesAsync();
				return ServiceResult.Ok();
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error while creating user.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}


		// Verifies a login attempt by phone, code, and user ID
		public async Task<ServiceResult> VerifyPhoneLoginAsync(string? strPhone, string verificationCode, int? intUserID)
		{
			if (string.IsNullOrWhiteSpace(strPhone) || string.IsNullOrWhiteSpace(verificationCode) || intUserID == null)
				return ServiceResult.Fail("Missing verification information.");

			try
			{
				var attempt = await _context.TLoginAttempts
					.FirstOrDefaultAsync(a => a.strPhone == strPhone &&
											 a.strVerificationCode == verificationCode &&
											 a.intUserID == intUserID &&
											 a.blnIsActive);
				if (attempt == null)
					return ServiceResult.Fail("Invalid verification code or phone number.");

				return ServiceResult.Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during phone verification.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}


		public async Task<ServiceResult> UpdateUserStatusAsync(int userId, bool isOnline)
		{
			try
			{
				var user = await _context.TUsers.FindAsync(userId);
				if (user == null)
				{
					return ServiceResult.Fail("User not found.");
				}

				user.blnIsOnline = isOnline;
				await _context.SaveChangesAsync();
				return ServiceResult.Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while updating user status.");
				return ServiceResult.Fail($"Error: {ex.Message}");
			}
		}


        // For login attempt: check TUsers for phone
        public async Task<LoginResult> AttemptLoginAsync(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return LoginResult.Fail("Phone number is required.");

                var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());

                var user = await _context.TUsers.FirstOrDefaultAsync(u => u.strPhone == normalizedPhone);

                if (user == null)
                {
                    // Optional: log failure
                    await _context.TLoginAttempts.AddAsync(new TLoginAttempts
                    {
                        strPhone = normalizedPhone,
                        blnIsActive = false,
                        dtmLoginDate = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();

                    return LoginResult.Fail("No user found with that phone number.");
                }

                // Optional: log success
                await _context.TLoginAttempts.AddAsync(new TLoginAttempts
                {
                    intUserID = user.intUserID,
                    strPhone = normalizedPhone,
                    blnIsActive = true,
                    dtmLoginDate = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                return LoginResult.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return LoginResult.Fail("An error occurred during login.");
            }
        }

        // For verification process (legacy, can be removed if not needed)
        public async Task<ServiceResult> VerifyPhoneAsync(int userId, string verificationCode) {
			try {
				var user = await _context.TUsers.FindAsync(userId);
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
