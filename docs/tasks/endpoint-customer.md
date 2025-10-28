# Tarefas para Implementação do CRUD de Customer

## 4.1.1 - Ajustar entidade de domínio para conter campos faltantes

**Descrição:** Adicionar campos de auditoria nas entidades Customer e CustomerRelationship para suportar controle de criação, atualização e soft delete.

**Critérios de Aceitação:**
- Adicionar em `Customer`: `CreatedDate` (DateTime UTC), `UpdatedDate` (DateTime? UTC), `IsDeleted` (bool) para soft delete
- Adicionar em `CustomerRelationship`: `CreatedDate` (DateTime UTC), `UpdatedDate` (DateTime? UTC) se apropriado
- Manter todas as propriedades e anotações existentes inalteradas
- Configurar valores padrão apropriados (`CreatedDate = DateTime.UtcNow`, `IsDeleted = false`)
- Usar UTC para datas; campos nullable para `UpdatedDate`
- Preparar para futuras migrations do EF Core
- Manter compatibilidade com código existente

---

## 4.1.2 - Criar DTOs necessários

**Descrição:** Criar DTOs reutilizáveis para Customer e CustomerRelationship que serão utilizados pelos endpoints da API.

**Critérios de Aceitação:**
- `CustomerCreateDto` - dados para criar customer + optional relationships array de `CustomerRelationshipCreateDto`
- `CustomerUpdateDto` - somente dados do customer que podem ser atualizados (não incluir relationships)
- `CustomerDto` - detalhado, usado em GET por id; inclui relationships (list de `CustomerRelationshipDto`)
- `CustomerListItemDto` - versão leve para listagem/pesquisa (id, name, phone, createdDate, updatedDate)
- `CustomerRelationshipDto` - detalhe do relacionamento (id, description, dateTime)
- `CustomerRelationshipCreateDto` - description required
- `CustomerRelationshipUpdateDto` - id required, description required
- Todos os DTOs devem compilar corretamente
- DTOs não devem expor entidades de domínio diretamente
- DTOs em inglês; reuso quando possível

---

## 4.1.3 - Implementar interface IBasicDto nos novos DTOs

**Descrição:** Implementar a interface `IBasicDto` em todos os DTOs criados, fornecendo validações básicas através dos métodos `Validate()` e `IsValid()`.

**Critérios de Aceitação:**
- Todos os DTOs implementam `List<string> Validate()` e `bool IsValid()`
- Validações básicas implementadas: nome obrigatório, descrição máximo 500 caracteres, campos required não vazios
- Validações simples e testáveis dentro do DTO (não depender de serviços externos)
- Retornar lista de mensagens de erro específicas em `Validate()`
- `IsValid()` deve retornar `true` quando `Validate().Count == 0`

---

## 4.1.4 - Implementar repositório

**Descrição:** Implementar `ICustomerRepository` e `CustomerRepository` herdando de `BaseRepository<Customer>` com métodos específicos para operações de Customer e CustomerRelationship.

**Critérios de Aceitação:**
- `ICustomerRepository` com métodos detalhados:
  
  **`GetByIdWithRelationshipsAsync(int id)`**
  - Entrada: `id` (int) - ID do customer
  - Saída: `Task<Customer?>` - Customer com relationships carregados via Include, ou null se não encontrado
  - Comportamento: Usar `.Include(c => c.CostumerRelationships)` e filtrar `IsDeleted = false`
  
  **`GetPagedByCompanyAsync(int companyId, int page, int pageSize, string? searchTerm)`**
  - Entrada: `companyId` (int), `page` (int), `pageSize` (int), `searchTerm` (string?, opcional)
  - Saída: `Task<PagedResult<Customer>>` - Resultado paginado com customers da company
  - Comportamento: Filtrar por `CompanyId` e `IsDeleted = false`, aplicar searchTerm em Name/Email se fornecido, ordenar por CreatedDate DESC
  
  **`GetRelationshipsByCustomerAsync(int customerId)`**
  - Entrada: `customerId` (int) - ID do customer
  - Saída: `Task<IEnumerable<CustomerRelationship>>` - Lista de relationships do customer
  - Comportamento: Filtrar por `CostumerId` e ordenar por DateTime DESC
  
  **`SoftDeleteAsync(int customerId)`**
  - Entrada: `customerId` (int) - ID do customer a ser deletado
  - Saída: `Task` - Operação assíncrona
  - Comportamento: Definir `IsDeleted = true` e `UpdatedDate = DateTime.UtcNow`, salvar no contexto
  
  **`AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationship> relationships)`**
  - Entrada: `customerId` (int), `relationships` (IEnumerable<CustomerRelationship>) - Lista de relationships para adicionar/atualizar
  - Saída: `Task<IEnumerable<CustomerRelationship>>` - Relationships processados com IDs atualizados
  - Comportamento: Para cada relationship, se Id > 0 atualizar existente, senão criar novo; definir CustomerId e datas de auditoria
  
  **`DeleteRelationshipsAsync(IEnumerable<int> relationshipIds, int customerId)`**
  - Entrada: `relationshipIds` (IEnumerable<int>), `customerId` (int) - IDs dos relationships e customer para validação
  - Saída: `Task` - Operação assíncrona
  - Comportamento: Remover relationships que pertencem ao customerId especificado, ignorar IDs inválidos

