# 📑 Endpoints da API - Módulo de Clientes

## 4.4.1 - Listar Clientes com Paginação e Filtros

**Descrição:** Endpoint para recuperar lista paginada de clientes com opções de filtro por status e busca por texto. Utilizado na página principal de listagem de clientes para exibir dados em tabela com controles de navegação.

**Endpoint:** `GET /api/customers`

**Request Params:**
```javascript
{
  page?: number,           // Página atual (padrão: 1)
  limit?: number,          // Itens por página (padrão: 10)
  search?: string,         // Busca por nome, email, telefone ou endereço
  status?: string          // Filtro por status: 'all', 'active', 'inactive' (padrão: 'all')
}
```

**Request Response (200 OK):**
```javascript
{
  "customers": [
    {
      "id": 1,
      "companyId": 1,
      "name": "Maria Silva Santos",
      "phone": "(11) 99999-1234",
      "email": "maria.silva@email.com",
      "address": "Rua das Flores, 123 - Vila Madalena, São Paulo - SP, 05435-000",
      "registrationDate": "2024-01-15T10:30:00Z",
      "status": "active"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalPages": 5,
    "totalItems": 45,
    "itemsPerPage": 10,
    "hasNext": true,
    "hasPrev": false
  }
}
```

---

## 4.4.2 - Obter Estatísticas de Clientes

**Descrição:** Endpoint para recuperar estatísticas resumidas dos clientes, incluindo total de clientes, ativos, inativos e novos cadastros do mês. Utilizado para exibir cards de métricas no dashboard de clientes.

**Endpoint:** `GET /api/customers/stats`

**Request Params:**
```javascript
// Nenhum parâmetro necessário
```

**Request Response (200 OK):**
```javascript
{
  "total": 45,
  "active": 38,
  "inactive": 7,
  "newThisMonth": 8
}
```

---

## 4.4.3 - Obter Cliente por ID

**Descrição:** Endpoint para recuperar dados completos de um cliente específico, incluindo informações pessoais e lista de relacionamentos/observações. Utilizado nas páginas de visualização e edição de cliente.

**Endpoint:** `GET /api/customers/{id}`

**Request Params:**
```javascript
{
  id: number  // ID do cliente (path parameter)
}
```

**Request Response (200 OK):**
```javascript
{
  "id": 1,
  "companyId": 1,
  "name": "Maria Silva Santos",
  "phone": "(11) 99999-1234",
  "email": "maria.silva@email.com",
  "address": "Rua das Flores, 123 - Vila Madalena, São Paulo - SP, 05435-000",
  "registrationDate": "2024-01-15T10:30:00Z",
  "status": "active",
  "relationships": [
    {
      "id": 1,
      "customerId": 1,
      "description": "Tem 2 filhos pequenos, prefere serviços no período da manhã",
      "registrationDate": "2024-01-15T10:35:00Z"
    },
    {
      "id": 2,
      "customerId": 1,
      "description": "Apartamento de 80m², 3 quartos",
      "registrationDate": "2024-01-16T14:20:00Z"
    }
  ]
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente não encontrado",
  "code": "CUSTOMER_NOT_FOUND"
}
```

---

## 4.4.4 - Criar Novo Cliente

**Descrição:** Endpoint para cadastrar um novo cliente no sistema. O cliente é automaticamente associado à empresa do usuário logado e criado com status ativo. Utilizado no formulário de criação de cliente.

**Endpoint:** `POST /api/customers`

**Request Body:**
```javascript
{
  "name": "João Silva Costa",
  "phone": "(11) 98765-4321",
  "email": "joao.silva@email.com",
  "address": "Av. Paulista, 1500 - Bela Vista, São Paulo - SP, 01310-200"
}
```

**Request Response (201 Created):**
```javascript
{
  "id": 7,
  "companyId": 1,
  "name": "João Silva Costa",
  "phone": "(11) 98765-4321",
  "email": "joao.silva@email.com",
  "address": "Av. Paulista, 1500 - Bela Vista, São Paulo - SP, 01310-200",
  "registrationDate": "2024-10-02T14:30:00Z",
  "status": "active",
  "relationships": []
}
```

**Request Response (400 Bad Request):**
```javascript
{
  "error": "Dados obrigatórios não fornecidos",
  "code": "VALIDATION_ERROR",
  "details": [
    {
      "field": "name",
      "message": "Nome é obrigatório"
    },
    {
      "field": "phone",
      "message": "Telefone é obrigatório"
    }
  ]
}
```

