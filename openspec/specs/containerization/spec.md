# containerization Specification

## Purpose

TBD - created by syncing change fase1-containerizacao. Update Purpose after archive.

## Requirements

### Requirement: Build de imagem Docker multi-stage
O sistema SHALL fornecer um `Dockerfile` multi-stage que compila e publica a TaskFlow API usando a imagem SDK do .NET e executa a aplicação publicada usando a imagem de runtime ASP.NET, sem incluir ferramentas de build na imagem final.

#### Scenario: Build local bem-sucedido
- **WHEN** o comando `docker build -t taskflow-api:local .` é executado na raiz do repositório
- **THEN** a imagem é gerada sem erros e contém apenas os artefatos publicados da aplicação, sem o SDK do .NET

### Requirement: Execução do container
O sistema SHALL permitir executar a API dentro de um container Docker, expondo os endpoints HTTP existentes.

#### Scenario: Container respondendo aos endpoints
- **WHEN** o container é iniciado com `docker run -p 8080:8080 taskflow-api:local`
- **THEN** requisições para `GET /health` e `GET /tasks` retornam respostas válidas (200 OK) a partir do container

### Requirement: Persistência do SQLite via volume
O sistema SHALL persistir o arquivo de banco SQLite fora do sistema de arquivos efêmero do container, usando um volume Docker (nomeado ou bind mount).

#### Scenario: Dados sobrevivem à recriação do container
- **WHEN** uma tarefa é criada via `POST /tasks` com o container em execução, o container é removido (mantendo o volume) e um novo container é iniciado a partir da mesma imagem com o mesmo volume montado
- **THEN** a tarefa criada anteriormente continua acessível via `GET /tasks/{id}`

### Requirement: Contexto de build otimizado
O sistema SHALL excluir arquivos irrelevantes do contexto de build Docker através de um arquivo `.dockerignore`.

#### Scenario: Artefatos locais não copiados para a imagem
- **WHEN** a imagem é construída a partir de um diretório de trabalho que contém `bin/`, `obj/`, `.git/` e `.vs/`
- **THEN** nenhum desses diretórios é copiado para o contexto de build enviado ao Docker daemon
