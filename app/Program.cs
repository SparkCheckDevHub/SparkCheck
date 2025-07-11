using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Serilog;
using SparkCheck;
using SparkCheck.Data;
using SparkCheck.Models;
using SparkCheck.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.WriteTo.File("Logs/myapp.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();

// Add services to the container.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<UserService>();  // UserService is scoped correctly
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // IHttpContextAccessor

// Register AppDbContext with the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity for user management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

// Configure MudBlazor
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Configure logging to use Serilog
builder.Logging.ClearProviders(); // Clear existing logging providers
builder.Logging.AddSerilog(); // Use Serilog for logging

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map razor components for the app
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

