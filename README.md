# Progresium ToDo

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)

**Progresium ToDo** is the backend API for Progresium, a task and project management SaaS. It's built on .NET 10 with a clean, layered architecture (Domain / Application / Infrastructure / API), CQRS via MediatR, and PostgreSQL for storage.

---

## ✨ Features

- **Tasks & Subtasks** — create, update, and organize tasks with status, priority, due dates, time ranges, attachments, custom ordering, and tags.
- **Projects** — group tasks into projects.
- **Tags** — flexible tagging system for tasks.
- **Authentication** — email/password registration with email verification, login, refresh tokens, logout, and password reset flows.
- **Google OAuth** — sign in with Google.
- **Billing & Subscriptions** — plans, regional pricing, subscriptions, and per-plan feature usage limits (e.g. task duration tracking).
- **Background Jobs** — async work (emails, subscription/usage processing, etc.) handled via Hangfire with PostgreSQL storage.
- **Email Service** — transactional emails (verification, password reset, contact form) via Mailtrap.
- **Waitlist** — endpoint for pre-launch sign-ups.
- **Support / Contact** — contact form endpoint with rate limiting.
- **API Documentation** — OpenAPI spec served through Scalar.

## 🛠️ Tech Stack

| Layer | Technology |
| --- | --- |
| Runtime | .NET 10 / ASP.NET Core |
| Architecture | Clean Architecture, CQRS (MediatR) |
| Database | PostgreSQL via EF Core (Npgsql, snake_case naming) |
| Auth | ASP.NET Core Identity, JWT Bearer, Google OAuth2 |
| Validation | FluentValidation |
| Background Jobs | Hangfire (PostgreSQL storage) |
| Email | Mailtrap API |
| API Docs | Scalar / Microsoft.AspNetCore.OpenApi |
| Containerization | Docker |

## 🏗️ Architecture

The solution follows Clean Architecture, split into four projects under `src/`:

```
src/
├── ProgresiumToDo.Domain          # Entities, enums, domain errors, core business rules
├── ProgresiumToDo.Application     # CQRS commands/queries, validators, abstractions
├── ProgresiumToDo.Infrastructure  # EF Core, repositories, identity, email, Hangfire, OAuth
└── ProgresiumToDo.API              # Controllers, middleware, DI wiring, Program.cs
```

Each feature area (Auth, Billing, Projects, Tags, Tasks, Waitlist, Support) is organized as a vertical slice within the Application layer, with its own Commands, Queries, and Repository interfaces.

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) instance
- A [Mailtrap](https://mailtrap.io/) account (for transactional email)
- A Google Cloud OAuth client (for Google sign-in)
- [Docker](https://www.docker.com/) (optional, for containerized runs)

### Environment Variables

Create a `.env` file at the repository root with the following variables:

```env
CONNECTION_STRING=Host=localhost;Port=5432;Database=progresium;Username=postgres;Password=postgres

JWT_SECRET=your-jwt-signing-secret
JWT_TOKEN_LIFETIME_IN_SECONDS=3600
REFRESH_TOKEN_LIFETIME_IN_DAYS=30

GOOGLE_CLIENT_ID=your-google-client-id
GOOGLE_CLIENT_SECRET=your-google-client-secret
BASE_URL=https://localhost:5001

MAILTRAP_API_KEY=your-mailtrap-api-token
```

### Running Locally

```bash
# Restore dependencies
dotnet restore

# Apply EF Core migrations (or let the app migrate on startup in Production)
dotnet ef database update --project src/ProgresiumToDo.Infrastructure --startup-project src/ProgresiumToDo.API

# Run the API
dotnet run --project src/ProgresiumToDo.API
```

### Running with Docker

```bash
docker build -f src/ProgresiumToDo.API/Dockerfile -t progresium-todo-api .
docker run -p 8080:8080 --env-file .env progresium-todo-api
```

## 📖 API Documentation

Once running, interactive API documentation (powered by Scalar) is available at:

```
/scalar/v1
```

with the raw OpenAPI document at:

```
/openapi/v1.json
```

## 📂 Solution Structure

| Project | Responsibility |
| --- | --- |
| `ProgresiumToDo.API` | Controllers (Auth, OAuth, Users, Billings, Tasks, Projects, Tags, Support, Waitlist), exception handling, rate limiting, Program.cs |
| `ProgresiumToDo.Application` | CQRS handlers and validators for Auth, OAuth, Billing, Projects, Tags, Tasks, Users, Waitlist, Support |
| `ProgresiumToDo.Domain` | Entities (`User`, `TaskItem`, `Project`, `Tag`, `Plan`, `Subscription`, etc.), enums, domain errors |
| `ProgresiumToDo.Infrastructure` | `ApplicationDbContext`, EF Core migrations, repositories, Identity, JWT/OAuth services, Hangfire jobs, Mailtrap email service |

## 📄 License

No license specified yet.