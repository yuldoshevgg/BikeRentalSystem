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

    public class RentalFilter
    {
        public string? CustomerName { get; set; }
        public string? BikeModel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortColumn { get; set; } = "RentalStartDate";
        public string SortOrder { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class RentalPageViewModel
    {
        public IEnumerable<Rental>? Rentals { get; set; }
        public RentalFilter? Filter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SortBy { get; set; }
        public bool SortAsc { get; set; }
    }
}
