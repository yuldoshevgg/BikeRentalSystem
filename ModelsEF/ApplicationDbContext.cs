using BikeRentalSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Bike> Bikes { get; set; }
    }

}
