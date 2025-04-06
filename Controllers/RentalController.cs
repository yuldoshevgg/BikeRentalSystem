using BikeRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using BikeRentalSystem.DAL;
using System.Text;

namespace BikeRentalSystem.Controllers
{
    public class RentalController : Controller
    {
        private readonly RentalDatabaseHelperEF _dbHelper;

        public RentalController(ChinookContext context)
        {
            _dbHelper = new RentalDatabaseHelperEF(context);
        }

        public IActionResult Index(RentalFilter filter, string sortBy = "DateRented", bool sortAsc = true, int page = 1, int pageSize = 10)
        {
            var rentals = _dbHelper.GetFilteredRentals(filter, sortBy, sortAsc, page, pageSize);
            var totalCount = _dbHelper.GetTotalFilteredRentalsCount(filter);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var viewModel = new RentalPageViewModel
            {
                Rentals = rentals,
                Filter = filter,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                SortBy = sortBy,
                SortAsc = sortAsc
            };

            return View(viewModel);
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

        [HttpGet]
        public IActionResult ExportToXml(RentalFilter filter)
        {
            try
            {
                var xmlContent = _dbHelper.ExportRentalsToXml(filter);
                byte[] bytes = Encoding.UTF8.GetBytes(xmlContent);

                return File(bytes, "application/xml", $"bike-rentals-{DateTime.Now:yyyyMMdd}.xml");
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = $"Failed to export XML: {ex.Message}";
                return RedirectToAction("Index", new
                {
                    CustomerName = filter.CustomerName,
                    BikeModel = filter.BikeModel,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate
                });
            }
        }

        [HttpGet]
        public IActionResult ExportToJson(RentalFilter filter)
        {
            try
            {
                var jsonContent = _dbHelper.ExportRentalsToJson(filter);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonContent);

                return File(bytes, "application/json", $"bike-rentals-{DateTime.Now:yyyyMMdd}.json");
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = $"Failed to export JSON: {ex.Message}";
                return RedirectToAction("Index", new
                {
                    CustomerName = filter.CustomerName,
                    BikeModel = filter.BikeModel,
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate
                });
            }
        }
    }
}
