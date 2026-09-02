using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CreatedAt", "DateOfBirth", "DriversLicenseNumber", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "Role" },
                values: new object[] { 1000, "DriveEase Headquarters", null, "Nablus", "Palestine", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ADMIN-001", "admin@gmail.com", "System", "Admin", "AQAAAAIAAYagAAAAEO+Hd+1TsOkqhR99zZq3UTpuJFvExJrwKjue7n/0TLN+yxYq2fxaHAAzx+FEQjaqiA==", "0000000000", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000);
        }
    }
}
