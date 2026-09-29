# Dive Center Manager

A web-based **Scuba Diving Center Management System** developed with **ASP.NET Core MVC, .NET 10, Dapper, and Microsoft SQL Server**.

The application is designed to manage the daily operational and administrative processes of a scuba diving center, including reservations, staff, boats, dive sites, equipment, financial transactions, reporting, and user authorization.

The project uses **Dive Hub** branding and is based on real-world scuba diving center workflows.

---

## Features

### Dashboard

The dashboard provides a quick overview of daily dive center operations.

It includes:

- Today's reservations
- Pending reservations
- Confirmed reservations
- Active staff count
- Active boat count
- Active equipment count
- Today's reservation list
- Quick access to new reservations
- Quick access to the calendar

---

## Reservations

The reservation module is the core operational part of the system.

Users can:

- Create reservations
- Edit reservations
- Assign activities
- Select dive sites
- Assign boats
- Assign staff members
- Define participant count
- Set reservation date and start time
- Add notes
- Manage reservation status

Supported reservation statuses:

- Pending
- Confirmed
- Completed
- Cancelled
- No Show

Reservation prices are automatically calculated using:

```text
Total Amount = Unit Price × Participant Count
```

---

## Reservation Price Snapshots

Activity prices may change over time.

To preserve historical financial accuracy, the application stores the activity price and currency directly inside each reservation when the reservation is created.

For example:

```text
Boat Dive
Current price: 80 EUR

Reservation:
2 participants
Unit Price: 80 EUR
Total Amount: 160 EUR
```

If the activity price is later changed to:

```text
100 EUR
```

the historical reservation remains:

```text
Unit Price: 80 EUR
Total Amount: 160 EUR
```

New reservations use the new activity price.

---

## Resource Availability

The system prevents operational conflicts when assigning boats and staff.

### Boat Rules

- A boat can only be assigned to one active reservation per day.
- Boat capacity must be equal to or greater than the number of participants.
- Inactive boats cannot be assigned to new reservations.

### Staff Rules

- A staff member can only be assigned to one active reservation per day.
- Inactive staff members cannot be assigned to new reservations.

The following reservation statuses block resources:

- Pending
- Confirmed
- Completed

The following statuses release resources:

- Cancelled
- No Show

Availability is checked both in the user interface and on the server side.

---

## Calendar

The application includes a monthly reservation calendar.

Reservations are displayed according to their reservation dates and can include:

- Activity
- Dive site
- Boat
- Staff
- Participant count
- Start time
- Reservation status

Reservations can be opened directly from the calendar for editing.

---

## Activities

The Activities module manages the services offered by the dive center.

Example activities:

- Try Dive
- Shore Dive
- Boat Dive
- Night Dive
- Open Water Diver Course

Each activity includes:

- Name
- Default price
- Currency
- Description
- Active / inactive status

Only active activities with active currencies can be selected for new reservations.

---

## Dive Sites

The Dive Sites module manages diving locations.

Each dive site can contain:

- Name
- Location
- Latitude
- Longitude
- Maximum depth
- Description
- Active / inactive status

Example dive sites may include:

- Palm Beach
- Tomofil
- Ray Cave
- Kocareis

---

## Staff

The Staff module manages diving center personnel.

Each staff member can contain:

- First name
- Last name
- Phone
- Email
- Certification agency
- Instructor number
- Notes
- Active / inactive status

---

## Operational Roles

Staff members can have multiple operational roles through a many-to-many relationship.

Available operational roles may include:

- Instructor
- Chief Instructor
- Divemaster
- Captain
- Life Guard
- Technician
- Owner
- Staff

Operational roles are separate from application authorization roles.

For example:

```text
Operational Role:
Instructor
Captain
Divemaster

System Role:
Admin
User
```

---

## Boats

The Boats module manages dive center boats.

Each boat can contain:

- Name
- Registration number
- Capacity
- Notes
- Active / inactive status

Boat capacity is automatically considered when creating reservations.

---

## Equipment

The Equipment module manages dive center equipment.

Equipment records can include:

- Name
- Category
- Serial number
- Brand
- Model
- Quantity
- Notes
- Active / inactive status

---

## Finance

The application includes basic financial management.

The finance section contains:

- Transactions
- Transaction Categories
- Currencies

---

## Transactions

Transactions can be recorded as income or expenses.

Each transaction contains:

- Transaction date
- Category
- Amount
- Currency
- Description

Transactions can be:

- Created
- Edited
- Deleted

---

## Transaction Categories

Transaction categories define whether a transaction represents income or an expense.

Example categories:

```text
Dive Income
Course Income
Fuel
Maintenance
```

Each category has one of the following types:

```text
Income
Expense
```

---

## Currencies

The application supports multiple currencies.

Example currencies:

- TRY
- EUR
- USD
- GBP

Currencies are managed independently.

The application intentionally does **not** automatically convert currencies.

For example:

