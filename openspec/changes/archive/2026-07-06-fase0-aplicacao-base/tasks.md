## 1. Estrutura da solução

- [x] 1.1 Criar solução `.slnx` na raiz do projeto (`TaskFlow.slnx`)
- [x] 1.2 Criar projeto `TaskFlow.Domain` (class library) sem dependências de outros projetos
- [x] 1.3 Criar projeto `TaskFlow.Application` (class library) referenciando `TaskFlow.Domain`
- [x] 1.4 Criar projeto `TaskFlow.Infrastructure` (class library) referenciando `TaskFlow.Application` e `TaskFlow.Domain`
- [x] 1.5 Criar projeto `TaskFlow.Api` (ASP.NET Core Web API - Minimal API) referenciando `TaskFlow.Application` e `TaskFlow.Infrastructure`
- [x] 1.6 Criar projeto `TaskFlow.Tests` (xUnit) referenciando todos os projetos acima
- [x] 1.7 Adicionar todos os projetos à solução e validar `dotnet build`

## 2. Domínio

- [x] 2.1 Implementar entidade `TaskItem` (`Id`, `Title`, `Description`, `IsDone`, `CreatedAt`, `DueDate`) em `TaskFlow.Domain`
- [x] 2.2 Adicionar validações básicas de domínio (ex.: `Title` obrigatório)

## 3. Camada de Aplicação

- [x] 3.1 Definir interface `ITaskRepository` (CRUD) em `TaskFlow.Application`
- [x] 3.2 Implementar `TaskService` orquestrando operações de CRUD via `ITaskRepository`
- [x] 3.3 Definir DTOs de entrada/saída (ex.: `CreateTaskRequest`, `UpdateTaskRequest`, `TaskResponse`)

## 4. Infraestrutura (EF Core + SQLite)

- [x] 4.1 Adicionar pacotes EF Core e provider SQLite ao `TaskFlow.Infrastructure`
- [x] 4.2 Configurar `TaskFlowDbContext` com `DbSet<TaskItem>` e connection string `Data Source=taskflow.db`
- [x] 4.3 Implementar `TaskRepository` (implementação de `ITaskRepository`) usando `TaskFlowDbContext`
- [x] 4.4 Gerar migration inicial `dotnet ef migrations add InitialCreate`
- [x] 4.5 Validar aplicação da migration e criação do arquivo `taskflow.db`

## 5. API (Minimal API)

- [x] 5.1 Configurar `Program.cs`: DI (DbContext, `ITaskRepository`, `TaskService`), pipeline HTTP
- [x] 5.2 Implementar endpoint `GET /tasks` (listar todas as tarefas)
- [x] 5.3 Implementar endpoint `GET /tasks/{id}` (buscar por id, 404 se não encontrado)
- [x] 5.4 Implementar endpoint `POST /tasks` (criar, 400 se `Title` inválido, 201 em sucesso)
- [x] 5.5 Implementar endpoint `PUT /tasks/{id}` (atualizar, 404 se não encontrado)
- [x] 5.6 Implementar endpoint `DELETE /tasks/{id}` (remover, 404 se não encontrado, 204 em sucesso)
- [x] 5.7 Agrupar endpoints de `/tasks` em método de extensão (ex.: `MapTaskEndpoints`)

## 6. Verificação de Saúde (Health Check)

- [x] 6.1 Adicionar `Microsoft.Extensions.Diagnostics.HealthChecks` e configurar check de acesso ao SQLite
- [x] 6.2 Expor endpoint `GET /health` mapeado no pipeline

## 7. Swagger / OpenAPI

- [x] 7.1 Adicionar Swashbuckle/Swagger ao `TaskFlow.Api`
- [x] 7.2 Habilitar Swagger UI apenas em ambiente de desenvolvimento
- [x] 7.3 Validar que todos os endpoints de `/tasks` aparecem documentados

## 8. Logs (Serilog)

- [x] 8.1 Adicionar pacotes Serilog (core + sink de console + formatação JSON) ao `TaskFlow.Api`
- [x] 8.2 Configurar Serilog em `Program.cs` como logger padrão da aplicação
- [x] 8.3 Validar logs estruturados aparecendo no console ao rodar `dotnet run`

## 9. Testes

- [x] 9.1 Escrever testes unitários de validação de domínio (`TaskItem`)
- [x] 9.2 Escrever testes unitários de `TaskService` (usando repositório fake/mock)
- [x] 9.3 Escrever teste de integração para `POST /tasks` via `WebApplicationFactory`
- [x] 9.4 Escrever teste de integração para `GET /tasks/{id}` via `WebApplicationFactory`
- [x] 9.5 Garantir isolamento de estado entre execuções de teste de integração (banco por execução)
- [x] 9.6 Validar `dotnet test` passando integralmente

## 10. Validação final

- [x] 10.1 Validar `dotnet build` sem erros/warnings relevantes
- [x] 10.2 Validar `dotnet run` com CRUD funcional via Swagger
- [x] 10.3 Validar migrations aplicadas e `taskflow.db` criado
- [x] 10.4 Validar `dotnet test` passando
- [x] 10.5 Validar logs estruturados no console
