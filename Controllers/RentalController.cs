using BikeRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using BikeRentalSystem.DAL;

namespace BikeRentalSystem.Controllers
{
    public class RentalController : Controller
    {
        private readonly RentalDatabaseHelperEF _dbHelper;

        public RentalController(ChinookContext context)
        {
            _dbHelper = new RentalDatabaseHelperEF(context);
        }

        public IActionResult Index()
        {
            var rentals = _dbHelper.GetAllRentals();
            return View(rentals);
        }

        public IActionResult Create()
        {

            ViewBag.Customers = _dbHelper.GetAllCustomers();
            ViewBag.Bikes = _dbHelper.GetAllBikes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rental rental)
        {
            rental.Bike = _dbHelper.GetBikeByID(rental.BikeID);
            rental.Customer = _dbHelper.GetCustomerByID(rental.CustomerID);

            if (ModelState.IsValid)
            {
                if (rental.Bike == null)
                {
                    ModelState.AddModelError(string.Empty, "Bike information is required.");
                    return View(rental);
                }

                if (rental?.RentalStartDate != null && rental?.RentalEndDate != null)
                {
                    rental.RentalDuration = (int)(rental.RentalEndDate - rental.RentalStartDate).TotalDays;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Both rental start and end dates are required.");
                    return View(rental);
                }

                _dbHelper.CreateRental(rental);
                return RedirectToAction(nameof(Index));
            }

            return View(rental);
        }

        public IActionResult Edit(int id)
        {
            Rental? rental = _dbHelper.GetRentalByID(id);
            if (rental == null)
            {
                return NotFound();
            }

            ViewBag.Customers = _dbHelper.GetAllCustomers();
            ViewBag.Bikes = _dbHelper.GetAllBikes();
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Rental rental)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(rental));
            rental.Bike = _dbHelper.GetBikeByID(rental.BikeID);
            rental.Customer = _dbHelper.GetCustomerByID(rental.CustomerID);

            if (ModelState.IsValid)
            {
                if (rental.Bike == null)
                {
                    return View(rental);
                }

                if (rental?.RentalStartDate != null && rental?.RentalEndDate != null)
                {
                    rental.RentalDuration = (int)(rental.RentalEndDate - rental.RentalStartDate).TotalDays;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Both rental start and end dates are required.");
                    return View(rental);
                }

                _dbHelper.UpdateRental(rental);
                return RedirectToAction(nameof(Index));
            }

            return View(rental);
        }

        public IActionResult Delete(int id)
        {
            Rental? rental = _dbHelper.GetRentalByID(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dbHelper.DeleteRental(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
