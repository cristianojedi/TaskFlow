# CLAUDE.md — TaskFlow API (.NET 10)

## Projeto (definição)

Projeto de estudo para consolidação de conhecimentos em fluxo de desenvolvimento e DevOps, com foco em entrevista técnica para vaga de Desenvolvedor Sênior .NET.

O sistema é uma API de gerenciamento de tarefas (to-do list), a **TaskFlow API**. O domínio é propositalmente simples — o foco do estudo não é a regra de negócio, e sim o ciclo completo: aplicação → containerização → deploy manual → automação (CI/CD) → infraestrutura como código.

O detalhamento das fases de estudo (Docker, ECR, EC2, GitHub Actions, Terraform) está documentado separadamente em `GUIA_ESTUDO_DEVOPS.md`. Este arquivo (`CLAUDE.md`) descreve o projeto em si — o que ele é, como está organizado e quais suas regras de desenvolvimento.

## Tecnologias

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core** — ORM
- **SQLite** — banco de dados (arquivo local)
- **xUnit** — testes unitários e de integração
- **WebApplicationFactory** — testes de integração da API
- **Serilog** — logging estruturado
- **Swagger / OpenAPI** — documentação da API
- **Docker** — containerização
- **GitHub Actions** — CI/CD
- **AWS (ECR, EC2, IAM)** — infraestrutura de deploy
- **Terraform** — infraestrutura como código

## Arquitetura

Organização em camadas simples, separando responsabilidades sem introduzir complexidade desnecessária para o tamanho do projeto.

### Organização

- **TaskFlow.Api** — camada de apresentação. Minimal API (endpoints, mapeamento de rotas, configuração de pipeline HTTP, Swagger, health check).
- **TaskFlow.Application** — casos de uso / serviços de aplicação. Orquestra chamadas entre API e Domain/Infrastructure.
- **TaskFlow.Domain** — entidades e regras de negócio (`TaskItem`).
- **TaskFlow.Infrastructure** — acesso a dados (EF Core, `DbContext`, migrations, repositórios).
- **TaskFlow.Tests** — testes unitários e de integração.

Fluxo de dependência: `Api → Application → Domain`, com `Infrastructure` implementando interfaces definidas em `Application`/`Domain` (Dependency Inversion).

## Escopo

Sistema de gerenciamento de tarefas com operações básicas de CRUD, persistidas em SQLite.

**Entidade principal:**
- `TaskItem`: `Id`, `Title`, `Description`, `IsDone`, `CreatedAt`, `DueDate`

### WebApi

Implementada como **Minimal API**. Endpoints:

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/tasks` | Lista todas as tarefas |
| GET | `/tasks/{id}` | Busca tarefa por id |
| POST | `/tasks` | Cria uma nova tarefa |
| PUT | `/tasks/{id}` | Atualiza uma tarefa existente |
| DELETE | `/tasks/{id}` | Remove uma tarefa |
| GET | `/health` | Health check da aplicação |

Documentação interativa disponível via Swagger em ambiente de desenvolvimento.

## Fora do Escopo

Para manter o projeto focado no objetivo (estudo de DevOps), os itens abaixo **não** serão implementados:

- Autenticação e autorização (login, JWT, OAuth)
- Multi-tenancy
- Interface gráfica (frontend/UI) — uso apenas via Swagger/HTTP client
- Paginação, filtros avançados ou ordenação nos endpoints
- Cache distribuído
- Mensageria / eventos assíncronos
- Múltiplos ambientes de banco de dados (apenas SQLite local, sem banco gerenciado em nuvem)
- Testes de carga/performance
- Observabilidade avançada (APM, tracing distribuído) — apenas logging estruturado básico

## Diretrizes de desenvolvimento

- **Estilo de API:** Minimal API (sem uso de Controllers/MVC).
- **Nomenclatura:** seguir convenções padrão da comunidade .NET (PascalCase para classes/métodos, camelCase para variáveis locais).
- **Testes:** toda funcionalidade nova deve ter cobertura mínima de teste unitário; endpoints críticos (CRUD principal) devem ter teste de integração via `WebApplicationFactory`.
- **Migrations:** toda alteração de modelo de dados deve gerar uma migration correspondente (`dotnet ef migrations add <Nome>`), nunca alterar o schema manualmente.
- **Logs:** usar Serilog para logging estruturado; evitar `Console.WriteLine`.
- **Commits:** mensagens curtas e descritivas, preferencialmente em português, indicando a fase do guia de estudo relacionada quando aplicável (ex: `fase0: implementa CRUD de tasks`).
- **Branches:** trabalhar direto na `main` é aceitável neste projeto de estudo (baixo risco), mas branches por fase (`fase-1-docker`, `fase-2-deploy-manual`, etc.) são recomendadas para manter histórico organizado.
- **Segredos:** nenhuma credencial (AWS, etc.) deve ser commitada no repositório — usar GitHub Secrets e variáveis de ambiente.

## Estrutura Física

```
EstudoDevops/
├── docs/
│   ├── GUIA_ESTUDO_DEVOPS.md
│   └── CLAUDE.md
├── src/
│   ├── TaskFlow.Api/
│   ├── TaskFlow.Application/
│   ├── TaskFlow.Domain/
│   └── TaskFlow.Infrastructure/
├── tests/
│   └── TaskFlow.Tests/
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── cd.yml
├── infra/
│   └── terraform/
│       ├── main.tf
│       ├── variables.tf
│       └── outputs.tf
├── Dockerfile
├── .dockerignore
├── docker-compose.yml
├── .gitignore
└── TaskFlow.sln
```
