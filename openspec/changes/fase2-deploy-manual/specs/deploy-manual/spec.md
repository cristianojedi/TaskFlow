## ADDED Requirements

### Requirement: Publicação da imagem no ECR
O sistema SHALL ter a imagem Docker da TaskFlow API publicada em um repositório Amazon ECR privado.

#### Scenario: Push bem-sucedido
- **WHEN** a imagem local `taskflow-api:local` é taggeada com a URI do repositório ECR e enviada via `docker push`
- **THEN** a imagem aparece listada no repositório ECR no console AWS, com a tag esperada

### Requirement: Execução do container na EC2
O sistema SHALL executar a imagem publicada em uma instância EC2, com o container em estado `running`.

#### Scenario: Container rodando na instância
- **WHEN** a EC2 executa `docker pull` da imagem do ECR seguido de `docker run` expondo a porta da API
- **THEN** `docker ps` na instância mostra o container em execução sem reinícios em loop

### Requirement: Autenticação via IAM Role de permissão mínima
O sistema SHALL autenticar a instância EC2 no ECR usando uma IAM Role (Instance Profile) anexada à instância, sem uso de credenciais de acesso (access key/secret key) fixas gravadas no servidor.

#### Scenario: Pull autenticado via role
- **WHEN** a EC2 executa `aws ecr get-login-password` usando a IAM Role anexada e autentica o Docker no registry
- **THEN** o `docker pull` da imagem é bem-sucedido sem que nenhuma access key ou secret key esteja armazenada em arquivo, variável de ambiente ou histórico de comandos na instância

#### Scenario: Permissão mínima da role
- **WHEN** a policy da IAM Role é inspecionada
- **THEN** ela concede apenas as ações necessárias para autenticação e pull no ECR (não inclui permissões de administração nem policies gerenciadas amplas como `AdministratorAccess`)

### Requirement: Restrição de acesso via Security Group
O sistema SHALL restringir o acesso SSH à instância EC2 ao IP público do usuário, mantendo a porta da API acessível externamente.

#### Scenario: SSH restrito
- **WHEN** as regras de entrada do Security Group da instância são inspecionadas
- **THEN** a porta 22 (SSH) permite tráfego apenas do IP público do usuário (`/32`), não de `0.0.0.0/0`

#### Scenario: API acessível externamente
- **WHEN** um cliente fora da rede da instância envia `GET /health` e `GET /tasks` para o IP público da EC2 na porta configurada
- **THEN** o sistema retorna respostas HTTP válidas (200 OK), confirmando exposição externa da API
