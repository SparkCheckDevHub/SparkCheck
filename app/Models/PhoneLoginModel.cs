using System.ComponentModel.DataAnnotations;

namespace SparkCheck.Models.ViewModels {
	public class PhoneLoginModel {
		[Required(ErrorMessage = "Phone number is required.")]
		[Phone(ErrorMessage = "Enter a valid phone number.")]
		[Display(Name = "Phone Number")]
		public string strPhone { get; set; } = "";
	}
}