- `CustomerRepository` implementando acima e herdando obrigatoriamente de `BaseRepository<Customer>`
- Métodos otimizados com EF Core (Include para relationships quando necessário)
- Usar async/await, AsNoTracking para leitura
- Consultas paginadas eficientes com Count separado
- Todos os métodos implementam soft delete e filtros por company

---

## 4.1.5 - Implementar testes de integração do repositório

**Descrição:** Escrever testes de integração para `CustomerRepository` usando o padrão de `UserRepositoryIntegrationTests.cs`.

**Critérios de Aceitação:**
- Testes cobrem: adicionar customer, relationships CRUD, consulta por id com relationships, paginação por company, soft delete
- Usar `TestDbContextFactory` e limpar dados entre testes (`ExecuteSqlRaw DELETE`)
- Testes determinísticos e isolados
- Seguir padrão existente do projeto para testes de integração
- Usar `ClassInitialize/TestCleanup` apropriadamente
- Validar comportamentos de soft delete e filtros de company
- Testes com nomes claros em português

---

## 4.1.6 - Implementar camada de serviço

**Descrição:** Implementar camada de serviços que encapsula regras de negócio e validações entre controller e repository.

**Critérios de Aceitação:**
- `ICustomerService` com métodos detalhados:

  **`CreateAsync(CustomerCreateDto dto, int currentUserId)`**
  - Entrada: `dto` (CustomerCreateDto) - dados do customer, `currentUserId` (int) - ID do usuário atual
  - Saída: `Task<CustomerDto>` - Customer criado com relationships incluídos
  - Comportamento: Validar DTO, mapear para entidade, definir CompanyId do user, criar relationships opcionais, retornar DTO mapeado
  
  **`UpdateAsync(int customerId, CustomerUpdateDto dto, int currentUserId)`**
  - Entrada: `customerId` (int), `dto` (CustomerUpdateDto), `currentUserId` (int)
  - Saída: `Task<CustomerDto>` - Customer atualizado
  - Comportamento: Buscar customer, validar company matching, aplicar mudanças do DTO, definir UpdatedDate, salvar e retornar DTO
  
  **`SoftDeleteAsync(int customerId, int currentUserId)`**
  - Entrada: `customerId` (int), `currentUserId` (int)
  - Saída: `Task` - Operação assíncrona
  - Comportamento: Buscar customer, validar company matching, chamar repository.SoftDeleteAsync
  
  **`GetByIdAsync(int customerId, int currentUserId)`**
  - Entrada: `customerId` (int), `currentUserId` (int)
  - Saída: `Task<CustomerDto?>` - Customer com relationships ou null se não encontrado/sem acesso
  - Comportamento: Buscar customer com relationships, validar company matching, mapear para DTO
  
  **`SearchAsync(string? searchTerm, int page, int pageSize, int companyId)`**
  - Entrada: `searchTerm` (string?, opcional), `page` (int), `pageSize` (int), `companyId` (int)
  - Saída: `Task<PagedResult<CustomerListItemDto>>` - Resultado paginado de customers
  - Comportamento: Chamar repository paginado, mapear entities para CustomerListItemDto

