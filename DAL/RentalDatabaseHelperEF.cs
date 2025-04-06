using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BikeRentalSystem.Models;
using Dapper;
using System.Data.SqlClient;
using System.Data;

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

        public IEnumerable<Rental> GetFilteredRentals(RentalFilter filter, string sortBy, bool sortAsc, int page, int pageSize)
        {
            IQueryable<Rental> rentalsQuery = _context.Rental.Include(r => r.Bike).Include(r => r.Customer);

            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                rentalsQuery = rentalsQuery.Where(r => r.Customer != null && r.Customer.FullName != null && r.Customer.FullName.Contains(filter.CustomerName));
            }

            if (!string.IsNullOrEmpty(filter.BikeModel))
            {
                rentalsQuery = rentalsQuery.Where(r => r.Bike != null && r.Bike.Model != null && r.Bike.Model.Contains(filter.BikeModel));
            }

            if (filter.StartDate.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalStartDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalEndDate <= filter.EndDate.Value);
            }

            if (sortAsc)
            {
                switch (sortBy.ToLower())
                {
                    case "rentalstartdate":
                        rentalsQuery = rentalsQuery.OrderBy(r => r.RentalStartDate);
                        break;
                    case "rentalenddate":
                        rentalsQuery = rentalsQuery.OrderBy(r => r.RentalEndDate);
                        break;
                    case "customername":
                        rentalsQuery = rentalsQuery.OrderBy(r => r.Customer != null && r.Customer.FullName != null ? r.Customer.FullName : string.Empty);
                        break;
                    case "bikemodel":
                        rentalsQuery = rentalsQuery.OrderBy(r => r.Bike != null && r.Bike.Model != null ? r.Bike.Model : string.Empty);
                        break;
                    default:
                        rentalsQuery = rentalsQuery.OrderBy(r => r.RentalStartDate);
                        break;
                }
            }
            else
            {
                switch (sortBy.ToLower())
                {
                    case "rentalstartdate":
                        rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalStartDate);
                        break;
                    case "rentalenddate":
                        rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalEndDate);
                        break;
                    case "customername":
                        rentalsQuery = rentalsQuery.OrderByDescending(r => r.Customer != null && r.Customer.FullName != null ? r.Customer.FullName : string.Empty);
                        break;
                    case "bikemodel":
                        rentalsQuery = rentalsQuery.OrderByDescending(r => r.Bike != null && r.Bike.Model != null ? r.Bike.Model : string.Empty);
                        break;
                    default:
                        rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalStartDate);
                        break;
                }
            }

            return rentalsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetTotalFilteredRentalsCount(RentalFilter filter)
        {
            IQueryable<Rental> rentalsQuery = _context.Rental.Include(r => r.Bike).Include(r => r.Customer);

            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                rentalsQuery = rentalsQuery.Where(r => r.Customer != null && r.Customer.FullName != null && r.Customer.FullName.Contains(filter.CustomerName));
            }

            if (filter.StartDate.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalStartDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalEndDate <= filter.EndDate.Value);
            }

            return rentalsQuery.Count();
        }
    }
}
