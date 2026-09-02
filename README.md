# 🚗 DriveEase - Car Rental System

DriveEase is a web-based Car Rental System built using ASP.NET Core MVC.

The application allows customers to create accounts, securely sign in, search for available cars, create and manage reservations, update their profiles, and recover forgotten passwords through email.

The system also includes an Admin Dashboard for managing cars, customers, and reservations.

---

## 🛠️ Technologies Used

The project was built using:

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server**
- **Razor Views**
- **Cookie Authentication**
- **MailKit / SMTP**

---

## ✨ Features

### 👤 Customer Features

- User registration with form validation
- Unique email validation
- Password hashing
- Secure sign in and sign out
- Cookie-based authentication
- Forgot password via email
- View available cars
- Search cars by:
  - Location
  - Pickup date
  - Return date
  - Car type
  - Transmission type
- View car details and rental price
- Create new reservations
- View reservation history
- Edit existing reservations
- Cancel reservations
- Update profile information
- Change account password

### 🛡️ Admin Features

- Separate Admin Dashboard
- View and manage all reservations
- Approve pending reservations
- Cancel reservations
- View all cars
- Add new cars
- Edit car information
- Activate or disable cars
- View all customers
- Add new customers
- Activate or disable customer accounts

---

## 📁 Project Structure

```text
CarRentalSystem/
│
├── Controllers/                 # Handles HTTP requests and application flow
│   ├── AccountController.cs     # Registration, login, profile and password recovery
│   ├── CarsController.cs        # Car listing, searching and details
│   ├── ReservationsController.cs # Customer reservation operations
│   └── AdminController.cs       # Admin management operations
│
├── Models/                      # Core domain entities and enums
│   ├── User.cs
│   ├── Car.cs
│   ├── Reservation.cs
│   └── PasswordResetToken.cs
│
├── ViewModels/                  # Models designed specifically for UI/forms
│
├── Views/                       # Razor views displayed to the user
│   ├── Account/                 # Authentication and account pages
│   ├── Cars/                    # Car listing and details pages
│   ├── Reservations/            # Customer reservation pages
│   ├── Admin/                   # Admin dashboard and management pages
│   └── Shared/                  # Shared Razor components/layouts
│
├── Data/
│   └── CarRentalDbContext.cs    # EF Core database context
│
├── Migrations/                  # Entity Framework Core database migrations
│
├── Services/                    # Application services
│   ├── IEmailService.cs         # Email service abstraction
│   └── EmailService.cs          # SMTP email implementation
│
├── Settings/
│   └── EmailSettings.cs         # SMTP configuration model
│
├── wwwroot/                     # Static files
│   ├── css/                     # Application stylesheets
│   ├── js/                      # Client-side JavaScript
│   ├── images/                  # Images and car assets
│   └── lib/                     # Client-side libraries
│
├── appsettings.json             # Application configuration
├── Program.cs                   # Application startup, middleware and DI configuration
└── CarRentalSystem.csproj       # Project configuration and dependencies
```

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/AfafNasr/CarRentalSystem
```

Navigate to the project:

```bash
cd CarRentalSystem
```

---

### 2. Restore Dependencies

```bash
dotnet restore
```

---

### 3. Configure the Database

Update the connection string inside `appsettings.json` if necessary:

```json
"ConnectionStrings": {
  "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
}
```

Then apply the Entity Framework Core migrations:

```bash
dotnet ef database update
```

---

### 4. Configure Email Service

Password recovery uses Gmail SMTP through MailKit.

Sensitive credentials should be configured using .NET User Secrets.

Initialize User Secrets if necessary:

```bash
dotnet user-secrets init
```

Set the sender email:

```bash
dotnet user-secrets set "EmailSettings:Username" "YOUR_EMAIL@gmail.com"
```

```bash
dotnet user-secrets set "EmailSettings:FromEmail" "YOUR_EMAIL@gmail.com"
```

Set the Google App Password:

```bash
dotnet user-secrets set "EmailSettings:Password" "YOUR_GOOGLE_APP_PASSWORD"
```

> Do not use your regular Gmail password. Use a Google App Password for SMTP authentication.

---

### 5. Run the Application

```bash
dotnet run
```

Open the local URL displayed in the terminal, for example:

```text
https://localhost:xxxx
```

You can then register a new customer account and start using DriveEase.

---

## 📌 Reservation Workflow

```text
Customer searches for available cars
                ↓
        Selects a vehicle
                ↓
       Chooses rental dates
                ↓
   System checks availability
                ↓
       Creates reservation
                ↓
         Pending status
                ↓
        Admin reviews it
                ↓
     Confirmed / Cancelled
```

Customers can later view their reservation history, modify eligible reservations, or cancel them.

---
