namespace SparkCheck.Services {
	// Scoped service to persist user session data across pages
	public class UserSessionService {
		public int? intUserID { get; set; }
		public string? strPhone { get; set; }
		public string? strUsername { get; set; }
		public string? strFirstName { get; set; }
		public string? strLastName { get; set; }
		public DateTime? dtmDateOfBirth { get; set; }
		public int? intGenderID { get; set; }
		public int? intZipCodeID { get; set; }
		public bool blnIsActive { get; set; }
		public bool blnIsOnline { get; set; }
		public bool blnInQueue { get; set; }
		public DateTime? dtmQueuedDate { get; set; }

		// Used only after successful 2FA in PhoneAuth.razor
		public bool blnIsAuthenticated { get; set; } = false;

		public void Clear() {
			intUserID = null;
			strPhone = null;
			strUsername = null;
			strFirstName = null;
			strLastName = null;
			dtmDateOfBirth = null;
			intGenderID = null;
			intZipCodeID = null;
			blnIsActive = false;
			blnIsOnline = false;
			blnInQueue = false;
			dtmQueuedDate = null;
			blnIsAuthenticated = false;
		}
	}
}
