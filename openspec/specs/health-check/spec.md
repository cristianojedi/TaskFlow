# health-check Specification

## Purpose

TBD - created by archiving change fase0-aplicacao-base. Update Purpose after archive.

## Requirements

### Requirement: Endpoint de health check
O sistema SHALL expor um endpoint `GET /health` que reporta o estado da aplicação usando `Microsoft.Extensions.Diagnostics.HealthChecks`.

#### Cenário: Aplicação saudável
- **QUANDO** um cliente envia `GET /health` e a aplicação (incluindo acesso ao banco SQLite) está operacional
- **ENTÃO** o sistema retorna `200 OK` indicando status saudável

#### Cenário: Dependência indisponível
- **QUANDO** um cliente envia `GET /health` e uma dependência verificada (ex.: acesso ao banco de dados) está indisponível
- **ENTÃO** o sistema retorna um status não saudável (ex.: `503 Service Unavailable`)
