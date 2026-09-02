using CarRentalSystem.Models;

namespace CarRentalSystem.ViewModels
{
    public class AdminReservationViewModel
    {
        public int ReservationId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        public string CarName { get; set; } = string.Empty;

        public DateTime PickupDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public decimal TotalPrice { get; set; }

        public ReservationStatus Status { get; set; }
    }
}