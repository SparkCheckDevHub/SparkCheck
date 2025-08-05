using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TOnboardingProgress {
	[Key]
	public int intOnboardingProgressID { get; set; }

	[ForeignKey("TUsers")]
	public int intUserID { get; set; }

	public bool blnProfileComplete { get; set; }
	public bool blnPreferencesComplete { get; set; }
}
