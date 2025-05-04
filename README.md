# To-Dooly

**To-Dooly** is a simple ASP.NET Core MVC + Razor Pages web application for managing projects, tasks, and labels with user authentication via ASP.NET Identity.

## Features

* **Authentication & Authorization**

  * User registration, login, logout, and account management.
  * Each user only sees their own projects, tasks, and labels.

* **Projects**

  * Create, read, update, and delete projects.
  * Each project has a title, description, and owner (user).

* **Tasks**

  * Create, edit, complete, and delete tasks within a project.
  * Tasks have title, description, due date, priority, and completion status.
  * AJAX-powered task creation, completion toggling, and deletion on the project details page.
  * Open and Completed sections for tasks.

* **Labels**

  * Create, edit, and delete labels to categorize tasks.
  * Assign multiple labels to a task via a multi-select interface.

* **Dashboard**

  * Overview of total projects, total tasks, and completed tasks.
  * Chart showing tasks completed over time.

## Tech Stack

* **.NET 8.0** with ASP.NET Core MVC & Razor Pages
* **Entity Framework Core 8.0** with SQL Server (LocalDB)
* **ASP.NET Identity** for user management
* **Bootstrap 5** for styling
* Vanilla JavaScript for AJAX task UI

## Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* SQL Server LocalDB (installed with Visual Studio)
* Visual Studio 2022+ or VS Code

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/Coreylsnodgrass/To-Dooly.git
   cd To-Dooly
   ```

2. **Configure Connection String**

   * In `appsettings.json`, update the `DefaultConnection` string if needed:

     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ToDoolyDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
     ```

3. **Apply EF Migrations & Seed Data**

   ```bash
   dotnet ef database update
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

   * Open your browser at `https://localhost:5001`.

5. **Register a new user** and start creating projects, tasks, and labels.

## Project Structure

```
/To-Dooly                # Main Web project
├─ Controllers           # MVC + API controllers
├─ Models                # Entities & ViewModels
├─ Services              # EF DbContext & repository services
├─ Views                 # Razor Views & Partials
├─ wwwroot               # Static files (js, css)
└─ Program.cs            # App startup configuration
```

## Contributing

Contributions, issues, and feature requests are welcome! Feel free to open a pull request.

## License

This project is licensed under the MIT License.
