using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TInterests")]
	public class TInterests {
		[Key]
		public int intInterestID { get; set; }

		[Required]
		[MaxLength(250)]
		public string strInterest { get; set; } = string.Empty;

		public int intInterestCategoryID { get; set; }
		public int? intInterestSubCategoryID { get; set; }
	}
}
