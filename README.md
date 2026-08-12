# EmailService

Asynchronous email microservice built with ASP.NET Core 9. Accepts messages via Azure Service Bus, renders HTML templates using Razor, and delivers emails via SMTP with automatic retry and idempotency support.

## 🏗️ Architecture

Clean Architecture with background processing:

```
API Request → Azure Service Bus → EmailWorker → Template Rendering → SMTP
                              ↓
                      SQL Server (EmailLog)
                              ↓
                    EmailRetryWorker (on failure)
```

### Layers

- **Controllers** — REST endpoints (publish to bus, preview templates)
- **Application** — handlers, services, interfaces
- **Domain** — `EmailLog` entity with retry scheduling logic
- **Infrastructure** — EF Core, Service Bus, Razor templates, SMTP
- **Worker** — Background services for message processing and retry logic

## ✨ Features

- **Azure Service Bus** — Decoupled async message consumption
- **Razor Templates** — HTML email rendering via RazorLight with CSS inlining (PreMailer.Net)
- **Retry with Exponential Backoff** — 1 → 5 → 15 → 30 → 60 minutes between attempts
- **Idempotency** — Prevents duplicate sends via unique key constraint
- **Email Persistence** — All attempts logged to SQL Server with status tracking
- **Strategy Pattern** — Dynamic handler resolution by email type
- **Background Workers** — `EmailWorker` (queue consumer) + `EmailRetryWorker` (retry scheduler)
- **Multi-language Support** — Template resolution based on language parameter

## 🛠️ Tech Stack

| Technology | Version |
|------------|---------|
| .NET / ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0 |
| SQL Server | — |
| Azure Service Bus | 7.20.1 |
| RazorLight | 2.3.1 |
| PreMailer.Net | 2.7.2 |
| Swashbuckle.AspNetCore | 9.0.6 |

## 📡 API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/email/bus/send` | Publish email message to Service Bus queue |
| `POST` | `/api/email/preview/verification` | Preview rendered HTML email template |

### Send Email Request

```bash
POST /api/email/bus/send
Content-Type: application/json
```

```json
{
  "type": "Verification",
  "to": "user@example.com",
  "subject": "Confirm your email",
  "language": "en",
  "data": {
    "Name": "John Doe",
    "Title": "Confirm your email",
    "Message": "Please confirm your account",
    "ActionUrl": "https://example.com/confirm?token=abc123",
    "ButtonText": "Confirm Email"
  }
}
```

### Email Types

| Type | Description | Template Fields |
|------|-------------|-----------------|
| Verification | Email confirmation | UserName, Title, Message, ActionUrl, ButtonText |
| Transaction | Transaction notification | UserName, Title, Message, Code |

## 🔄 Retry Logic

Failed emails are rescheduled automatically with exponential backoff:

```
Attempt 1 → wait 1 min  → Attempt 2
Attempt 2 → wait 5 min  → Attempt 3
Attempt 3 → wait 15 min → Attempt 4
Attempt 4 → wait 30 min → Attempt 5
Attempt 5 → wait 60 min → dead
```

Max retry count is configurable via `EmailOptions:MaxRetryCount`.

## ⚙️ Configuration

### Requirements

- .NET 9.0 SDK
- SQL Server (local or Azure)
- Azure Service Bus namespace
- SMTP server access

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Oollego/EmailService.git
   cd EmailService
   ```

2. **Configure appsettings.json**
   ```json
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
   ```

   > **Note:** `appsettings.json` in this repo contains test credentials for local development only. In production use environment variables or Azure Key Vault.

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   ```
   https://localhost:{port}/swagger
   ```
   (Available in Development mode only)

## 📁 Project Structure

```
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
│   ├── Extensions/           # DI configuration, Swagger setup
│   ├── Persistence/          # EF Core DbContext, EmailRepository
│   ├── ServiceBus/           # Publisher, Consumer
│   └── Templates/            # Razor templates (.cshtml)
├── Migrations/               # Database migrations
├── Worker/                   # Background services
│   ├── EmailWorker.cs        # Service Bus queue consumer
│   └── EmailRetryWorker.cs   # Retry scheduler
└── Program.cs                # Application entry point
```

## 🔧 Development

### Adding New Email Type

1. Create new enum value in `Contracts/Enums/EmailType.cs`
2. Create handler in `Application/Handlers/` inheriting from `BaseEmailHandler<T>`
3. Register handler in `Infrastructure/Extensions/ServiceCollectionExtensions.cs`
4. Create template in `Infrastructure/Templates/{TypeName}/`

### Database Migration

```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update
```

## 🚀 Production Considerations

- **Security**: Use Azure Key Vault or environment variables for secrets
- **Monitoring**: Add Health Checks, Application Insights, or OpenTelemetry
- **Scaling**: Configure `MaxConcurrentCalls` based on your throughput needs
- **Error Handling**: Consider implementing dead-letter queue monitoring
- **Rate Limiting**: Add rate limiting to API endpoints to prevent abuse
