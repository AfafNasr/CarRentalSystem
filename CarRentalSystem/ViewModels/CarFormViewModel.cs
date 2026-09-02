using CarRentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CarFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1990, 2100)]
        public int Year { get; set; }

        [Required]
        public CarType Type { get; set; }

        [Required]
        public TransmissionType Transmission { get; set; }

        [Required]
        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }

        [Required]
        [Range(1, 20)]
        public int Seats { get; set; }

        [Required]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Daily Rate")]
        public decimal DailyRate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Display(Name = "Car Image")]
        public IFormFile? ImageFile { get; set; }
    }
}