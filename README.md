
# SCM ProcureTrack
## Supply Chain Procurement Management System

### 1. Project Overview

SCM ProcureTrack is a web-based Supply Chain Procurement Management System developed using ASP.NET Core MVC, C#, Entity Framework Core and SQL Server.

The application manages the procurement lifecycle, starting from vendor and material management to purchase order creation, goods receipt processing and inventory tracking.

The main objective is to improve procurement operations, maintain accurate inventory records and provide visibility into purchase order status.

### 2. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Backend programming |
| ASP.NET Core MVC | Web application framework |
| Entity Framework Core | ORM and database operations |
| SQL Server | Relational database |
| HTML5 | Page structure |
| CSS3 | Styling |
| Bootstrap 5 | Responsive UI |
| JavaScript | Client-side functionality |
| LINQ | Data querying |
| ASP.NET Core Identity | Authentication and authorization |
| xUnit | Unit testing |
| Visual Studio Code | Development environment |

### 3. Architecture

The application follows the ASP.NET Core MVC architecture.

**Presentation Layer**
- ASP.NET Core MVC Controllers
- Razor Views
- Bootstrap UI

**Business Logic**
- Purchase order calculations
- Goods receipt validation
- Procurement business rules
- Stock update and PO status management

**Data Access Layer**
- Entity Framework Core
- ApplicationDbContext
- LINQ queries

**Database Layer**
- SQL Server
- Business tables
- ASP.NET Core Identity tables

### 4. Application Modules

#### 4.1 Vendor Master
- Create new vendors.
- View vendor details.
- Edit vendor information.
- Activate and deactivate vendors.
- Prevent vendor deactivation when open purchase orders exist.
- Validate duplicate vendor codes.

#### 4.2 Item / Material Master
- Create and manage materials.
- Maintain item code and item name.
- Maintain category and unit of measure.
- Configure reorder levels.
- Track current stock.

#### 4.3 Purchase Order Management
- Create purchase orders.
- Generate automatic PO numbers.
- Select vendors and materials.
- Support multiple items in a purchase order.
- Calculate subtotal, tax and grand total.
- Approve and cancel eligible purchase orders.
- Maintain purchase order status.
- Prevent editing after approval.

Purchase Order Lifecycle:

Draft → Approved → Partially Received → Completed

Cancelled is also supported as a separate status.

#### 4.4 Goods Receipt Note (GRN)
- Create GRN against an approved purchase order.
- Record received quantity.
- Record accepted quantity.
- Record rejected quantity.
- Validate cumulative accepted quantity against ordered quantity.
- Update stock after successful receipt posting.
- Update purchase order status.
- Use database transactions for GRN posting.

#### 4.5 Inventory Management
- Maintain inventory transaction history.
- Track stock-in transactions.
- Display item-wise stock balance.
- Filter ledger entries by item.
- Maintain GRN reference numbers.

#### 4.6 Dashboard

The dashboard displays:
- Total Vendors
- Total Purchase Orders
- Open Purchase Orders
- Pending Delivery Value
- Low Stock Items

### 5. Role-Based Access Control

The application supports two roles.

**Procurement Officer**
- Dashboard
- Vendor Master
- Item Master
- Purchase Order Management

**Warehouse Storekeeper**
- Dashboard
- Goods Receipt Notes
- Inventory Ledger

ASP.NET Core Identity is used for authentication and role-based authorization.

### 6. Database Design

Database Name: `SCMProcureTrackDB`

Main business tables:

| Table | Description |
|---|---|
| Vendors | Vendor information |
| Items | Material information |
| PurchaseOrders | Purchase order headers |
| PurchaseOrderDetails | Purchase order line items |
| GoodsReceiptNotes | GRN headers |
| GoodsReceiptDetails | GRN line items |
| InventoryLedgers | Stock transaction history |

ASP.NET Core Identity tables are also created for authentication and authorization.

### 7. Database Scripts

The `DatabaseScripts` folder contains:

- `Schema.sql` – Idempotent SQL migration script generated from Entity Framework Core migrations.
- `StoredProcedures.sql` – Six stored procedures for procurement and inventory operations.
- `SeedData.sql` – Sample vendor and item data. Existing records are not overwritten.

**Note:** The application uses Entity Framework Core migrations for database setup. The generated `Schema.sql` is an alternative SQL deployment script. Do not run both setup methods unnecessarily.

The MVC application uses Entity Framework Core transactions for GRN stock posting. Do not separately execute the stock update stored procedure for the same GRN, as this may duplicate stock updates.

### 8. Installation and Setup

#### Prerequisites
- .NET SDK 11
- SQL Server 2025
- SQL Server Management Studio
- Visual Studio Code

#### Step 1: Open the project

Clone or extract the project and open the project folder in Visual Studio Code.

#### Step 2: Configure the database

Update the connection string in `appsettings.json` according to your SQL Server configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SCMProcureTrackDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### Step 3: Restore packages

```powershell
dotnet restore
```

#### Step 4: Apply database migrations

```powershell
dotnet ef database update
```

This creates or updates the database schema using the EF Core migrations.

#### Step 5: Build the application

```powershell
dotnet build
```

#### Step 6: Run the application

```powershell
dotnet run
```

Open the application using the URL displayed in the terminal.

On application startup, the `IdentitySeeder` class creates the required roles and demo users if they do not already exist.

#### Optional: Insert sample data

After database creation, execute `SeedData.sql` in SQL Server Management Studio if sample vendor and item data are required.

### 9. Demo Login Credentials

**Procurement Officer**

Email: `procurement@scm.local`

Password: `Procure@123`

**Warehouse Storekeeper**

Email: `warehouse@scm.local`

Password: `Warehouse@123`

These are development/demo credentials. Change them before any production deployment.

### 10. Unit Testing

The project uses xUnit for unit testing.

Test coverage includes:
- Vendor model validation.
- Purchase order subtotal calculation.
- Purchase order tax calculation.
- Purchase order grand total calculation.
- GRN quantity validation.
- Cumulative accepted quantity validation.

Run tests using:

```powershell
dotnet test .\DotNetAssignmentApp.Tests\DotNetAssignmentApp.Tests.csproj
```

**Latest test result:**

- Total Tests: 8
- Passed: 8
- Failed: 0

### 11. Error Handling and Security

- Centralized exception handling middleware.
- ILogger-based application logging.
- ASP.NET Core Identity authentication.
- Role-based authorization.
- Anti-forgery token validation.
- Entity Framework Core parameterized database operations.
- GRN stock updates within database transactions.
- Server-side validation of business rules.

### 12. Java to .NET Learning Reflection

With approximately 3.6 years of Java development experience, this project provided an opportunity to learn and implement equivalent concepts using the .NET ecosystem.

Key learning areas:

- C# programming compared with Java.
- ASP.NET Core MVC compared with Spring MVC.
- Entity Framework Core compared with Hibernate/JPA.
- LINQ compared with Java Streams.
- Dependency Injection in ASP.NET Core.
- ASP.NET Core Identity and authorization.
- SQL Server integration and migrations.
- xUnit unit testing.

This project helped strengthen my understanding of full-stack .NET application development and enterprise procurement workflows.

### 13. Conclusion

SCM ProcureTrack provides an integrated solution for vendor management, material management, purchase order processing, goods receipt handling and inventory tracking.

The application demonstrates the use of ASP.NET Core MVC, Entity Framework Core, SQL Server, authentication, authorization, transaction management and unit testing in a practical business application.
