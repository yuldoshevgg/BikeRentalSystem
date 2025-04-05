using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BikeRentalSystem.Models;

namespace BikeRentalSystem.DAL
{
    public class RentalDatabaseHelperEF
    {
        private readonly ChinookContext _context;

        public RentalDatabaseHelperEF(ChinookContext context)
        {
            _context = context;
        }

        public IEnumerable<Bike> GetAllBikes()
        {
            return _context.Bike.ToList();
        }

        public Bike? GetBikeByID(int id)
        {
            return _context.Bike.FirstOrDefault(c => c.BikeID == id);
        }

        public Customer? GetCustomerByID(int id)
        {
            return _context.Customer.FirstOrDefault(c => c.CustomerID == id);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.Customer.ToList();
        }

        public List<Rental> GetAllRentals()
        {
            return _context.Rental.Include(r => r.Customer).Include(r => r.Bike).ToList();
        }

        public Rental? GetRentalByID(int id)
        {
            return _context.Rental.Include(r => r.Customer).Include(r => r.Bike)
                                  .FirstOrDefault(r => r.RentalID == id);
        }

        public void CreateRental(Rental rental)
        {
            if (rental.Customer == null || rental.Bike == null)
                throw new ArgumentNullException("Customer or Bike cannot be null");

            _context.Rental.Add(rental);
            _context.SaveChanges();
        }

        public void UpdateRental(Rental rental)
        {
            var existing = _context.Rental.FirstOrDefault(r => r.RentalID == rental.RentalID);
            if (existing == null) throw new KeyNotFoundException("Rental not found");

            existing.CustomerID = rental.CustomerID;
            existing.BikeID = rental.BikeID;
            existing.RentalStartDate = rental.RentalStartDate;
            existing.RentalEndDate = rental.RentalEndDate;
            existing.RentalDuration = rental.RentalDuration;
            existing.TotalCost = rental.TotalCost;

            _context.SaveChanges();
        }

        public void DeleteRental(int id)
        {
            var rental = _context.Rental.FirstOrDefault(r => r.RentalID == id);
            if (rental != null)
            {
                _context.Rental.Remove(rental);
                _context.SaveChanges();
            }
        }
    }
}
