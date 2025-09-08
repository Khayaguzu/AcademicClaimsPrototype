

#  Academic Claims Prototype

The **Academic Claims Prototype** is an ASP.NET Core MVC web application that simulates an **academic claims management system**.
It allows **lecturers** to submit claims for academic work (e.g., marking, invigilation) and enables **managers/coordinators** to approve or reject them.

This project is built as a **prototype** using an **in-memory data store** (no external database).

---

##  Features

###  Authentication & Authorization

* User **login** and **registration** with role assignment.
* Session-based authentication (email & role stored in session).
* Role-based access using a custom `AuthorizeRole` attribute.

###  Lecturer Features

* View submitted claims (with status: Pending, Approved, Rejected).
* Submit new claims with:

  * Description
  * File upload (`.pdf`, `.docx`, `.xlsx`) — max 5MB
* Track claims by status (counts displayed on dashboard).

###  Manager/Coordinator Features

* View **all submitted claims**.
* Approve or reject claims.
* Track who processed each claim (with timestamp).

###  Dashboard

* Redirects users based on their role:

  * **Lecturers** → My Claims page.
  * **Managers/Coordinators** → Claims Management page.

---

##  Tech Stack

* **ASP.NET Core MVC**
* **Razor Views**
* **C#**
* **In-memory storage** (`InMemoryStore.cs`)
* **Session-based authentication**
* **File upload handling**

---

##  Project Structure

```
AcademicClaimsPrototype/
│── Controllers/
│   ├── AccountController.cs      # Handles login, registration, logout
│   ├── ClaimsController.cs       # Lecturer claims submission & listing
│   ├── ManagementController.cs   # Managers & coordinators approve/reject claims
│   ├── DashboardController.cs    # Role-based redirection
│   └── HomeController.cs         # Home, Privacy, Error pages
│
│── Models/
│   ├── User.cs                   # User entity
│   ├── Claim.cs                  # Claim entity
│   ├── AcademicClaim.cs          # Extended claim model
│   └── ClaimStatus.cs            # Enum for claim statuses
│
│── Services/
│   └── InMemoryStore.cs          # Stores users & claims in memory
│
│── Filters/
│   └── AuthorizeRoleAttribute.cs # Custom role-based access filter
│
│── Views/
│   ├── Account/ (Login, Register)
│   ├── Claims/ (Index, Create)
│   ├── Management/ (Index)
│   ├── Dashboard/ (Index)
│   ├── Home/ (Index, Privacy, Error)
│   └── Shared/ (_Layout.cshtml)
```

---

##  Default Users

The system comes with predefined users:

| Email                                                 | Password | Role                 |
| ----------------------------------------------------- | -------- | -------------------- |
| [lecturer1@uni.ac.za](mailto:lecturer1@uni.ac.za)     | 123      | Lecturer             |
| [lecturer2@uni.ac.za](mailto:lecturer2@uni.ac.za)     | 123      | Lecturer             |
| [manager@uni.ac.za](mailto:manager@uni.ac.za)         | 123      | AcademicManager      |
| [coordinator@uni.ac.za](mailto:coordinator@uni.ac.za) | 123      | ProgrammeCoordinator |

---

##  Setup & Run

1. **Clone the repository**

   ```bash
   git clone https://github.com/yourusername/AcademicClaimsPrototype.git
   cd AcademicClaimsPrototype
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Run the project**

   ```bash
   dotnet run
   ```

4. **Access the application**
   Open your browser and go to:
    `http://localhost:5000`

---

##  Future Improvements

* Replace in-memory storage with a **real database (Entity Framework + SQL)**.
* Add **password hashing** for security.
* Implement **email notifications** for claim updates.
* Add **unit tests** and integration tests.

---

##  License

This project is for **educational & prototype purposes only**.
Not intended for production use.

---
