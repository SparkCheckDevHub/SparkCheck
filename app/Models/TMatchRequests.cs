using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
    [Table("TMatchRequests")]
    public class TMatchRequests {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intMatchRequestID { get; set; }

        [Required]
        public int intFirstUserID { get; set; }

        [Required]
        public int intSecondUserID { get; set; }

        [Required]
        public bool blnFirstUserDeclined { get; set; }

        [Required]
        public bool blnSecondUserDeclined { get; set; }

        [Required]
        public DateTime dtmRequestStarted { get; set; }

        public DateTime? dtmRequestEnded { get; set; }
    }
}