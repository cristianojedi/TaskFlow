> Runbook manual — todas as tasks abaixo são executadas por você (console AWS / AWS CLI / SSH), não por automação. Marque `[x]` conforme for concluindo e validando cada uma.

## 0. Conta AWS e acesso inicial

- [ ] 0.1 Criar uma conta AWS em https://aws.amazon.com/pt/free/ (é preciso cartão de crédito para verificação, mesmo usando o free tier)
- [ ] 0.2 Ativar MFA (autenticação multifator) no usuário **root** da conta (IAM > Security credentials > Assign MFA device) — o root nunca deve ser usado no dia a dia
- [ ] 0.3 Criar um usuário **IAM** administrativo para uso diário (IAM > Users > Create user), com acesso ao console e política `AdministratorAccess` — este usuário substitui o root nas tarefas seguintes
- [ ] 0.4 Ativar MFA também neste usuário IAM administrativo
- [ ] 0.5 Gerar uma **Access Key** para este usuário IAM (IAM > Users > Security credentials > Create access key), necessária para autenticar o AWS CLI
- [ ] 0.6 Instalar o AWS CLI localmente (https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) e configurar com `aws configure`, informando a Access Key/Secret Key do passo 0.5, a região padrão (ex.: `us-east-1`) e formato de saída `json`
- [ ] 0.7 Validar a configuração com `aws sts get-caller-identity` — deve retornar o ARN do usuário IAM criado, não do root
- [ ] 0.8 (Recomendado) Ativar um **AWS Budget** de baixo valor (ex.: US$ 5) em Billing > Budgets, com alerta por e-mail, para evitar surpresas de cobrança durante o estudo

## 1. Repositório ECR

- [ ] 1.1 Criar um repositório privado no Amazon ECR (console: ECR > Repositories > Create repository), nome sugerido `taskflow-api`
- [ ] 1.2 Autenticar o Docker local no ECR:
  ```
  aws ecr get-login-password --region <sua-regiao> | docker login --username AWS --password-stdin <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com
  ```
- [ ] 1.3 Taggear a imagem local com a URI do repositório:
  ```
  docker tag taskflow-api:local <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com/taskflow-api:latest
  ```
- [ ] 1.4 Enviar a imagem para o ECR:
  ```
  docker push <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com/taskflow-api:latest
  ```
- [ ] 1.5 Confirmar no console ECR que a imagem aparece listada no repositório com a tag `latest`

## 2. IAM Role de permissão mínima

- [ ] 2.1 Criar uma IAM Policy customizada com permissão apenas de pull no ECR (ações: `ecr:GetAuthorizationToken`, `ecr:BatchCheckLayerAvailability`, `ecr:GetDownloadUrlForLayer`, `ecr:BatchGetImage`)
- [ ] 2.2 Criar uma IAM Role para o serviço EC2, anexando a policy criada em 2.1 (não usar `AdministratorAccess` nem `AmazonEC2ContainerRegistryFullAccess`)

## 3. Provisionamento da EC2

- [ ] 3.1 Criar um Key Pair novo (ou reutilizar um existente) para acesso SSH, salvando a chave privada `.pem` fora do repositório
- [ ] 3.2 Criar um Security Group com: porta 22 (SSH) liberada apenas para o seu IP público (`<seu-ip>/32`); porta 8080 (API) liberada para `0.0.0.0/0`
- [ ] 3.3 Provisionar uma instância EC2 Amazon Linux 2023 (t2.micro / free tier), anexando a IAM Role criada em 2.2 e o Security Group criado em 3.2
- [ ] 3.4 Configurar o campo "User data" da instância para instalar o Docker no boot:
  ```bash
  #!/bin/bash
  dnf update -y
  dnf install -y docker
  systemctl enable --now docker
  usermod -aG docker ec2-user
  ```
- [ ] 3.5 Após o boot, validar via SSH que o Docker está instalado e rodando (`docker --version`, `systemctl status docker`)

## 4. Deploy do container na EC2

- [ ] 4.1 Na instância EC2, autenticar o Docker no ECR usando a IAM Role anexada (mesma lógica do passo 1.2, executada a partir da EC2, sem access key/secret key):
  ```
  aws ecr get-login-password --region <sua-regiao> | docker login --username AWS --password-stdin <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com
  ```
- [ ] 4.2 Fazer `docker pull` da imagem publicada:
  ```
  docker pull <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com/taskflow-api:latest
  ```
- [ ] 4.3 Rodar o container na EC2, expondo a porta 8080:
  ```
  docker run -d --name taskflow-api -p 8080:8080 <account-id>.dkr.ecr.<sua-regiao>.amazonaws.com/taskflow-api:latest
  ```
- [ ] 4.4 Confirmar com `docker ps` que o container está em estado `running` (sem reinícios em loop)

## 5. Validação de segurança e acesso externo

- [ ] 5.1 Inspecionar as regras do Security Group (console ou `aws ec2 describe-security-groups`) e confirmar que a porta 22 não está aberta para `0.0.0.0/0`
- [ ] 5.2 Inspecionar a IAM Role anexada à instância e confirmar que ela concede apenas as permissões mínimas de pull no ECR (sem policies administrativas)
- [ ] 5.3 A partir de uma máquina fora da rede da EC2 (ex.: seu computador local), acessar `http://<ip-publico-da-ec2>:8080/health` e confirmar `200 OK`
- [ ] 5.4 Validar `GET /tasks` e `POST /tasks` a partir do IP público da instância

## 6. Encerramento (evitar custos indevidos)

- [ ] 6.1 Ao concluir os testes da fase, parar ou terminar a instância EC2 para evitar cobranças contínuas
- [ ] 6.2 (Opcional) Remover a imagem do repositório ECR se não for mais necessária para as próximas fases
