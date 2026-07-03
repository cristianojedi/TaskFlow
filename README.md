# TaskFlow API

API de gerenciamento de tarefas (to-do list) construída em **.NET 10**, como projeto de estudo para consolidação de conhecimentos em fluxo de desenvolvimento e DevOps — o foco não é a regra de negócio, e sim o ciclo completo: aplicação → containerização → deploy → automação (CI/CD) → infraestrutura como código.

> **Status:** Fase 0 concluída — aplicação base funcional, testada e rodando localmente com persistência em SQLite. Próximas fases (Docker, CI/CD, deploy AWS, Terraform) ainda não implementadas.

## Stack

- **.NET 10** — ASP.NET Core Web API (Minimal API)
- **Entity Framework Core** — ORM, com **SQLite** como banco de dados
- **Serilog** — logging estruturado (console, formato JSON)
- **Swagger / OpenAPI** (Swashbuckle) — documentação interativa em desenvolvimento
- **xUnit** + **WebApplicationFactory** — testes unitários e de integração
- **Health Checks** (`Microsoft.Extensions.Diagnostics.HealthChecks`)

## Arquitetura

Organização em camadas, cada uma como um projeto `.csproj` separado, reforçando a regra de dependência em tempo de compilação:

```
TaskFlow.Api ──────► TaskFlow.Application ──────► TaskFlow.Domain
     │                        ▲
     └────────► TaskFlow.Infrastructure ──────────────┘
```

- **TaskFlow.Domain** — entidade `TaskItem` e regras de negócio. Sem dependências de outros projetos.
- **TaskFlow.Application** — casos de uso (`TaskService`), interfaces (`ITaskRepository`) e DTOs. Depende só de `Domain`.
- **TaskFlow.Infrastructure** — `DbContext`, migrations e implementação do repositório com EF Core.
- **TaskFlow.Api** — Minimal API: endpoints, composição de DI, pipeline HTTP, Swagger, health check.
- **TaskFlow.Tests** — testes unitários e de integração (xUnit).

## Estrutura de pastas

```
TaskFlow/
├── TaskFlow.slnx
├── src/
│   ├── TaskFlow.Api/
│   ├── TaskFlow.Application/
│   ├── TaskFlow.Domain/
│   └── TaskFlow.Infrastructure/
│       └── Migrations/
└── tests/
    └── TaskFlow.Tests/
        ├── Domain/
        ├── Application/
        └── Integration/
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/tasks` | Lista todas as tarefas |
| GET | `/tasks/{id}` | Busca tarefa por id (404 se não existir) |
| POST | `/tasks` | Cria uma nova tarefa (400 se `title` vazio) |
| PUT | `/tasks/{id}` | Atualiza uma tarefa existente (404 se não existir) |
| DELETE | `/tasks/{id}` | Remove uma tarefa (404 se não existir) |
| GET | `/health` | Health check da aplicação (verifica acesso ao SQLite) |

Documentação interativa via Swagger UI disponível em `/swagger` quando rodando em ambiente de desenvolvimento.

## Como rodar localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Ferramenta `dotnet-ef` (para gerenciar migrations): `dotnet tool install --global dotnet-ef`

### Rodando a API

```bash
git clone https://github.com/cristianojedi/TaskFlow.git
cd TaskFlow
dotnet run --project src/TaskFlow.Api
```

Ao iniciar, a aplicação aplica automaticamente as migrations pendentes e cria o arquivo `taskflow.db` (SQLite) caso não exista. A API sobe por padrão nas portas configuradas em `src/TaskFlow.Api/Properties/launchSettings.json`, com Swagger disponível em `/swagger`.

### Rodando os testes

```bash
dotnet test
```

## Roadmap do estudo

- [x] **Fase 0** — Aplicação base (CRUD, EF Core + SQLite, Swagger, Serilog, testes)
- [ ] **Fase 1** — Containerização (Docker)
- [ ] **Fase 2** — Deploy manual (AWS ECR/EC2)
- [ ] **Fase 3** — CI/CD (GitHub Actions)
- [ ] **Fase 4** — Infraestrutura como código (Terraform)

## Fora do escopo

Este projeto propositalmente **não** implementa: autenticação/autorização, multi-tenancy, interface gráfica, paginação/filtros avançados, cache distribuído, mensageria, múltiplos ambientes de banco gerenciado, testes de carga, ou observabilidade avançada (APM/tracing distribuído).

## Licença

Distribuído sob a licença MIT. Veja [LICENSE](LICENSE) para mais detalhes.
