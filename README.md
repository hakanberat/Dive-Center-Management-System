# Dive Center Management System

**Developed: 2026**

A web-based scuba diving center management system built with **.NET 10, ASP.NET Core MVC, Dapper, and Microsoft SQL Server**.

The application is designed around real-world dive center workflows and manages reservations, staff, boats, dive sites, activities, equipment, financial transactions, reporting, and application users from a single web interface.

The project uses Dive Hub branding and was developed as a portfolio project based on practical scuba diving center operations.

## Features

- Reservation management
- Monthly reservation calendar
- Dive activity management
- Dive site management
- Staff management
- Operational role assignments
- Boat management
- Equipment management
- Financial transactions
- Transaction categories
- Multiple currencies
- Operational and financial reports
- User management
- Role-based authorization
- Secure password hashing
- Cookie authentication
- Dashboard statistics

## Dashboard

The dashboard provides a quick overview of dive center operations, including:

- Today's reservations
- Pending reservations
- Confirmed reservations
- Active staff
- Active boats
- Active equipment
- Today's reservation list
- Quick access to reservations and calendar

## Reservations

The reservation module is the core operational component of the system.

Reservations can include:

- Activity
- Dive site
- Boat
- Staff member
- Participant count
- Reservation date
- Start time
- Notes
- Status
- Unit price
- Currency
- Total amount

Supported statuses include:

- Pending
- Confirmed
- Completed
- Cancelled
- No Show

Reservation totals are calculated using:

Total Amount = Unit Price × Participant Count

## Historical Price Preservation

Activity prices may change over time.

To preserve historical reservation values, the application stores a price snapshot when a reservation is created.

Example:

Boat Dive
Unit Price: 80 EUR
Participants: 2
Total: 160 EUR

If the activity price later changes, the original reservation keeps its stored historical price.

## Resource Availability

The system includes operational rules that help prevent resource conflicts.

### Boats

- A boat cannot be assigned to multiple active reservations on the same day
- Boat capacity must support the reservation participant count
- Inactive boats cannot be assigned to new reservations

### Staff

- A staff member cannot be assigned to multiple active reservations on the same day
- Inactive staff members cannot be assigned to new reservations

Cancelled and No Show reservations release assigned resources.

## Calendar

The application includes a monthly reservation calendar.

Calendar entries can display:

- Activity
- Dive site
- Boat
- Staff
- Participant count
- Start time
- Reservation status

Reservations can be opened from the calendar for further management.

## Activities

Dive center services can be managed through the Activities module.

Examples include:

- Try Dive
- Shore Dive
- Boat Dive
- Night Dive
- Open Water Diver Course

Each activity can include:

- Name
- Price
- Currency
- Description
- Active / inactive status

## Dive Sites

Dive sites can include:

- Name
- Location
- Latitude
- Longitude
- Maximum depth
- Description
- Active / inactive status

## Staff

Staff records can include:

- First name
- Last name
- Phone
- Email
- Certification agency
- Instructor number
- Notes
- Active / inactive status

## Operational Roles

Staff members can have multiple operational roles.

Examples:

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

Operational Roles:
Instructor
Captain

System Role:
Admin

## Boats

Boat records can include:

- Name
- Registration number
- Capacity
- Notes
- Active / inactive status

Boat capacity is considered during reservation creation.

## Equipment

Equipment records can include:

- Name
- Category
- Serial number
- Brand
- Model
- Quantity
- Notes
- Active / inactive status

## Finance

The application includes basic financial management through:

- Transactions
- Transaction Categories
- Currencies

Transactions can be recorded as income or expenses.

Each transaction can include:

- Transaction date
- Category
- Amount
- Currency
- Description

## Multiple Currencies

The application supports independent currencies such as:

- TRY
- EUR
- USD
- GBP

Currencies are intentionally not automatically converted.

Financial totals remain grouped by currency.

## Reports

Reports can provide operational and financial summaries for a selected date range, including:

- Reservation count
- Participant count
- Reservation status breakdown
- Activity statistics
- Income totals by currency
- Expense totals by currency

## Authentication and Authorization

The application uses **ASP.NET Core Cookie Authentication**.

Passwords are hashed using ASP.NET Core `PasswordHasher` and are not stored as plain text.

The system supports:

### Admin

