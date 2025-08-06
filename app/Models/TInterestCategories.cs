using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TInterestCategories")]
	public class TInterestCategories {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int intInterestCategoryID { get; set; }

		[Required]
		[MaxLength(250)]
		public string strInterestCategory { get; set; } = string.Empty;
	}
}
