# Getting Started

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# Dev Kit
- .NET MAUI workload: `dotnet workload install maui`

## Setup

```bash
# Clone the repository
git clone https://github.com/Amutai/Junior-Golf-App.git
cd Junior-Golf-App

# Copy environment file
# Linux/macOS:
cp .env.example .env
# Windows:
# copy .env.example .env
# Edit .env with your values

# Restore dependencies
dotnet restore

# Start infrastructure (PostgreSQL + Redis)
docker-compose up postgres redis -d

# Run the API
dotnet run --project src/JuniorGolf.Api

# Run the web app (separate terminal)
dotnet run --project src/JuniorGolf.Web

# Run the MAUI app (Windows)
dotnet run --project src/JuniorGolf.App -f net10.0-windows10.0.19041.0

# Run tests
dotnet test
```

## Project Structure

| Project | Purpose | Port |
|---------|---------|------|
| JuniorGolf.Api | REST API | 8080 |
| JuniorGolf.Web | Blazor WASM (browser) | 5000 |
| JuniorGolf.Admin | Admin dashboard | 5001 |
| JuniorGolf.App | MAUI (mobile/desktop) | — |

## Workflow

1. Pick an issue from the current milestone
2. Branch from `develop`: `git checkout -b feature/<issue-number>-description`
3. Implement, test, commit (conventional commits)
4. Push and open a PR → `Closes #<issue-number>`
