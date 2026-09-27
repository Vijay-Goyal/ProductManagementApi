# Product Management API

A RESTful Product Management Web API built with ASP.NET Core and .NET 8.

The project provides product CRUD operations, JWT-based authentication, refresh tokens, SQL Server persistence using Entity Framework Core, automated tests, Swagger/OpenAPI documentation, and Docker containerization.

## Features

- Product CRUD operations
- SQL Server database
- Entity Framework Core
- Repository Pattern
- JWT authentication
- Refresh token support
- Protected product endpoints
- Input validation using Data Annotations
- Swagger/OpenAPI documentation
- Unit tests with xUnit and Moq
- Integration testing with WebApplicationFactory
- Docker and Docker Compose support
- EF Core database migrations

## Technology Stack

- **C#**
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server 2022**
- **JWT Bearer Authentication**
- **Swagger / OpenAPI**
- **xUnit**
- **Moq**
- **Docker**
- **Docker Compose**

## Project Structure

```text
ProductManagementApi
│
├── ProductApi
│   ├── Controllers
│   ├── Data
│   ├── Migrations
│   ├── Models
│   ├── Repositories
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
│
├── ProductApi.Tests
│   ├── AuthControllerTests.cs
│   ├── ProductsControllerTests.cs
│   └── ProductsApiIntegrationTests.cs
│
├── docker-compose.yml
├── docker-compose.override.yml
├── ProductApi.sln
└── .gitignore
```

## API Endpoints

### Authentication

| Method | Endpoint             | Description                              |
| ------ | -------------------- | ---------------------------------------- |
| POST   | `/api/Auth/register` | Register a new user                      |
| POST   | `/api/Auth/login`    | Login and receive JWT + refresh token    |
| POST   | `/api/Auth/refresh`  | Generate a new JWT using a refresh token |

### Products

| Method | Endpoint             | Description         |
| ------ | -------------------- | ------------------- |
| GET    | `/api/Products`      | Get all products    |
| GET    | `/api/Products/{id}` | Get a product by ID |
| POST   | `/api/Products`      | Create a product    |
| PUT    | `/api/Products/{id}` | Update a product    |
| DELETE | `/api/Products/{id}` | Delete a product    |

The product endpoints require JWT authentication.

## Running with Docker

### Prerequisites

Install:

- Docker Desktop
- Git

### Environment Variables

The project uses environment variables for sensitive Docker configuration.

Create a `.env` file in the project root:

```text
MSSQL_SA_PASSWORD=YourStrongPassword
JWT_KEY=YourSecretJwtKey
```

The `.env` file is intentionally excluded from Git using `.gitignore`.

### Start the application

From the project root:

```powershell
docker compose up -d --build
```

Check the running containers:

```powershell
docker compose ps
```

The application runs as two Docker services:

- `productapi`
- `sqlserver`

### Stop the application

```powershell
docker compose down
```

## Swagger

After starting the Docker containers, open the Swagger UI using the HTTP port shown by:

```powershell
docker compose ps
```

The URL will be similar to:

```text
http://localhost:<port>/swagger
```

Swagger can be used to register users, login, obtain a JWT, authorize requests, and test the Product CRUD endpoints.

## Authentication Flow

1. Register a user using `/api/Auth/register`.
2. Login using `/api/Auth/login`.
3. The API returns an access token and refresh token.
4. Use the JWT access token to authorize protected Product endpoints.
5. When the access token expires, the refresh token can be used through `/api/Auth/refresh`.

## Database

The application uses:

- SQL Server
- Entity Framework Core
- Code First migrations

The Docker Compose configuration creates a SQL Server container with persistent storage using a Docker volume.

EF Core migrations are stored in:

```text
ProductApi/Migrations/
```

## Testing

Run all automated tests from the solution root:

```powershell
dotnet test .\ProductApi.sln
```

The test project contains:

- Controller unit tests
- Authentication tests
- Integration tests using `WebApplicationFactory`

## Repository Pattern

The Product API uses the Repository Pattern to separate controller logic from database access.

The repository abstraction is defined in:

```text
ProductApi/Repositories/IProductRepository.cs
```

and implemented by:

```text
ProductApi/Repositories/ProductRepository.cs
```

This improves separation of concerns and makes the application easier to test.

## Security Notes

- Passwords are securely hashed using ASP.NET Core Identity's `PasswordHasher<User>`.
- Password hashes are never returned in API responses.
- JWT signing keys are stored outside the committed `appsettings.json`.
- Docker secrets are supplied through environment variables.
- `.env` is excluded from Git using `.gitignore`.

## Future Improvements

- Secure password hashing
- Refresh token rotation and revocation improvements
- Additional API validation
- More comprehensive integration tests
- Production-ready configuration and deployment

## Author

**Vijay Goyal**

GitHub: [Vijay-Goyal](https://github.com/Vijay-Goyal)
