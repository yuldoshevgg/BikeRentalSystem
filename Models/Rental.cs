using System;

namespace BikeRentalSystem.Models
{
    public class Rental
    {
        public int RentalID { get; set; }
        public int CustomerID { get; set; }
        public int BikeID { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public decimal TotalCost { get; set; }
        public int RentalDuration { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Bike? Bike { get; set; }
    }
}
