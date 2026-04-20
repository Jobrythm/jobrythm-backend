# Jobrythm Backend

Jobrythm is a SaaS-focused backend system for managing clients, jobs, quotes, and invoices. This repository contains the core Domain, Application, and API layers, containerized with Docker for easy development and deployment.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/macOS) or [Docker Engine](https://docs.docker.com/engine/install/) with [Docker Compose](https://docs.docker.com/compose/install/) (Linux).

## Getting Started

### 1. Build and Run with Docker Compose

To build the latest version and start the services (API and Database), run the following command from the project root:

```bash
docker compose up -d --build
```

- `-d`: Runs containers in detached mode (in the background).
- `--build`: Forces a rebuild of the API image.

### 2. Verify the Setup

- **API**: The API should be accessible at [http://localhost:5000](http://localhost:5000). You can check the health by visiting [http://localhost:5000/weatherforecast](http://localhost:5000/weatherforecast).
- **Database**: PostgreSQL is accessible on port `5432`.

## Port Mappings

The following ports are forwarded from the host machine to the Docker containers:

| Service | Host Port | Container Port | Description |
| :--- | :--- | :--- | :--- |
| **API** | `5000` | `8080` | ASP.NET Core Web API |
| **Database** | `5432` | `5432` | PostgreSQL 17 |

## Persistent Data

All database data is stored in a named Docker volume called `jobrythm-db-data`. This ensures that your data is **not lost** even if you stop or remove the containers or rebuild the images.

## Configuration

The system is pre-configured with the following default URLs:

- **API Base URL**: `https://api.jobrythm.com` (Mapped to `localhost:5000` via environment variables)
- **Client/SaaS App URL**: `https://app.jobrythm.com`

These can be overridden in the `docker-compose.yml` file under the `environment` section for the `jobrythm-api` service.

## Useful Commands

- **Stop Services**: `docker compose stop`
- **Stop and Remove Containers**: `docker compose down`
- **View Logs**: `docker compose logs -f jobrythm-api`
- **Enter Database CLI**: `docker exec -it jobrythm-db psql -U postgres -d jobrythm`

---

For more details on the architecture and agent responsibilities, see [agents.md](./agents.md).