---

## 4.4.5 - Atualizar Cliente

**Descrição:** Endpoint para atualizar dados de um cliente existente. Permite modificar informações pessoais, de contato e endereço. Utilizado no formulário de edição de cliente.

**Endpoint:** `PUT /api/customers/{id}`

**Request Params:**
```javascript
{
  id: number  // ID do cliente (path parameter)
}
```

**Request Body:**
```javascript
{
  "name": "Maria Silva Santos Oliveira",
  "phone": "(11) 99999-1234",
  "email": "maria.oliveira@email.com",
  "address": "Rua das Flores, 456 - Vila Madalena, São Paulo - SP, 05435-000",
  "status": "active"
}
```

**Request Response (200 OK):**
```javascript
{
  "id": 1,
  "companyId": 1,
  "name": "Maria Silva Santos Oliveira",
  "phone": "(11) 99999-1234",
  "email": "maria.oliveira@email.com",
  "address": "Rua das Flores, 456 - Vila Madalena, São Paulo - SP, 05435-000",
  "registrationDate": "2024-01-15T10:30:00Z",
  "status": "active",
  "relationships": [
    // ... relacionamentos existentes
  ]
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente não encontrado",
  "code": "CUSTOMER_NOT_FOUND"
}
```

---

## 4.4.6 - Excluir Cliente

**Descrição:** Endpoint para exclusão permanente de um cliente do sistema. Remove o cliente e todos os relacionamentos associados. Utilizado na ação de exclusão da listagem de clientes com confirmação.

**Endpoint:** `DELETE /api/customers/{id}`

**Request Params:**
```javascript
{
  id: number  // ID do cliente (path parameter)
}
```

**Request Response (200 OK):**
```javascript
{
  "success": true,
  "message": "Cliente excluído com sucesso"
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente não encontrado",
  "code": "CUSTOMER_NOT_FOUND"
}
```

**Request Response (409 Conflict):**
```javascript
{
  "error": "Cliente não pode ser excluído pois possui orçamentos associados",
  "code": "CUSTOMER_HAS_DEPENDENCIES"
}
```

---

## 4.4.7 - Adicionar Relacionamento/Observação ao Cliente

**Descrição:** Endpoint para adicionar uma nova observação ou informação de relacionamento a um cliente específico. Permite armazenar informações importantes sobre preferências, características pessoais ou observações relevantes para o atendimento.

**Endpoint:** `POST /api/customers/{id}/relationships`

**Request Params:**
```javascript
{
  id: number  // ID do cliente (path parameter)
}
```

**Request Body:**
```javascript
{
  "description": "Cliente prefere horários matutinos e tem alergia a produtos com cheiro forte"
}
```

**Request Response (201 Created):**
```javascript
{
  "id": 15,
  "customerId": 1,
  "description": "Cliente prefere horários matutinos e tem alergia a produtos com cheiro forte",
  "registrationDate": "2024-10-02T15:45:00Z"
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente não encontrado",
  "code": "CUSTOMER_NOT_FOUND"
}
```

---

## 4.4.8 - Excluir Relacionamento/Observação do Cliente

**Descrição:** Endpoint para remover uma observação ou relacionamento específico de um cliente. Utilizado para manter as informações de relacionamento atualizadas e relevantes.

**Endpoint:** `DELETE /api/customers/{customerId}/relationships/{relationshipId}`

**Request Params:**
```javascript
{
  customerId: number,      // ID do cliente (path parameter)
  relationshipId: number   // ID do relacionamento (path parameter)
}
```

**Request Response (200 OK):**
```javascript
{
  "success": true,
  "message": "Relacionamento excluído com sucesso"
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente ou relacionamento não encontrado",
  "code": "RELATIONSHIP_NOT_FOUND"
}
```

---

## 4.4.9 - Importar Clientes da Agenda Telefônica

**Descrição:** Endpoint para importar contatos da agenda telefônica do dispositivo móvel e criar clientes em lote. Implementa o requisito RF04 de cadastro via agenda telefônica, permitindo seleção múltipla de contatos para importação.

**Endpoint:** `POST /api/customers/import-contacts`

