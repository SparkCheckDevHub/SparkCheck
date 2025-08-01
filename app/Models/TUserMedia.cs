using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TUserMedia")]
	public class TUserMedia {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int intUserMediaID { get; set; }

		[Required]
		[ForeignKey("User")]
		public int intUserID { get; set; }
		public TUsers? User { get; set; }

		[Required]
		[MaxLength(250)]
		public string strMediaURL { get; set; } = "";

		[Required]
		public bool blnOnProfile { get; set; } = false;

		[Required]
		public bool blnIsFace { get; set; } = false;

		[Required]
		public bool blnIsActive { get; set; } = false;

		[Required]
		public DateTime dtmUploadDate { get; set; } = DateTime.Now;
	}
}