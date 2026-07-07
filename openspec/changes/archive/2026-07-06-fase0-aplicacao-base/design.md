## Contexto

O TaskFlow ainda não tem código-fonte. Esta é a primeira implementação do projeto, definindo a estrutura de camadas, o modelo de dados e a superfície de API que servirão de base para todas as fases seguintes (Docker, CI/CD, deploy AWS, Terraform). As decisões aqui priorizam simplicidade e aderência às convenções .NET, conforme `CLAUDE.md`, já que o objetivo do projeto é o ciclo DevOps completo, não a complexidade do domínio.

## Objetivos / Não-Objetivos

**Objetivos:**
- Estrutura de solução em camadas (`Api → Application → Domain`, com `Infrastructure` implementando interfaces via Dependency Inversion).
- CRUD completo de `TaskItem` via Minimal API, persistido em SQLite com EF Core.
- Endpoint `/health` funcional.
- Logging estruturado via Serilog (console, formato JSON).
- Cobertura de teste unitário (domínio/serviço) e de integração (mínimo 2 endpoints via `WebApplicationFactory`).
- Solução compilável (`dotnet build`) e executável localmente (`dotnet run`).

**Não-Objetivos:**
- Autenticação/autorização, multi-tenancy, UI, paginação/filtros, cache distribuído, mensageria, múltiplos ambientes de banco, testes de carga, observabilidade avançada (todos fora de escopo conforme `CLAUDE.md`).
- Containerização, CI/CD e infraestrutura como código (fases posteriores do guia de estudo).

## Decisões

**1. Minimal API em vez de Controllers/MVC**
Aderente à diretriz do projeto. Minimal API reduz boilerplate e é adequado ao escopo pequeno (5 endpoints + health check). Trade-off: menos estrutura built-in para versionamento/filtros complexos, mas isso está fora de escopo aqui de qualquer forma.

**2. Camadas físicas separadas em projetos (.csproj) distintos**
`TaskFlow.Domain` não referencia nenhum outro projeto (entidades puras). `TaskFlow.Application` referencia `Domain` e define interfaces de repositório (ex.: `ITaskRepository`) para inversão de dependência. `TaskFlow.Infrastructure` referencia `Application`/`Domain` e implementa essas interfaces com EF Core. `TaskFlow.Api` referencia `Application` e `Infrastructure` (apenas para composição/DI na `Program.cs`), mas os endpoints só dependem de `Application`.
Alternativa considerada: pastas dentro de um único projeto — descartada porque a separação física reforça a regra de dependência em tempo de compilação, o que é um ponto de entrevista explícito no guia (`Por que separar em camadas?`).

**3. EF Core + SQLite com migrations versionadas**
Toda alteração de schema via `dotnet ef migrations add`, nunca alteração manual — conforme `CLAUDE.md`. SQLite é aceitável para estudo (zero infra, arquivo local), mas não seria escolha de produção (sem suporte a acesso concorrente robusto, sem replicação) — ponto de entrevista já documentado.

**4. Camada de serviço em `Application` orquestrando `Domain`/`Infrastructure`**
Os endpoints da Minimal API chamam serviços de aplicação (ex.: `TaskService`), que chamam o repositório (interface definida em `Application`, implementada em `Infrastructure`). Isso mantém a `Api` fina (mapeamento de rotas + DTOs) e testável isoladamente da camada de dados.

**5. Serilog configurado direto na `Program.cs` da Api**
Sink de console com formatação JSON (`Serilog.Formatting.Compact` ou equivalente). Sem sinks externos (arquivo, ELK, etc.) — fora de escopo.

**6. Testes: xUnit + WebApplicationFactory**
Projeto único `TaskFlow.Tests` contendo testes unitários (serviços/domínio, com repositório em memória ou mock) e testes de integração (via `WebApplicationFactory<Program>`, banco SQLite em arquivo temporário ou in-memory por execução).

## Riscos / Trade-offs

- [SQLite com acesso concorrente limitado] → Mitigação: aceitável pois é ambiente de estudo local, sem carga concorrente real; documentado como não-produção.
- [Minimal API pode crescer confuso se endpoints forem todos definidos em `Program.cs`] → Mitigação: agrupar endpoints em métodos de extensão (ex.: `MapTaskEndpoints`) separados por recurso.
- [Testes de integração usando arquivo SQLite podem deixar estado entre execuções] → Mitigação: usar banco isolado por execução de teste (arquivo temporário ou `WebApplicationFactory` com `ConfigureServices` substituindo a connection string).

## Plano de Migração

Não aplicável — não há versão anterior em produção. Esta é a criação inicial da solução; não há dados existentes a migrar nem estratégia de rollback além de reverter commits.

## Questões em Aberto

Nenhuma pendente — escopo e decisões técnicas cobrem o necessário para a Fase 0.
