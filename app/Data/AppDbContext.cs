using Microsoft.EntityFrameworkCore;
using VibeCheck.Models; 

namespace VibeCheck.Data {
	public class AppDbContext : DbContext {
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<TUsers> TUsers { get; set; }
	}
}
