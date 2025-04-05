using BikeRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using BikeRentalSystem.DAL;

namespace BikeRentalSystem.Controllers
{
    public class BikeController : Controller
    {
        private readonly DatabaseHelperDapper _dbHelper;

        public BikeController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelperDapper(configuration);
        }

        public IActionResult Index()
        {
            var bikes = _dbHelper.GetAllBikes();
            return View(bikes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bike bike, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (bike.DateAdded == default(DateTime))
                {
                    bike.DateAdded = DateTime.Now;
                }

                byte[] imageBytes = Array.Empty<byte>();

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                var newBike = new Bike
                {
                    Model = bike.Model,
                    Type = bike.Type,
                    Status = bike.Status,
                    RentalPricePerHour = bike.RentalPricePerHour,
                    HasInsurance = bike.HasInsurance,
                    Image = imageBytes,
                    DateAdded = DateTime.Now
                };

                _dbHelper.CreateBike(newBike);
                return RedirectToAction(nameof(Index));
            }
            return View(bike);
        }

        public IActionResult Edit(int id)
        {
            Bike? bike = _dbHelper.GetBikeByID(id);
            if (bike == null)
            {
                return NotFound();
            }

            return View(bike);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Bike bike, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                        bike.Image = memoryStream.ToArray();
                    }
                }
                else
                {
                    var existingBike = _dbHelper.GetBikeByID(bike.BikeID);
                    if (existingBike != null)
                    {
                        bike.Image = existingBike.Image;
                    }
                }

                _dbHelper.UpdateBike(bike);
                return RedirectToAction(nameof(Index));
            }

            return View(bike);
        }

        public IActionResult Delete(int id)
        {
            Bike? bike = _dbHelper.GetBikeByID(id);
            if (bike == null)
            {
                return NotFound();
            }
            return View(bike);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dbHelper.DeleteBike(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
