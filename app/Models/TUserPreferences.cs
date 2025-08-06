using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models
{
	[Table("TUserPreferences")]
	public class TUserPreferences
	{
		[Key]
		[ForeignKey("User")]
		public int intUserID { get; set; }
		public TUsers? User { get; set; }

		[Range(1, 299)]
		public int intMatchDistance { get; set; } = 50;

		[Range(18, 99)]
		public int intMinAge { get; set; } = 18;

		[Range(19, 100)]
		public int intMaxAge { get; set; } = 35;

		[ForeignKey("TGenders")]
		[Required]
		public int intGenderPreferenceID { get; set; } = 1;

		[Required]
		public bool blnReceiveEmails { get; set; } = true;

		[Required]
		public bool blnShowProfile { get; set; } = true;

		[Required]
		[MaxLength(350)]
		public string strBio { get; set; } = "";

		[Required]
		public int intAppUsageTypeID { get; set; } = 1;
	}

	public class TAppUsageTypes
	{
		[Key]
		public int intAppUsageTypeID { get; set; }
		public string strAppUsageType { get; set; } = "";
	}
}
