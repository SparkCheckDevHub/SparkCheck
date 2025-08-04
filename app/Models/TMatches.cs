using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models
{
    [Table("TMatches")]
    public class TMatches
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intMatchID { get; set; }

        [Required]
        public int intMatchRequestID { get; set; }

        [Required]
        public bool blnFirstUserDeleted { get; set; }

        [Required]
        public bool blnSecondUserDeleted { get; set; }

        [Required]
        public DateTime dtmMatchStarted { get; set; }

        public DateTime? dtmMatchEnded { get; set; }
    }
}