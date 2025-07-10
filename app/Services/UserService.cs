using VibeCheck.Data;
using VibeCheck.Models;
using Microsoft.EntityFrameworkCore;

namespace VibeCheck.Services {
	public class UserService {
		private readonly AppDbContext _db;

		public UserService(AppDbContext db) {
			_db = db;
		}

		public async Task<ServiceResult> CreateUserAsync(CreateAccountModel account) {
			try {

				if (await _db.TUsers.AnyAsync(u => u.strUsername == account.strUsername))
					return ServiceResult.Fail("Username already exists.");

				if (await _db.TUsers.AnyAsync(u => u.strEmail == account.strEmail))
					return ServiceResult.Fail("Email already exists.");

				if (await _db.TUsers.AnyAsync(u => u.strPhone == account.strPhoneNumber))
					return ServiceResult.Fail("Email already exists.");

				var user = new TUsers {
					strUsername = account.strUsername,
					strFirstName = account.strFirstName,
					strLastName = account.strLastName,
					dtmDateOfBirth = account.dtmDateOfBirth!.Value,
					strPhone = account.strPhoneNumber,
					strEmail = account.strEmail,
					dtmCreatedDate = DateTime.UtcNow,
					intGenderID = 1,
					intZipCodeID = 1,
					decLatitude = 0,
					decLongitude = 0,
					blnIsActive = true,
					blnIsOnline = false,
					blnInQueue = false
				};

				_db.TUsers.Add(user);
				await _db.SaveChangesAsync();

				return ServiceResult.Ok();
			}
			catch (DbUpdateException dbEx) {
				return ServiceResult.Fail($"A database error occurred: {dbEx.InnerException?.Message ?? dbEx.Message}");
			}
			catch (Exception ex) {
				return ServiceResult.Fail("An unexpected error occurred.");
			}
		}
	}
}
