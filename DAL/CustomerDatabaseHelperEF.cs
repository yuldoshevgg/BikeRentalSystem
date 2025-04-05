using System.Collections.Generic;
using System.Linq;
using BikeRentalSystem.Models;

namespace BikeRentalSystem.DAL
{
    public class CustomerDatabaseHelperEF
    {
        private readonly ChinookContext _context;

        public CustomerDatabaseHelperEF(ChinookContext context)
        {
            _context = context;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customer.ToList();
        }

        public Customer? GetCustomerByID(int id)
        {
            return _context.Customer.FirstOrDefault(c => c.CustomerID == id);
        }

        public void CreateCustomer(Customer customer)
        {
            _context.Customer.Add(customer);
            _context.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            var existing = _context.Customer.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            if (existing == null) throw new KeyNotFoundException("Customer not found");

            existing.FullName = customer.FullName;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.Image = customer.Image;
            existing.DateOfBirth = customer.DateOfBirth;
            existing.IsMarried = customer.IsMarried;
            existing.Email = customer.Email;

            _context.SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var customer = _context.Customer.FirstOrDefault(c => c.CustomerID == id);
            if (customer != null)
            {
                _context.Customer.Remove(customer);
                _context.SaveChanges();
            }
        }
    }
}
