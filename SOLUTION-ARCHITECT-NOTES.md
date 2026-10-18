# TMS Solution Architecture and Technical Development Notes

## Purpose

This document captures the architecture research, proof-of-concept work, and practical engineering experience used while designing and developing the backend systems for the Transportation Management System (TMS).

## Backup and Replication

The commands below use Docker because the current applications are containerized and have not yet been deployed to Kubernetes. Once the development and end-to-end integration of the backend services are complete and validated, they can be deployed to Kubernetes to validate microservice orchestration and operational readiness.

### 1. Keycloak (On-premises Active Directory / Identity Provider)

```cmd
:: 1. Copy the realm export file into the running container
docker cp .\realm-export.json keycloak:/tmp/realm-export.json

:: 2. Import the realm configuration inside the container
docker exec -it keycloak /opt/keycloak/bin/kc.sh import --file /tmp/realm-export.json

:: 3. Optional: Remove the temporary file from the container
docker exec -it keycloak rm /tmp/realm-export.json
```

The commands above import the exported realm configuration and the application's authentication and authorization settings for SSO logins. This allows developers and architects to replicate the setup across environments reliably.
