using BikeRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using BikeRentalSystem.DAL;

namespace BikeRentalSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerDatabaseHelperEF _dbHelper;

        public CustomerController(ChinookContext context)
        {
            _dbHelper = new CustomerDatabaseHelperEF(context);
        }

        public IActionResult Index()
        {
            var customers = _dbHelper.GetAllCustomers();
            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                byte[] imageBytes = Array.Empty<byte>();

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                _dbHelper.CreateCustomer(new Customer
                {
                    FullName = customer.FullName,
                    Address = customer.Address,
                    DateOfBirth = customer.DateOfBirth,
                    IsMarried = customer.IsMarried,
                    PhoneNumber = customer.PhoneNumber,
                    Image = imageBytes,
                    Email = customer.Email
                });
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        public IActionResult Edit(int id)
        {
            Customer? customer = _dbHelper.GetCustomerByID(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                        customer.Image = memoryStream.ToArray();
                    }
                }
                else
                {
                    var existingCustomer = _dbHelper.GetCustomerByID(customer.CustomerID);
                    if (existingCustomer != null)
                    {
                        customer.Image = existingCustomer.Image;
                    }
                }

                _dbHelper.UpdateCustomer(customer);
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            Customer? customer = _dbHelper.GetCustomerByID(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dbHelper.DeleteCustomer(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