```text
100 EUR
5000 TRY
200 USD
```

remain separate financial values.

---

## Reports

The Reports module provides operational and financial summaries for a selected date range.

Reports include:

- Reservation count
- Participant count
- Reservation status breakdown
- Activity statistics
- Income totals by currency
- Expense totals by currency

Because currencies are not automatically converted, financial totals remain grouped by currency.

---

## User Management

The system contains its own application user management system.

Application users are stored in the:

```text
AppUsers
```

table.

Two system roles are supported:

### Admin

Administrators have access to:

- Dashboard
- Reservations
- Calendar
- Reports
- Activities
- Dive Sites
- Staff
- Roles
- Boats
- Equipment
- Users
- Transactions
- Transaction Categories
- Currencies

### User

Standard users have access to:

- Reservations
- Calendar

---

## Authentication

The application uses **ASP.NET Core Cookie Authentication**.

Users log in using:

- Username
- Password

Passwords are never stored in plain text.

ASP.NET Core `PasswordHasher` is used to securely generate password hashes.

---

## Authentication Security

The authentication system includes several security mechanisms.

### Password Hashing

Passwords are stored only as hashes.

The original password cannot be retrieved from the database.

### Active / Inactive Users

Administrators can activate or deactivate user accounts.

Inactive users cannot log in.

If an already logged-in user is deactivated, their existing session is invalidated.

### Role Changes

If an administrator changes another user's system role, the user's existing authentication session becomes invalid.

The user must log in again to receive the new authorization permissions.

### Password Reset

Administrators can reset user passwords.

The existing password is never displayed.

After a password reset:

```text
SecurityVersion
```

is incremented.

Existing login sessions become invalid and the user must log in again using the new password.

### Security Version

Authentication cookies contain the user's current security version.

Example:

```text
Cookie SecurityVersion: 1
Database SecurityVersion: 2
```

If these values do not match, the authentication cookie is rejected.

This allows the application to invalidate old login sessions after important account changes.

### Last Administrator Protection

The system prevents the final active administrator from being:

- Deactivated
- Changed from Admin to User

This ensures that the application cannot accidentally be left without an administrator.

### Duplicate Username Protection

Usernames must be unique.

The application validates duplicate usernames during both:

- User creation
- User editing

---

## Initial Administrator Setup

When the database contains no application users, the first administrator can be created from:

```text
/Account/Setup
```

The administrator provides:

- Username
- Display name
- Password
- Password confirmation

The password is hashed before being stored in the database.

Once the first user exists, the initial setup page redirects to the login page.

Additional users are managed by administrators through:

```text
Management → Users
```

---

## User Administration

Administrators can:

- Create users
- Edit usernames
- Edit display names
- Change system roles
- Reset passwords
- Activate users
- Deactivate users

The Users page displays:

- Username
- Display name
- Role
- Status
- Creation date
- Actions

---

## Technology Stack

### Backend

- C#
- .NET 10
- ASP.NET Core MVC
- ASP.NET Core Cookie Authentication
- ASP.NET Core PasswordHasher

### Database

- Microsoft SQL Server
- Dapper
- Microsoft.Data.SqlClient

### Frontend

- Razor Views
- HTML
- CSS
- JavaScript
- Bootstrap

### Development Tools

- Visual Studio Code
- PowerShell
- SQL Server Management Studio
- Git
- GitHub

---

## Architecture

The application uses a simple service-based architecture:

```text
Browser
   ↓
ASP.NET Core MVC
   ↓
Controllers
   ↓
Services
   ↓
Dapper
   ↓
Microsoft SQL Server
```

The application intentionally uses **Dapper instead of Entity Framework Core**.

This allows the project to work directly with SQL queries and database structures.

---

## Project Structure

```text
DiveCenterManager
│
├── Controllers
│
│   ├── AccountController.cs
│   ├── ActivitiesController.cs
│   ├── BoatsController.cs
│   ├── CalendarController.cs
│   ├── CurrenciesController.cs
│   ├── DashboardController.cs
│   ├── DiveSitesController.cs
│   ├── EquipmentController.cs
│   ├── ReportsController.cs
│   ├── ReservationsController.cs
│   ├── RolesController.cs
│   ├── StaffController.cs
│   ├── TransactionCategoriesController.cs
│   ├── TransactionsController.cs
│   └── UsersController.cs
│
├── Data
│   └── SqlConnectionFactory.cs
│
├── Models
│
├── Services
│
├── ViewModels
│
├── Views
│
├── wwwroot
│   ├── css
│   ├── images
│   ├── js
│   └── lib
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── DiveCenterManager.csproj
```

---

## Database

The project uses:

```text
Microsoft SQL Server
```

Default database name:

```text
DiveCenterDb
```

Development connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=DiveCenterDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

The project uses Windows authentication for the local SQL Server connection.

---

## Database Structure

The database currently contains tables including:

