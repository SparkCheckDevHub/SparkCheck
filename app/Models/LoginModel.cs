using System.ComponentModel.DataAnnotations;

public class LoginModel {
	[Required(ErrorMessage = "Phone number is required.")]
	[RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a 10-digit phone number with no symbols.")]
	public string strPhone { get; set; } = "";
}
