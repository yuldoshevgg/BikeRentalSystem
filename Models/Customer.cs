using System;

namespace BikeRentalSystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsMarried { get; set; }
        public byte[]? Image { get; set; }
        public string? Email { get; set; }
    }
}
