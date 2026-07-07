## Context

A TaskFlow API roda hoje apenas via `dotnet run` na máquina de desenvolvimento, persistindo dados em um arquivo SQLite local (`taskflow.db`). Para avançar às fases seguintes do guia de estudo (deploy manual em EC2 via ECR, automação com GitHub Actions, Terraform), a aplicação precisa ser distribuída como imagem Docker. O projeto é propositalmente simples (sem regra de negócio complexa) — o foco aqui é o empacotamento correto, não mudanças de comportamento da API.

## Goals / Non-Goals

**Goals:**
- Empacotar a API em uma imagem Docker multi-stage, minimizando tamanho final e superfície de ataque.
- Garantir que o container seja executável localmente e responda corretamente em `/health` e `/tasks`.
- Definir e documentar a estratégia de persistência do SQLite (volume vs. efêmero), já que essa decisão será reaproveitada na Fase 2 (EC2).

**Non-Goals:**
- Não há mudança de código-fonte da API (endpoints, regras de domínio, DTOs).
- Não inclui publicação da imagem em registry (ECR) — isso é escopo da Fase 2.
- Não inclui orquestração multi-container além de um `docker-compose.yml` opcional para conveniência local.

## Decisions

- **Multi-stage build (SDK → ASPNET runtime)**: usar `mcr.microsoft.com/dotnet/sdk:10.0` para restore/build/publish e `mcr.microsoft.com/dotnet/aspnet:10.0` como imagem final. Alternativa considerada: imagem única baseada só no SDK — descartada por gerar imagem muito maior e expor ferramentas de build desnecessárias em produção.
- **Persistência via volume nomeado**: o SQLite (`taskflow.db`) será montado em um volume Docker (nomeado ou bind mount) apontando para o diretório de dados da aplicação, evitando perda de dados ao recriar o container. Alternativa considerada: banco efêmero dentro da imagem — descartada porque perderia os dados a cada `docker run`/redeploy, o que inviabilizaria testes manuais e a futura Fase 2.
- **`.dockerignore` explícito**: excluir `bin/`, `obj/`, `.git/`, `.vs/`, `*.db` para reduzir contexto de build e evitar copiar artefatos de build da máquina host para dentro da imagem.
- **`docker-compose.yml` opcional**: incluído para facilitar `docker compose up` local com o volume já configurado, mas não é um entregável obrigatório da fase.
- **Ordem de camadas no Dockerfile**: copiar primeiro os arquivos de projeto (`.csproj`) e rodar `dotnet restore` antes de copiar o restante do código-fonte, aproveitando cache de camadas do Docker em builds subsequentes.

## Risks / Trade-offs

- [Volume mal configurado pode causar perda de dados entre execuções do container] → Mitigar testando explicitamente: criar tarefas, remover o container (não o volume) e recriar, validando que os dados persistem.
- [Imagem final ainda maior que o ideal se dependências não usadas forem publicadas] → Mitigar rodando `dotnet publish` com trimming padrão do .NET 10 e validando tamanho final com `docker images`.
- [Divergência entre ambiente de container e ambiente local (variáveis de ambiente, connection string do SQLite)] → Mitigar usando variável de ambiente `ConnectionStrings__TaskFlowDb` (chave real usada em `appsettings.json`) configurável via `docker run -e` ou `docker-compose.yml`, com valor padrão sensato.

## Migration Plan

Não aplicável — não há dados em produção nem versão anterior containerizada para migrar. O rollout consiste em: build da imagem local → validação manual (`/health`, `/tasks`, persistência via volume) → uso da imagem nas fases seguintes do guia de estudo.
