# Supermarket Survey System

A mobile-first responsive web application for supermarket merchandisers to conduct customer satisfaction surveys in the field.

Built with **ASP.NET Core MVC (.NET 8)** — one command runs the entire system.

---

## Tech Stack

| Layer          | Technology                        |
|----------------|-----------------------------------|
| Framework      | ASP.NET Core MVC (.NET 8)         |
| Views          | Razor Pages (`.cshtml`)           |
| Database       | MySQL 8.0                         |
| ORM            | Entity Framework Core 8 (Pomelo)  |
| Authentication | Cookie-based (ASP.NET Core Auth)  |
| Session State  | ASP.NET Core Session              |
| Styling        | Mobile-first CSS (no framework)   |

---

## Project Structure

```
SupermarketSurveySystem/
├── backend/
│   └── SupermarketSurvey.Mvc/
│       ├── Controllers/
│       │   ├── AccountController.cs     # Login, Logout
│       │   ├── DashboardController.cs   # Home screen
│       │   ├── SurveyController.cs      # Full survey workflow
│       │   └── ReportsController.cs     # Participation + Statistics
│       ├── Data/
│       │   └── AppDbContext.cs          # EF Core DbContext
│       ├── Models/
│       │   ├── Entities/                # 8 database entity classes
│       │   └── ViewModels/              # Form & display models
│       ├── Views/
│       │   ├── Account/                 # Login page
│       │   ├── Dashboard/               # Dashboard home
│       │   ├── Survey/                  # 6-step survey workflow
│       │   ├── Reports/                 # 2 report pages
│       │   └── Shared/_Layout.cshtml    # Shared mobile layout
│       ├── wwwroot/css/styles.css       # Mobile-first stylesheet
│       ├── Program.cs                   # App startup & config
│       ├── appsettings.json
│       └── SupermarketSurvey.Mvc.csproj
├── database/
│   ├── schema.sql                       # Table definitions (8 tables)
│   ├── seed.sql                         # Sample data (Malay locale)
│   └── reports.sql                      # Standalone SQL report queries
├── docs/
│   ├── ERD.md                           # Entity Relationship Diagram
│   └── Routes.md                        # MVC controller/action reference
└── README.md
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- MySQL 8.0 running locally

---

## Setup & Run

### Step 1 — Start MySQL

```bash
brew services start mysql   # macOS with Homebrew
```

### Step 2 — Create the database (first time only)

```bash
mysql -u root < database/schema.sql
mysql -u root supermarket_survey < database/seed.sql
```

Then set the merchandiser passwords:

```bash
python3 -c "
import bcrypt
h = bcrypt.hashpw(b'password123', bcrypt.gensalt()).decode()
print(f\"UPDATE Merchandisers SET PasswordHash = '{h}';\")
" | mysql -u root supermarket_survey
```

### Step 3 — Run the application

```bash
cd backend/SupermarketSurvey.Mvc
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://localhost:5000" dotnet run
```

Open **http://localhost:5000** in your browser.

> That's it — no separate frontend server, no JavaScript build step.

---

## Demo Credentials

| Email                         | Password    | Assigned Outlet           |
|-------------------------------|-------------|---------------------------|
| `ahmad.hafizi@supermart.my`   | password123 | SuperMart Kuala Lumpur    |
| `nurul.aina@supermart.my`     | password123 | SuperMart Shah Alam       |
| `syafiq@supermart.my`         | password123 | SuperMart Petaling Jaya   |
| `siti.nabilah@supermart.my`   | password123 | SuperMart Johor Bahru     |
| `admin@supermart.my`          | password123 | SuperMart Kuala Lumpur    |

---

## Survey Workflow

```
/ (root)
  └── Login  (/Account/Login)
        └── Dashboard  (/Dashboard/Index)
              └── Select Outlet  (/Survey/SelectOutlet)
                    └── Select Survey  (/Survey/SelectSurvey)
                          └── Respondent Details  (/Survey/Respondent)
                                └── Answer Questions  (/Survey/Questions)
                                      └── Review  (/Survey/Review)
                                            └── Submit  (/Survey/Submit)
                                                  └── Success  (/Survey/Success)
```

---

## MVC Routes

| Method | Route                       | Controller Action             | Description                    |
|--------|-----------------------------|-------------------------------|--------------------------------|
| GET    | `/`                         | Account → Index               | Redirect to Login              |
| GET    | `/Account/Login`            | Account → Login               | Login form                     |
| POST   | `/Account/Login`            | Account → Login               | Validate credentials           |
| GET    | `/Account/Logout`           | Account → Logout              | Clear session & cookie         |
| GET    | `/Dashboard/Index`          | Dashboard → Index             | Home screen with stats         |
| GET    | `/Survey/SelectOutlet`      | Survey → SelectOutlet         | Choose outlet                  |
| POST   | `/Survey/SelectOutlet`      | Survey → SelectOutlet         | Save outlet to session         |
| GET    | `/Survey/SelectSurvey`      | Survey → SelectSurvey         | Choose survey                  |
| POST   | `/Survey/SelectSurvey`      | Survey → SelectSurvey         | Save survey to session         |
| GET    | `/Survey/Respondent`        | Survey → Respondent           | Respondent details form        |
| POST   | `/Survey/Respondent`        | Survey → Respondent           | Save respondent to session     |
| GET    | `/Survey/Questions`         | Survey → Questions            | Display all questions          |
| POST   | `/Survey/Questions`         | Survey → Questions            | Save answers to session        |
| GET    | `/Survey/Review`            | Survey → Review               | Review all data before submit  |
| POST   | `/Survey/Submit`            | Survey → Submit               | Save to DB, redirect to Success|
| GET    | `/Survey/Success`           | Survey → Success              | Confirmation screen            |
| GET    | `/Reports/Index`            | Reports → Index               | Participation summary report   |
| GET    | `/Reports/Statistics`       | Reports → Statistics          | Answer statistics report       |

Full route reference: [docs/Routes.md](docs/Routes.md)

---

## Database Design

See [docs/ERD.md](docs/ERD.md) for the full Entity Relationship Diagram.

### Tables

| Table                   | Description                                        |
|-------------------------|----------------------------------------------------|
| `Outlets`               | Supermarket branches                               |
| `Merchandisers`         | Field staff with login credentials                 |
| `Surveys`               | Reusable survey templates                          |
| `Questions`             | Ordered questions per survey                       |
| `AnswerOptions`         | Multiple-choice options per question               |
| `Respondents`           | Customer demographic profile                       |
| `SurveyResponses`       | Links respondent + outlet + merchandiser + survey  |
| `SurveyResponseDetails` | One row per answered question                      |

---

## Reports

### Report 1 — Survey Participation Summary (`/Reports/Index`)
Displays total responses grouped by survey, outlet, and merchandiser.

### Report 2 — Question Answer Statistics (`/Reports/Statistics`)
Shows the percentage breakdown of each answer option per question, with progress bar visualisation.

Raw SQL queries for both reports: [database/reports.sql](database/reports.sql)

---

## How It Works (MVC vs Web API)

| Concern              | This App (MVC)                                  |
|----------------------|-------------------------------------------------|
| Rendering            | Server-rendered Razor views (`.cshtml`)         |
| Authentication       | Cookie stored in browser, verified per request  |
| State between pages  | `HttpContext.Session` (stored server-side)      |
| Form submission      | Standard HTML `<form method="post">`            |
| No JavaScript needed | Search filter on outlet page uses 5 lines of JS |
