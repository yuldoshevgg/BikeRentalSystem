using System.ComponentModel.DataAnnotations;

namespace BikeRentalSystem.Models
{
    public class Bike
    {
        public int BikeID { get; set; }
        public string? Model { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public decimal RentalPricePerHour { get; set; }

        public byte[]? Image { get; set; }
        public DateTime DateAdded { get; set; }
        public bool HasInsurance { get; set; }
    }
}
