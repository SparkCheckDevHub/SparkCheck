using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
	[Table("TGenders")]
	public class TGenders {
		[Key]  
		public int intGenderID { get; set; }

		public string strGender { get; set; } = string.Empty;
	}
}
