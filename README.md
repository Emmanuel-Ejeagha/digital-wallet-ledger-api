# Digital Wallet – Bank‑Grade Ledger API

A production‑ready digital wallet and double‑entry ledger system built with **.NET 10**, **Clean Architecture**, **PostgreSQL**, and **Auth0**. Designed for financial accuracy, idempotency, and regulatory compliance.

---

## Features

- ✅ **Double‑entry accounting** – every transaction creates balanced debit/credit entries.
- ✅ **Idempotent operations** – prevent duplicate processing with idempotency keys.
- ✅ **Auth0 integration** – secure, passwordless authentication with OAuth2 / OpenID Connect.
- ✅ **Real‑time email notifications** – confirm transfers to sender and receiver.
- ✅ **Multi‑currency support** – USD, EUR, GBP, NGN with proper decimal precision.
- ✅ **Wallet operations** – create accounts, deposit, withdraw, transfer.
- ✅ **Transaction history** – paginated, filterable by date range.
- ✅ **Admin endpoints** – manage users, view system accounts.
- ✅ **Webhook support** – for Auth0 user signup and external payment providers.
- ✅ **Global error handling** – consistent JSON error responses.
- ✅ **API versioning** – future‑proof your API.
- ✅ **Swagger UI** – interactive documentation with JWT support.
- ✅ **Dockerised** – run with PostgreSQL in containers.

---

## Technology Stack

| Layer          | Technology                                |
|----------------|-------------------------------------------|
| **Domain**     | .NET 10 (C#) – no external dependencies   |
| **Application**| .NET 10, MediatR, FluentValidation, AutoMapper |
| **Infrastructure** | .NET 10, Entity Framework Core, Npgsql |
| **API**        | .NET 10, ASP.NET Core, Auth0, Swashbuckle |
| **Database**   | PostgreSQL 15+ (ACID, row‑level locking)  |
| **Container**  | Docker, Docker Compose                     |
| **Testing**    | xUnit, Moq, Testcontainers                 |

---

## Clean Architecture Overview

```
src/
├── DigitalWallet.Domain          # Entities, value objects, domain services
├── DigitalWallet.Application      # Use cases, DTOs, interfaces, behaviours
├── DigitalWallet.Infrastructure   # EF Core, repositories, email, idempotency
├── DigitalWallet.API              # Controllers, middleware, Auth0 config
tests/
├── Domain.UnitTests
├── Application.UnitTests
├── Infrastructure.IntegrationTests
└── API.IntegrationTests
```

- **Domain** has **zero dependencies**.
- **Application** depends only on Domain.
- **Infrastructure** implements Application interfaces.
- **API** composes everything and handles HTTP.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (with Docker Compose)
- [PostgreSQL](https://www.postgresql.org/) (if running locally without Docker)
- [Auth0 account](https://auth0.com/) (free tier is sufficient)
- An SMTP server (e.g., Gmail, SendGrid) for email notifications

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/digital-wallet.git
cd digital-wallet
```

### 2. Configure Auth0

1. Create a new **API** in Auth0 with identifier `https://digital-wallet-api` (or your own).
2. Create a **Machine to Machine Application** for the API (or use the default one).
3. Note the **Domain**, **Client ID**, and **Audience**.
4. In Auth0, add roles (`User`, `Admin`, `Compliance`) and assign them to users.

### 3. Configure Email (SMTP)

Obtain SMTP credentials. For Gmail, you may need an [App Password](https://support.google.com/accounts/answer/185833).

### 4. Update Configuration

Copy `appsettings.json` to `appsettings.Development.json` (or modify directly). Fill in your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=WalletLedger;Username=postgres;Password=yourpassword"
  },
  "Auth0": {
    "Domain": "your-tenant.auth0.com",
    "ClientId": "your-client-id",
    "Audience": "https://your-api-identifier"
  },
  "SmtpSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "UseSsl": true,
    "FromEmail": "noreply@digitalwallet.com",
    "FromName": "Digital Wallet"
  },
  "WebhookApiKey": "a-very-secret-key"
}
```

### 5. Run Database Migrations (if using local PostgreSQL)

```bash
dotnet ef database update --project src/DigitalWallet.Infrastructure --startup-project src/DigitalWallet.API
```

### 6. Run the Application

```bash
dotnet run --project src/DigitalWallet.API
```

The API will be available at `https://localhost:5001`. Swagger UI at `https://localhost:5001/swagger`.

---

## Docker Setup

The easiest way to run the entire stack:

```bash
docker-compose up -d
```

This starts:
- PostgreSQL container (port `5432`)
- API container (port `8080`)

The database is automatically migrated and seeded on startup.

To stop:

```bash
docker-compose down
```

To remove volumes (reset database):

```bash
docker-compose down -v
```

---

## API Documentation

Once running, visit `http://localhost:8080/swagger` (or HTTPS version).  
Click **Authorize** and enter your Auth0 JWT token (format: `Bearer <token>`).

### Key Endpoints

| Endpoint                          | Method | Description                          | Auth Required |
|-----------------------------------|--------|--------------------------------------|---------------|
| `/api/v1/auth/config`             | GET    | Get Auth0 configuration              | ❌            |
| `/api/v1/wallet/accounts`         | POST   | Create a new wallet account          | ✅ (User)     |
| `/api/v1/wallet/accounts/{id}/balance` | GET | Get account balance                | ✅ (User)     |
| `/api/v1/transaction/transfer`    | POST   | Transfer between accounts            | ✅ (User)     |
| `/api/v1/transaction/deposit`     | POST   | Deposit from system reserve          | ✅ (User)     |
| `/api/v1/transaction/withdraw`    | POST   | Withdraw to system payout            | ✅ (User)     |
| `/api/v1/transaction/history/{accountId}` | GET | Get transaction history           | ✅ (User)     |
| `/api/v1/admin/users`             | GET    | List all users (admin only)          | ✅ (Admin)    |

---

## Testing

Run unit and integration tests with:

```bash
dotnet test
```

Integration tests use **Testcontainers** to spin up a real PostgreSQL instance, ensuring full database fidelity.

---

## Deployment to Production

1. **Build Docker image**:
   ```bash
   docker build -t digital-wallet-api -f docker/Dockerfile .
   ```

2. **Push to your container registry**.

3. **Set environment variables** for production (never commit secrets):
   - `ConnectionStrings__DefaultConnection`
   - `Auth0__Domain`
   - `Auth0__Audience`
   - `SmtpSettings__*`
   - `WebhookApiKey`

4. **Use a reverse proxy** (nginx, Traefik) to handle TLS.

5. **Ensure database** is backed up regularly and monitored.

---

## Contributing

We welcome contributions! Please follow these guidelines:

- Use **Clean Architecture** – do not leak infrastructure concerns into Domain/Application.
- Write **unit tests** for domain logic and **integration tests** for database operations.
- Ensure **idempotency** for all write operations.
- Keep **migrations** in Infrastructure project.
- Document new endpoints in Swagger (via attributes).
- Run `dotnet format` before committing.

---

## License

This project is licensed under the **MIT License**. See [LICENSE](LICENSE) for details.

---

## Acknowledgements

- Built with [.NET 10](https://dotnet.microsoft.com/)
- Identity by [Auth0](https://auth0.com/)
- Database by [PostgreSQL](https://www.postgresql.org/)
- Inspired by Martin Fowler’s [Clean Architecture](https://martinfowler.com/articles/microservices.html)

---