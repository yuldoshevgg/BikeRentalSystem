using Microsoft.EntityFrameworkCore;
using BikeRentalSystem.Models;

namespace BikeRentalSystem.DAL
{
    public class ChinookContext : DbContext
    {
        public ChinookContext(DbContextOptions<ChinookContext> options)
            : base(options)
        {
        }

        public DbSet<Bike> Bike { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Rental> Rental { get; set; }
    }
}
