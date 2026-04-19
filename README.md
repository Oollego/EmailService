# EmailService

Asynchronous email microservice built with ASP.NET Core 9. Accepts messages via Azure Service Bus, renders HTML templates using Razor, and delivers emails via SMTP with automatic retry and idempotency support.

## Architecture

Clean Architecture with background processing:
API Request → Azure Service Bus → EmailWorker → Template Rendering → SMTP
↓
SQL Server (EmailLog)
↓
EmailRetryWorker (on failure)

## Features

- **Azure Service Bus** — decoupled async message consumption
- **Razor Templates** — HTML email rendering via RazorLight with CSS inlining (PreMailer.Net)
- **Retry with Exponential Backoff** — 1 → 5 → 15 → 30 → 60 minutes between attempts
- **Idempotency** — prevents duplicate sends via unique key constraint
- **Email Persistence** — all attempts logged to SQL Server with status tracking
- **Strategy Pattern** — dynamic handler resolution by email type
- **Background Workers** — `EmailWorker` (queue consumer) + `EmailRetryWorker` (retry scheduler)

## Tech Stack

| Technology | Version |
|---|---|
| .NET / ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0 |
| SQL Server | — |
| Azure Service Bus | 7.20.1 |
| RazorLight | 2.3.1 |
| PreMailer.Net | 2.7.2 |

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/email/bus/send` | Publish email message to Service Bus queue |
| `POST` | `/api/email/preview/verification` | Preview rendered HTML email template |

### Send Email Request

```json
POST /api/email/bus/send
{
  "to": "user@example.com",
  "type": "Verification",
  "userName": "John",
  "title": "Confirm your email",
  "message": "Please confirm your account",
  "actionUrl": "https://example.com/confirm?token=abc123",
  "idempotencyKey": "unique-request-id"
}
Email Types
Type	        Description	                                     Fields
Verification	Email confirmation	                             UserName, Title, Message, ActionUrl, ButtonText
Transaction	  Transaction notification	                       UserName, Title, Message, Code

Retry Logic
Failed emails are rescheduled automatically:
Attempt 1 → wait 1 min  → Attempt 2
Attempt 2 → wait 5 min  → Attempt 3
Attempt 3 → wait 15 min → Attempt 4
Attempt 4 → wait 30 min → Attempt 5
Attempt 5 → wait 60 min → dead
Max retry count is configurable via EmailOptions:MaxRetryCount.

Configuration
Copy appsettings.json and fill in your values:


{
  "ConnectionStrings": {
    "EmailDb": "Server=.;Database=EmailServiceDb;Trusted_Connection=true;"
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UserName": "your@email.com",
    "Password": "your_password",
    "EnableSsl": true
  },
  "ServiceBus": {
    "ConnectionString": "your_service_bus_connection_string",
    "QueueName": "mailservice",
    "MaxConcurrentCalls": 5
  },
  "EmailOptions": {
    "MaxRetryCount": 3,
    "RetryIntervalSeconds": 30,
    "SendDelayMilliseconds": 100
  },
  "Templates": {
    "TemplatePath": "Infrastructure/Templates",
    "Extension": ".cshtml"
  }
}
Note: appsettings.json in this repo contains test credentials for local development only.
In production use environment variables or Azure Key Vault.

Getting Started

# Clone
git clone https://github.com/Oollego/EmailService.git
cd EmailService

# Apply migrations
dotnet ef database update

# Run
dotnet run
Swagger UI available at https://localhost:{port}/swagger in Development mode.

Project Structure

EmailService/
├── Application/
│   ├── DTO/                  # EmailBusDto, EmailPreviewRequest
│   ├── Handlers/             # TransactionHandler, VerificationHandler
│   ├── Interfaces/           # Service and repository contracts
│   ├── Models/               # Template view models
│   └── Services/             # EmailProcessorService, EmailSendService
├── Configuration/            # Options: Smtp, ServiceBus, Email, Templates
├── Contracts/                # EmailMessage, EmailType enum
├── Controllers/              # EmailBusController, EmailPreviewController
├── Domain/
│   ├── Entities/             # EmailLog with retry scheduling
│   └── ValueObjects/         # EmailAddress, EmailBody
├── Infrastructure/
│   ├── Persistence/          # EF Core DbContext, EmailRepository
│   ├── ServiceBus/           # Publisher, Consumer
│   └── Templates/            # Razor templates (.cshtml)
└── Worker/
    ├── EmailWorker.cs        # Service Bus queue consumer
    └── EmailRetryWorker.cs   # Retry scheduler
