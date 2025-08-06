using System.Threading.Tasks;
using Microsoft.JSInterop;
using SparkCheck.Data;
using SparkCheck.Models;
using Microsoft.EntityFrameworkCore;

namespace SparkCheck.Services
{
    public class LocationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AppDbContext _context;

        public LocationService(IJSRuntime jsRuntime, AppDbContext context)
        {
            _jsRuntime = jsRuntime;
            _context = context;
        }

        // Call JS to get live browser location (returns null if denied or fails)
        public async Task<(decimal Latitude, decimal Longitude)?> GetBrowserLocationAsync()
        {
            try
            {
                // JS function returns { latitude: number, longitude: number }
                var result = await _jsRuntime.InvokeAsync<GeoLocationResult>("getBrowserLocation");
                if (result != null)
                {
                    return (result.Latitude, result.Longitude);
                }
            }
            catch
            {
                Console.WriteLine("[DEBUG] Failed to get browser location. User may have denied permission or JS error occurred.");
            }
            return null;
        }

        // Retrieve stored user fallback location from DB
        public async Task<(decimal? Latitude, decimal? Longitude)> GetUserFallbackLocationAsync(int userId)
        {
            var user = await _context.TUsers.AsNoTracking().FirstOrDefaultAsync(u => u.intUserID == userId);
            if (user != null)
            {
                return (user.decLatitude, user.decLongitude); // Use your actual column names here
            }
            return (null, null);
        }

        // Save fallback location to DB (user's manual input)
        public async Task<bool> SaveUserFallbackLocationAsync(int userId, decimal latitude, decimal longitude)
        {
            var user = await _context.TUsers.FirstOrDefaultAsync(u => u.intUserID == userId);
            if (user != null)
            {
                user.decLatitude = latitude;
                user.decLongitude = longitude;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Decide which location to use: Browser live location or fallback
        public async Task<UserLocation> GetEffectiveUserLocationAsync(int userId)
        {
            var browserLocation = await GetBrowserLocationAsync();
            if (browserLocation.HasValue)
            {
                return new UserLocation
                {
                    Latitude = browserLocation.Value.Latitude,
                    Longitude = browserLocation.Value.Longitude,
                    IsFromBrowser = true
                };
            }

            var fallback = await GetUserFallbackLocationAsync(userId);
            return new UserLocation
            {
                Latitude = fallback.Latitude,
                Longitude = fallback.Longitude,
                IsFromBrowser = false
            };
        }
    }

    // Helper class for JS interop deserialization
    public class GeoLocationResult
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}