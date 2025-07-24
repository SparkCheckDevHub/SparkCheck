
using Microsoft.EntityFrameworkCore;
using SparkCheck.Models;

namespace SparkCheck.Data {
	public class AppDbContext : DbContext{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<TUsers> TUsers { get; set; }
		public DbSet<TLoginAttempts> TLoginAttempts { get; set; }
		public DbSet<TGenders> TGenders { get; set; }
	}
}
