using SparkCheck.Data;
using SparkCheck.Models;

namespace SparkCheck.Services
{
	public class UserSessionService
	{
		private int? _intUserID;
		public int? intUserID
		{
			get => _intUserID;
			set
			{
				Console.WriteLine($"[SESSION] intUserID set to {value}");
				_intUserID = value;
			}
		}

		private string? _strPhone;
		public string? strPhone
		{
			get => _strPhone;
			set
			{
				Console.WriteLine($"[SESSION] strPhone set to {value}");
				_strPhone = value;
			}
		}

		private string? _strUsername;
		public string? strUsername
		{
			get => _strUsername;
			set
			{
				Console.WriteLine($"[SESSION] strUsername set to {value}");
				_strUsername = value;
			}
		}

		private string? _strFirstName;
		public string? strFirstName
		{
			get => _strFirstName;
			set
			{
				Console.WriteLine($"[SESSION] strFirstName set to {value}");
				_strFirstName = value;
			}
		}

		private string? _strLastName;
		public string? strLastName
		{
			get => _strLastName;
			set
			{
				Console.WriteLine($"[SESSION] strLastName set to {value}");
				_strLastName = value;
			}
		}

		private DateTime? _dtmDateOfBirth;
		public DateTime? dtmDateOfBirth
		{
			get => _dtmDateOfBirth;
			set
			{
				Console.WriteLine($"[SESSION] dtmDateOfBirth set to {value?.ToShortDateString()}");
				_dtmDateOfBirth = value;
			}
		}

		private int? _intGenderID;
		public int? intGenderID
		{
			get => _intGenderID;
			set
			{
				Console.WriteLine($"[SESSION] intGenderID set to {value}");
				_intGenderID = value;
			}
		}

		private int? _intZipCodeID;
		public int? intZipCodeID
		{
			get => _intZipCodeID;
			set
			{
				Console.WriteLine($"[SESSION] intZipCodeID set to {value}");
				_intZipCodeID = value;
			}
		}

		private bool _blnIsActive;
		public bool blnIsActive
		{
			get => _blnIsActive;
			set
			{
				Console.WriteLine($"[SESSION] blnIsActive set to {value}");
				_blnIsActive = value;
			}
		}

		private bool _blnIsOnline;
		public bool blnIsOnline
		{
			get => _blnIsOnline;
			set
			{
				Console.WriteLine($"[SESSION] blnIsOnline set to {value}");
				_blnIsOnline = value;
			}
		}

		private bool _blnInQueue;
		public bool blnInQueue
		{
			get => _blnInQueue;
			set
			{
				Console.WriteLine($"[SESSION] blnInQueue set to {value}");
				_blnInQueue = value;
			}
		}

		private DateTime? _dtmQueuedDate;
		public DateTime? dtmQueuedDate
		{
			get => _dtmQueuedDate;
			set
			{
				Console.WriteLine($"[SESSION] dtmQueuedDate set to {value}");
				_dtmQueuedDate = value;
			}
		}

		private bool _blnIsAuthenticated = false;
		public bool blnIsAuthenticated
		{
			get => _blnIsAuthenticated;
			set
			{
				Console.WriteLine($"[SESSION] blnIsAuthenticated set to {value}");
				_blnIsAuthenticated = value;
			}
		}
		public TUsers? GetUser()
		{
			if (!intUserID.HasValue)
			{
				Console.WriteLine("[SESSION] Cannot get user – intUserID is null.");
				return null;
			}

			return new TUsers
			{
				intUserID = intUserID.Value,
				strPhone = strPhone,
				strUsername = strUsername,
				strFirstName = strFirstName,
				strLastName = strLastName,
				dtmDateOfBirth = dtmDateOfBirth,
				intGenderID = intGenderID ?? 0,
				intZipCodeID = intZipCodeID,
				blnIsActive = blnIsActive,
				blnIsOnline = blnIsOnline,
				blnInQueue = blnInQueue,
				dtmQueuedDate = dtmQueuedDate
			};
		}
		public void Clear()
		{
			Console.WriteLine("[SESSION] Clearing session state...");
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
			Console.WriteLine("[SESSION] All fields cleared.");
		}
	}
}
