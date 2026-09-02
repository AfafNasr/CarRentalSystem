using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class AdminCreateReservationViewModel
    {
        public int UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Car")]
        public int CarId { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }


        public List<AdminCarOptionViewModel> Cars { get; set; }
            = new();
    }


    public class AdminCarOptionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public decimal DailyRate { get; set; }
    }
}