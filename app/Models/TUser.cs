using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TUsers")]
	public class TUsers {
		[Key]
		public int intUserID { get; set; }

		[MaxLength(250)]
		public string? strEmail { get; set; }

		[MaxLength(250)]
		public string? strPhone { get; set; }

		[Required, MaxLength(250)]
		public string strUsername { get; set; } = "";

		[Required, MaxLength(250)]
		public string strFirstName { get; set; } = "";

		[Required, MaxLength(250)]
		public string strLastName { get; set; } = "";

		[Required]
		public DateTime dtmDateOfBirth { get; set; }

		public int intGenderID { get; set; } = 1;
		public decimal decLatitude { get; set; } = 0;
		public decimal decLongitude { get; set; } = 0;
		public int intZipCodeID { get; set; } = 1;
		public bool blnIsActive { get; set; } = true;
		public bool blnIsOnline { get; set; } = false;
		public bool blnInQueue { get; set; } = false;
		public DateTime dtmCreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime? dtmQueuedDate { get; set; }
		public string? strUserToken { get; set; }
	}
}