**Request Body:**
```javascript
{
  "contacts": [
    {
      "name": "Pedro Santos",
      "phone": "(11) 94567-8901",
      "email": "pedro.santos@email.com"
    },
    {
      "name": "Ana Costa Silva",
      "phone": "(11) 93456-7890",
      "email": ""
    }
  ]
}
```

**Request Response (201 Created):**
```javascript
{
  "success": true,
  "imported": 2,
  "failed": 0,
  "customers": [
    {
      "id": 8,
      "name": "Pedro Santos",
      "phone": "(11) 94567-8901",
      "email": "pedro.santos@email.com",
      "status": "active"
    },
    {
      "id": 9,
      "name": "Ana Costa Silva",
      "phone": "(11) 93456-7890",
      "email": "",
      "status": "active"
    }
  ],
  "errors": []
}
```

**Request Response (207 Multi-Status):**
```javascript
{
  "success": true,
  "imported": 1,
  "failed": 1,
  "customers": [
    {
      "id": 8,
      "name": "Pedro Santos",
      "phone": "(11) 94567-8901",
      "email": "pedro.santos@email.com",
      "status": "active"
    }
  ],
  "errors": [
    {
      "contact": {
        "name": "Ana Costa Silva",
        "phone": "(11) 93456-7890"
      },
      "error": "Telefone já cadastrado para outro cliente"
    }
  ]
}
```

---

## 4.4.10 - Buscar Clientes por Texto

**Descrição:** Endpoint para busca rápida de clientes por nome, email ou telefone. Utilizado em campos de autocomplete e seleção rápida de clientes em outras funcionalidades como criação de orçamentos.

**Endpoint:** `GET /api/customers/search`

**Request Params:**
```javascript
{
  q: string,      // Termo de busca
  limit?: number  // Limite de resultados (padrão: 10)
}
```

**Request Response (200 OK):**
```javascript
{
  "customers": [
    {
      "id": 1,
      "name": "Maria Silva Santos",
      "phone": "(11) 99999-1234",
      "email": "maria.silva@email.com",
      "status": "active"
    },
    {
      "id": 4,
      "name": "Carlos Eduardo Lima",
      "phone": "(11) 96666-3456",
      "email": "carlos.lima@email.com",
      "status": "active"
    }
  ]
}
```

---

## 4.4.11 - Alterar Status do Cliente

**Descrição:** Endpoint para ativar ou desativar um cliente no sistema. Permite gerenciar clientes inativos sem excluí-los permanentemente, mantendo histórico de relacionamento e orçamentos.

**Endpoint:** `PATCH /api/customers/{id}/status`

**Request Params:**
```javascript
{
  id: number  // ID do cliente (path parameter)
}
```

**Request Body:**
```javascript
{
  "status": "inactive"  // "active" ou "inactive"
}
```

**Request Response (200 OK):**
```javascript
{
  "id": 1,
  "name": "Maria Silva Santos",
  "status": "inactive",
  "updatedAt": "2024-10-02T16:30:00Z"
}
```

**Request Response (404 Not Found):**
```javascript
{
  "error": "Cliente não encontrado",
  "code": "CUSTOMER_NOT_FOUND"
}
```

---

## Códigos de Erro Comuns

**400 Bad Request:**
- `VALIDATION_ERROR`: Dados obrigatórios não fornecidos ou inválidos
- `INVALID_PHONE_FORMAT`: Formato de telefone inválido
- `INVALID_EMAIL_FORMAT`: Formato de email inválido

**401 Unauthorized:**
- `AUTHENTICATION_REQUIRED`: Token de autenticação necessário
- `INVALID_TOKEN`: Token inválido ou expirado

**403 Forbidden:**
- `INSUFFICIENT_PERMISSIONS`: Usuário não tem permissão para esta operação
- `COMPANY_ACCESS_DENIED`: Acesso negado aos dados da empresa

**404 Not Found:**
- `CUSTOMER_NOT_FOUND`: Cliente não encontrado
- `RELATIONSHIP_NOT_FOUND`: Relacionamento não encontrado

**409 Conflict:**
- `CUSTOMER_HAS_DEPENDENCIES`: Cliente possui dependências e não pode ser excluído
- `PHONE_ALREADY_EXISTS`: Telefone já cadastrado para outro cliente
- `EMAIL_ALREADY_EXISTS`: Email já cadastrado para outro cliente

**500 Internal Server Error:**
- `DATABASE_ERROR`: Erro interno do banco de dados
- `SYSTEM_ERROR`: Erro interno do sistema
