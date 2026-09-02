using CarRentalSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Data
{
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = 1000,

        FirstName = "System",
        LastName = "Admin",

        Email = "admin@gmail.com",

        PasswordHash = "AQAAAAIAAYagAAAAEO+Hd+1TsOkqhR99zZq3UTpuJFvExJrwKjue7n/0TLN+yxYq2fxaHAAzx+FEQjaqiA==",

        PhoneNumber = "0000000000",

        DateOfBirth = new DateTime(1990, 1, 1),

        AddressLine1 = "DriveEase Headquarters",

        AddressLine2 = null,

        City = "Nablus",

        Country = "Palestine",

        DriversLicenseNumber = "ADMIN-001",

        Role = UserRole.Admin,

        CreatedAt = new DateTime(
            2026,
            1,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc)
    }
);

            modelBuilder.Entity<Car>()
                .HasData(
                    new Car
                    {
                        Id = 1,
                        Brand = "Toyota",
                        Model = "Corolla",
                        Year = 2025,
                        Type = CarType.Sedan,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Petrol,
                        Seats = 5,
                        Color = "White",
                        DailyRate = 45m,
                        Location = "Nablus",
                        ImageUrl = "/images/cars/toyota-corolla.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    },

                    new Car
                    {
                        Id = 2,
                        Brand = "Hyundai",
                        Model = "Elantra",
                        Year = 2025,
                        Type = CarType.Sedan,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Petrol,
                        Seats = 5,
                        Color = "Black",
                        DailyRate = 50m,
                        Location = "Ramallah",
                        ImageUrl = "/images/cars/hyundai-elantra.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    },

                    new Car
                    {
                        Id = 3,
                        Brand = "Kia",
                        Model = "Sportage",
                        Year = 2025,
                        Type = CarType.SUV,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Hybrid,
                        Seats = 5,
                        Color = "Gray",
                        DailyRate = 70m,
                        Location = "Nablus",
                        ImageUrl = "/images/cars/kia-sportage.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    },

                    new Car
                    {
                        Id = 4,
                        Brand = "Hyundai",
                        Model = "Tucson",
                        Year = 2024,
                        Type = CarType.SUV,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Petrol,
                        Seats = 5,
                        Color = "Blue",
                        DailyRate = 68m,
                        Location = "Jenin",
                        ImageUrl = "/images/cars/hyundai-tucson.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    },

                    new Car
                    {
                        Id = 5,
                        Brand = "Tesla",
                        Model = "Model 3",
                        Year = 2025,
                        Type = CarType.Sedan,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Electric,
                        Seats = 5,
                        Color = "Red",
                        DailyRate = 95m,
                        Location = "Ramallah",
                        ImageUrl = "/images/cars/tesla-model-3.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    },

                    new Car
                    {
                        Id = 6,
                        Brand = "BMW",
                        Model = "X5",
                        Year = 2025,
                        Type = CarType.SUV,
                        Transmission = TransmissionType.Automatic,
                        FuelType = FuelType.Petrol,
                        Seats = 5,
                        Color = "Black",
                        DailyRate = 140m,
                        Location = "Bethlehem",
                        ImageUrl = "/images/cars/bmw-x5.jpg",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 8, 30)
                    }
                );
        }
    }
}