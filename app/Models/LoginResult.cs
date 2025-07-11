using SparkCheck.Models;
public class LoginResult
{
	public bool blnSuccess { get; set; }
	public string? strErrorMessage { get; set; }
	public TUsers? User { get; set; }

	public static LoginResult Ok(TUsers user) =>
		new LoginResult { blnSuccess = true, User = user };

	public static LoginResult Fail(string message) =>
		new LoginResult { blnSuccess = false, strErrorMessage = message };
}
