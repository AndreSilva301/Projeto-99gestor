# Tarefas para Implementação do Controller de Company

## Objetivo
Construir um controller de company que permita operações CRUD com controle de acesso baseado em perfis de usuário.

---

## Tarefa 1: Implementar Role SystemAdmin
**Descrição:** Adicionar o perfil SystemAdmin ao enum UserProfile para permitir acesso global a todas as empresas.

**Input:** 
- Modificação do enum UserProfile

**Output:** 
- Enum UserProfile atualizado com valor SystemAdmin = 3

**Critérios de Aceitação:**
- [x] Enum UserProfile deve conter SystemAdmin = 3
- [x] Aplicação deve compilar sem erros após a mudança

---

## Tarefa 2: Expandir Entidade Company
**Descrição:** Adicionar objetos complexos Address e Phone à entidade Company seguindo o mesmo padrão usado na entidade Customer.

**Input:** 
- Entidade Company atual
- Entidades Address e Phone existentes no projeto
- Padrão usado na entidade Customer

**Output:** 
- Entidade Company expandida com propriedades Address e Phone

**Critérios de Aceitação:**
- [x] Propriedade Address deve ser do tipo Address (não nullable, inicializada como new Address())
- [x] Propriedade Phone deve ser do tipo Phone (não nullable, inicializada como new Phone())
- [x] Seguir exatamente o mesmo padrão usado na entidade Customer
- [x] Address deve incluir: Street, Number, Complement, Neighborhood, City, State, ZipCode
- [x] Phone deve incluir: Mobile, Landline (ambos nullable)
- [x] Migration deve ser criada para atualizar o banco de dados com estrutura complexa
- [x] EF Core deve ser configurado para mapear objetos de valor (Owned Types)

---

## Tarefa 2.1: Configurar Entity Framework para Objetos Complexos
**Descrição:** Configurar o Entity Framework para mapear corretamente os objetos Address e Phone como Owned Types na entidade Company.

**Input:** 
- Entidade Company expandida com Address e Phone
- ApplicationDbContext existente
- Padrão de configuração usado para Customer

**Output:** 
- Configuração EF Core para Company com Owned Types
- Migration gerada corretamente

**Critérios de Aceitação:**
- [x] Configurar Company.Address como Owned Type no modelBuilder
- [x] Configurar Company.Phone como Owned Type no modelBuilder
- [x] Seguir o mesmo padrão de nomeação de colunas usado em Customer
- [x] Verificar se configuração não conflita com Customer existente
- [x] Migration deve criar colunas separadas para cada propriedade de Address e Phone
- [x] Testes de integração devem passar após migration

---

## Tarefa 3: Criar DTOs para Company
**Descrição:** Criar DTOs para operações de leitura e atualização da empresa utilizando objetos complexos Address e Phone.

**Input:** 
- Estrutura da entidade Company expandida
- Objetos Address e Phone
- Campos editáveis: Name, CNPJ, Address (completo), Phone (completo)

**Output:** 
- CompanyDto: para retorno de dados completos
- UpdateCompanyDto: para operações de atualização
- AddressDto: DTO para objeto Address (se não existir)
- PhoneDto: DTO para objeto Phone (se não existir)

**Critérios de Aceitação:**
- [x] CompanyDto deve incluir: Id, Name, CNPJ, Address, Phone, DateTime
- [x] UpdateCompanyDto deve incluir: Name, CNPJ, Address, Phone
- [x] AddressDto deve mapear todos os campos de Address: Street, Number, Complement, Neighborhood, City, State, ZipCode
- [x] PhoneDto deve mapear todos os campos de Phone: Mobile, Landline
- [x] DTOs devem ter validações appropriadas (Required para campos obrigatórios, MaxLength, etc.)
- [x] UpdateCompanyDto deve validar estrutura completa do endereço quando fornecido
- [x] DTOs devem estar na pasta Application/Dtos
- [x] Usar AutoMapper ou mapeamento manual para conversão entre entidade e DTOs

---

## Tarefa 4: Expandir Interface ICompanyServices
**Descrição:** Adicionar métodos necessários para o controller na interface de serviços.

**Input:** 
- Interface ICompanyServices atual
- Novos métodos necessários

**Output:** 
- Interface expandida com métodos GetAllAsync(), GetByIdAsync()

**Critérios de Aceitação:**
- [x] Método GetAllAsync() deve retornar Task<IEnumerable<Company>>
- [x] Método GetByIdAsync(int id) deve retornar Task<Company?>
- [x] Interface deve manter compatibilidade com implementação existente

---

## Tarefa 5: Atualizar CompanyServices
**Descrição:** Implementar os novos métodos na classe CompanyServices.

**Input:** 
- Classe CompanyServices atual
- Métodos a implementar: GetAllAsync(), GetByIdAsync()

**Output:** 
- CompanyServices com implementação completa

**Critérios de Aceitação:**
- [x] GetAllAsync() deve usar o repositório para buscar todas as empresas
- [x] GetByIdAsync() deve usar o repositório para buscar empresa por ID
- [x] Métodos devem incluir tratamento de erros apropriado
- [x] Deve manter padrão de exceções existente

---

## Tarefa 6: Expandir ICompanyRepository
**Descrição:** Adicionar método GetAllAsync() na interface do repositório se não existir.

**Input:** 
- Interface ICompanyRepository atual
- Herança de IBaseRepository<Company>

**Output:** 
- Interface com método GetAllAsync() disponível

**Critérios de Aceitação:**
- [x] Verificar se GetAllAsync() já existe via herança de IBaseRepository
- [x] Adicionar método se necessário
- [x] Manter compatibilidade com implementação existente

---

