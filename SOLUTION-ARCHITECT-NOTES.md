# TMS Solution Architecture and Technical Development Notes

## Purpose

This document captures the architecture research, proof-of-concept work, and practical engineering experience used while designing and developing the backend systems for the Transportation Management System (TMS).

## Backup and Replication

The development environment uses Docker for supporting infrastructure and some applications. The backend services have not yet been deployed to Kubernetes. Once their development and end-to-end integration are complete and validated, they can be deployed to Kubernetes to evaluate orchestration and operational readiness.

### 1. Keycloak (Identity Provider / SSO)

```cmd
:: 1. Copy the realm export file into the running container
docker cp .\realm-export.json keycloak:/tmp/realm-export.json

:: 2. Import the realm configuration inside the container
docker exec -it keycloak /opt/keycloak/bin/kc.sh import --file /tmp/realm-export.json

:: 3. Optional: Remove the temporary file from the container
docker exec -it keycloak rm /tmp/realm-export.json
```

The commands above import the exported realm configuration and the application's authentication and authorization settings for SSO logins. This allows developers and architects to replicate the setup across environments reliably.

### 2. Testing the Order Service from an Android Device

The Android app must be able to reach the Order Service over the network. The correct address depends on whether the app runs in an Android emulator or on a physical device:

- For the standard Android Studio emulator, use `http://10.0.2.2:5230` to reach port `5230` on the development computer.
- For a physical Android device, use the development computer's LAN IPv4 address, such as `http://192.168.1.25:5230`. The phone and computer must be connected to the same reachable network. Do not use `localhost` or `127.0.0.1` in the phone app; those addresses refer to the phone itself.

When running the API directly on the development computer, configure the development launch profile to listen on all network interfaces. For example:

```launchSettings.json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5230;http://0.0.0.0:5230",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Binding to `0.0.0.0` allows connections addressed to the computer's network interfaces; it is a listen address, not the address to put in the Android app. Use the emulator or physical-device URL above. If the API runs in Docker instead, configure the application inside the container to listen on `0.0.0.0:5230` and publish the port to the host (for example, `5230:5230`). A `launchSettings.json` profile affects local `dotnet run`; it does not configure a containerized process.

Before testing from a physical device:

1. Start the API and confirm it responds locally on the development computer.
2. Find the computer's current LAN IPv4 address and configure the Android app's API base URL to use that address and port.
3. Allow inbound TCP traffic on port `5230` through the computer's firewall for the trusted development network.
4. Confirm the phone and computer can communicate over the same network; guest Wi-Fi, VPNs, or client isolation may prevent device-to-device connections.
5. If the app authenticates directly with Keycloak, make sure the phone can also reach the Keycloak endpoint used by the app and that the configured issuer/authority matches the issuer in the tokens. The current Compose file binds Keycloak to `127.0.0.1:8080`, which makes it host-only; that binding must be deliberately changed for LAN access, with appropriate firewall restrictions.

Android may also block cleartext HTTP depending on the app's target SDK and network security configuration. If HTTP is enabled for local testing, restrict that setting to the development build; otherwise, use HTTPS with a certificate trusted by the Android device.

This setup is intended for local development over a trusted network. Do not expose development services or credentials to an untrusted network or use this HTTP configuration in production.
