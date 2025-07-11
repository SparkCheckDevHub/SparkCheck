using Microsoft.AspNetCore.Identity;
using System;

namespace SparkCheck.Models {
	public class ApplicationUser : IdentityUser {
		public string FirstName { get; set; }  // Renamed for consistency
		public string LastName { get; set; }  // Renamed for consistency
		public DateTime? DateOfBirth { get; set; }  // Nullable to handle missing DOB
		public int GenderID { get; set; } = 1;  // Default value can be set
		public int ZipCodeID { get; set; } = 1;  // Default value for Zip Code
		public bool IsActive { get; set; } = true;  // Default to active
		public bool IsOnline { get; set; } = false;  // Default to offline
		public bool InQueue { get; set; } = false;  // Default to not in queue

		// Optionally, you could also add:
		public override string ToString() {
			return $"{FirstName} {LastName} ({PhoneNumber})";
		}
	}
}
