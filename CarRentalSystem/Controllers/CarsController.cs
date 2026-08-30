using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly CarRentalDbContext _context;

        public CarsController(CarRentalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CarSearchViewModel model)
        {
            if (model.PickupDate.HasValue &&
                model.ReturnDate.HasValue &&
                model.ReturnDate.Value <= model.PickupDate.Value)
            {
                ModelState.AddModelError(
                    nameof(model.ReturnDate),
                    "Return date must be after pickup date.");
            }

            var query = _context.Cars
                .AsNoTracking()
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                query = query.Where(c =>
                    c.Location == model.Location);
            }

            if (model.Type.HasValue)
            {
                query = query.Where(c =>
                    c.Type == model.Type.Value);
            }

            if (model.Transmission.HasValue)
            {
                query = query.Where(c =>
                    c.Transmission == model.Transmission.Value);
            }

            if (model.PickupDate.HasValue &&
                model.ReturnDate.HasValue &&
                ModelState.IsValid)
            {
                var pickupDate = model.PickupDate.Value;
                var returnDate = model.ReturnDate.Value;

                query = query.Where(car =>
                    !car.Reservations.Any(reservation =>
                        reservation.Status != ReservationStatus.Cancelled &&
                        reservation.PickupDate < returnDate &&
                        reservation.ReturnDate > pickupDate));
            }

            model.Cars = await query.ToListAsync();

            return View(model);
        }
    }
}