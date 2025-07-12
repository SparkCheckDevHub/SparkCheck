using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TLoginAttempts")]
public class TLoginAttempts {
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int intLoginAttemptID { get; set; }

	[Required]
	[MaxLength(20)]
	public string strPhone { get; set; } = "";

	[Required]
	[MaxLength(20)]
	public string strVerificationCode { get; set; } = "";

	[Required]
	public DateTime dtmLoginDate { get; set; }

	[MaxLength(250)]
	public string? strIPAddress { get; set; }

	[MaxLength(250)]
	public string? strUserAgent { get; set; }

	[Required]
	public int intUserID { get; set; }

	[Required]
	public bool blnIsActive { get; set; }

	[Required]
	public int intAttempts { get; set; } = 0;

}
