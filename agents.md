# Jobrythm Backend Agents

This document describes the agents and their responsibilities within the Jobrythm backend system.

## Domain Agent
- **Responsibilities**: Manages core business logic, entities, and domain-driven design principles.
- **Location**: `src/Jobrythm.Domain`
- **Key Entities**: `ApplicationUser`, `Client`, `Job`, `Quote`, `Invoice`.

## Application Agent
- **Responsibilities**: Orchestrates the application flow, handles requests, and implements business use cases.
- **Location**: `src/Jobrythm.Application`
- **Patterns**: MediatR (Behaviors), DTOs, Repository Interfaces.

## API Agent
- **Responsibilities**: RESTful endpoints, API versioning, and request/response mapping.
- **Location**: `src/Jobrythm.Api`

## System Configuration
- **SaaS App**: `app.jobrythm.com`
- **API**: `api.jobrythm.com`
- **Infrastructure**: Docker & Docker Compose
  - **Database**: PostgreSQL (containerized)
  - **Volumes**: Data persistent in `jobrythm-db-data` volume.

## Future Agents (Infrastructure)
The system is designed to be extensible. Infrastructure layer will be implemented to provide persistence (Entity Framework Core).
