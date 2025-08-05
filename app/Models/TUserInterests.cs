using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TUserInterests")]
	public class TUserInterests {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int intUserInterestID { get; set; }

		[Required]
		public int intUserID { get; set; }

		[Required]
		public int intInterestID { get; set; }
	}
}
