# Transportation Management Systems (TMS) Backend Applications

A distributed backend platform for a transportation and logistics ecosystem. This repository contains the core backend services for order management, shipment tracking, document processing, identity integration, and supporting infrastructure used across the TMS ecosystem.

## Overview

The platform is designed around a modular microservice-oriented architecture where separate services handle specific business concerns:

- Order processing and shipment lifecycle management
- Real-time tracking and event monitoring
- Document and file management
- Authentication and authorization with Keycloak
- Messaging and asynchronous integration via RabbitMQ
- Search and routing capability with GraphHopper
- Storage and file handling with SeaweedFS
- Time-series and transactional data storage using TimescaleDB/PostgreSQL

## Solution Architecture

The repository combines a set of backend services and infrastructure components that work together to support the Transportation Management System. Refer to the [Solution Architecture Notes](./SOLUTION-ARCHITECT-NOTES.md).

### Core services

1. Order Service
   - Project: `Transport-Management-Systems-Portal-Order-Service-REST-API`
   - Stack: ASP.NET Core Web API
   - Responsibility: order creation, shipment coordination, address handling, client management, quota and rate-related workflows, and document integration

2. Tracking Service
   - Project: `Transportation-Management-System-Tracking-Service-REST-API`
   - Stack: ASP.NET Core Web API + SignalR
   - Responsibility: real-time tracking events, event publishing, event persistence, and live updates for operational visibility

3. Document Service
   - Project: `Transportation-Management-Systems-Document-Service-REST-API`
   - Stack: Spring Boot + Kotlin
   - Responsibility: document handling, metadata management, and file/document-related business logic

### Shared infrastructure

- Keycloak: identity and access management / SSO
- PostgreSQL / TimescaleDB: relational and time-series data persistence
- Redis: caching and lightweight session/data caching
- RabbitMQ: messaging backbone for event-driven communication
- SeaweedFS: object storage for files and documents
- GraphHopper: route and map data processing for logistics and distance calculations

## Repository Structure

```text
Transportation-Management-Systems-Backend-Applications/
├── README.md
├── docker-compose.yml
├── realm-export.json
├── tms_backup_clean.sql
├── dotnet-tools.json
├── plugins/
├── graphhopper_data/
├── tms-file-storage-data/
├── Transport-Management-Systems-Portal-Order-Service-REST-API/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── Middlewares/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── docker-compose.yml
│   └── ...
├── Transportation-Management-System-Tracking-Service-REST-API/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── SignalRControllers/
│   ├── Program.cs
│   ├── appsettings.json
│   └── ...
├── Transportation-Management-Systems-Document-Service-REST-API/
│   ├── src/
│   ├── build.gradle
│   ├── settings.gradle
│   └── gradlew
└── SOLUTION-ARCHITECT-NOTES.md
```

## Technology Stack

### Backend services

- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL / Npgsql
- RabbitMQ
- SignalR
- JWT / Keycloak integration
- Amazon S3-compatible client for SeaweedFS integration

### Document service

- Kotlin
- Spring Boot 4.x
- Spring Data JPA
- PostgreSQL
- Gradle

### Infrastructure / supporting systems

- Docker / Docker Compose
- Redis
- TimescaleDB
- SeaweedFS
- Keycloak
- GraphHopper

## Prerequisites

Before running the project, ensure the following are installed:

- .NET SDK
- Java JDK (for the document service)
- Gradle or use the included Gradle wrapper
- Docker and Docker Compose
- Git

Optional but helpful:

- Postman or Swagger UI for API testing
- RabbitMQ management console
- A browser for Keycloak admin console and service frontend access

## Local Infrastructure Startup

The project includes a shared Docker Compose setup for local infrastructure services.

From the repository root, run:

```bash
docker compose up -d
```

This starts the supporting components used by the platform, including:

- Keycloak
- TimescaleDB / PostgreSQL
- Redis
- SeaweedFS
- RabbitMQ
- GraphHopper

## Service Ports and Access Points

The current Docker configuration exposes the following local endpoints:

| Service                 |  Port | Notes                    |
| ----------------------- | ----: | ------------------------ |
| GraphHopper             |  8989 | Routing / map processing |
| Keycloak                |  8080 | Identity provider        |
| TimescaleDB/PostgreSQL  |  5432 | Primary DB               |
| Redis                   |  6379 | Cache / data store       |
| SeaweedFS S3 API        |  8333 | Object storage access    |
| SeaweedFS Master UI     |  9333 | Cluster UI               |
| SeaweedFS Filer UI      |  8888 | File browsing            |
| SeaweedFS Volume Server |  9340 | Storage volume           |
| RabbitMQ                |  5672 | Messaging port           |
| RabbitMQ Management UI  | 15672 | Broker admin UI          |

## Running the .NET Services

### 1. Order Service

```bash
cd "Transport-Management-Systems-Portal-Order-Service-REST-API"
dotnet restore
dotnet run
```

### 2. Tracking Service

```bash
cd "Transportation-Management-System-Tracking-Service-REST-API"
dotnet restore
dotnet run
```

## Running the Document Service

The document service uses Gradle and Kotlin.

```bash
cd "Transportation-Management-Systems-Document-Service-REST-API"
./gradlew bootRun
```

On Windows PowerShell:

```powershell
cd "Transportation-Management-Systems-Document-Service-REST-API"
.\gradlew.bat bootRun
```

## Identity and Security

Keycloak is used for authentication and authorization. The services are configured to validate JWT tokens and map roles to application authorization policies. This supports minimal security boundaries for the backend APIs and supports enterprise SSO-style integration.

The included `realm-export.json` file is used to import the Keycloak realm configuration and replicate identity settings across environments.

## Messaging and Integration

The platform relies on RabbitMQ for asynchronous messaging between services and integration events. This allows operational events, user related events, and backend workflows to be coordinated without tightly coupling service-to-service communication.

## Data Model and Persistence

The project makes use of:

- PostgreSQL for primary application data
- TimescaleDB for time-based telemetry and tracking data
- Redis for caching and transient data
- SeaweedFS for file/document storage

This hybrid approach supports both transactional and analytical workloads while keeping storage responsibilities aligned to service needs.

## Development Notes

This repository is structured as a multi-service backend platform that is intended to be run locally using Docker and multiple backend services. It is suitable for:

- architecture exploration
- proof-of-concept work
- backend service integration
- distributed system learning and experimentation
- enterprise logistics and transportation domain modeling

## Important Notes

- Several default credentials and configuration values are currently defined directly in the local development stack and should not be reused in production.
- Docker Compose is intended for local development and proof-of-concept deployment scenarios.
- Production-grade hardening should include environment-specific secrets management, TLS, secure networking, and access policy review.

## Future Considerations

Recommended next steps for the project include:

- centralizing configuration and secrets management
- adding automated tests across services
- introducing API gateway patterns and service discovery
- improving observability with logging, traces, and metrics
- formalizing CI/CD pipelines
- preparing the platform for Kubernetes deployment and service orchestration

## Related Documents

- [SOLUTION-ARCHITECT-NOTES.md](./SOLUTION-ARCHITECT-NOTES.md)
- [docker-compose.yml](./docker-compose.yml)

## License

This project is currently maintained as a development and architecture-oriented repository. Please confirm the intended licensing arrangement before using it in production or distributing it externally.
