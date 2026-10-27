# SwimTrack

SwimTrack is an ASP.NET Core MVC web application for planning, recording, and
tracking swimming workouts. Users can create personal training plans, add
workouts to those plans, and record what happened during each completed
workout.

The project follows a
layered structure with separate projects for the web application, business
services, data access, DTOs, domain models, and shared validation logic.

## Features

- User registration, login, and logout through ASP.NET Core Identity.
- Personal training plans with:
  - a name and optional goal;
  - a start and end date;
  - any number of planned workouts.
- Create, view, edit, and remove training plans.
- Add and remove workouts while creating or editing a training plan.
- Planned workout details:
  - date;
  - planned distance;
  - planned duration;
  - notes.
- Workout lifecycle tracking:
  - **Planned** - the workout is scheduled;
  - **Completed** - actual distance, duration, completion date, and notes are
    recorded;
  - **Skipped** - the workout was not completed and can be planned again.
- Validation of training-plan date ranges, workout dates, positive distance and
  duration values, and workout completion dates.
- User data isolation: authenticated users can access only their own training
  plans and workouts.
- Entity Framework Core migrations for creating and updating the SQL Server
  database schema.

## Application flow

1. A visitor registers or logs in.
2. The user opens the training-plan list and creates a plan with its date range
   and planned workouts.
3. The user reviews the plan details and edits the plan or individual workouts
   when needed.
4. A planned workout can be completed by entering actual results, or skipped.
5. A skipped workout can be planned again.
6. Completed workout results remain visible together with the original plan.

## Technologies

- **C#**
- **.NET 10**
- **ASP.NET Core MVC**
- **Razor Views and Razor Pages**
- **ASP.NET Core Identity**
- **Entity Framework Core 10**
- **Microsoft SQL Server**
- **Bootstrap**
- **jQuery Validation and jQuery Unobtrusive Validation**

## Project structure

```text
SwimTrack/
├── SwimTrack.Web/          ASP.NET Core MVC application, controllers, views,
│                           Identity pages, static assets, and configuration
├── SwimTrack.Services/     Application services and service contracts
├── SwimTrack.Data/         EF Core DbContext, SQL Server configuration, and
│                           database migrations
├── SwimTrack.Data.Models/  Entity classes used by the data layer
├── SwimTrack.DTOs/         Input models and view models
├── SwimTrack.Shared/       Enums, constants, validation, and shared rules
└── SwimTrack.slnx          Solution file
```

## Prerequisites

Install the following before running the application:

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
2. A local Microsoft SQL Server instance. SQL Server Express or LocalDB is
   suitable for local development.
3. Optional: the Entity Framework Core CLI tool, if the `dotnet ef` command is
   not already available:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

## Configuration

The default development connection string is stored in
`SwimTrack.Web/appsettings.json`:

```json
"DefaultConnection": "Server=localhost;Database=SwimTrack;Trusted_Connection=True;Encrypt=False;"
```

Update the `Server` value if your local SQL Server instance uses a different name.
For example, if using LocalDB (bundled with Visual Studio), update the connection
string to:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SwimTrack;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
```

No external environment variables or predefined credentials are required for local
testing. Once the database is created, simply register a new account through the
web interface to begin using the application.

## Getting started

Clone the repository and move into its directory:

```bash
git clone https://github.com/emo-nikoloff/SwimTrack.git
cd SwimTrack
```

Restore dependencies:

```bash
dotnet restore
```

Apply the existing Entity Framework Core migrations to create the database:

```bash
dotnet ef database update --project SwimTrack.Data --startup-project SwimTrack.Web
```

Build the solution:

```bash
dotnet build
```

Run the web application:

```bash
dotnet run --project SwimTrack.Web
```

Open the HTTPS URL printed by the application, register an account, and start
creating training plans.

In Visual Studio or VS Code, `SwimTrack.Web` can also be selected as the
startup project and launched with the standard ASP.NET Core launch profile.
