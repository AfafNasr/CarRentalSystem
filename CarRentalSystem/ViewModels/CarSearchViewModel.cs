using CarRentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CarSearchViewModel
    {
        public string? Location { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime? PickupDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Car Type")]
        public CarType? Type { get; set; }

        public TransmissionType? Transmission { get; set; }

        public List<Car> Cars { get; set; } = new();
    }
}