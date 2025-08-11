using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
    [Table("TZipCodes")]
    public class TZipCodes
    {
        [Key]
        public int intZipCodeID { get; set; }

        public string strZipCode { get; set; } = string.Empty;
        public int intCityID { get; set; }
        
		public decimal decLatitude { get; set; }
		public decimal decLongitude { get; set; }
	}
}
