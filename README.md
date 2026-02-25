# Prompt Processor

A system for submitting prompts to Claude AI and tracking their processing status in real-time.

## Stack

- **Backend**: ASP.NET Core (.NET 9), Entity Framework Core, PostgreSQL
- **Message Queue**: MassTransit + RabbitMQ
- **LLM**: Claude via Anthropic.SDK
- **Frontend**: Next.js 16, TypeScript, Tailwind CSS

## Architecture
```
POST /api/prompts → saves to DB (Pending) → publishes to RabbitMQ
PromptJobConsumer → picks up message → calls Claude API → updates status
Frontend → polls GET /api/prompts every 3s → displays live status
```

## Prerequisites

- Docker Desktop
- .NET 9 SDK
- Node.js 18+

## Quick Start

1. Clone the repository
2. Create `.env` file in the root:
```
ANTHROPIC_API_KEY=your_api_key_here
```

3. Start infrastructure:
```bash
docker-compose up postgres rabbitmq -d
```

4. Start backend:
```bash
dotnet run --project src/PromptProcessor.API
```

5. Start frontend:
```bash
cd frontend
npm install
npm run dev
```

6. Open http://localhost:3000

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/prompts | Submit a new prompt |
| GET | /api/prompts | Get all prompts with status |
| GET | /api/prompts/{id} | Get prompt by ID |

## Prompt Status Lifecycle

`Pending` → `Processing` → `Completed` / `Failed`