## Tarefa 7: Implementar CompanyController
**Descrição:** Criar o controller com endpoints para listar, obter e atualizar empresas usando objetos complexos Address e Phone.

**Input:** 
- Estrutura base dos controllers existentes
- AuthBaseController como classe base
- ICompanyServices como dependência
- DTOs com objetos Address e Phone

**Output:** 
- CompanyController com endpoints:
  - GET /api/company (listar todas - apenas SystemAdmin)
  - GET /api/company/{id} (obter por ID - Admin da empresa ou SystemAdmin)
  - PUT /api/company/{id} (atualizar - Admin da empresa ou SystemAdmin)

**Critérios de Aceitação:**
- [x] Controller deve herdar de AuthBaseController
- [x] Deve ter rota [Route("api/[controller]")]
- [x] Endpoint GET all deve verificar se usuário é SystemAdmin
- [x] Endpoint GET all deve retornar CompanyDto com Address e Phone completos
- [x] Endpoints GET by ID e PUT devem verificar se usuário é admin da empresa OU SystemAdmin
- [x] Endpoint PUT deve aceitar UpdateCompanyDto com objetos Address e Phone completos
- [x] Endpoint PUT deve validar estrutura de Address (campos obrigatórios: Street, Number, Neighborhood, City, State, ZipCode)
- [x] Endpoint PUT deve permitir Phone com Mobile e/ou Landline opcionais
- [x] Todos os endpoints devem retornar respostas HTTP apropriadas
- [x] Deve incluir tratamento de erros e validações para objetos complexos
- [x] Mapeamento adequado entre DTOs e entidades

---

## Tarefa 8: Implementar Validações de Autorização
**Descrição:** Criar métodos auxiliares para validação de acesso às empresas.

**Input:** 
- CurrentUser do contexto
- ID da empresa a ser acessada

**Output:** 
- Métodos de validação no controller

**Critérios de Aceitação:**
- [x] Método IsSystemAdmin() deve verificar se usuário tem perfil SystemAdmin
- [x] Método IsCompanyAdmin(int companyId) deve verificar se usuário é admin da empresa específica
- [x] Métodos devem retornar ActionResult apropriado em caso de acesso negado
- [x] Validações devem ser aplicadas em todos os endpoints

---

## Tarefa 9: Criar Testes Unitários
**Descrição:** Criar testes unitários para o CompanyController.

**Input:** 
- CompanyController implementado
- Padrão de testes existente no projeto

**Output:** 
- Classe CompanyControllerTests com cobertura completa

**Critérios de Aceitação:**
- [x] Testes para todos os endpoints
- [x] Testes para cenários de autorização (SystemAdmin, Admin da empresa, usuário sem permissão)
- [x] Testes para validações de entrada
- [x] Testes para tratamento de erros
- [x] Cobertura mínima de 80%

---

## Tarefa 10: Documentação da API
**Descrição:** Documentar os endpoints no Swagger/OpenAPI.

**Input:** 
- Controller implementado
- Padrão de documentação existente

**Output:** 
- Endpoints documentados com exemplos e descrições

**Critérios de Aceitação:**
- [x] Todos os endpoints devem ter summary e description
- [x] Parâmetros devem estar documentados
- [x] Modelos de response devem estar definidos
- [x] Códigos de status HTTP devem estar documentados
- [x] Exemplos de requisição e resposta devem estar incluídos

---

## Dependências entre Tarefas
1. Tarefas 1 e 2 podem ser executadas em paralelo
2. Tarefa 2.1 depende da Tarefa 2 (configuração EF após expansão da entidade)
3. Tarefa 3 depende da Tarefa 2.1 (DTOs após configuração EF)
4. Tarefas 4, 5 e 6 podem ser executadas após Tarefa 3
5. Tarefa 7 depende das Tarefas 1, 3, 4, 5 e 6
6. Tarefa 8 pode ser executada em paralelo com Tarefa 7
7. Tarefas 9 e 10 dependem das Tarefas 7 e 8

### Ordem Recomendada de Execução:
**Fase 1:** Tarefas 1 e 2 (paralelo)
**Fase 2:** Tarefa 2.1 (configuração EF)
**Fase 3:** Tarefa 3 (DTOs)
**Fase 4:** Tarefas 4, 5, 6 (paralelo - serviços e repositórios)
**Fase 5:** Tarefas 7 e 8 (paralelo - controller e validações)
**Fase 6:** Tarefas 9 e 10 (paralelo - testes e documentação)

## Notas Técnicas

### Padrões do Projeto
- Utilizar padrão Repository/Service existente no projeto
- Manter consistência com controllers existentes (especialmente CustomerController)
- Aplicar validações de entrada em todos os endpoints
- Usar DTOs para evitar exposição desnecessária de dados
- Implementar logs apropriados para auditoria

### Objetos Complexos (Address e Phone)
- Seguir exatamente o mesmo padrão usado na entidade Customer
- Configurar Address e Phone como Owned Types no Entity Framework
- Address deve ter campos obrigatórios: Street, Number, Neighborhood, City, State, ZipCode
- Address.Complement é opcional
- Phone.Mobile e Phone.Landline são ambos opcionais
- Mapear corretamente os objetos complexos nos DTOs

### Configuração Entity Framework
```csharp
// Exemplo de configuração no DbContext ou EntityTypeConfiguration
modelBuilder.Entity<Company>()
    .OwnsOne(c => c.Address);
    
modelBuilder.Entity<Company>()
    .OwnsOne(c => c.Phone);
```

### Validações Específicas
- Validar estrutura completa do Address quando fornecido
- Permitir Address e Phone vazios/nulos em cenários apropriados
- Manter consistência com validações usadas em Customer 


