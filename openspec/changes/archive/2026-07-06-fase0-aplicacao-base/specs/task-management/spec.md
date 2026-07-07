## ADDED Requirements

### Requirement: Criação de tarefa
O sistema SHALL permitir a criação de uma nova tarefa via `POST /tasks`, persistindo-a em SQLite com `Id` gerado automaticamente, `CreatedAt` definido no momento da criação e `IsDone` inicializado como `false`.

#### Cenário: Criação bem-sucedida
- **QUANDO** um cliente envia `POST /tasks` com `Title` válido (e opcionalmente `Description` e `DueDate`)
- **ENTÃO** o sistema cria a tarefa, retorna `201 Created` com a representação da tarefa criada, incluindo `Id` e `CreatedAt`

#### Cenário: Título ausente ou vazio
- **QUANDO** um cliente envia `POST /tasks` sem `Title` ou com `Title` vazio
- **ENTÃO** o sistema retorna `400 Bad Request` sem persistir a tarefa

### Requirement: Listagem de tarefas
O sistema SHALL permitir listar todas as tarefas existentes via `GET /tasks`.

#### Cenário: Listagem com tarefas existentes
- **QUANDO** um cliente envia `GET /tasks` e existem tarefas persistidas
- **ENTÃO** o sistema retorna `200 OK` com a lista de todas as tarefas

#### Cenário: Listagem vazia
- **QUANDO** um cliente envia `GET /tasks` e não existem tarefas persistidas
- **ENTÃO** o sistema retorna `200 OK` com uma lista vazia

### Requirement: Busca de tarefa por id
O sistema SHALL permitir buscar uma tarefa específica por `Id` via `GET /tasks/{id}`.

#### Cenário: Tarefa encontrada
- **QUANDO** um cliente envia `GET /tasks/{id}` com um `id` existente
- **ENTÃO** o sistema retorna `200 OK` com os dados da tarefa correspondente

#### Cenário: Tarefa não encontrada
- **QUANDO** um cliente envia `GET /tasks/{id}` com um `id` que não existe
- **ENTÃO** o sistema retorna `404 Not Found`

### Requirement: Atualização de tarefa
O sistema SHALL permitir atualizar uma tarefa existente via `PUT /tasks/{id}`, incluindo `Title`, `Description`, `IsDone` e `DueDate`.

#### Cenário: Atualização bem-sucedida
- **QUANDO** um cliente envia `PUT /tasks/{id}` com um `id` existente e dados válidos
- **ENTÃO** o sistema atualiza a tarefa e retorna `200 OK` (ou `204 No Content`) com os dados atualizados

#### Cenário: Atualização de tarefa inexistente
- **QUANDO** um cliente envia `PUT /tasks/{id}` com um `id` que não existe
- **ENTÃO** o sistema retorna `404 Not Found` sem criar nova tarefa

### Requirement: Remoção de tarefa
O sistema SHALL permitir remover uma tarefa existente via `DELETE /tasks/{id}`.

#### Cenário: Remoção bem-sucedida
- **QUANDO** um cliente envia `DELETE /tasks/{id}` com um `id` existente
- **ENTÃO** o sistema remove a tarefa e retorna `204 No Content`

#### Cenário: Remoção de tarefa inexistente
- **QUANDO** um cliente envia `DELETE /tasks/{id}` com um `id` que não existe
- **ENTÃO** o sistema retorna `404 Not Found`

### Requirement: Persistência via EF Core e SQLite
O sistema SHALL persistir tarefas em um banco SQLite local através de migrations do EF Core, sem alteração manual de schema.

#### Cenário: Aplicação de migration inicial
- **QUANDO** a aplicação é iniciada pela primeira vez em um ambiente novo
- **ENTÃO** o banco SQLite (`taskflow.db`) é criado/atualizado com o schema definido pela migration `InitialCreate`

### Requirement: Documentação interativa via Swagger
O sistema SHALL expor documentação interativa dos endpoints de CRUD via Swagger/OpenAPI em ambiente de desenvolvimento.

#### Cenário: Acesso ao Swagger em desenvolvimento
- **QUANDO** a aplicação está rodando em ambiente de desenvolvimento e um cliente acessa a rota do Swagger UI
- **ENTÃO** o sistema exibe a documentação interativa com todos os endpoints de `/tasks`
