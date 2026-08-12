## Context

A TaskFlow API já existe como imagem Docker validada localmente (Fase 1: multi-stage build, persistência via volume, 312MB, sem SDK). A Fase 2 exige colocá-la em execução em uma instância real na AWS, publicada via ECR, com o mínimo de automação — o objetivo é sentir o processo manual (e suas dores) antes de automatizá-lo nas Fases 3 e 4. Todas as ações desta fase são executadas manualmente pelo usuário (console AWS, AWS CLI e SSH); não há acesso de agente/CI à conta AWS nesta fase.

## Goals / Non-Goals

**Goals:**
- Publicar a imagem da API em um repositório ECR.
- Executar o container em uma instância EC2 acessível externamente via HTTP.
- Autenticar a EC2 no ECR usando IAM Role (Instance Profile), sem gravar credenciais fixas no servidor.
- Restringir superfície de ataque via Security Group (SSH apenas do IP do usuário; porta da API liberada ao público).

**Non-Goals:**
- Não inclui automação via GitHub Actions (Fase 3) ou Terraform (Fase 4) — aqui tudo é feito manualmente.
- Não inclui HTTPS/TLS na API (fora de escopo do guia — ver "Fora do Escopo" em CLAUDE.md).
- Não inclui alta disponibilidade, auto scaling ou balanceamento de carga — uma única instância EC2 é suficiente para o objetivo pedagógico.
- Não inclui rollback automatizado — a limitação de rollback manual é, propositalmente, um dos "pontos para entrevista" desta fase.

## Decisions

- **ECR como registry**: usar Amazon ECR (privado) em vez de Docker Hub, para exercitar autenticação via IAM e manter tudo dentro do ecossistema AWS já usado nas fases seguintes.
- **IAM Role anexada à EC2 (Instance Profile) em vez de credenciais fixas**: a instância assume uma role com permissão apenas de `ecr:GetAuthorizationToken`, `ecr:BatchGetImage`, `ecr:GetDownloadUrlForLayer` (pull-only) — nunca `AdministratorAccess` nem access keys gravadas na instância. Alternativa descartada: gerar um IAM user com access key/secret key e colocar em variável de ambiente na EC2 — descartada por expor credenciais de longa duração em texto claro no servidor.
- **Amazon Linux 2023 + Docker via user data**: escolhido por ser a AMI recomendada atual da AWS com suporte via `dnf`, permitindo instalar Docker no boot através do campo "User data" da instância, sem exigir configuração manual por SSH (embora o SSH continue disponível como alternativa/depuração).
- **Security Group restrito**: porta 22 (SSH) liberada apenas para o IP público do usuário (`/32`); porta 8080 (API) liberada para `0.0.0.0/0` já que o objetivo é validar acesso externo. Alternativa descartada: liberar 0.0.0.0/0 também na porta 22 — descartada por expor a instância a scans/brute-force de SSH na internet.
- **t2.micro / free tier**: dimensionamento mínimo, suficiente para uma API de estudo sem carga real.
- **`docker run` direto (sem compose) na EC2**: mantém o processo simples e manual, coerente com o objetivo pedagógico da fase; `docker-compose.yml` já existe da Fase 1 e pode ser usado como referência opcional.

## Risks / Trade-offs

- [Security Group mal configurado expõe a instância desnecessariamente] → Mitigar validando explicitamente as regras de entrada (`aws ec2 describe-security-groups` ou console) antes de considerar a task concluída.
- [IAM Role com permissões excessivas (ex.: `AmazonEC2ContainerRegistryFullAccess` em vez de pull-only)] → Mitigar criando uma policy customizada com as três ações mínimas de pull, em vez de usar uma managed policy ampla.
- [Esquecer de terminar a instância EC2 após o estudo, gerando custo contínuo] → Mitigar documentando explicitamente, ao final do runbook, o passo de terminar/parar a instância e (opcionalmente) remover a imagem do ECR.
- [Processo 100% manual é propenso a erro humano e não tem histórico/rollback] → Este é o ponto pedagógico da fase — documentado explicitamente como limitação a ser resolvida nas Fases 3 e 4, não mitigado aqui.
- [Chave privada do Key Pair perdida ou exposta] → Mitigar armazenando a `.pem` fora do repositório (nunca commitada) e ajustando permissões do arquivo (`chmod 400` em ambientes Unix-like).

## Migration Plan

Não aplicável no sentido de dados — não há ambiente anterior em produção. O rollout consiste no runbook manual em `tasks.md`: criar ECR → autenticar Docker local → tag/push → provisionar EC2 (Security Group, Key Pair, IAM Role) → instalar Docker → autenticar EC2 no ECR via role → pull/run → validar acesso externo. Como "rollback", a única opção nesta fase é parar/terminar a instância e, se necessário, recriar do zero — limitação que reforça o valor das fases seguintes de automação.
