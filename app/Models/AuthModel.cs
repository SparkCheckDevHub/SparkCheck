using System.ComponentModel.DataAnnotations;

public class AuthModel {
	[Required(ErrorMessage = "Verification code is required.")]
	public string VerificationCode { get; set; } = "";

	public string strPhone { get; set; } = "";  // This can be pre-filled from PhoneLogin
}