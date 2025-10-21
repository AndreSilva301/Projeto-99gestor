# Tarefas para Implementação do Controller de Coworkers (Users)

## Objetivo
Construir um controller de coworkers que permita gerenciar colaboradores da empresa com controle de acesso baseado em perfis de usuário.

---

## Tarefa 1: Expandir UserProfile com Status Inactive
**Descrição:** Adicionar o status Inactive ao enum UserProfile para permitir desativação de usuários.

**Input:** 
- Enum UserProfile atual
- Novo valor: Inactive = 3

**Output:** 
- Enum UserProfile expandido com status de inativo

**Critérios de Aceitação:**
- [x] Enum UserProfile deve conter Inactive = 3
- [x] Aplicação deve compilar sem erros após a mudança
- [x] Migration deve ser criada se necessário

---

## Tarefa 2: Criar UserListDto
**Descrição:** Criar DTO simplificado para listagem de usuários, incluindo apenas dados essenciais.

**Input:** 
- Estrutura da entidade User
- Campos necessários para listagem

**Output:** 
- UserListDto com campos: Id, Name, Email, Profile, CreatedDate, IsActive

**Critérios de Aceitação:**
- [x] DTO deve incluir apenas campos essenciais para listagem
- [x] Campo IsActive deve ser calculado baseado no Profile != Inactive
- [x] DTO deve estar na pasta Application/Dtos
- [x] Deve implementar validações se necessário

---

## Tarefa 3: Criar CreateEmployeeDto
**Descrição:** Criar DTO para criação de novos colaboradores via convite.

**Input:** 
- Campos necessários para criação de employee
- Validações de entrada

**Output:** 
- CreateEmployeeDto com campos: Name, Email

**Critérios de Aceitação:**
- [x] DTO deve incluir validações de email
- [x] DTO deve incluir validação de nome obrigatório
- [x] Email deve ser único na empresa
- [x] Deve implementar IBasicDto se padrão do projeto

---

## Tarefa 4: Criar UpdateUserDto
**Descrição:** Criar DTO para atualização de dados de usuários.

**Input:** 
- Campos editáveis do usuário
- Validações necessárias

**Output:** 
- UpdateUserDto com campos: Name, Email (apenas para próprio usuário ou admin)

**Critérios de Aceitação:**
- [x] DTO deve permitir atualização de Name sempre
- [x] DTO deve permitir atualização de Email apenas para admin ou próprio usuário
- [x] Deve incluir validações apropriadas
- [x] Não deve permitir alteração de Profile ou CompanyId

---

## Tarefa 5: Criar Interface IEmailService
**Descrição:** Criar interface para serviço de envio de emails para convites.

**Input:** 
- Necessidade de envio de convites por email
- Boilerplate de email

**Output:** 
- Interface IEmailService com método SendInviteEmailAsync

**Critérios de Aceitação:**
- [x] Interface deve ter método SendInviteEmailAsync(string email, string userName, string tempPassword)
- [x] Deve estar na pasta Application/Interfaces
- [x] Deve retornar Task<bool> indicando sucesso/falha

---

## Tarefa 6: Implementar EmailService (Boilerplate)
**Descrição:** Implementar serviço básico de email para envio de convites.

**Input:** 
- Interface IEmailService
- Template básico de email de convite

**Output:** 
- Classe EmailService com implementação básica

**Critérios de Aceitação:**
- [x] Implementação deve ser um boilerplate (pode ser mock inicialmente)
- [x] Deve logar tentativas de envio de email
- [x] Deve estar na pasta Application/Services
- [x] Deve implementar IScopedDependency
- [x] Template de email deve incluir dados do usuário e senha temporária

---

## Tarefa 7: Expandir IUserService
**Descrição:** Adicionar métodos necessários para gerenciamento de coworkers.

**Input:** 
- Interface IUserService atual
- Novos métodos necessários

**Output:** 
- Interface expandida com novos métodos

**Critérios de Aceitação:**
- [x] Método GetUsersByCompanyIdAsync(int companyId, bool includeInactive = false)
- [x] Método CreateEmployeeAsync(string name, string email, int companyId)
- [x] Método UpdateUserAsync(User user)
- [x] Método DeactivateUserAsync(int userId)
- [x] Método ReactivateUserAsync(int userId)

---

## Tarefa 8: Atualizar UserService
**Descrição:** Implementar os novos métodos na classe UserService.

**Input:** 
- Classe UserService atual
- Métodos a implementar da interface expandida

**Output:** 
- UserService com implementação completa

**Critérios de Aceitação:**
- [x] GetUsersByCompanyIdAsync deve filtrar por empresa e status ativo/inativo
- [x] CreateEmployeeAsync deve criar usuário com perfil Employee e senha temporária
- [x] CreateEmployeeAsync deve enviar email de convite
- [x] UpdateUserAsync deve permitir atualização de dados básicos
- [x] DeactivateUserAsync deve alterar Profile para Inactive
- [x] ReactivateUserAsync deve alterar Profile para Employee
- [x] Todos os métodos devem incluir tratamento de erros

---

## Tarefa 9: Expandir IUserRepository
**Descrição:** Adicionar métodos de consulta por empresa no repositório.

**Input:** 
- Interface IUserRepository
- Necessidade de filtros por empresa e status

**Output:** 
- Interface com métodos específicos para coworkers

**Critérios de Aceitação:**
- [x] Método GetByCompanyIdAsync(int companyId, bool includeInactive = false)
- [x] Método existente GetByEmailAsync deve ser mantido
- [x] Verificar se outros métodos necessários já existem via IBaseRepository

---

## Tarefa 10: Atualizar UserRepository
**Descrição:** Implementar os novos métodos no repositório.

