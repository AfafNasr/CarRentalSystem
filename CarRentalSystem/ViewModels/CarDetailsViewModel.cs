using CarRentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CarDetailsViewModel
    {
        public Car Car { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime? PickupDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }
    }
}