# Academic Claims System - Database Version

##  Project Overview

The Academic Claims System is a web-based application that allows lecturers to submit claims for their work, which can then be reviewed and approved by Programme Coordinators and Academic Managers. The system now uses Entity Framework with SQL Server for data persistence.

##  Features

### User Roles & Capabilities

####  Lecturers
- Submit new claims with mandatory documentation
- View all their submitted claims
- Edit pending claims
- Upload supporting documents (PDF, Word, JPG, PNG)
- View claim status and history

####  Programme Coordinators & Academic Managers
- Review all lecturer claims
- Approve or reject claims
- Provide rejection reasons
- View uploaded documents
- Monitor claim statistics

### Core Functionality
- **Role-based access control**
- **Document upload and management**
- **Real-time amount calculation**
- **Claim status tracking** (Pending, Approved, Rejected)
- **Session-based authentication**
- **Responsive design**

##  Technology Stack

### Backend
- **ASP.NET Core MVC**
- **Entity Framework Core** (ORM)
- **SQL Server** (Database)
- **Session Management**

### Frontend
- **HTML5, CSS3, JavaScript**
- **Razor Views**
- **Bootstrap-inspired styling**
- **Responsive design**

##  Project Structure

```
AcademicClaimsPrototype/
├── Controllers/
│   ├── AccountController.cs
│   ├── ClaimsController.cs
│   ├── ManagementController.cs
│   ├── DashboardController.cs
│   └── HomeController.cs
├── Models/
│   ├── Claim.cs
│   ├── User.cs
│   └── ErrorViewModel.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Filters/
│   └── AuthorizeRoleAttribute.cs
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Claims/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   └── Edit.cshtml
│   ├── Management/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   └── uploads/ (auto-created)
├── Program.cs
└── appsettings.json
```

##  Database Schema

### Users Table
- `Email` (Primary Key) - User's email address
- `Password` - User password (plain text for demo)
- `Role` - User role (Lecturer, ProgrammeCoordinator, AcademicManager)

### Claims Table
- `Id` (Primary Key) - Unique claim identifier
- `LecturerEmail` - Email of the lecturer who submitted the claim
- `Date` - Date of the claim
- `Hours` - Number of hours worked
- `Rate` - Hourly rate in Rands
- `Description` - Claim description
- `DocumentPath` - Path to uploaded document
- `Status` - Claim status (Pending, Approved, Rejected)
- `SubmittedAt` - Timestamp of submission
- `ProcessedBy` - Email of user who processed the claim
- `ProcessedAt` - Timestamp of processing
- `RejectionReason` - Reason for rejection (if applicable)

##  Setup Instructions

### Prerequisites
- .NET 7.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone or download the project**

2. **Update connection string** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AcademicClaimsDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. **Install required NuGet packages**:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0" />
```

4. **Run the application**:
```bash
dotnet run
```

### Database Initialization
The application automatically:
- Creates the database on first run
- Seeds initial user accounts
- Creates sample claim data
- Creates the uploads directory

##  Default User Accounts

The system comes with pre-configured users:

| Email | Password | Role |
|-------|----------|------|
| lecturer1@uni.ac.za | 123 | Lecturer |
| lecturer2@uni.ac.za | 123 | Lecturer |
| manager@uni.ac.za | 123 | AcademicManager |
| coordinator@uni.ac.za | 123 | ProgrammeCoordinator |

##  Usage Guide

### For Lecturers

1. **Login** with lecturer credentials
2. **Submit New Claim**:
   - Fill in date, description, hours, and rate
   - Upload mandatory supporting document
   - Submit claim for review

3. **View Claims**:
   - See all submitted claims
   - View status (Pending, Approved, Rejected)
   - Edit pending claims if needed

### For Managers/Coordinators

1. **Login** with manager/coordinator credentials
2. **Review Claims**:
   - View all lecturer claims
   - See claim details and uploaded documents
   - Approve or reject claims
   - Provide rejection reasons when applicable

##  Security Features

- **Role-based authorization**
- **Session management**
- **File type validation** (PDF, DOC, DOCX, JPG, PNG)
- **File size limits** (10MB maximum)
- **Input validation** on all forms

##  File Upload Specifications

- **Supported formats**: PDF, Word documents (.doc, .docx), Images (.jpg, .jpeg, .png)
- **Maximum file size**: 10MB
- **Storage location**: `wwwroot/uploads/`
- **File naming**: Unique GUID-based names to prevent conflicts

##  UI/UX Features

- **Currency display**: All amounts in South African Rands (R)
- **Real-time calculations**: Automatic amount calculation
- **Status badges**: Color-coded claim status indicators
- **Responsive tables**: Horizontal scrolling for mobile devices
- **Form validation**: Client and server-side validation
- **Success/error messages**: Clear user feedback

##  Workflow

1. **Lecturer submits claim** → Status: Pending
2. **Manager/Coordinator reviews** → Approve or Reject
3. **If approved** → Status: Approved, claim processed
4. **If rejected** → Status: Rejected, reason provided

##  Development Notes

### Key Dependencies
- Entity Framework Core for data access
- SQL Server for database
- ASP.NET Core Session for authentication
- Built-in validation attributes

### Custom Components
- `AuthorizeRoleAttribute` for role-based authorization
- `ApplicationDbContext` for database context
- File upload handling with validation
- Real-time amount calculation

### Configuration
- Database connection in `appsettings.json`
- Session configuration in `Program.cs`
- File upload settings in `ClaimsController.cs`

##  Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in `appsettings.json`
   - Ensure database permissions are correct

2. **File Upload Issues**
   - Check `wwwroot/uploads/` directory exists
   - Verify file size and type restrictions
   - Check IIS/IIS Express permissions

3. **Login Issues**
   - Verify user credentials match seeded data
   - Check session configuration
   - Clear browser cache and cookies

### Logs
- Check Visual Studio Output window for detailed error messages
- Application logs errors to console for debugging

##  Support

For technical issues or questions about the Academic Claims System, please contact the development team or refer to the application logs for detailed error information.

---

**Version**: 2.0 (Database Edition)  
**Last Updated**: 2025  
**Framework**: ASP.NET Core 7.0  
**Database**: SQL Server with Entity Framework Core
