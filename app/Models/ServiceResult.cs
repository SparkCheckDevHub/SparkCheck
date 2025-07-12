using SparkCheck.Models;

public class ServiceResult {
	public bool blnSuccess { get; set; }
	public string? strErrorMessage { get; set; }
	public TUsers? User { get; set; }
	public TLoginAttempts? LoginAttempt { get; set; } // Add this

	public static ServiceResult Ok() => new() { blnSuccess = true };
	public static ServiceResult Fail(string message) => new() { blnSuccess = false, strErrorMessage = message };
}
