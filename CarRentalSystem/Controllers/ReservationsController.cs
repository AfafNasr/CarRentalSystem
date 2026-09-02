using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRentalSystem.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly CarRentalDbContext _context;

        public ReservationsController(CarRentalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            int carId,
            DateTime? pickupDate,
            DateTime? returnDate)
        {
            var car = await _context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.Id == carId &&
                    c.IsActive);

            if (car == null)
            {
                return NotFound();
            }

            var model = new CreateReservationViewModel
            {
                CarId = car.Id,
                Car = car,

                PickupDate = pickupDate ?? DateTime.Today,
                ReturnDate = returnDate ?? DateTime.Today.AddDays(1)
            };

            CalculatePrice(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    CreateReservationViewModel model)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c =>
                    c.Id == model.CarId &&
                    c.IsActive);

            if (car == null)
            {
                return NotFound();
            }

            model.Car = car;

            if (model.PickupDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.PickupDate),
                    "Pickup date cannot be in the past.");
            }

            if (model.ReturnDate.Date <= model.PickupDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.ReturnDate),
                    "Return date must be after pickup date.");
            }

            if (!ModelState.IsValid)
            {
                CalculatePrice(model);
                return View(model);
            }

            var isUnavailable = await _context.Reservations
                .AnyAsync(r =>
                    r.CarId == model.CarId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.PickupDate < model.ReturnDate &&
                    r.ReturnDate > model.PickupDate);

            if (isUnavailable)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This car is already reserved for the selected dates.");

                CalculatePrice(model);

                return View(model);
            }

            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var rentalDays =
                (model.ReturnDate.Date - model.PickupDate.Date).Days;

            var totalPrice =
                rentalDays * car.DailyRate;

            var reservation = new Reservation
            {
                UserId = userId,
                CarId = car.Id,
                PickupDate = model.PickupDate.Date,
                ReturnDate = model.ReturnDate.Date,
                TotalPrice = totalPrice,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Confirmation),
                new { id = reservation.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        private static void CalculatePrice(
            CreateReservationViewModel model)
        {
            if (model.ReturnDate <= model.PickupDate)
            {
                model.RentalDays = 0;
                model.TotalPrice = 0;

                return;
            }

            model.RentalDays =
                (model.ReturnDate.Date - model.PickupDate.Date).Days;

            model.TotalPrice =
                model.RentalDays * model.Car.DailyRate;
        }

        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var reservations = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdClaim =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == ReservationStatus.Cancelled ||
                reservation.Status == ReservationStatus.Completed)
            {
                return BadRequest();
            }


            var rentalDays =
                (reservation.ReturnDate.Date -
                 reservation.PickupDate.Date).Days;

            var viewModel = new EditReservationViewModel
            {
                ReservationId = reservation.Id,

                CarName =
                    reservation.Car.Brand + " " +
                    reservation.Car.Model,

                DailyRate =
                    reservation.Car.DailyRate,

                PickupDate =
                    reservation.PickupDate,

                ReturnDate =
                    reservation.ReturnDate,

                RentalDays =
                    rentalDays,

                TotalPrice =
                    reservation.TotalPrice
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    EditReservationViewModel viewModel)
        {
            var userIdClaim =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r =>
                    r.Id == viewModel.ReservationId &&
                    r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == ReservationStatus.Cancelled ||
                reservation.Status == ReservationStatus.Completed)
            {
                return BadRequest();
            }


            if (viewModel.PickupDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(viewModel.PickupDate),
                    "Pickup date cannot be in the past.");
            }

            if (viewModel.ReturnDate.Date <=
                viewModel.PickupDate.Date)
            {
                ModelState.AddModelError(
                    nameof(viewModel.ReturnDate),
                    "Return date must be after pickup date.");
            }

            if (ModelState.IsValid)
            {
                var hasOverlap =
                    await _context.Reservations
                        .AnyAsync(r =>
                            r.CarId == reservation.CarId &&
                            r.Id != reservation.Id &&
                            r.Status != ReservationStatus.Cancelled &&
                            r.PickupDate < viewModel.ReturnDate &&
                            r.ReturnDate > viewModel.PickupDate);

                if (hasOverlap)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The car is already reserved during the selected dates.");
                }
            }

            if (!ModelState.IsValid)
            {
                viewModel.CarName =
                    reservation.Car.Brand + " " +
                    reservation.Car.Model;

                viewModel.DailyRate =
                    reservation.Car.DailyRate;

                return View(viewModel);
            }

            var rentalDays =
                (viewModel.ReturnDate.Date -
                 viewModel.PickupDate.Date).Days;

            reservation.PickupDate =
                viewModel.PickupDate.Date;

            reservation.ReturnDate =
                viewModel.ReturnDate.Date;

            reservation.TotalPrice =
                rentalDays * reservation.Car.DailyRate;

            await _context.SaveChangesAsync();

            TempData["ReservationSuccess"] =
                "Reservation updated successfully.";

            return RedirectToAction(nameof(MyReservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdClaim =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == ReservationStatus.Cancelled ||
                reservation.Status == ReservationStatus.Completed)
            {
                return BadRequest();
            }

            reservation.Status =
                ReservationStatus.Cancelled;

            await _context.SaveChangesAsync();

            TempData["ReservationSuccess"] =
                "Reservation cancelled successfully.";

            return RedirectToAction(nameof(MyReservations));
        }
    }
}