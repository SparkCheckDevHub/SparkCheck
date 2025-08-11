using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
    [Table("TStates")]
    public class TStates
    {
        [Key]
        public int intStateID { get; set; }
        public string strState { get; set; } = string.Empty;
        public string strStateCode { get; set; } = string.Empty;
	}
}
