using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TUsers")]
	public class TUsers {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int intUserID { get; set; }

		[Required]
		[Display(Name = "Phone Number")]
		[MaxLength(250)]
		public string strPhone { get; set; } = "";

		[Required]
		[Display(Name = "Email Address")]
		[MaxLength(250)]
		public string strEmail { get; set; } = "";

		[Required]
		[Display(Name = "Username")]
		[MaxLength(250)]
		public string strUsername { get; set; } = "";

		[Required]
		[Display(Name = "First Name")]
		[MaxLength(250)]
		public string strFirstName { get; set; } = "";

		[Required]
		[Display(Name = "Last Name")]
		[MaxLength(250)]
		public string strLastName { get; set; } = "";

		[Required]
		[Display(Name = "Date of Birth")]
		public DateTime? dtmDateOfBirth { get; set; }

		[Display(Name = "Gender")]
		public int intGenderID { get; set; } = 1;

		public decimal? decLatitude { get; set; }
		public decimal? decLongitude { get; set; }
		public int? intZipCodeID { get; set; }
		public bool blnIsActive { get; set; } = true;
		public bool blnIsOnline { get; set; } = false;
		public bool blnInQueue { get; set; } = false;
		public DateTime dtmCreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime? dtmQueuedDate { get; set; }
		public string? strUserToken { get; set; }
	}
}