- `CustomerService` implementando validações de company matching entre user e customer
- Todas as operações validam que `user.CompanyId == customer.CompanyId`
- Service retorna apenas DTOs, nunca entidades
- Lança `BusinessException` para violações de regras (ex: "Customer não pertence à company do usuário")
- Mapeamento entre DTOs e Entities (AutoMapper ou manual)
- Separar responsabilidades: service coordena repository e validações
- Validações claras e mensagens de erro informativas

---

## 4.1.7 - Testes unitários da camada de serviço

**Descrição:** Criar testes unitários para `CustomerService` mockando `ICustomerRepository` e outras dependências.

**Critérios de Aceitação:**
- Cobrir cenários: criação com sucesso, tentativa de acesso cross-company (deve falhar), updates, soft delete
- Usar Moq para `ICustomerRepository`, `IMapper` e outras dependências
- Validar que `BusinessExceptions` são lançadas nos casos apropriados
- Um teste por regra de negócio; nomes claros em português
- Não depender do banco; usar mocks para controlar respostas
- Verificar calls nos mocks para garantir que repository é chamado corretamente

---

## 4.1.8 - Implementar controller com ações de Customer

**Descrição:** Implementar `CustomerController` herdando de `AuthBaseController` com endpoints básicos de CRUD para Customer.

**Critérios de Aceitação:**
- Endpoints implementados com detalhes:

  **`POST /api/customers`**
  - Entrada: `CustomerCreateDto` no body
  - Saída: `ActionResult<CustomerDto>` - 201 Created com customer criado
  - Comportamento: Validar ModelState, chamar service.CreateAsync, retornar CreatedAtAction referenciando GET por ID
  
  **`PUT /api/customers/{id}`**
  - Entrada: `id` (int) na rota, `CustomerUpdateDto` no body
  - Saída: `ActionResult<CustomerDto>` - 200 OK com customer atualizado
  - Comportamento: Validar ModelState, chamar service.UpdateAsync, retornar DTO atualizado ou 404 se não encontrado
  
  **`DELETE /api/customers/{id}`**
  - Entrada: `id` (int) na rota
  - Saída: `ActionResult` - 204 NoContent
  - Comportamento: Chamar service.SoftDeleteAsync, retornar NoContent ou 404 se não encontrado
  
  **`GET /api/customers/{id}`**
  - Entrada: `id` (int) na rota
  - Saída: `ActionResult<CustomerDto>` - 200 OK com customer e relationships
  - Comportamento: Chamar service.GetByIdAsync, retornar DTO ou 404 se não encontrado/sem acesso
  
  **`GET /api/customers`**
  - Entrada: Query params `searchTerm` (string?), `page` (int, default=1), `pageSize` (int, default=10)
  - Saída: `ActionResult<PagedResult<CustomerListItemDto>>` - 200 OK com resultados paginados
  - Comportamento: Obter companyId do CurrentUser, chamar service.SearchAsync, retornar resultado paginado

- Controller herda obrigatoriamente de `AuthBaseController`
- Todos métodos recebem DTOs e retornam DTOs (nunca entidades)
- Usar `GetCurrentUserId()/CurrentUser` para obter dados do usuário autenticado
- Status codes corretos: 200 OK, 201 Created, 204 NoContent, 400 BadRequest, 404 NotFound
- Tratamento de erros com try/catch para BusinessException → 400 BadRequest
- Actions pequenas e focadas; usar `CreatedAtAction` para resources criados
- ModelState validation automática com retorno de erros padronizados

---

## 4.1.9 - Implementar testes de integração de Customer controller

**Descrição:** Criar testes de integração para os endpoints básicos de Customer seguindo `AuthControllerIntegrationTests.cs`.

**Critérios de Aceitação:**
- Cobrir endpoints: create (201), update (200), delete (204), get by id (200), search paged (200)
- Validar cenários de erro: cross-company access (403/404), validations (400), not found (404)
- Usar `CustomWebApplicationFactory` e cleanup entre testes
- Testes legíveis com setup/act/assert claro
- Reutilizar helpers de autenticação e seed de dados
- Verificar response bodies e status codes precisos

---

## 4.1.10 - Implementar métodos de controller de Customer Relationship

**Descrição:** Adicionar métodos no `CustomerController` para gerenciar CustomerRelationships.

