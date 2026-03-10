# Prompt Processor

A system for submitting prompts to Claude AI and tracking their processing status in real-time.

## Stack

- **Backend**: ASP.NET Core (.NET 9), Entity Framework Core, PostgreSQL
- **Message Queue**: MassTransit + RabbitMQ
- **LLM**: Claude via Anthropic.SDK
- **Frontend**: Next.js 16, TypeScript, Tailwind CSS

## Architecture

```
POST /api/prompts → SubmitPromptCommand (MediatR) → saves to DB (Pending) → publishes to RabbitMQ
PromptJobConsumer → picks up message → calls ILlmService (Claude API) → updates status via domain methods
Frontend → polls GET /api/prompts every 3s → displays live status
```

### Project Structure

```
src/
├── PromptProcessor.Domain/          # Entities, interfaces, domain exceptions
├── PromptProcessor.Application/     # MediatR commands/queries, DTOs, validators, pipeline behaviors
├── PromptProcessor.Infrastructure/  # EF Core, repository, Anthropic client, MassTransit consumer
└── PromptProcessor.API/             # Controllers, exception middleware, Program.cs

tests/
└── PromptProcessor.Tests/           # Domain, application, and validator tests (xUnit)
```

## Quick Start (Docker)

**Requirements:** Docker Desktop

1. Clone the repository
2. Create a `.env` file in the root directory:
```
ANTHROPIC_API_KEY=your_api_key_here
```
3. Start everything:
```bash
docker-compose up
```
4. Open http://localhost:3000

This single command starts PostgreSQL, RabbitMQ, the backend API, and the frontend.

## Manual Setup (Alternative)

**Requirements:** .NET 9 SDK, Node.js 18+, Docker Desktop (for infrastructure only)

1. Start infrastructure:
```bash
docker-compose up postgres rabbitmq -d
```
2. Start backend:
```bash
dotnet run --project src/PromptProcessor.API
```
3. Start frontend:
```bash
cd frontend
npm install
npm run dev
```
4. Open http://localhost:3000

## Running Tests

```bash
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/prompts | Submit a new prompt |
| GET | /api/prompts | Get all prompts with status |
| GET | /api/prompts/{id} | Get prompt by ID |

Errors are returned as `ProblemDetails` (RFC 9110). Validation failures return HTTP 400 with field-level error details.

## Prompt Status Lifecycle

`Pending` → `Processing` → `Completed` / `Failed`
