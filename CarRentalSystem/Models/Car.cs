using Microsoft.VisualBasic.FileIO;

namespace CarRentalSystem.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public CarType Type { get; set; }

        public TransmissionType Transmission { get; set; }

        public FuelType FuelType { get; set; }

        public int Seats { get; set; }

        public string Color { get; set; } = string.Empty;

        public decimal DailyRate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Reservation> Reservations { get; set; }
            = new List<Reservation>();
    }
}