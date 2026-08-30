namespace CarRentalSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        public DateTime PickupDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public decimal TotalPrice { get; set; }

        public ReservationStatus Status { get; set; }
            = ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}