namespace SparkCheck.Services {
	public class AuthService {
		public Task<bool> CheckPhoneNumberExistsAsync(string phoneNumber) {
			// Simulate checking if the phone number exists in the database
			return Task.FromResult(phoneNumber == "5551234567");
		}
	}
}
