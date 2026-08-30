using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CreateReservationViewModel
    {
        public int CarId { get; set; }

        [ValidateNever]
        public Car Car { get; set; } = null!;

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