## Why

A TaskFlow API já roda como imagem Docker local (Fase 1), mas ainda não foi exposta fora da máquina de desenvolvimento. A Fase 2 do guia de estudo DevOps exige sentir na prática as dores do deploy manual (publicar imagem, provisionar servidor, autenticar, rodar container, expor externamente) antes de automatizar esse processo nas fases seguintes — esse é o gancho pedagógico central da Fase 2 e um dos temas mais cobrados em entrevista sênior (IAM, segurança de infraestrutura, limitações de processos manuais).

## What Changes

- Pré-requisito: criar conta AWS (free tier), proteger o usuário root com MFA e criar um usuário IAM administrativo para uso diário (o root não deve ser usado nas tarefas operacionais).
- Publicar a imagem Docker da TaskFlow API em um repositório Amazon ECR.
- Provisionar uma instância EC2 (Amazon Linux 2023, free tier) com Security Group restrito (porta 22 apenas do IP do usuário, porta da API liberada).
- Autenticar a EC2 no ECR via **IAM Role** anexada à instância (sem credenciais fixas gravadas no servidor).
- Instalar Docker na EC2 (via `user data` no boot ou manualmente por SSH).
- Fazer `docker pull` da imagem publicada e `docker run` na EC2.
- Validar acesso externo à API pelo IP público da instância (`/health` e `/tasks`).

Esta é uma fase de infraestrutura manual: não há alteração de código-fonte da aplicação. Todas as ações são executadas pelo usuário no console AWS / CLI / SSH — o papel deste change é documentar o runbook, as decisões de segurança e os critérios de validação, não implementar código.

## Capabilities

### New Capabilities
- `deploy-manual`: publicação da imagem no ECR, provisionamento de EC2 com IAM Role de permissão mínima, execução do container na instância e exposição externa da API via IP público.

### Modified Capabilities

(nenhuma — não há mudança de comportamento da API, apenas de onde e como ela é executada)

## Impact

- Novos recursos AWS: repositório ECR, instância EC2, Security Group, Key Pair, IAM Role/Instance Profile.
- Nenhuma mudança de código-fonte em `src/` ou `tests/`.
- Custos reais na conta AWS do usuário (mesmo em free tier, exige atenção para não deixar recursos órfãos rodando).
- Dependência de ferramentas externas: AWS CLI (opcional, pode ser tudo via console) e um cliente SSH.
- Decisões desta fase (IAM Role de permissão mínima, Security Group restrito) servem de base para a Fase 3 (GitHub Actions) e Fase 4 (Terraform), que devem reproduzir os mesmos princípios de segurança de forma automatizada.
