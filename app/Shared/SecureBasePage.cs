using Microsoft.AspNetCore.Components;
using SparkCheck.Services;

namespace SparkCheck.Shared {
	public class SecureBasePage : ComponentBase {
		[Inject] protected UserSessionService UserSession { get; set; } = default!;
		[Inject] protected NavigationManager Navigation { get; set; } = default!;

		private bool hasRedirected = false;

		protected override Task OnAfterRenderAsync(bool firstRender) {
			if (firstRender && !UserSession.blnIsAuthenticated && !hasRedirected) {
				hasRedirected = true;
				Console.WriteLine("[SECURITY] Unauthorized access. Redirecting to Welcome.razor...");
				Navigation.NavigateTo("/", forceLoad: true);
			}

			return Task.CompletedTask;
		}
	}
}
