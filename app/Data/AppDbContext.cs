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
		public DbSet<TUserMedia> TUserMedia { get; set; }
		public DbSet<TUserPreferences> TUserPreferences { get; set; }
		public DbSet<TMatches> TMatches { get; set; }
		public DbSet<TMatchRequests> TMatchRequests { get; set; }
		public DbSet<TInterestCategories> TInterestCategories { get; set; }
		public DbSet<TInterests> TInterests { get; set; }
		public DbSet<TUserInterests> TUserInterests { get; set; }
		public DbSet<TOnboardingProgress> TOnboardingProgress { get; set; }


	}
}