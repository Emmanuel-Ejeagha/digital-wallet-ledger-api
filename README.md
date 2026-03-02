# Digital Wallet – Bank-Grade Ledger API

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
- ✅ **KYC verification** – submit identity documents, approve/reject via admin.
- ✅ **Admin endpoints** – manage users, view system accounts, reconcile ledger.
- ✅ **Webhook support** – for Auth0 user signup and external payment providers.
- ✅ **Global error handling** – consistent JSON error responses.
- ✅ **API versioning** – future‑proof your API.
- ✅ **Swagger UI** – interactive documentation with JWT support.
- ✅ **Dockerised** – run with PostgreSQL in containers.
- ✅ **Comprehensive tests** – unit and integration tests with Testcontainers.

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
| **Testing**    | xUnit, Moq, Testcontainers, FluentAssertions |

---

## Architecture Overview

```
┌─────────────────────────────────────┐
│           API (Controllers)         │
├─────────────────────────────────────┤
│         Application (Use Cases)      │
├─────────────────────────────────────┤
│            Domain (Entities)         │
└─────────────────────────────────────┘
         ▲              ▲
         │              │
┌─────────────────────────────────────┐
│       Infrastructure (EF, Repos)     │
└─────────────────────────────────────┘
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

1. Create a new **API** in Auth0 with identifier `https://digital-wallet-api`.
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
    "DefaultConnection": "Host=localhost;Port=5432;Database=DigitalWallet;Username=postgres;Password=yourpassword"
  },
  "Auth0": {
    "Domain": "your-tenant.auth0.com",
    "ClientId": "your-client-id",
    "Audience": "https://digital-wallet-api"
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
  "FileStorage": {
    "BasePath": "/app/uploads"
  },
  "WebhookApiKey": "your-secure-webhook-key"
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
cd docker
docker compose up -d
```

This starts:
- PostgreSQL container (port `5432`)
- API container (port `8080`)

The database is automatically migrated and seeded on startup.

To stop:

```bash
docker compose down
```

To remove volumes (reset database):

```bash
docker compose down -v
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
| `/api/v1/kyc/submit`              | POST   | Submit KYC documents (multipart)     | ✅ (User)     |
| `/api/v1/kyc/status`              | GET    | Get current user's KYC status        | ✅ (User)     |
| `/api/v1/admin/users`             | GET    | List all users (admin only)          | ✅ (Admin)    |
| `/api/v1/admin/kyc-submissions`   | GET    | List KYC submissions (admin only)    | ✅ (Admin)    |
| `/api/v1/admin/reconcile`         | POST   | Run ledger reconciliation (admin)    | ✅ (Admin)    |

---

## Testing

Run all tests (unit + integration):

```bash
dotnet test
```

Integration tests use **Testcontainers** to spin up a real PostgreSQL instance, ensuring full database fidelity. The first run may take a few minutes to download the PostgreSQL image.

---

## Deployment to Render

1. Push your code to a GitHub repository.
2. Create a new **Web Service** on [Render](https://render.com) connected to your repo.
3. Use the following settings:
   - **Environment**: Docker
   - **Build Command**: (leave blank – Render uses your Dockerfile)
   - **Start Command**: (leave blank)
4. Add the environment variables listed in `appsettings.json` (use double underscores for nesting, e.g., `ConnectionStrings__DefaultConnection`).
5. Create a **PostgreSQL** database on Render and set the `ConnectionStrings__DefaultConnection` to its internal URL.
6. Deploy.

Your API will be live at `https://your-app.onrender.com`. Swagger is available at `/swagger`.

---

## Contributing

We welcome contributions! Please follow these guidelines:

- Use **Clean Architecture** – do not leak infrastructure concerns into Domain/Application.
- Write **unit tests** for domain logic and **integration tests** for database operations.
- Ensure **idempotency** for all write operations.
- Keep **migrations** in Infrastructure project.
- Document new endpoints in Swagger (via XML comments).
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
