## Why

A TaskFlow API hoje só roda via `dotnet run` na máquina local. A Fase 1 do guia de estudo DevOps exige empacotar a aplicação em uma imagem Docker enxuta e reproduzível, pré-requisito para as fases seguintes (deploy manual em EC2 via ECR, CI/CD e Terraform), que dependem de a aplicação ser distribuída como container.

## What Changes

- Criar `Dockerfile` multi-stage: stage de build (`mcr.microsoft.com/dotnet/sdk`) faz restore + build + publish; stage de runtime (`mcr.microsoft.com/dotnet/aspnet`) copia apenas o publish, reduzindo tamanho e superfície de ataque da imagem final.
- Criar `.dockerignore` excluindo `bin/`, `obj/`, `.git/`, `.vs/` e artefatos de build.
- Definir estratégia de persistência do SQLite dentro do container (volume nomeado/montado vs. banco efêmero) e documentar a decisão.
- Validar build local (`docker build`) e execução (`docker run`), confirmando que `/health` e `/tasks` respondem corretamente a partir do container.
- (Opcional) Criar `docker-compose.yml` para facilitar a execução local com volume de persistência já configurado.

## Capabilities

### New Capabilities
- `containerization`: empacotamento da API em imagem Docker multi-stage, com persistência do SQLite via volume e execução local via `docker run`/`docker-compose`.

### Modified Capabilities

(nenhuma — não há mudança de requisitos de comportamento da API, apenas de empacotamento/infraestrutura)

## Impact

- Novos arquivos na raiz do repositório: `Dockerfile`, `.dockerignore`, `docker-compose.yml` (opcional).
- Nenhuma mudança de código-fonte em `src/` ou `tests/` é esperada — o escopo é puramente de empacotamento.
- Dependência de runtime: Docker Engine instalado no ambiente de desenvolvimento/estudo.
- Decisão de persistência do SQLite impacta a Fase 2 (deploy manual em EC2), onde o mesmo critério de volume será reaplicado.
