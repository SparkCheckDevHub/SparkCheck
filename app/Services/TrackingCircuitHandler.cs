using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;
using SparkCheck.Services;

public class TrackingCircuitHandler : CircuitHandler {
	private readonly UserSessionService _session;
	private readonly IServiceProvider _serviceProvider;

	public TrackingCircuitHandler(UserSessionService session, IServiceProvider serviceProvider) {
		_session = session;
		_serviceProvider = serviceProvider;
	}

	public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken) {
		Console.WriteLine("[DISCONNECT] Circuit closed. Checking if user is logged in...");

		if (_session.intUserID.HasValue) {
			_ = Task.Run(async () => {
				using var scope = _serviceProvider.CreateScope();
				var userService = scope.ServiceProvider.GetRequiredService<UserService>();

				try {
					await userService.UpdateUserStatusAsync(_session.intUserID.Value, false);
					Console.WriteLine($"[DISCONNECT] UserID {_session.intUserID} marked offline.");
				}
				catch (Exception ex) {
					Console.WriteLine($"[DISCONNECT ERROR] Failed to mark user offline: {ex.Message}");
				}
			});
		}

		return Task.CompletedTask;
	}
}
