## Why

O projeto TaskFlow ainda não existe em código — apenas a definição em `CLAUDE.md` e o guia de estudo. A Fase 0 do guia DevOps exige uma aplicação base funcional, testada e rodando localmente antes de avançar para containerização (Fase 1). Sem essa base, nenhuma das fases seguintes (Docker, CI/CD, deploy, Terraform) tem o que empacotar ou automatizar.

## What Changes

- Criar a solução `.slnx` com os projetos em camadas: `TaskFlow.Api`, `TaskFlow.Application`, `TaskFlow.Domain`, `TaskFlow.Infrastructure`, `TaskFlow.Tests`.
- Definir a entidade de domínio `TaskItem` (`Id`, `Title`, `Description`, `IsDone`, `CreatedAt`, `DueDate`).
- Configurar `DbContext` (EF Core) com SQLite (`Data Source=taskflow.db`) e gerar a migration inicial (`InitialCreate`).
- Implementar os endpoints de CRUD como Minimal API: `GET /tasks`, `GET /tasks/{id}`, `POST /tasks`, `PUT /tasks/{id}`, `DELETE /tasks/{id}`.
- Adicionar endpoint `GET /health` usando `Microsoft.Extensions.Diagnostics.HealthChecks`.
- Adicionar Swagger/OpenAPI para documentação interativa em ambiente de desenvolvimento.
- Configurar Serilog para logging estruturado em console (formato JSON).
- Escrever testes unitários (validações de domínio/serviço) e testes de integração (mínimo 2 endpoints via `WebApplicationFactory`).

## Capacidades

### Novas Capacidades
- `task-management`: CRUD completo de tarefas (`TaskItem`) via Minimal API, com persistência em SQLite através de EF Core.
- `health-check`: Endpoint `/health` para verificação do estado da aplicação.

### Capacidades Modificadas
(nenhuma — projeto greenfield, não há specs existentes)

## Impacto

- **Código afetado**: todo o código-fonte novo em `src/TaskFlow.Api`, `src/TaskFlow.Application`, `src/TaskFlow.Domain`, `src/TaskFlow.Infrastructure`, e testes em `tests/TaskFlow.Tests`.
- **Dependências novas**: EF Core (+ provider SQLite), Serilog (+ sinks), Swashbuckle/Swagger, `Microsoft.Extensions.Diagnostics.HealthChecks`, xUnit + `Microsoft.AspNetCore.Mvc.Testing`.
- **Infraestrutura**: banco SQLite local (`taskflow.db`), sem dependências externas de rede.
- **Fases seguintes**: habilita a Fase 1 (Containerização), que dependerá desta aplicação já compilando e rodando localmente.
