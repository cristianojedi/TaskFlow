## 1. Dockerfile

- [x] 1.1 Criar `Dockerfile` multi-stage com stage `build` baseado em `mcr.microsoft.com/dotnet/sdk:10.0`
- [x] 1.2 No stage `build`, copiar apenas os arquivos `.csproj` primeiro e rodar `dotnet restore` (aproveitar cache de camadas)
- [x] 1.3 Copiar o restante do código-fonte e rodar `dotnet publish -c Release -o /app/publish`
- [x] 1.4 Criar stage final baseado em `mcr.microsoft.com/dotnet/aspnet:10.0`, copiando `/app/publish` do stage anterior
- [x] 1.5 Definir `ENTRYPOINT`/`CMD` apontando para `TaskFlow.Api.dll`
- [x] 1.6 Expor a porta usada pela aplicação (`EXPOSE 8080`, alinhado ao `ASPNETCORE_URLS`)

## 2. Contexto de build

- [x] 2.1 Criar `.dockerignore` excluindo `bin/`, `obj/`, `.git/`, `.vs/`, `*.db`, `openspec/`

## 3. Persistência do SQLite

- [x] 3.1 Definir diretório de dados dentro do container (ex.: `/app/data`) e configurar a connection string via variável de ambiente `ConnectionStrings__TaskFlowDb` (chave real usada em `appsettings.json`)
- [x] 3.2 Documentar/criar volume Docker (nomeado ou bind mount) apontando para o diretório de dados

## 4. docker-compose (opcional)

- [x] 4.1 Criar `docker-compose.yml` com o serviço da API, mapeamento de porta e volume de persistência configurados

## 5. Validação

- [x] 5.1 Buildar a imagem localmente (`docker build -t taskflow-api:local .`) e confirmar build sem erros
- [x] 5.2 Rodar o container (`docker run -p 8080:8080 taskflow-api:local`) e validar `GET /health` retornando 200 OK
- [x] 5.3 Validar `POST /tasks` e `GET /tasks` respondendo corretamente a partir do container
- [x] 5.4 Testar persistência: criar uma tarefa, remover o container mantendo o volume, recriar o container e confirmar que a tarefa persiste via `GET /tasks/{id}`
- [x] 5.5 Checar tamanho final da imagem com `docker images` e confirmar que não inclui o SDK do .NET
