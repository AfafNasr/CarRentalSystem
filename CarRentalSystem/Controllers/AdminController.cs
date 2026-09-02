using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CarRentalDbContext _context;

        public AdminController(CarRentalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalCars = await _context.Cars.CountAsync(),

                ActiveCars = await _context.Cars
                    .CountAsync(c => c.IsActive),

                TotalReservations = await _context.Reservations
                    .CountAsync(),

                PendingReservations = await _context.Reservations
                    .CountAsync(r =>
                        r.Status == ReservationStatus.Pending),

                TotalCustomers = await _context.Users
                    .CountAsync(u =>
                        u.Role == UserRole.Customer)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Reservations()
        {
            var reservations = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Car)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminReservationViewModel
                {
                    ReservationId = r.Id,

                    CustomerName =
                        r.User.FirstName + " " + r.User.LastName,

                    CustomerEmail = r.User.Email,

                    CarName =
                        r.Car.Brand + " " + r.Car.Model,

                    PickupDate = r.PickupDate,

                    ReturnDate = r.ReturnDate,

                    TotalPrice = r.TotalPrice,

                    Status = r.Status
                })
                .ToListAsync();

            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status != ReservationStatus.Pending)
            {
                return BadRequest();
            }

            reservation.Status =
                ReservationStatus.Confirmed;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Reservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == ReservationStatus.Completed)
            {
                return BadRequest();
            }

            reservation.Status =
                ReservationStatus.Cancelled;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Reservations));
        }

        [HttpGet]
        public async Task<IActionResult> Cars()
        {
            var cars = await _context.Cars
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(cars);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCarStatus(int id)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            car.IsActive = !car.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Cars));
        }

        [HttpGet]
        public IActionResult CreateCar()
        {
            return View(new CarFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(
    CarFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string? imageUrl = null;

            if (viewModel.ImageFile != null &&
                viewModel.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "cars");

                Directory.CreateDirectory(uploadsFolder);

                var extension =
                    Path.GetExtension(viewModel.ImageFile.FileName);

                var fileName =
                    $"{Guid.NewGuid()}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await viewModel.ImageFile
                    .CopyToAsync(stream);

                imageUrl =
                    $"/images/cars/{fileName}";
            }

            var car = new Car
            {
                Brand = viewModel.Brand.Trim(),
                Model = viewModel.Model.Trim(),
                Year = viewModel.Year,
                Type = viewModel.Type,
                Transmission = viewModel.Transmission,
                FuelType = viewModel.FuelType,
                Seats = viewModel.Seats,
                Color = viewModel.Color.Trim(),
                DailyRate = viewModel.DailyRate,
                Location = viewModel.Location.Trim(),
                ImageUrl = imageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cars.Add(car);

            await _context.SaveChangesAsync();

            TempData["CarSuccess"] =
                "Car added successfully.";

            return RedirectToAction(nameof(Cars));
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            var car = await _context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var model = new CarFormViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Type = car.Type,
                Transmission = car.Transmission,
                FuelType = car.FuelType,
                Seats = car.Seats,
                Color = car.Color,
                DailyRate = car.DailyRate,
                Location = car.Location,
                ImageUrl = car.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(CarFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (!viewModel.Id.HasValue)
            {
                return BadRequest();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(c =>
                    c.Id == viewModel.Id.Value);

            if (car == null)
            {
                return NotFound();
            }

            car.Brand = viewModel.Brand.Trim();
            car.Model = viewModel.Model.Trim();
            car.Year = viewModel.Year;
            car.Type = viewModel.Type;
            car.Transmission = viewModel.Transmission;
            car.FuelType = viewModel.FuelType;
            car.Seats = viewModel.Seats;
            car.Color = viewModel.Color.Trim();
            car.DailyRate = viewModel.DailyRate;
            car.Location = viewModel.Location.Trim();

            if (viewModel.ImageFile != null &&
                viewModel.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "cars");

                Directory.CreateDirectory(uploadsFolder);

                var extension =
                    Path.GetExtension(viewModel.ImageFile.FileName);

                var fileName =
                    $"{Guid.NewGuid()}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await viewModel.ImageFile
                    .CopyToAsync(stream);

                car.ImageUrl =
                    $"/images/cars/{fileName}";
            }

            await _context.SaveChangesAsync();

            TempData["CarSuccess"] =
                "Car updated successfully.";

            return RedirectToAction(nameof(Cars));
        }

        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            var customers = await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Customer)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminCustomerViewModel
                {
                    Id = u.Id,

                    FullName =
                        u.FirstName + " " + u.LastName,

                    Email = u.Email,

                    PhoneNumber = u.PhoneNumber,

                    City = u.City,

                    Country = u.Country,

                    DriversLicenseNumber =
                        u.DriversLicenseNumber,

                    ReservationsCount =
                        u.Reservations.Count,

                    CreatedAt = u.CreatedAt,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return View(customers);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View(new AdminCustomerFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(
            AdminCustomerFormViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Password))
            {
                ModelState.AddModelError(
                    nameof(viewModel.Password),
                    "Password is required.");
            }

            var normalizedEmail =
                viewModel.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(viewModel.Email),
                    "An account with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var customer = new User
            {
                FirstName = viewModel.FirstName.Trim(),
                LastName = viewModel.LastName.Trim(),
                Email = normalizedEmail,
                PhoneNumber = viewModel.PhoneNumber.Trim(),
                DateOfBirth = viewModel.DateOfBirth,
                AddressLine1 = viewModel.AddressLine1.Trim(),
                AddressLine2 = viewModel.AddressLine2?.Trim(),
                City = viewModel.City.Trim(),
                Country = viewModel.Country.Trim(),
                DriversLicenseNumber =
                    viewModel.DriversLicenseNumber.Trim(),

                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

            customer.PasswordHash =
                passwordHasher.HashPassword(
                    customer,
                    viewModel.Password!);

            _context.Users.Add(customer);

            await _context.SaveChangesAsync();

            TempData["CustomerSuccess"] =
                "Customer created successfully.";

            return RedirectToAction(nameof(Customers));
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Id == id &&
                    u.Role == UserRole.Customer);

            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new AdminCustomerFormViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth,
                AddressLine1 = customer.AddressLine1,
                AddressLine2 = customer.AddressLine2,
                City = customer.City,
                Country = customer.Country,
                DriversLicenseNumber =
                    customer.DriversLicenseNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(
            AdminCustomerFormViewModel viewModel)
        {
            if (!viewModel.Id.HasValue)
            {
                return BadRequest();
            }

            var customer = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == viewModel.Id.Value &&
                    u.Role == UserRole.Customer);

            if (customer == null)
            {
                return NotFound();
            }

            var normalizedEmail =
                viewModel.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == normalizedEmail &&
                    u.Id != customer.Id);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(viewModel.Email),
                    "An account with this email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            customer.FirstName = viewModel.FirstName.Trim();
            customer.LastName = viewModel.LastName.Trim();
            customer.Email = normalizedEmail;
            customer.PhoneNumber = viewModel.PhoneNumber.Trim();
            customer.DateOfBirth = viewModel.DateOfBirth;
            customer.AddressLine1 = viewModel.AddressLine1.Trim();
            customer.AddressLine2 = viewModel.AddressLine2?.Trim();
            customer.City = viewModel.City.Trim();
            customer.Country = viewModel.Country.Trim();
            customer.DriversLicenseNumber =
                viewModel.DriversLicenseNumber.Trim();

            if (!string.IsNullOrWhiteSpace(viewModel.Password))
            {
                var passwordHasher =
                    new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

                customer.PasswordHash =
                    passwordHasher.HashPassword(
                        customer,
                        viewModel.Password);
            }

            await _context.SaveChangesAsync();

            TempData["CustomerSuccess"] =
                "Customer updated successfully.";

            return RedirectToAction(nameof(Customers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == id &&
                    u.Role == UserRole.Customer);

            if (customer == null)
            {
                return NotFound();
            }

            customer.IsActive = !customer.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Customers));
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation(int userId)
        {
            var customer = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.Role == UserRole.Customer);

            if (customer == null)
            {
                return NotFound();
            }

            var cars = await _context.Cars
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new AdminCarOptionViewModel
                {
                    Id = c.Id,

                    Name =
                        c.Brand + " " +
                        c.Model + " (" +
                        c.Year + ")",

                    Location = c.Location,

                    DailyRate = c.DailyRate
                })
                .ToListAsync();


            var viewModel =
                new AdminCreateReservationViewModel
                {
                    UserId = customer.Id,

                    CustomerName =
                        customer.FirstName + " " +
                        customer.LastName,

                    CustomerEmail = customer.Email,

                    PickupDate =
                        DateTime.Today.AddDays(1),

                    ReturnDate =
                        DateTime.Today.AddDays(2),

                    Cars = cars
                };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation(
    AdminCreateReservationViewModel viewModel)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == viewModel.UserId &&
                    u.Role == UserRole.Customer);

            if (customer == null)
            {
                return NotFound();
            }


            if (!customer.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cannot create a reservation for a disabled customer.");
            }


            var car = await _context.Cars
                .FirstOrDefaultAsync(c =>
                    c.Id == viewModel.CarId &&
                    c.IsActive);


            if (car == null)
            {
                ModelState.AddModelError(
                    nameof(viewModel.CarId),
                    "The selected car is not available.");
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


            if (car != null &&
                viewModel.ReturnDate.Date >
                viewModel.PickupDate.Date)
            {
                var hasOverlap =
                    await _context.Reservations
                        .AnyAsync(r =>
                            r.CarId == car.Id &&
                            r.Status != ReservationStatus.Cancelled &&
                            r.PickupDate < viewModel.ReturnDate &&
                            r.ReturnDate > viewModel.PickupDate);

                if (hasOverlap)
                {
                    ModelState.AddModelError(
                        nameof(viewModel.CarId),
                        "This car is already reserved during the selected dates.");
                }
            }


            if (!ModelState.IsValid)
            {
                viewModel.CustomerName =
                    customer.FirstName + " " +
                    customer.LastName;

                viewModel.CustomerEmail =
                    customer.Email;


                viewModel.Cars =
                    await _context.Cars
                        .AsNoTracking()
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.Brand)
                        .ThenBy(c => c.Model)
                        .Select(c =>
                            new AdminCarOptionViewModel
                            {
                                Id = c.Id,

                                Name =
                                    c.Brand + " " +
                                    c.Model + " (" +
                                    c.Year + ")",

                                Location =
                                    c.Location,

                                DailyRate =
                                    c.DailyRate
                            })
                        .ToListAsync();


                return View(viewModel);
            }


            var rentalDays =
                (viewModel.ReturnDate.Date -
                 viewModel.PickupDate.Date).Days;


            var totalPrice =
                rentalDays * car!.DailyRate;


            var reservation = new Reservation
            {
                UserId = customer.Id,

                CarId = car.Id,

                PickupDate =
                    viewModel.PickupDate.Date,

                ReturnDate =
                    viewModel.ReturnDate.Date,

                TotalPrice =
                    totalPrice,

                Status =
                    ReservationStatus.Confirmed,

                CreatedAt =
                    DateTime.UtcNow
            };


            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();


            TempData["ReservationSuccess"] =
                $"Reservation created successfully for {customer.FirstName} {customer.LastName}.";


            return RedirectToAction(nameof(Reservations));
        }

    }
}