**Critérios de Aceitação:**
- Endpoints implementados com detalhes:

  **`POST /api/customers/{id}/relationships`**
  - Entrada: `id` (int) na rota, array de `CustomerRelationshipCreateDto/UpdateDto` no body
  - Saída: `ActionResult<IEnumerable<CustomerRelationshipDto>>` - 201 Created com relationships processados
  - Comportamento: Validar ModelState, verificar se customer existe e pertence ao user, chamar service.AddOrUpdateRelationshipsAsync, retornar array de DTOs criados/atualizados
  
  **`GET /api/customers/{id}/relationships`**
  - Entrada: `id` (int) na rota - ID do customer
  - Saída: `ActionResult<IEnumerable<CustomerRelationshipDto>>` - 200 OK com lista de relationships
  - Comportamento: Verificar acesso ao customer, chamar service.ListRelationshipsAsync, retornar array de DTOs ordenados por data
  
  **`DELETE /api/customers/{id}/relationships`**
  - Entrada: `id` (int) na rota, array de `int` (relationship IDs) no body
  - Saída: `ActionResult` - 204 NoContent
  - Comportamento: Verificar acesso ao customer, validar que relationships pertencem ao customer, chamar service.DeleteRelationshipsAsync

- Validações obrigatórias:
  - Company matching: `customer.CompanyId == CurrentUser.CompanyId`
  - Relationship ownership: todos relationship IDs devem pertencer ao customer especificado
  - ModelState validation para DTOs de entrada
- Suporte para operações em lote (criar/atualizar múltiplos relationships de uma vez)
- Tratamento de erros: BusinessException → 400 BadRequest, customer não encontrado → 404 NotFound
- Status codes corretos: 200 OK para GET, 201 Created para POST, 204 NoContent para DELETE

---

## 4.1.11 - Implementar camada de serviço para Customer Relationship

**Descrição:** Adicionar métodos no `ICustomerService` e `CustomerService` para gerenciar CustomerRelationships.

**Critérios de Aceitação:**
- Métodos adicionados com detalhes:

  **`AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationshipCreateOrUpdateDto> dtos, int currentUserId)`**
  - Entrada: `customerId` (int), `dtos` (IEnumerable de DTOs mistos create/update), `currentUserId` (int)
  - Saída: `Task<IEnumerable<CustomerRelationshipDto>>` - Relationships processados
  - Comportamento: Validar customer exists e company matching, para cada DTO: se tem Id > 0 atualizar existente (validar ownership), senão criar novo; definir auditoria; mapear retorno para DTOs
  
  **`ListRelationshipsAsync(int customerId, int currentUserId)`**
  - Entrada: `customerId` (int), `currentUserId` (int)
  - Saída: `Task<IEnumerable<CustomerRelationshipDto>>` - Lista de relationships do customer
  - Comportamento: Validar customer exists e company matching, buscar relationships via repository, mapear para DTOs ordenados por DateTime DESC
  
  **`DeleteRelationshipsAsync(int customerId, IEnumerable<int> relationshipIds, int currentUserId)`**
  - Entrada: `customerId` (int), `relationshipIds` (IEnumerable<int>), `currentUserId` (int)
  - Saída: `Task` - Operação assíncrona
  - Comportamento: Validar customer exists e company matching, verificar que todos relationshipIds pertencem ao customer, chamar repository.DeleteRelationshipsAsync

- Validações obrigatórias implementadas:
  - Company matching: `customer.CompanyId == user.CompanyId` antes de qualquer operação
  - Relationship ownership: ao atualizar/deletar, verificar que relationship.CustomerId == customerId fornecido
  - DTO validation: chamar dto.IsValid() e lançar BusinessException se inválido
- Suporte para operações em lote com validações por item
- Lançar `BusinessException` com mensagens específicas:
  - "Customer não encontrado ou não pertence à sua company"
  - "Relationship {id} não pertence ao customer {customerId}"
  - "Dados do relationship inválidos: {erros de validação}"
- Mapeamento adequado entre DTOs e entities usando AutoMapper ou mapeamento manual
- Definir campos de auditoria (CreatedDate, UpdatedDate) automaticamente

---

## 4.1.12 - Testes unitários da camada de serviço para Customer Relationship

