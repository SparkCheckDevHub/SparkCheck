using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkCheck.Models;

namespace SparkCheck.Data {
	public class AppDbContext : IdentityDbContext<ApplicationUser> {
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<TUsers> TUsers { get; set; }
		public DbSet<TLoginAttempts> TLoginAttempts { get; set; }
	}
}
