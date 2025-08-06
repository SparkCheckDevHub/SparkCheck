
using Microsoft.EntityFrameworkCore;
using SparkCheck.Models; 

namespace SparkCheck.Data {
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<TUsers> TUsers { get; set; }
		public DbSet<TLoginAttempts> TLoginAttempts { get; set; }
		public DbSet<TGenders> TGenders { get; set; }
		public DbSet<TInterests> TInterests { get; set; }
		public DbSet<TUserMedia> TUserMedia { get; set; }
		public DbSet<TInterestCategory> TInterestCategory { get; set; }
		public DbSet<TInterestSubCategory> TInterestSubCategory { get; set; }
		public DbSet<TUserPreferences> TUserPreferences { get; set; }
		public DbSet<TMatches> TMatches { get; set; }
		public DbSet<TMatchRequests> TMatchRequests { get; set; }
		public DbSet<TAppUsageTypes> TAppUsageTypes { get; set; }
	}
}