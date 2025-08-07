using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models {
    [Table("TChatMessages")]
    public class TChatMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intChatMessageID { get; set; }

        [Required]
        public int intSenderUserID { get; set; }

        public int? intUserMediaID { get; set; }

        [Required]
        public int intMatchID { get; set; }

		[Required]
		[MaxLength(1000)]
		public string strMessageText { get; set; } = "";

        [Required]
        public DateTime dtmSentAt { get; set; }

        public bool blnIsActive { get; set; } = true;
    }
}