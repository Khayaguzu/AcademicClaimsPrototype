# Academic Claims System

A role-based ASP.NET Core MVC application that digitises the submission, review, approval, and tracking of lecturer claims. The project demonstrates database-backed web development, workflow design, validation, document handling, and responsive user interfaces.

## Highlights

- Lecturer claim submission with supporting documents
- Programme Coordinator and Academic Manager review workflows
- Approve and reject actions with rejection reasons
- Claim status and processing history
- Automatic claim amount calculation in South African rand
- Searchable, responsive management views
- SQL Server persistence through Entity Framework Core
- Server-side validation and file type and size checks

## Technology

- C# and ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor views, HTML, CSS, JavaScript
- Session-based role checks

## Roles and workflow

1. A lecturer creates a claim and uploads supporting evidence.
2. The system validates and stores the pending claim.
3. A coordinator or manager reviews the claim.
4. The reviewer approves it or rejects it with a reason.
5. The lecturer can view the updated status and history.

## Run locally

### Requirements

- .NET 7 SDK or a compatible later SDK
- SQL Server LocalDB, Express, or SQL Server
- Visual Studio 2022 or Visual Studio Code

### Setup

1. Clone the repository.

```bash
git clone https://github.com/Khayaguzu/AcademicClaimsPrototype.git
cd AcademicClaimsPrototype
```

2. Configure `DefaultConnection` in `appsettings.json` for your SQL Server instance.

3. Restore dependencies and start the application.

```bash
dotnet restore
dotnet run
```

4. Open the local address printed in the terminal.

## Project structure

- `Controllers/`, HTTP actions and workflow coordination
- `Models/`, application data and validation rules
- `Data/`, Entity Framework Core database context
- `Filters/`, custom role authorisation
- `Views/`, Razor user interfaces
- `wwwroot/`, static assets and uploaded documents

## Security note

This is an educational portfolio project. Its seeded demo accounts and session-based authentication are intended for local demonstration only. A production deployment should use ASP.NET Core Identity, salted password hashing, anti-forgery protection, stricter upload isolation, secrets management, HTTPS enforcement, audit logging, and least-privilege database credentials.

## Suggested recruiter walkthrough

1. Sign in as a lecturer and submit a claim.
2. Sign in as a coordinator or manager and process it.
3. Return to the lecturer view and verify the recorded status.
4. Review the controllers, Entity Framework context, custom role filter, and validation logic.

## Roadmap

- Replace demo authentication with ASP.NET Core Identity
- Add automated unit and integration tests
- Add audit logs and notification support
- Package local setup with containers
- Add screenshots and a short demonstration video

## License

Created as an educational portfolio project. Contact the repository owner before reusing the source commercially.
