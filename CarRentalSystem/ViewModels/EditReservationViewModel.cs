using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class EditReservationViewModel
    {
        public int ReservationId { get; set; }

        public string CarName { get; set; } = string.Empty;

        public decimal DailyRate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        public int RentalDays { get; set; }

        public decimal TotalPrice { get; set; }
    }
}