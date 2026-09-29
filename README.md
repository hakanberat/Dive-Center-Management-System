# Dive Center Manager

A web-based **Scuba Diving Center Management System** developed with **ASP.NET Core MVC, .NET 10, Dapper and SQL Server**.

The application is designed to manage the daily operations of a scuba diving center, including reservations, staff, boats, dive sites, equipment, financial transactions and user authorization.

The project uses **Dive Hub** branding and was developed as a portfolio project based on real-world scuba diving center workflows.

---

## Features

### Dashboard

- Today's reservations
- Pending reservations
- Confirmed reservations
- Active staff count
- Active boat count
- Active equipment count
- Today's operational overview

### Reservations

- Create and edit reservations
- Activity selection
- Dive site assignment
- Boat assignment
- Staff assignment
- Participant count
- Reservation status management
- Automatic price calculation
- Historical price snapshots

Supported reservation statuses:

- Pending
- Confirmed
- Completed
- Cancelled
- No Show

### Resource Availability

The system prevents operational conflicts.

- A boat can only be assigned to one active reservation per day.
- A staff member can only be assigned to one active reservation per day.
- Boat capacity must be sufficient for the participant count.
- Cancelled and No Show reservations do not block resources.
- Availability is checked both in the UI and on the server.

### Calendar

Monthly reservation calendar with:

- Reservation date
- Activity
- Dive site
- Boat
- Staff
- Participant count
- Reservation status

### Activities

Manage diving activities and their default prices.

Examples:

- Try Dive
- Shore Dive
- Boat Dive
- Night Dive
- Diving Courses

Each activity has its own currency and price.

Reservation prices are stored as snapshots, so changing an activity price does not modify historical reservations.

### Dive Sites

Manage dive locations with:

- Name
- Location
- Latitude
- Longitude
- Maximum depth
- Description
- Active / inactive status

### Staff

Manage diving center personnel.

Staff members can have multiple operational roles through a many-to-many relationship.

Example roles:

- Instructor
- Chief Instructor
- Divemaster
- Captain
- Life Guard
- Technician
- Owner
- Staff

### Boats

Manage boats with:

- Name
- Registration number
- Capacity
- Notes
- Active / inactive status

### Equipment

Manage diving center equipment with:

- Name
- Category
- Brand
- Model
- Serial number
- Quantity
- Notes
- Active / inactive status

### Finance

Financial management includes:

- Transactions
- Transaction categories
- Currencies
- Income records
- Expense records

Supported currencies can be managed independently.

The application intentionally does not perform automatic currency conversion.

### Reports

Reports include:

- Reservation count
- Participant count
- Reservation status breakdown
- Activity statistics
- Income totals by currency
- Expense totals by currency
- Custom date ranges

### User Management

Two system roles are supported:

#### Admin

Full access to:

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

#### User

Access to:

- Reservations
- Calendar

### Authentication & Security

The application uses ASP.NET Core cookie authentication.

Security features include:

- Secure password hashing
- Role-based authorization
- Active / inactive user accounts
- Password reset
- Session invalidation after password changes
- Session invalidation after role changes
- Session invalidation after account deactivation
- Protection against removing the last active Admin
- Duplicate username validation

Passwords are never stored as plain text.

---

## Technology Stack

- C#
- .NET 10
- ASP.NET Core MVC
- Razor Views
- Dapper
- Microsoft SQL Server
- Microsoft.Data.SqlClient
- ASP.NET Core Cookie Authentication
- ASP.NET Core PasswordHasher
- Bootstrap
- JavaScript
- HTML
- CSS

---

## Architecture

The project follows a simple layered structure:

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
SQL Server