```text
Activities
AppUsers
Boats
Currencies
DiveSites
Equipment
Reservations
Roles
Staff
StaffRoles
TransactionCategories
Transactions
```

---

## Running the Application

### Requirements

Install:

- .NET 10 SDK
- Microsoft SQL Server
- SQL Server Management Studio or another SQL Server client
- Git

---

### Clone the Repository

```bash
git clone https://github.com/hakanberat/DiveCenterManager.git
```

---

### Enter the Project Directory

```bash
cd DiveCenterManager
```

---

### Restore Dependencies

```bash
dotnet restore
```

---

### Build the Application

```bash
dotnet build
```

---

### Run the Application

```bash
dotnet run
```

ASP.NET Core will display the local application address in the terminal.

Example:

```text
http://localhost:5098
```

---

## First Run

Before running the application, create the SQL Server database and required tables.

The project will include a database setup script under:

```text
Database/DiveCenterDb.sql
```

After the database has been created, run:

```bash
dotnet run
```

Then open:

```text
/Account/Setup
```

to create the first administrator account.

---

## Main Business Rules

The application implements several real-world scuba diving center business rules.

### Reservations

- Participant count must be greater than zero.
- Reservation prices are calculated automatically.
- Historical prices are preserved.
- Reservations have defined operational statuses.

### Boats

- Boats cannot be double-booked on the same day.
- Boat capacity must support the reservation participant count.
- Inactive boats cannot be assigned to new reservations.

### Staff

- Staff members cannot be double-booked on the same day.
- Inactive staff members cannot be assigned to new reservations.

### Reservation Status

The following statuses block operational resources:

```text
Pending
Confirmed
Completed
```

The following statuses release operational resources:

```text
Cancelled
No Show
```

### Finance

- Income and expense transactions are stored separately by category.
- Currency conversion is not automatically performed.
- Historical transaction currency values remain unchanged.

### Users

- Usernames must be unique.
- Passwords are securely hashed.
- Inactive users cannot log in.
- Role changes invalidate existing sessions.
- Password resets invalidate existing sessions.
- At least one active Admin must always remain.

---

## User Interface

The project includes a custom **Dive Hub Management System** interface.

The interface contains:

- Dive Hub branding
- Dive Hub logo
- Dark blue sidebar navigation
- Turquoise accent colors
- Dashboard cards
- Role-based navigation
- Responsive Bootstrap components
- Dedicated login interface

The visual identity is inspired by the Dive Hub brand.

---

## Navigation

### Admin Navigation

```text
Main
├── Dashboard
├── Reservations
├── Calendar
└── Reports

Management
├── Activities
├── Dive Sites
├── Staff
├── Roles
├── Boats
├── Equipment
└── Users

Finance
├── Transactions
├── Transaction Categories
└── Currencies
```

### User Navigation

```text
Reservations
Calendar
```

---

## Reservation Workflow

Typical reservation workflow:

```text
New Reservation
      ↓
Pending
      ↓
Confirmed
      ↓
Completed
```

Reservations may also become:

```text
Cancelled
No Show
```

---

## Example Operational Flow

A typical scuba diving center workflow supported by the application:

```text
Customer requests a dive
        ↓
Reservation created
        ↓
Activity selected
        ↓
Participant count entered
        ↓
Available boat selected
        ↓
Available instructor selected
        ↓
Dive site assigned
        ↓
Reservation confirmed
        ↓
Dive completed
        ↓
Reservation marked Completed
```

---

## Soft Delete Strategy

Master data is generally not permanently deleted.

Instead, records use:

```text
IsActive
```

This allows historical records to remain valid even if a resource is no longer available.

Examples:

- Staff
- Boats
- Activities
- Dive Sites
- Equipment
- Roles
- Currencies
- Transaction Categories
- Users

Financial transactions may be permanently deleted when entered incorrectly.

---

## Future Improvements

Potential future improvements include:

- REST API
- .NET MAUI mobile client
- Customer and diver profiles
- Diver certification records
- Equipment assignment to reservations
- Online public reservation system
- Email notifications
- SMS notifications
- Advanced financial reporting
- Excel export
- PDF reporting
- Docker support
- Automated tests
- Cloud deployment
- Audit logs
- Multi-location support

---

## Development Status

The project is actively being developed and improved.

The current version includes the core operational modules required for a functional scuba diving center management system.

---

## Purpose

This project was developed as a portfolio project demonstrating practical experience with:

- ASP.NET Core MVC
- C#
- SQL Server
- Dapper
- Authentication
- Authorization
- Relational database design
- Business rules
- Role-based access control
- Reservation systems
- Financial records
- Responsive web interfaces
- Git and GitHub

The system is based on real operational requirements encountered in scuba diving center management.

---

## Author

**Hakan Berat Demircan**

GitHub:

https://github.com/hakanberat

Repository:

https://github.com/hakanberat/DiveCenterManager

---

## License

This project is currently intended for educational and portfolio purposes.
