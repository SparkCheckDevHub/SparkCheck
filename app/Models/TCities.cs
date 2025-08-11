using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models
{
    [Table("TCities")]
    public class TCities
    {
        [Key]
        public int intCityID { get; set; }
        public string strCity { get; set; } = string.Empty;
        public int intStateID { get; set; }
    }
}
