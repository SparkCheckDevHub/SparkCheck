namespace SparkCheck.Models
{
    public class UserLocation
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsFromBrowser { get; set; } // true = live, false = fallback
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
    }
}