**Descrição:** Criar testes unitários para os novos métodos de CustomerRelationship no `CustomerService`.

**Critérios de Aceitação:**
- Cobrir cenários: criação/atualização com sucesso, validações de ownership, cross-company access
- Testar operações em lote (mix de create/update)
- Validar que `BusinessExceptions` são lançadas para violações
- Usar mocks para controlar respostas do repository
- Testes focados em regras de negócio específicas de relationships

---

## 4.1.13 - Testes de integração dos métodos de Customer Relationship

**Descrição:** Criar testes de integração para os novos endpoints de CustomerRelationship.

**Critérios de Aceitação:**
- Cobrir endpoints: create/update relationships (201), list relationships (200), delete relationships (204)
- Validar cenários de erro: relationship não pertence ao customer, cross-company access
- Testar operações em lote e validações de integridade
- Usar padrão existente de testes de integração do projeto
- Cleanup adequado entre testes

---

## 4.1.14 - Criar AutoMapper profile para Customer/DTOs

**Descrição:** Criar `CustomerProfile` (AutoMapper) para mapear entre todas as combinações de Customer/CustomerRelationship entities e seus DTOs.

**Critérios de Aceitação:**
- Profiles configurados com mapeamentos detalhados:

  **Customer Entity Mappings:**
  - `Customer → CustomerDto`: Incluir relationships mapeados para CustomerRelationshipDto
  - `Customer → CustomerListItemDto`: Projeção apenas de campos necessários (Id, Name, Phone, CreatedDate, UpdatedDate)
  - `CustomerCreateDto → Customer`: Mapear campos básicos, ignorar Id/auditoria, processar relationships opcionais
  - `CustomerUpdateDto → Customer`: Aplicar apenas campos atualizáveis, definir UpdatedDate = DateTime.UtcNow
  
  **CustomerRelationship Entity Mappings:**
  - `CustomerRelationship → CustomerRelationshipDto`: Mapeamento direto de todos os campos
  - `CustomerRelationshipCreateDto → CustomerRelationship`: Definir CustomerId, CreatedDate = DateTime.UtcNow
  - `CustomerRelationshipUpdateDto → CustomerRelationship`: Manter Id, definir UpdatedDate = DateTime.UtcNow
  
  **Configurações específicas com ForMember:**
  - `ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))` para creates
  - `ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))` para updates
  - `ForMember(dest => dest.Id, opt => opt.Ignore())` para CreateDtos
  - `ForMember(dest => dest.Company, opt => opt.Ignore())` para evitar lazy loading
  
- Mapeamentos registrados no DI container via `services.AddAutoMapper(typeof(CustomerProfile))`
- Cobrir collections: `CostumerRelationships` mapeado corretamente para arrays de DTOs
- Transformações de data sempre em UTC
- Configurar `ReverseMap()` onde apropriado para mapeamentos bidirecionais
- Testes básicos do mapeamento em CustomerProfileTests para garantir funcionamento:
  - Teste Customer → CustomerDto (com relationships)
  - Teste CustomerCreateDto → Customer (auditoria definida)
  - Teste CustomerUpdateDto → Customer (UpdatedDate definido)
  - Teste collections não quebram o mapeamento

---

## Resumo dos Endpoints Implementados

### Customer Endpoints
1. `POST /api/customers` - Criar customer
2. `PUT /api/customers/{id}` - Atualizar customer  
3. `DELETE /api/customers/{id}` - Soft delete customer
4. `GET /api/customers/{id}` - Obter customer por ID
5. `GET /api/customers` - Buscar/listar customers (paginado)

### Customer Relationship Endpoints
6. `POST /api/customers/{id}/relationships` - Criar/atualizar relationships
7. `GET /api/customers/{id}/relationships` - Listar relationships
8. `DELETE /api/customers/{id}/relationships` - Excluir relationships

### Regras de Negócio Implementadas
- **Company Isolation**: Users só acessam customers da mesma company
- **Soft Delete**: Exclusão lógica com flag `IsDeleted`
- **Relationship Ownership**: Relationships validados contra customer correto
- **DTO Validation**: Implementação de `IBasicDto` em todos os DTOs
- **Auditoria**: Campos `CreatedDate`, `UpdatedDate` em todas as entidades