Administrators can access and manage the full application.

### User

Standard users have limited access focused on reservations and calendar operations.

## Session Security

Authentication includes additional session validation.

The application checks:

- User active status
- Current system role
- Security version

Important account changes can invalidate existing authentication sessions.

Examples include:

- User deactivation
- Role changes
- Password reset

The system also protects the final active administrator from being removed or demoted.

## Initial Administrator Setup

If no application user exists, the first administrator can be created through:

/Account/Setup

The password is hashed before being stored.

Additional users can then be managed by administrators.

## Soft Delete Strategy

Master data generally uses an `IsActive` status instead of permanent deletion.

This helps preserve historical references while preventing inactive records from being used in new operations.

Examples include:

- Staff
- Boats
- Activities
- Dive Sites
- Equipment
- Roles
- Currencies
- Transaction Categories
- Users

## Technology Stack

### Backend

- C#
- .NET 10
- ASP.NET Core MVC
- ASP.NET Core Cookie Authentication
- ASP.NET Core PasswordHasher

### Data Access

- Dapper
- Microsoft.Data.SqlClient
- Microsoft SQL Server

### Frontend

- Razor Views
- HTML
- CSS
- JavaScript
- Bootstrap

### Development

- Visual Studio Code
- PowerShell
- SQL Server Management Studio
- Git
- GitHub

## Architecture

The project uses a service-based MVC architecture:

Browser
   |
   v
ASP.NET Core MVC
   |
   v
Controllers
   |
   v
Services
   |
   v
Dapper
   |
   v
Microsoft SQL Server

The application intentionally uses **Dapper instead of Entity Framework Core** to work directly with SQL queries and database structures.

## Project Structure

DiveCenterManager
|
|-- Controllers
|-- Data
|-- Models
|-- Services
|-- ViewModels
|-- Views
|-- wwwroot
|-- Program.cs
|-- appsettings.json
`-- DiveCenterManager.csproj

## Database

The project uses Microsoft SQL Server.

Default local database name:

DiveCenterDb

The current development connection uses a local SQL Server Express instance with Windows authentication.

The database includes domain tables for areas such as:

- Activities
- AppUsers
- Boats
- Currencies
- DiveSites
- Equipment
- Reservations
- Roles
- Staff
- StaffRoles
- TransactionCategories
- Transactions

## Running the Application

Requirements:

- .NET 10 SDK
- Microsoft SQL Server
- SQL Server Management Studio or another SQL Server client
- Git

Clone the repository:

git clone https://github.com/hakanberat/DiveCenterManager.git

Enter the project directory:

cd DiveCenterManager

Restore packages:

dotnet restore

Build:

dotnet build

Run:

dotnet run

The application will display its local address in the terminal.

Example:

http://localhost:5098

The SQL Server database and required tables must be created before normal application use.

## Main Business Rules

The project implements business rules based on practical dive center operations.

### Reservations

- Participant count must be valid
- Reservation totals are calculated automatically
- Historical reservation prices are preserved
- Reservations use defined operational statuses

### Boats

- Same-day double booking is prevented
- Capacity must support participant count
- Inactive boats cannot be assigned

### Staff

- Same-day double booking is prevented
- Inactive staff cannot be assigned

### Finance

- Income and expenses use categories
- Currencies are stored independently
- Automatic currency conversion is not performed

### Users

- Usernames must be unique
- Passwords are hashed
- Inactive users cannot log in
- Role changes invalidate sessions
- Password resets invalidate sessions
- At least one active administrator must remain

## User Interface

The project includes a custom Dive Hub management interface with:

- Dive Hub branding
- Sidebar navigation
- Dashboard cards
- Role-based navigation
- Responsive Bootstrap components
- Dedicated login interface

## Development Status

The project is actively developed and already includes the core modules required for a functional dive center management system.

Possible future improvements include:

- REST API
- .NET MAUI mobile client
- Customer and diver profiles
- Certification records
- Equipment assignment to reservations
- Public online reservations
- Email and SMS notifications
- Advanced reporting
- Excel and PDF export
- Docker support
- Automated tests
- Cloud deployment
- Audit logs
- Multi-location support

## Purpose

This project demonstrates practical experience with:

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

The system is based on real scuba diving center operational requirements.

## Author

**Hakan Berat Demircan**
