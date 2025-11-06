Project: ProductManager (.NET 8, FastEndpoints, EF Core, RabbitMQ)

Scope
- This document captures project-specific build, configuration, testing, and development notes for advanced contributors. It omits generic .NET guidance.

1) Build and Configuration
- Solution layout
  - ProductManager.Api: ASP.NET Core API using FastEndpoints and a background consumer.
  - ProductManager.Application: Endpoint handlers (FastEndpoints) and application logic.
  - ProductManager.Domain: Entities, DTOs, interfaces. Must not depend on other layers (enforced by tests).
  - ProductManager.Infrastructure: EF Core (SQL Server) and RabbitMQ implementations; DI registration and migrations.
  - ProductManager.Tests: xUnit test suite (unit + architecture rules).

- Target framework: net8.0 (see global.json and csproj files).

- Dependency Injection and startup
  - InfrastructureDependency.RegisterInfrastructureDependencies registers:
    - DbContext ProductContext using SQL Server.
    - Repositories: ICategoryRepository, IProductRepository, IProductEventsRepository.
    - Queue events: IQueueEvents implemented by RabbitMqQueueEvents as a Singleton.
  - Program.cs bootstraps FastEndpoints (assemblies scanned include Application’s handlers) and registers ProductCreatedConsumer as a hosted service.
  - Migrations: On API startup, Program.cs invokes app.Services.ExecuteDataBaseMigrationAsync(); which calls dbContext.Database.MigrateAsync(). Ensure DB configuration is valid before starting API.

- Required environment configuration
  - Database connection string is resolved in InfrastructureDependency via:
    - Environment variable DB_CONNECTION, or
    - appsettings ConnectionStrings:DefaultConnection.
  - If missing or empty, startup throws InvalidOperationException("Database can't be configured.").
  - Example SQL Server connection string (used in compose):
    Server=sqlserver,1433;Database=ProductManager;User ID=sa;Password=1q2w3e4r@#$;TrustServerCertificate=True;

  - RabbitMQ configuration (RabbitMqQueueEvents reads from IConfiguration):
    - RabbitMq:Host (default "localhost")
    - RabbitMq:Username (default "guest")
    - RabbitMq:Password (default "guest")
    - RabbitMq:Port (optional integer)
  - In Docker, these are provided as env vars using double-underscore syntax (maps to RabbitMq:*)
    - RabbitMq__Host, RabbitMq__Username, RabbitMq__Password, RabbitMq__Port

- Running locally (without Docker)
  - Set DB_CONNECTION to a reachable SQL Server.
  - Optionally set RabbitMq:* if not using defaults.
  - Launch ProductManager.Api. On first run, EF Core applies migrations automatically.
  - The API enables Swagger via FastEndpoints.Swagger; HTTPS redirection is enabled.

- Running via Docker Compose
  - See compose.yaml at repo root. It provisions:
    - sqlserver (2022-latest), port 1433 exposed.
    - rabbitmq (3-management), ports 5672/15672 exposed.
    - productmanager.api built from ProductManager.Api/Dockerfile; configured with DB_CONNECTION and RabbitMq__* env vars and depends_on sqlserver and rabbitmq.
  - Quickstart:
    - docker compose up -d --build
    - Browse API at http://localhost:8080 (Swagger UI available) and RabbitMQ UI at http://localhost:15672 (guest/guest).
  - Data persistence uses named volumes: sqlserver_data, rabbitmq_data.

2) Testing
- Test framework: xUnit.
- How to run tests
  - From repository root: use the solution test run (Rider/Visual Studio) or CLI.
  - CLI examples (project/solution):
    - dotnet test ProductManager.sln -c Debug
    - dotnet test ProductManager.Tests/ProductManager.Tests.csproj -c Debug
  - Filter tests:
    - dotnet test ProductManager.Tests/ProductManager.Tests.csproj --filter FullyQualifiedName~ProductManager.Tests.Product

