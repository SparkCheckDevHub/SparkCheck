using System.ComponentModel.DataAnnotations;

public class CreateAccountModel {
	[Required]
	[Display(Name = "Username")]
	[StringLength(20, MinimumLength = 8, ErrorMessage = "Username must be 8–20 characters.")]
	[RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Username must be alphanumeric.")]
	public string strUsername { get; set; } = "";

	[Required]
	[Display(Name = "First Name")]
	[StringLength(20, ErrorMessage = "First name must be no more than 20 characters.")]
	[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
	public string strFirstName { get; set; } = "";

	[Required]
	[Display(Name = "Last Name")]
	[StringLength(20, ErrorMessage = "Last name must be no more than 20 characters.")]
	[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
	public string strLastName { get; set; } = "";

	[Display(Name = "Date Of Birth")]
	[Required(ErrorMessage = "Date of Birth is required.")]
	public DateTime? dtmDateOfBirth { get; set; }

	[Required]
	[Display(Name = "Phone Number")]
	[RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits with no symbols.")]
	public string strPhone { get; set; } = "";

	[Required]
	[Display(Name = "Email Address")]
	[EmailAddress(ErrorMessage = "Enter a valid email address.")]
	public string strEmail { get; set; } = "";
}
