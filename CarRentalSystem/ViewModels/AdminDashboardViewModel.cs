namespace CarRentalSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalCars { get; set; }

        public int ActiveCars { get; set; }

        public int TotalReservations { get; set; }

        public int PendingReservations { get; set; }

        public int TotalCustomers { get; set; }
    }
}