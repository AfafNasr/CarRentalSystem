using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarRentalSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "Color", "CreatedAt", "DailyRate", "FuelType", "ImageUrl", "IsActive", "Location", "Model", "Seats", "Transmission", "Type", "Year" },
                values: new object[,]
                {
                    { 1, "Toyota", "White", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 45m, 1, "/images/cars/toyota-corolla.jpg", true, "Nablus", "Corolla", 5, 1, 1, 2025 },
                    { 2, "Hyundai", "Black", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 50m, 1, "/images/cars/hyundai-elantra.jpg", true, "Ramallah", "Elantra", 5, 1, 1, 2025 },
                    { 3, "Kia", "Gray", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 70m, 3, "/images/cars/kia-sportage.jpg", true, "Nablus", "Sportage", 5, 1, 2, 2025 },
                    { 4, "Hyundai", "Blue", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 68m, 1, "/images/cars/hyundai-tucson.jpg", true, "Jenin", "Tucson", 5, 1, 2, 2024 },
                    { 5, "Tesla", "Red", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 95m, 4, "/images/cars/tesla-model-3.jpg", true, "Ramallah", "Model 3", 5, 1, 1, 2025 },
                    { 6, "BMW", "Black", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 140m, 1, "/images/cars/bmw-x5.jpg", true, "Bethlehem", "X5", 5, 1, 2, 2025 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