- Architecture rules
  - ProductManager.Domain must not depend on Application, Infrastructure, or Api (enforced by NetArchTest in ArchitectureTests.Domain_Should_Not_Depend_On_Other_Layers).
  - When adding new types, ensure references from Domain stay within Domain only.

- Unit test patterns used in this repo
  - Handlers tested in isolation using Moq for repositories and queues.
  - FastEndpoints handlers throw ValidationFailureException to signal validation errors; tests assert ThrowsAsync for those cases.
  - Repositories are validated via Verify() calls to ensure no persistence operations occur when preconditions fail.

- Adding new tests
  - Prefer creating tests under ProductManager.Tests/<Area> (e.g., Product, Category, Architecture).
  - For handler tests:
    - Mock ILogger<T>, repositories (e.g., IProductRepository), and IQueueEvents as needed.
    - Arrange repository responses, execute handler.HandleAsync(request, CancellationToken.None), then assert outcomes and Verify() side effects.
  - For architecture tests:
    - Extend NetArchTest rules carefully; keep Domain independent.

- Example: creating and running a simple test (validated on 2025-11-06)
  - Example file created temporarily: ProductManager.Tests/Demo/SanityTests.cs
    - Content:
      using Xunit;
      
      namespace ProductManager.Tests.Demo;
      
      public class SanityTests
      {
          [Fact]
          public void Always_Passes()
          {
              Assert.True(true);
          }
      }
  - Execution results (dotnet test full solution): All tests passed, including SanityTests.Always_Passes. We removed this file after verification per requirements.

3) Additional Development Notes
- FastEndpoints usage
  - Endpoints/Handlers live in ProductManager.Application.Endpoints.* namespaces. Program.cs registers FastEndpoints and points Assemblies to locate handlers (e.g., typeof(ProductCreateHandler).Assembly).
  - Handlers follow the request/response and validation flow typical to FastEndpoints and may throw ValidationFailureException for validation issues.

- Background processing and messaging
  - ProductCreatedConsumer (hosted service) subscribes to RabbitMqQueueEvents.ProductCreatedQueue and logs consumed ProductDto events. It persists raw event payloads (ProductEvents) using IProductEventsRepository.
  - RabbitMqQueueEvents declares queues on publish/subscribe, uses persistent messages, and manual acks. Failures Nack and requeue; logs include queue name context.

- Data access
  - SQL Server with EF Core; migrations run automatically on API startup. Ensure the DB user has rights to create/modify schema.
  - TrustServerCertificate=True is set in compose for simplicity; set appropriately in production.

- Configuration tips and pitfalls
  - If DB_CONNECTION is not set and no DefaultConnection exists, API will fail fast at startup with "Database can't be configured.".
  - When running locally against Dockerized SQL Server from Windows host, use Server=localhost,1433; ... for the API process running on host; when running inside compose, use Server=sqlserver,1433;.
  - RabbitMQ defaults to localhost/guest; override via RabbitMq:Host, Username, Password, Port if needed. In Docker, use RabbitMq__* env vars.

- Code style and conventions
  - C# 12 / .NET 8 features are acceptable.
  - Use DI for all infrastructure concerns; do not instantiate concrete repos or queue clients directly in handlers.
  - Keep Domain layer free of external dependencies and framework references.
  - Favor async APIs end-to-end; cancellation tokens are expected in handler methods and repository calls.

- Debugging tips
  - For queue-related issues, confirm RabbitMQ connectivity and queue existence; RabbitMqQueueEvents auto-declares queues but will fail if broker is unreachable.
  - For startup failures, log output will indicate configuration issues (e.g., connection string). Verify environment variables and appsettings.
  - To inspect consumed events, query the ProductEvents table (persisted by the consumer) after publishing a product-created event.

- Docker notes
  - Compose exposes: API 8080, RabbitMQ 5672/15672, SQL Server 1433.
  - Named volumes persist broker and database state between restarts. Use docker volume rm to reset if needed.

Changelog
- 2025-11-06: Document created. Verified test sample addition/removal and full test run succeeded (6/6 initially with demo test; 5/5 baseline).
