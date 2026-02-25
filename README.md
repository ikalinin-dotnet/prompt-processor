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

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/prompts | Submit a new prompt |
| GET | /api/prompts | Get all prompts with status |
| GET | /api/prompts/{id} | Get prompt by ID |

## Prompt Status Lifecycle

`Pending` → `Processing` → `Completed` / `Failed`