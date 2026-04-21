# Jobrythm Backend

Professional Job Management SaaS for small businesses and tradespeople.

## Prerequisites
- .NET 10 SDK
- Docker Desktop
- PostgreSQL (if running locally)

## Quick Start (Docker)
1. Ensure Docker is running.
2. Run `docker compose up -d --build` from the root directory.
3. API will be available at [http://localhost:5000](http://localhost:5000).
4. pgAdmin will be available at [http://localhost:5050](http://localhost:5050) (Login: admin@jobrythm.com / admin).

## Database Migrations
When running via Docker, migrations are applied automatically on startup in Development environment.
To apply manually:
```bash
dotnet ef database update --project src/Jobrythm.Infrastructure --startup-project src/Jobrythm.Api
```

## Running Tests
Run all tests in the solution:
```bash
dotnet test
```

## Environment Variables (Production)
For production deployment, ensure the following secrets are set:
- `JwtSettings__Key`
- `ConnectionStrings__DefaultConnection`
- `Stripe__ApiKey`
- `Stripe__WebhookSecret`
- `Resend__ApiKey`

## Project Structure
- `src/Jobrythm.Domain`: Core entities and business logic.
- `src/Jobrythm.Application`: MediatR commands, queries, and business use cases.
- `src/Jobrythm.Infrastructure`: Persistence (EF Core), External Services (Stripe, Resend, QuestPDF).
- `src/Jobrythm.Api`: RESTful API, Controllers, and Middleware.
- `tests/`: xUnit test projects for Domain and Application layers.
