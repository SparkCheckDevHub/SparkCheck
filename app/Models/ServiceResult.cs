namespace SparkCheck.Models {
	public class ServiceResult {
		public bool Success { get; set; }
		public string? ErrorMessage { get; set; }

		public static ServiceResult Ok() => new() { Success = true };
		public static ServiceResult Fail(string error) => new() { Success = false, ErrorMessage = error };
	}

	public class ServiceResult<T> {
		public bool Success { get; set; }
		public string? ErrorMessage { get; set; }
		public T? Result { get; set; }

		public static ServiceResult<T> Ok(T result) => new() { Success = true, Result = result };
		public static ServiceResult<T> Fail(string error) => new() { Success = false, ErrorMessage = error };
	}
}