**Input:** 
- Classe UserRepository
- Métodos da interface expandida

**Output:** 
- UserRepository com implementação dos novos métodos

**Critérios de Aceitação:**
- [x] GetByCompanyIdAsync deve usar LINQ para filtrar por CompanyId
- [x] Parâmetro includeInactive deve controlar se inclui usuários inativos
- [x] Deve usar Include para carregar dados da Company se necessário
- [x] Performance deve ser otimizada com consultas eficientes

---

## Tarefa 11: Implementar CoworkersController
**Descrição:** Criar o controller principal para gerenciamento de colaboradores.

**Input:** 
- Estrutura base dos controllers existentes
- AuthBaseController como classe base
- IUserService como dependência

**Output:** 
- CoworkersController com endpoints:
  - GET /api/coworkers (listar colaboradores da empresa - apenas Admin)
  - POST /api/coworkers (criar employee - apenas Admin)
  - PUT /api/coworkers/{id} (atualizar usuário - Admin ou próprio usuário)
  - DELETE /api/coworkers/{id} (desativar usuário - apenas Admin)
  - POST /api/coworkers/{id}/reactivate (reativar usuário - apenas Admin)

**Critérios de Aceitação:**
- [x] Controller deve herdar de AuthBaseController
- [x] Deve ter rota [Route("api/[controller]")]
- [x] GET deve verificar se usuário é Admin da empresa
- [x] GET deve retornar UserListDto com filtro de inativos opcional
- [x] POST deve verificar se usuário é Admin e criar Employee
- [x] PUT deve verificar se é próprio usuário OU Admin da mesma empresa
- [x] DELETE deve apenas desativar (não deletar fisicamente)
- [x] Todos os endpoints devem verificar acesso apenas à própria empresa

---

## Tarefa 12: Implementar Validações de Autorização
**Descrição:** Criar métodos auxiliares para validação de acesso específicos para coworkers.

**Input:** 
- CurrentUser do contexto
- IDs de usuários a serem acessados

**Output:** 
- Métodos de validação no controller

**Critérios de Aceitação:**
- [x] Método IsAdminOfCompany() deve verificar se usuário é Admin da empresa
- [x] Método CanEditUser(int userId) deve verificar se pode editar (próprio usuário OU admin da empresa)
- [x] Método IsFromSameCompany(int userId) deve verificar se usuário pertence à mesma empresa
- [x] Validações devem retornar ActionResult apropriado em caso de acesso negado

---

## Tarefa 13: Implementar Geração de Senha Temporária
**Descrição:** Criar utilitário para geração de senhas temporárias seguras.

**Input:** 
- Necessidade de senhas temporárias para novos colaboradores
- Critérios de segurança de senha

**Output:** 
- Classe PasswordHelper ou método no UserService

**Critérios de Aceitação:**
- [x] Senha deve ter 8-12 caracteres
- [x] Deve incluir letras maiúsculas, minúsculas e números
- [x] Deve ser aleatória e segura
- [x] Deve seguir padrões de validação existentes no projeto

---

## Tarefa 14: Criar Testes Unitários
**Descrição:** Criar testes unitários para o CoworkersController e serviços relacionados.

**Input:** 
- CoworkersController implementado
- UserService expandido
- Padrão de testes existente no projeto

**Output:** 
- Classes de teste com cobertura completa

**Critérios de Aceitação:**
- [x] Testes para CoworkersController (todos os endpoints)
- [x] Testes para UserService (novos métodos)
- [x] Testes para EmailService (envio de convites)
- [x] Testes para cenários de autorização
- [x] Testes para validações de entrada
- [x] Cobertura mínima de 80%

---

## Tarefa 15: Documentação da API
**Descrição:** Documentar os endpoints de coworkers no Swagger/OpenAPI.

**Input:** 
- Controller implementado
- DTOs criados
- Padrão de documentação existente

**Output:** 
- Endpoints documentados com exemplos e descrições

**Critérios de Aceitação:**
- [x] Todos os endpoints devem ter summary e description
- [x] Parâmetros e DTOs devem estar documentados
- [x] Códigos de status HTTP devem estar documentados
- [x] Exemplos de requisição e resposta devem estar incluídos
- [x] Documentação deve incluir regras de autorização

---

## Dependências entre Tarefas

### Fase 1 - Fundação (paralelo)
- Tarefas 1, 2, 3, 4, 5: Podem ser executadas em paralelo

### Fase 2 - Serviços (sequencial)
- Tarefa 6: Depende da Tarefa 5
- Tarefas 7, 9: Podem ser executadas em paralelo após Fase 1
- Tarefas 8, 10: Dependem das Tarefas 7 e 9 respectivamente

### Fase 3 - Controller (sequencial)
- Tarefa 13: Pode ser executada em paralelo com Fase 2
- Tarefa 11: Depende das Tarefas 8, 10, 13
- Tarefa 12: Pode ser executada em paralelo com Tarefa 11

### Fase 4 - Qualidade (paralelo)
- Tarefas 14, 15: Dependem das Tarefas 11 e 12

---

## Notas Técnicas

### Segurança
- Implementar rate limiting para criação de usuários
- Validar unicidade de email por empresa
- Logs de auditoria para ações administrativas

### Performance
- Usar paginação na listagem de colaboradores
- Implementar cache para consultas frequentes
- Otimizar consultas com Include apropriados

### Email
- Configurar SMTP settings no appsettings
- Implementar template HTML para convites
- Considerar fila de emails para ambiente de produção

### Padrões do Projeto
- Utilizar padrão Repository/Service existente
- Manter consistência com controllers existentes
- Aplicar validações de entrada em todos os endpoints
- Usar DTOs para evitar exposição desnecessária de dados 


