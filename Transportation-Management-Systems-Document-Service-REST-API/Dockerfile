# --- Stage 1: Build the Application --- #
FROM gradle:9-jdk25 AS builder
WORKDIR /builder

# Cache dependencies by copying Gradle configuration files first
COPY gradlew .
COPY gradle gradle
COPY build.gradle .
COPY settings.gradle .

# Download dependencies (this layer will be cached unless build files change)
RUN ./gradlew dependencies --no-daemon

# Copy source code and build the executable bootJar
COPY src src
RUN ./gradlew bootJar --no-daemon

# --- Stage 2: Runtime Environment --- #
FROM eclipse-temurin:25.0.5_8-jre-alpine-3.24
WORKDIR /app

# Copy the built jar from the builder stage
COPY --from=builder /builder/build/libs/*.jar app.jar

EXPOSE 9000

# Execute the application
ENTRYPOINT [ "java", "-jar", "app.jar" ]