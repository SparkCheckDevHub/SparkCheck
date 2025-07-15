using System.ComponentModel.DataAnnotations;

namespace SparkCheck.Models.ViewModels {
	public class PhoneAuthModel {
		[Required(ErrorMessage = "Verification code is required.")]
		[StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits.")]
		public string Code { get; set; } = "";
	}
}
