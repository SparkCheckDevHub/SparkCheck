namespace SparkCheck.Models {
	public class ServiceResult {
		public bool blnSuccess { get; set; }
		public string? strErrorMessage { get; set; }

		public static ServiceResult Ok() => new() { blnSuccess = true };
		public static ServiceResult Fail(string error) => new() { blnSuccess = false, strErrorMessage = error };
	}

	public class ServiceResult<T> {
		public bool blnSuccess { get; set; }
		public string? strErrorMessage { get; set; }
		public T? Result { get; set; }

		public static ServiceResult<T> Ok(T result) => new() { blnSuccess = true, Result = result };
		public static ServiceResult<T> Fail(string error) => new() { blnSuccess = false, strErrorMessage = error };
	}
}
