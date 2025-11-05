# 📋 Tarefas - Implementação dos Endpoints de Orçamentos (Quotes)

## Visão Geral
Este documento descreve todas as tarefas necessárias para implementar os endpoints de gerenciamento de orçamentos no sistema ManiaDeLimpeza. As tarefas estão organizadas em camadas (Domínio, DTOs, Persistência, Serviço, API e Testes).

---

## 5.1 - Adequação da Entidade Quote e QuoteItem

### Descrição
Revisar e adequar as entidades `Quote` e `QuoteItem` do domínio para garantir que atendem aos requisitos do MVP, incluindo relacionamentos corretos, validações e campos necessários.

### Critérios de Aceitação
- [ ] Entidade `Quote` possui todos os campos necessários (Id, CustomerId, UserId, CreatedAt, TotalPrice, PaymentMethod, PaymentConditions, CashDiscount)
- [ ] Entidade `QuoteItem` possui campos necessários (Id, QuoteId, Description, Quantity, UnitPrice, TotalPrice, campos customizáveis)
- [ ] Relacionamentos entre Quote ↔ Customer, Quote ↔ User, Quote ↔ QuoteItems estão corretos
- [ ] Data Annotations apropriadas estão aplicadas ([Required], [ForeignKey], etc.)
- [ ] Typo "Costumer" corrigido para "Customer" em toda a entidade

### Exemplo de Estrutura Esperada

**Quote.cs**
```csharp
public class Quote
{
    public int Id { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    
    public List<QuoteItem> QuoteItems { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    [MaxLength(500)]
    public string PaymentConditions { get; set; } = string.Empty;
    
    public decimal? CashDiscount { get; set; }
}
```

**QuoteItem.cs**
```csharp
public class QuoteItem
{
    public int Id { get; set; }
    
    [Required]
    public int QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public Quote Quote { get; set; } = null!;
    
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    public int Order { get; set; } // Para ordenação dos itens
    
    // Campos customizáveis dinâmicos (implementar conforme UC08)
    // Armazenado como JSON no banco de dados
    public Dictionary<string, string> CustomFields { get; set; } = new();
}
```

---

## 5.2 - Criação dos DTOs Base para Quote

### Descrição
Criar DTOs base para operações com orçamentos, utilizando herança para reutilização de código e evitar duplicação.

### Critérios de Aceitação
- [ ] `QuoteDto` base criado com propriedades comuns implementando `IBasicDto`
- [ ] `CreateQuoteDto` criado para criação de orçamentos implementando `IBasicDto`
- [ ] `UpdateQuoteDto` criado para atualização de orçamentos implementando `IBasicDto`
- [ ] `QuoteResponseDto` criado para retorno de orçamentos (não precisa implementar IBasicDto - apenas leitura)
- [ ] DTOs utilizam herança apropriadamente
- [ ] DTOs implementam métodos `Validate()` e `IsValid()` da interface `IBasicDto`
- [ ] Todos os DTOs estão no namespace `ManiaDeLimpeza.Application.Dtos.Quote`

### Exemplos de Entrada/Saída

**CreateQuoteDto (Exemplo de Implementação)**
```csharp
public class CreateQuoteDto : IBasicDto
{
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentConditions { get; set; } = string.Empty;
    public decimal? CashDiscount { get; set; }
    public List<CreateQuoteItemDto> Items { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();
        
        if (CustomerId <= 0)
            errors.Add("CustomerId é obrigatório.");
        
        if (string.IsNullOrWhiteSpace(PaymentMethod))
            errors.Add("Método de pagamento é obrigatório.");
        
        if (PaymentConditions?.Length > 500)
            errors.Add("Condições de pagamento não podem ter mais de 500 caracteres.");
        
        if (CashDiscount.HasValue && CashDiscount.Value < 0)
            errors.Add("Desconto não pode ser negativo.");
        
        if (Items == null || Items.Count == 0)
            errors.Add("O orçamento deve conter pelo menos um item.");
        
        // Validar cada item (delegação)
        if (Items != null)
        {
            foreach (var item in Items)
                errors.AddRange(item.Validate());
        }
        
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}
```

**Exemplo JSON de Entrada:**
```json
{
  "customerId": 1,
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "cashDiscount": 10.5,
  "items": [
    {
      "description": "Limpeza completa - Sala",
      "quantity": 1,
      "unitPrice": 150.00
    },
    {
      "description": "Limpeza completa - Quartos",
      "quantity": 2,
      "unitPrice": 100.00
    }
  ]
}
```

**QuoteResponseDto (Não precisa implementar IBasicDto - apenas leitura)**
```csharp
public class QuoteResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentConditions { get; set; } = string.Empty;
    public decimal? CashDiscount { get; set; }
    public decimal FinalPrice { get; set; }
    public List<QuoteItemResponseDto> Items { get; set; } = new();
}
```

**Exemplo JSON de Saída:**
```json
{
  "customerId": 1,
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "cashDiscount": 10.5,
  "items": [
    {
      "description": "Limpeza completa - Sala",
      "quantity": 1,
      "unitPrice": 150.00
    },
    {
      "description": "Limpeza completa - Quartos",
      "quantity": 2,
      "unitPrice": 100.00
    }
  ]
}
```

**Exemplo JSON de Saída:**
```json
{
  "id": 1,
  "customerId": 1,
  "customerName": "João Silva",
  "userId": 5,
  "userName": "Maria Santos",
  "createdAt": "2025-11-05T10:30:00Z",
  "updatedAt": null,
  "totalPrice": 350.00,
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "cashDiscount": 10.5,
  "finalPrice": 339.50,
  "items": [
    {
      "id": 1,
      "description": "Limpeza completa - Sala",
      "quantity": 1,
      "unitPrice": 150.00,
      "totalPrice": 150.00,
      "order": 1
    },
    {
      "id": 2,
      "description": "Limpeza completa - Quartos",
      "quantity": 2,
      "unitPrice": 100.00,
      "totalPrice": 200.00,
      "order": 2
    }
  ]
}
```

---

## 5.3 - Criação dos DTOs para QuoteItem

### Descrição
Criar DTOs específicos para operações com itens de orçamento, permitindo CRUD granular de itens.

### Critérios de Aceitação
- [ ] `QuoteItemDto` base criado implementando `IBasicDto`
- [ ] `CreateQuoteItemDto` criado implementando `IBasicDto`
- [ ] `UpdateQuoteItemDto` criado implementando `IBasicDto`
- [ ] `QuoteItemResponseDto` criado (não precisa implementar IBasicDto - apenas leitura)
- [ ] DTOs implementam métodos `Validate()` e `IsValid()` da interface `IBasicDto`

### Exemplos de Entrada/Saída

**CreateQuoteItemDto (Exemplo de Implementação)**
```csharp
public class CreateQuoteItemDto : IBasicDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Descrição é obrigatória.");
        
        if (Description?.Length > 200)
            errors.Add("Descrição não pode ter mais de 200 caracteres.");
        
        if (Quantity <= 0)
            errors.Add("Quantidade deve ser maior que zero.");
        
        if (UnitPrice <= 0)
            errors.Add("Valor unitário deve ser maior que zero.");
        
        // Validar CustomFields
        if (CustomFields != null)
        {
            foreach (var field in CustomFields)
            {
                if (string.IsNullOrWhiteSpace(field.Key))
                    errors.Add("Chaves do campo customizado não podem ser vazias.");
                else if (field.Key.Length > 50)
                    errors.Add("Chaves do campo customizado não podem ter mais de 50 caracteres.");
                
                if (string.IsNullOrWhiteSpace(field.Value))
                    errors.Add("Valores do campo customizado não podem ser vazios.");
                else if (field.Value.Length > 200)
                    errors.Add("Valores do campo customizado não podem ter mais de 200 caracteres.");
            }
        }
        
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}
```

**Exemplo JSON de Entrada:**
```json
{
  "description": "Limpeza de estofados",
  "quantity": 3,
  "unitPrice": 80.00,
  "customFields": {
    "tipo": "Sofá 3 lugares",
    "tecido": "Tecido claro",
    "cor": "Bege"
  }
}
```

**QuoteItemResponseDto (Não precisa implementar IBasicDto - apenas leitura)**
```csharp
public class QuoteItemResponseDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int Order { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();
}
```

**Exemplo JSON de Saída:**
```json
{
  "description": "Limpeza de estofados",
  "quantity": 3,
  "unitPrice": 80.00,
  "customFields": {
    "tipo": "Sofá 3 lugares",
    "tecido": "Tecido claro",
    "cor": "Bege"
  }
}
```

**Exemplo JSON de Saída:**
```json
{
  "id": 3,
  "quoteId": 1,
  "description": "Limpeza de estofados",
  "quantity": 3,
  "unitPrice": 80.00,
  "totalPrice": 240.00,
  "order": 3,
  "customFields": {
    "tipo": "Sofá 3 lugares",
    "tecido": "Tecido claro",
    "cor": "Bege"
  }
}
```

---

## 5.4 - Implementação das Validações de DTOs de Quote

### Descrição
Implementar validações customizadas nos DTOs de Quote através da interface `IBasicDto`, garantindo validações robustas e mensagens de erro claras em português.

### Critérios de Aceitação
- [ ] `CreateQuoteDto` implementa métodos `Validate()` e `IsValid()`
- [ ] `UpdateQuoteDto` implementa métodos `Validate()` e `IsValid()`
- [ ] Validações incluem: CustomerId obrigatório, Items não vazio, valores numéricos válidos, desconto válido
- [ ] Validações delegam para DTOs filhos (validação de cada item da lista)
- [ ] Mensagens de erro em português seguindo padrão do projeto
- [ ] Validação de regras de negócio (ex: desconto não pode ser maior que total - quando aplicável)

### Cenários de Validação

**Happy Path**
```csharp
var dto = new CreateQuoteDto
{
    CustomerId = 1,
    PaymentMethod = "Cash",
    PaymentConditions = "À vista",
    CashDiscount = 5.0m,
    Items = new List<CreateQuoteItemDto>
    {
        new() { Description = "Serviço de limpeza", Quantity = 1, UnitPrice = 100.00m }
    }
};

var errors = dto.Validate();
Assert.Empty(errors); // ✅ Válido
Assert.True(dto.IsValid());
```

**Edge Cases**
```csharp
// ❌ CustomerId inválido (zero ou negativo)
var dto1 = new CreateQuoteDto { CustomerId = 0, Items = [...] };
var errors1 = dto1.Validate();
// Contém: "CustomerId é obrigatório."

// ❌ Items vazio ou nulo
var dto2 = new CreateQuoteDto { CustomerId = 1, Items = new List<CreateQuoteItemDto>() };
var errors2 = dto2.Validate();
// Contém: "O orçamento deve conter pelo menos um item."

// ❌ Items com validações inválidas (delegação)
var dto3 = new CreateQuoteDto 
{ 
    CustomerId = 1, 
    Items = new List<CreateQuoteItemDto> 
    { 
        new() { Description = "", Quantity = 1, UnitPrice = 100 } // Descrição vazia
    } 
};
var errors3 = dto3.Validate();
// Contém: "Descrição é obrigatória." (propagado do item)

// ❌ Desconto negativo
var dto4 = new CreateQuoteDto { CustomerId = 1, CashDiscount = -10, Items = [...] };
var errors4 = dto4.Validate();
// Contém: "Desconto não pode ser negativo."

// ❌ PaymentConditions muito longo
var dto5 = new CreateQuoteDto 
{ 
    CustomerId = 1, 
    PaymentConditions = new string('A', 600), 
    Items = [...] 
};
var errors5 = dto5.Validate();
// Contém: "Condições de pagamento não podem ter mais de 500 caracteres."
```

---

## 5.5 - Implementação das Validações de DTOs de QuoteItem

### Descrição
Implementar validações customizadas nos DTOs de QuoteItem através da interface `IBasicDto` com validações específicas de itens.

### Critérios de Aceitação
- [ ] `CreateQuoteItemDto` implementa métodos `Validate()` e `IsValid()`
- [ ] `UpdateQuoteItemDto` implementa métodos `Validate()` e `IsValid()`
- [ ] Validações incluem: descrição obrigatória e com tamanho máximo, quantidade > 0, unitPrice > 0
- [ ] Validação de Dictionary CustomFields (chaves e valores não podem ser nulos/vazios)
- [ ] Validação de tamanho máximo de chaves (50 caracteres) e valores (200 caracteres) do CustomFields
- [ ] Mensagens de erro em português seguindo padrão do projeto

### Cenários de Validação

**Happy Path**
```csharp
var dto = new CreateQuoteItemDto
{
    Description = "Limpeza básica",
    Quantity = 2,
    UnitPrice = 50.00m
};

var errors = dto.Validate();
Assert.Empty(errors); // ✅ Válido
Assert.True(dto.IsValid());
```

**Edge Cases**
```csharp
// ❌ Descrição vazia
var dto1 = new CreateQuoteItemDto { Description = "", Quantity = 1, UnitPrice = 50 };
var errors1 = dto1.Validate();
// Contém: "Descrição é obrigatória."

// ❌ Descrição muito longa
var dto2 = new CreateQuoteItemDto 
{ 
    Description = new string('A', 300), 
    Quantity = 1, 
    UnitPrice = 50 
};
var errors2 = dto2.Validate();
// Contém: "Descrição não pode ter mais de 200 caracteres."

// ❌ Quantidade zero ou negativa
var dto3 = new CreateQuoteItemDto { Description = "Item", Quantity = 0, UnitPrice = 50 };
var errors3 = dto3.Validate();
// Contém: "Quantidade deve ser maior que zero."

// ❌ Preço zero ou negativo
var dto4 = new CreateQuoteItemDto { Description = "Item", Quantity = 1, UnitPrice = -10 };
var errors4 = dto4.Validate();
// Contém: "Valor unitário deve ser maior que zero."

// ❌ CustomFields com chave vazia
var dto5 = new CreateQuoteItemDto 
{ 
    Description = "Item", 
    Quantity = 1, 
    UnitPrice = 50,
    CustomFields = new Dictionary<string, string> { { "", "valor" } }
};
var errors5 = dto5.Validate();
// Contém: "Chaves do campo customizado não podem ser vazias."

// ❌ CustomFields com valor vazio
var dto6 = new CreateQuoteItemDto 
{ 
    Description = "Item", 
    Quantity = 1, 
    UnitPrice = 50,
    CustomFields = new Dictionary<string, string> { { "chave", "" } }
};
var errors6 = dto6.Validate();
// Contém: "Valores do campo customizado não podem ser vazios."

// ❌ CustomFields com chave muito longa
var dto7 = new CreateQuoteItemDto 
{ 
    Description = "Item", 
    Quantity = 1, 
    UnitPrice = 50,
    CustomFields = new Dictionary<string, string> { { new string('A', 100), "valor" } }
};
var errors7 = dto7.Validate();
// Contém: "Chaves do campo customizado não podem ter mais de 50 caracteres."

// ❌ CustomFields com valor muito longo
var dto8 = new CreateQuoteItemDto 
{ 
    Description = "Item", 
    Quantity = 1, 
    UnitPrice = 50,
    CustomFields = new Dictionary<string, string> { { "chave", new string('A', 500) } }
};
var errors8 = dto8.Validate();
// Contém: "Valores do campo customizado não podem ter mais de 200 caracteres."

// ❌ Múltiplos erros
var dto9 = new CreateQuoteItemDto 
{ 
    Description = "", 
    Quantity = 0, 
    UnitPrice = -10,
    CustomFields = new Dictionary<string, string> { { "", "" } }
};
var errors9 = dto9.Validate();
Assert.True(errors9.Count >= 4); // Múltiplos erros retornados
```

---

## 5.6 - Implementação da Camada de Persistência - Repository Quote

### Descrição
Criar interface e implementação do repositório para operações de persistência de Quote, incluindo queries otimizadas com relacionamentos.

### Critérios de Aceitação
- [ ] Interface `IQuoteRepository` criada em `ManiaDeLimpeza.Domain.Interfaces`
- [ ] Implementação `QuoteRepository` criada em `ManiaDeLimpeza.Persistence.Repositories`
- [ ] Métodos incluem: GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync
- [ ] Queries incluem relacionamentos (Customer, User, QuoteItems) com Include/ThenInclude
- [ ] Suporte a filtros (por customer, por user, por período)
- [ ] Suporte a paginação

### Exemplo de Interface
```csharp
public interface IQuoteRepository
{
    Task<IEnumerable<Quote>> GetAllAsync(
        int? customerId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10
    );
    
    Task<Quote?> GetByIdAsync(int id);
    Task<Quote> CreateAsync(Quote quote);
    Task<Quote> UpdateAsync(Quote quote);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync(int? customerId = null, int? userId = null);
    Task<bool> ExistsAsync(int id);
}
```

### Exemplos de Uso

**GetAllAsync com filtros**
```csharp
// Buscar orçamentos de um cliente específico
var quotes = await repository.GetAllAsync(customerId: 1);

// Buscar orçamentos criados por um usuário em um período
var quotes = await repository.GetAllAsync(
    userId: 5,
    startDate: new DateTime(2025, 11, 1),
    endDate: new DateTime(2025, 11, 30)
);
```

**GetByIdAsync com relacionamentos**
```csharp
// Retorna Quote com Customer, User e QuoteItems carregados
var quote = await repository.GetByIdAsync(1);
// quote.Customer ✅ Populado
// quote.User ✅ Populado
// quote.QuoteItems ✅ Populado
```

---

## 5.7 - Implementação da Camada de Persistência - Repository QuoteItem

### Descrição
Criar interface e implementação do repositório para operações específicas de QuoteItem.

### Critérios de Aceitação
- [ ] Interface `IQuoteItemRepository` criada
- [ ] Implementação `QuoteItemRepository` criada
- [ ] Métodos incluem: GetByQuoteIdAsync, CreateAsync, UpdateAsync, DeleteAsync, ReorderAsync
- [ ] Validação de QuoteId existente antes de operações

### Exemplo de Interface
```csharp
public interface IQuoteItemRepository
{
    Task<IEnumerable<QuoteItem>> GetByQuoteIdAsync(int quoteId);
    Task<QuoteItem?> GetByIdAsync(int id);
    Task<QuoteItem> CreateAsync(QuoteItem item);
    Task<QuoteItem> UpdateAsync(QuoteItem item);
    Task<bool> DeleteAsync(int id);
    Task<bool> ReorderAsync(int quoteId, List<int> itemIdsInOrder);
}
```

---

## 5.8 - Configuração do Entity Framework para Quote

### Descrição
Criar configurações do Entity Framework para as entidades Quote e QuoteItem, definindo relacionamentos, índices e constraints.

### Critérios de Aceitação
- [ ] `QuoteConfiguration.cs` criado em `ManiaDeLimpeza.Persistence.Configurations`
- [ ] `QuoteItemConfiguration.cs` criado
- [ ] Relacionamentos configurados corretamente (Quote → Customer, Quote → User, Quote → QuoteItems)
- [ ] Índices criados para otimização (CustomerId, UserId, CreatedAt)
- [ ] Precisão decimal configurada para campos monetários
- [ ] Cascade delete configurado apropriadamente
- [ ] CustomFields do QuoteItem configurado como JSON no banco de dados (conversão automática)

### Exemplo de Configuração
```csharp
public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasKey(q => q.Id);
        
        builder.Property(q => q.TotalPrice)
            .HasPrecision(18, 2);
            
        builder.Property(q => q.CashDiscount)
            .HasPrecision(18, 2);
        
        builder.HasOne(q => q.Customer)
            .WithMany()
            .HasForeignKey(q => q.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(q => q.User)
            .WithMany()
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(q => q.QuoteItems)
            .WithOne(qi => qi.Quote)
            .HasForeignKey(qi => qi.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(q => q.CustomerId);
        builder.HasIndex(q => q.UserId);
        builder.HasIndex(q => q.CreatedAt);
    }
}

public class QuoteItemConfiguration : IEntityTypeConfiguration<QuoteItem>
{
    public void Configure(EntityTypeBuilder<QuoteItem> builder)
    {
        builder.HasKey(qi => qi.Id);
        
        builder.Property(qi => qi.Quantity)
            .HasPrecision(18, 2);
            
        builder.Property(qi => qi.UnitPrice)
            .HasPrecision(18, 2);
            
        builder.Property(qi => qi.TotalPrice)
            .HasPrecision(18, 2);
        
        // Configurar CustomFields para ser armazenado como JSON
        builder.Property(qi => qi.CustomFields)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
            )
            .HasColumnType("nvarchar(max)");
        
        builder.HasIndex(qi => qi.QuoteId);
        builder.HasIndex(qi => qi.Order);
    }
}
```

---

## 5.9 - Testes de Persistência - QuoteRepository

### Descrição
Implementar testes de integração para o QuoteRepository usando banco de dados em memória ou container de teste.

### Critérios de Aceitação
- [ ] Classe `QuoteRepositoryTests` criada em `ManiaDeLimpeza.Persistence.IntegrationTests`
- [ ] Todos os métodos do repository possuem testes
- [ ] Happy path e edge cases cobertos
- [ ] Uso de fixtures para setup de dados de teste
- [ ] Limpeza de dados entre testes

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task CreateAsync_ValidQuote_ReturnsCreatedQuote()
{
    // Arrange
    var quote = new Quote { CustomerId = 1, UserId = 1, ... };
    
    // Act
    var result = await _repository.CreateAsync(quote);
    
    // Assert
    Assert.NotNull(result);
    Assert.True(result.Id > 0);
    Assert.Equal(quote.CustomerId, result.CustomerId);
}

[Fact]
public async Task GetByIdAsync_ExistingId_ReturnsQuoteWithRelationships()
{
    // Arrange
    var quoteId = 1;
    
    // Act
    var result = await _repository.GetByIdAsync(quoteId);
    
    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.Customer);
    Assert.NotNull(result.User);
    Assert.NotEmpty(result.QuoteItems);
}

[Fact]
public async Task GetAllAsync_WithFilters_ReturnsFilteredQuotes()
{
    // Arrange
    var customerId = 1;
    
    // Act
    var result = await _repository.GetAllAsync(customerId: customerId);
    
    // Assert
    Assert.All(result, q => Assert.Equal(customerId, q.CustomerId));
}
```

**Edge Cases**
```csharp
[Fact]
public async Task GetByIdAsync_NonExistingId_ReturnsNull()
{
    // Arrange
    var nonExistingId = 99999;
    
    // Act
    var result = await _repository.GetByIdAsync(nonExistingId);
    
    // Assert
    Assert.Null(result);
}

[Fact]
public async Task CreateAsync_InvalidCustomerId_ThrowsException()
{
    // Arrange
    var quote = new Quote { CustomerId = 99999, UserId = 1, ... };
    
    // Act & Assert
    await Assert.ThrowsAsync<DbUpdateException>(
        () => _repository.CreateAsync(quote)
    );
}

[Fact]
public async Task DeleteAsync_QuoteWithItems_DeletesQuoteAndItems()
{
    // Arrange
    var quoteId = 1;
    
    // Act
    var result = await _repository.DeleteAsync(quoteId);
    
    // Assert
    Assert.True(result);
    var deletedQuote = await _repository.GetByIdAsync(quoteId);
    Assert.Null(deletedQuote);
}

[Fact]
public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
{
    // Arrange
    // Database vazio
    
    // Act
    var result = await _repository.GetAllAsync();
    
    // Assert
    Assert.Empty(result);
}
```

---

## 5.10 - Testes de Persistência - QuoteItemRepository

### Descrição
Implementar testes de integração para o QuoteItemRepository.

### Critérios de Aceitação
- [ ] Classe `QuoteItemRepositoryTests` criada
- [ ] Testes para todos os métodos do repository
- [ ] Validação de relacionamento com Quote
- [ ] Testes de ordenação de itens

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task CreateAsync_ValidItem_ReturnsCreatedItem()

[Fact]
public async Task GetByQuoteIdAsync_ExistingQuote_ReturnsAllItems()

[Fact]
public async Task UpdateAsync_ExistingItem_ReturnsUpdatedItem()

[Fact]
public async Task ReorderAsync_ValidOrder_UpdatesItemOrder()
```

**Edge Cases**
```csharp
[Fact]
public async Task CreateAsync_InvalidQuoteId_ThrowsException()

[Fact]
public async Task DeleteAsync_NonExistingItem_ReturnsFalse()

[Fact]
public async Task GetByQuoteIdAsync_NonExistingQuote_ReturnsEmpty()
```

---

## 5.11 - Implementação da Camada de Serviço - QuoteService (Create & Read)

### Descrição
Implementar serviço de aplicação para operações de criação e leitura de orçamentos, incluindo lógica de negócio e cálculos automáticos.

### Critérios de Aceitação
- [ ] Interface `IQuoteService` criada em `ManiaDeLimpeza.Application.Interfaces`
- [ ] Implementação `QuoteService` criada em `ManiaDeLimpeza.Application.Services`
- [ ] Métodos CreateAsync e GetAllAsync implementados
- [ ] Cálculo automático de TotalPrice baseado em QuoteItems
- [ ] Cálculo de preço final considerando desconto à vista
- [ ] Validação de existência de Customer antes de criar
- [ ] Mapping entre entidades e DTOs usando AutoMapper

### Exemplo de Interface
```csharp
public interface IQuoteService
{
    Task<QuoteResponseDto> CreateAsync(CreateQuoteDto dto, int userId);
    Task<IEnumerable<QuoteResponseDto>> GetAllAsync(
        int? customerId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10
    );
    Task<QuoteResponseDto?> GetByIdAsync(int id);
    Task<QuoteResponseDto> UpdateAsync(int id, UpdateQuoteDto dto);
    Task<bool> DeleteAsync(int id);
}
```

### Exemplos de Entrada/Saída

**CreateAsync**

*Entrada:*
```json
{
  "customerId": 1,
  "paymentMethod": "Cash",
  "paymentConditions": "À vista",
  "cashDiscount": 10.0,
  "items": [
    {
      "description": "Limpeza sala",
      "quantity": 1,
      "unitPrice": 100.00
    },
    {
      "description": "Limpeza quartos",
      "quantity": 2,
      "unitPrice": 80.00
    }
  ]
}
```

*Saída:*
```json
{
  "id": 1,
  "customerId": 1,
  "customerName": "João Silva",
  "userId": 5,
  "userName": "Maria Santos",
  "createdAt": "2025-11-05T14:30:00Z",
  "totalPrice": 260.00,
  "paymentMethod": "Cash",
  "paymentConditions": "À vista",
  "cashDiscount": 10.0,
  "finalPrice": 250.00,
  "items": [
    {
      "id": 1,
      "description": "Limpeza sala",
      "quantity": 1,
      "unitPrice": 100.00,
      "totalPrice": 100.00,
      "order": 1
    },
    {
      "id": 2,
      "description": "Limpeza quartos",
      "quantity": 2,
      "unitPrice": 80.00,
      "totalPrice": 160.00,
      "order": 2
    }
  ]
}
```

**GetAllAsync com filtros**

*Entrada:*
```
customerId: 1
pageNumber: 1
pageSize: 10
```

*Saída:*
```json
[
  {
    "id": 1,
    "customerId": 1,
    "customerName": "João Silva",
    "totalPrice": 260.00,
    "finalPrice": 250.00,
    "createdAt": "2025-11-05T14:30:00Z",
    "items": [...]
  }
]
```

---

## 5.12 - Implementação da Camada de Serviço - QuoteService (Update & Delete)

### Descrição
Implementar operações de atualização e exclusão de orçamentos no QuoteService.

### Critérios de Aceitação
- [ ] Método UpdateAsync implementado
- [ ] Método DeleteAsync implementado
- [ ] Atualização recalcula automaticamente valores totais
- [ ] Validação de existência antes de atualizar/deletar
- [ ] Suporte a atualização de itens (adicionar, remover, modificar)
- [ ] UpdatedAt atualizado automaticamente

### Exemplos de Entrada/Saída

**UpdateAsync**

*Entrada:*
```json
{
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "cashDiscount": null,
  "items": [
    {
      "id": 1,
      "description": "Limpeza completa sala",
      "quantity": 1,
      "unitPrice": 120.00
    },
    {
      "description": "Limpeza de janelas",
      "quantity": 5,
      "unitPrice": 30.00
    }
  ]
}
```

*Saída:*
```json
{
  "id": 1,
  "customerId": 1,
  "customerName": "João Silva",
  "userId": 5,
  "userName": "Maria Santos",
  "createdAt": "2025-11-05T14:30:00Z",
  "updatedAt": "2025-11-05T15:45:00Z",
  "totalPrice": 270.00,
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "cashDiscount": null,
  "finalPrice": 270.00,
  "items": [
    {
      "id": 1,
      "description": "Limpeza completa sala",
      "quantity": 1,
      "unitPrice": 120.00,
      "totalPrice": 120.00,
      "order": 1
    },
    {
      "id": 3,
      "description": "Limpeza de janelas",
      "quantity": 5,
      "unitPrice": 30.00,
      "totalPrice": 150.00,
      "order": 2
    }
  ]
}
```

**DeleteAsync**

*Entrada:*
```
quoteId: 1
```

*Saída:*
```
true
```

---

## 5.13 - Implementação da Camada de Serviço - QuoteItemService

### Descrição
Implementar serviço específico para operações com itens individuais de orçamento.

### Critérios de Aceitação
- [ ] Interface `IQuoteItemService` criada
- [ ] Implementação `QuoteItemService` criada
- [ ] Métodos: AddItemAsync, UpdateItemAsync, DeleteItemAsync, ReorderItemsAsync
- [ ] Recalcula total do Quote ao modificar itens
- [ ] Validação de Quote existente antes de operações

### Exemplo de Interface
```csharp
public interface IQuoteItemService
{
    Task<QuoteItemResponseDto> AddItemAsync(int quoteId, CreateQuoteItemDto dto);
    Task<QuoteItemResponseDto> UpdateItemAsync(int itemId, UpdateQuoteItemDto dto);
    Task<bool> DeleteItemAsync(int itemId);
    Task<bool> ReorderItemsAsync(int quoteId, List<int> itemIdsInOrder);
}
```

### Exemplos de Entrada/Saída

**AddItemAsync**

*Entrada:*
```
quoteId: 1
dto: {
  "description": "Limpeza de tapetes",
  "quantity": 2,
  "unitPrice": 40.00,
  "customFields": {
    "material": "Lã",
    "tamanho": "2x3m"
  }
}
```

*Saída:*
```json
{
  "id": 4,
  "quoteId": 1,
  "description": "Limpeza de tapetes",
  "quantity": 2,
  "unitPrice": 40.00,
  "totalPrice": 80.00,
  "order": 3,
  "customFields": {
    "material": "Lã",
    "tamanho": "2x3m"
  }
}
```

---

## 5.14 - Testes da Camada de Serviço - QuoteService

### Descrição
Implementar testes unitários para QuoteService usando mocks de repositórios.

### Critérios de Aceitação
- [ ] Classe `QuoteServiceTests` criada em `ManiaDeLimpeza.Application.UnitTests`
- [ ] Testes para todos os métodos públicos
- [ ] Uso de Moq para simular repositórios
- [ ] Validação de lógica de cálculos
- [ ] Validação de regras de negócio

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task CreateAsync_ValidDto_ReturnsCreatedQuote()
{
    // Verifica criação bem-sucedida com cálculos corretos
}

[Fact]
public async Task CreateAsync_CalculatesTotalPrice_Correctly()
{
    // Entrada: items com unitPrice 100, 200, 50
    // Saída: totalPrice = 350
}

[Fact]
public async Task CreateAsync_AppliesCashDiscount_Correctly()
{
    // Entrada: totalPrice 100, cashDiscount 10
    // Saída: finalPrice = 90
}

[Fact]
public async Task GetAllAsync_WithFilters_CallsRepositoryWithCorrectParams()
{
    // Verifica que filtros são repassados corretamente ao repository
}

[Fact]
public async Task UpdateAsync_ExistingQuote_UpdatesAndRecalculates()
{
    // Verifica atualização e recálculo de valores
}
```

**Edge Cases**
```csharp
[Fact]
public async Task CreateAsync_NonExistingCustomer_ThrowsNotFoundException()
{
    // CustomerId não existe
}

[Fact]
public async Task CreateAsync_EmptyItems_ThrowsValidationException()
{
    // Lista de items vazia
}

[Fact]
public async Task UpdateAsync_NonExistingQuote_ThrowsNotFoundException()
{
    // Quote não existe
}

[Fact]
public async Task DeleteAsync_NonExistingQuote_ReturnsFalse()
{
    // Quote não existe
}

[Fact]
public async Task CreateAsync_CashDiscountGreaterThanTotal_ThrowsException()
{
    // Desconto maior que total
}

[Fact]
public async Task GetByIdAsync_NonExistingId_ReturnsNull()
{
    // ID não existe
}
```

---

## 5.15 - Testes da Camada de Serviço - QuoteItemService

### Descrição
Implementar testes unitários para QuoteItemService.

### Critérios de Aceitação
- [ ] Classe `QuoteItemServiceTests` criada
- [ ] Testes para AddItemAsync, UpdateItemAsync, DeleteItemAsync, ReorderItemsAsync
- [ ] Validação de recálculo do total do Quote
- [ ] Uso de mocks

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task AddItemAsync_ValidItem_AddsAndRecalculatesQuoteTotal()

[Fact]
public async Task DeleteItemAsync_ExistingItem_RemovesAndRecalculatesQuoteTotal()

[Fact]
public async Task ReorderItemsAsync_ValidOrder_UpdatesOrderCorrectly()
```

**Edge Cases**
```csharp
[Fact]
public async Task AddItemAsync_NonExistingQuote_ThrowsNotFoundException()

[Fact]
public async Task UpdateItemAsync_NonExistingItem_ThrowsNotFoundException()

[Fact]
public async Task DeleteItemAsync_LastItem_ThrowsException()
// Orçamento deve ter pelo menos 1 item
```

---

## 5.16 - Implementação dos Endpoints - QuotesController (CRUD Básico)

### Descrição
Implementar controller REST API para operações CRUD de orçamentos.

### Critérios de Aceitação
- [ ] `QuotesController` criado em `ManiaDeLimpeza.Api.Controllers`
- [ ] Endpoints: GET /api/quotes, GET /api/quotes/{id}, POST /api/quotes, PUT /api/quotes/{id}, DELETE /api/quotes/{id}
- [ ] Autenticação e autorização aplicadas
- [ ] Documentação Swagger completa
- [ ] Tratamento de erros adequado
- [ ] UserId extraído do token JWT automaticamente

### Exemplos de Endpoints

**POST /api/quotes**
```http
POST /api/quotes
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": 1,
  "paymentMethod": "Cash",
  "paymentConditions": "À vista",
  "cashDiscount": 10.0,
  "items": [
    {
      "description": "Limpeza completa",
      "quantity": 1,
      "unitPrice": 200.00
    }
  ]
}

Response: 201 Created
{
  "id": 1,
  "customerId": 1,
  "customerName": "João Silva",
  "userId": 5,
  "userName": "Maria Santos",
  "totalPrice": 200.00,
  "finalPrice": 190.00,
  ...
}
```

**GET /api/quotes?customerId=1&pageNumber=1&pageSize=10**
```http
GET /api/quotes?customerId=1&pageNumber=1&pageSize=10
Authorization: Bearer {token}

Response: 200 OK
[
  {
    "id": 1,
    "customerId": 1,
    "customerName": "João Silva",
    "totalPrice": 200.00,
    ...
  }
]
```

**GET /api/quotes/{id}**
```http
GET /api/quotes/1
Authorization: Bearer {token}

Response: 200 OK
{
  "id": 1,
  "customerId": 1,
  "customerName": "João Silva",
  "items": [...],
  ...
}

Response: 404 Not Found (se não existir)
{
  "message": "Orçamento não encontrado"
}
```

**PUT /api/quotes/{id}**
```http
PUT /api/quotes/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "paymentMethod": "CreditCard",
  "paymentConditions": "3x sem juros",
  "items": [...]
}

Response: 200 OK
{
  "id": 1,
  "updatedAt": "2025-11-05T16:00:00Z",
  ...
}
```

**DELETE /api/quotes/{id}**
```http
DELETE /api/quotes/1
Authorization: Bearer {token}

Response: 204 No Content
```

---

## 5.17 - Implementação dos Endpoints - QuoteItemsController

### Descrição
Implementar controller para operações específicas com itens de orçamento.

### Critérios de Aceitação
- [ ] `QuoteItemsController` criado
- [ ] Endpoints: POST /api/quotes/{quoteId}/items, PUT /api/quote-items/{id}, DELETE /api/quote-items/{id}, POST /api/quotes/{quoteId}/items/reorder
- [ ] Validação de autorização (usuário só pode modificar seus próprios orçamentos)
- [ ] Documentação Swagger

### Exemplos de Endpoints

**POST /api/quotes/{quoteId}/items**
```http
POST /api/quotes/1/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "description": "Limpeza de vidros",
  "quantity": 10,
  "unitPrice": 15.00,
  "customFields": {
    "tipo": "Janelas grandes",
    "local": "Fachada"
  }
}

Response: 201 Created
{
  "id": 5,
  "quoteId": 1,
  "description": "Limpeza de vidros",
  "quantity": 10,
  "unitPrice": 15.00,
  "totalPrice": 150.00,
  "order": 3,
  "customFields": {
    "tipo": "Janelas grandes",
    "local": "Fachada"
  }
}
```

**PUT /api/quote-items/{id}**
```http
PUT /api/quote-items/5
Authorization: Bearer {token}
Content-Type: application/json

{
  "description": "Limpeza de vidros externos",
  "quantity": 12,
  "unitPrice": 18.00,
  "customFields": {
    "tipo": "Janelas grandes",
    "local": "Fachada",
    "altura": "2 andares"
  }
}

Response: 200 OK
{
  "id": 5,
  "description": "Limpeza de vidros externos",
  "quantity": 12,
  "unitPrice": 18.00,
  "totalPrice": 216.00,
  "customFields": {
    "tipo": "Janelas grandes",
    "local": "Fachada",
    "altura": "2 andares"
  },
  ...
}
```

**POST /api/quotes/{quoteId}/items/reorder**
```http
POST /api/quotes/1/items/reorder
Authorization: Bearer {token}
Content-Type: application/json

{
  "itemIds": [3, 1, 5, 2]
}

Response: 204 No Content
```

**DELETE /api/quote-items/{id}**
```http
DELETE /api/quote-items/5
Authorization: Bearer {token}

Response: 204 No Content

Response: 400 Bad Request (se for último item)
{
  "message": "Não é possível remover o último item do orçamento"
}
```

---

## 5.18 - Testes de Integração - QuotesController

### Descrição
Implementar testes de integração end-to-end para QuotesController usando WebApplicationFactory.

### Critérios de Aceitação
- [ ] Classe `QuotesControllerIntegrationTests` criada em `ManiaDeLimpeza.Api.IntegrationTests`
- [ ] Testes para todos os endpoints
- [ ] Uso de banco de dados de teste
- [ ] Autenticação simulada nos testes
- [ ] Validação de status codes e payloads de resposta

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task CreateQuote_ValidData_Returns201Created()
{
    // Arrange
    var createDto = new CreateQuoteDto { ... };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/quotes", createDto);
    
    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var result = await response.Content.ReadFromJsonAsync<QuoteResponseDto>();
    Assert.NotNull(result);
    Assert.True(result.Id > 0);
}

[Fact]
public async Task GetQuotes_WithFilters_Returns200WithFilteredData()
{
    // Arrange
    var customerId = 1;
    
    // Act
    var response = await _client.GetAsync($"/api/quotes?customerId={customerId}");
    
    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = await response.Content.ReadFromJsonAsync<List<QuoteResponseDto>>();
    Assert.All(result, q => Assert.Equal(customerId, q.CustomerId));
}

[Fact]
public async Task GetQuoteById_ExistingId_Returns200WithQuote()

[Fact]
public async Task UpdateQuote_ValidData_Returns200WithUpdatedQuote()

[Fact]
public async Task DeleteQuote_ExistingId_Returns204NoContent()
```

**Edge Cases**
```csharp
[Fact]
public async Task CreateQuote_InvalidCustomerId_Returns400BadRequest()

[Fact]
public async Task CreateQuote_EmptyItems_Returns400BadRequest()

[Fact]
public async Task CreateQuote_Unauthorized_Returns401Unauthorized()

[Fact]
public async Task GetQuoteById_NonExistingId_Returns404NotFound()

[Fact]
public async Task UpdateQuote_NonExistingId_Returns404NotFound()

[Fact]
public async Task UpdateQuote_OtherUserQuote_Returns403Forbidden()
// Usuário tentando editar orçamento de outro usuário

[Fact]
public async Task DeleteQuote_NonExistingId_Returns404NotFound()

[Fact]
public async Task CreateQuote_InvalidDiscount_Returns400BadRequest()
// Desconto maior que total
```

---

## 5.19 - Testes de Integração - QuoteItemsController

### Descrição
Implementar testes de integração para QuoteItemsController.

### Critérios de Aceitação
- [ ] Classe `QuoteItemsControllerIntegrationTests` criada
- [ ] Testes para todos os endpoints de items
- [ ] Validação de autorização entre usuários
- [ ] Validação de regra de negócio (não remover último item)

### Cenários de Teste

**Happy Path**
```csharp
[Fact]
public async Task AddItem_ValidData_Returns201Created()

[Fact]
public async Task UpdateItem_ValidData_Returns200OK()

[Fact]
public async Task DeleteItem_ExistingItem_Returns204NoContent()

[Fact]
public async Task ReorderItems_ValidOrder_Returns204NoContent()
```

**Edge Cases**
```csharp
[Fact]
public async Task AddItem_NonExistingQuote_Returns404NotFound()

[Fact]
public async Task AddItem_OtherUserQuote_Returns403Forbidden()

[Fact]
public async Task DeleteItem_LastItem_Returns400BadRequest()

[Fact]
public async Task UpdateItem_NonExistingItem_Returns404NotFound()

[Fact]
public async Task ReorderItems_InvalidItemIds_Returns400BadRequest()
```

---

## 5.20 - Implementação de AutoMapper Profiles para Quote

### Descrição
Criar perfis de mapeamento AutoMapper para conversão entre entidades Quote/QuoteItem e seus respectivos DTOs.

### Critérios de Aceitação
- [ ] Classe `QuoteProfile` criada em `ManiaDeLimpeza.Application.Common.Mappings`
- [ ] Mapeamentos configurados: Quote ↔ QuoteResponseDto, CreateQuoteDto → Quote, UpdateQuoteDto → Quote
- [ ] Mapeamentos configurados: QuoteItem ↔ QuoteItemResponseDto, CreateQuoteItemDto → QuoteItem
- [ ] Mapeamentos customizados para campos calculados (FinalPrice)
- [ ] Mapeamentos customizados para relacionamentos (CustomerName, UserName)

### Exemplo de Profile
```csharp
public class QuoteProfile : Profile
{
    public QuoteProfile()
    {
        CreateMap<Quote, QuoteResponseDto>()
            .ForMember(dest => dest.CustomerName, 
                opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.UserName, 
                opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.FinalPrice,
                opt => opt.MapFrom(src => 
                    src.TotalPrice - (src.CashDiscount ?? 0)));
        
        CreateMap<CreateQuoteDto, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()); // Calculado pelo service
        
        CreateMap<UpdateQuoteDto, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        
        CreateMap<QuoteItem, QuoteItemResponseDto>();
        
        CreateMap<CreateQuoteItemDto, QuoteItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuoteId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()); // Calculado
    }
}
```

---

## 📊 Resumo da Estrutura de Tarefas

| Tarefa | Descrição | Complexidade |
|--------|-----------|--------------|
| 5.1 | Adequação das Entidades | Baixa |
| 5.2 | DTOs Base Quote | Média |
| 5.3 | DTOs QuoteItem | Baixa |
| 5.4 | Validações Quote | Média |
| 5.5 | Validações QuoteItem | Baixa |
| 5.6 | Repository Quote | Média |
| 5.7 | Repository QuoteItem | Baixa |
| 5.8 | EF Configuration | Média |
| 5.9 | Testes Repository Quote | Alta |
| 5.10 | Testes Repository QuoteItem | Média |
| 5.11 | Service Quote (Create/Read) | Alta |
| 5.12 | Service Quote (Update/Delete) | Média |
| 5.13 | Service QuoteItem | Média |
| 5.14 | Testes Service Quote | Alta |
| 5.15 | Testes Service QuoteItem | Média |
| 5.16 | Controller Quotes | Média |
| 5.17 | Controller QuoteItems | Média |
| 5.18 | Testes Integração Quotes | Alta |
| 5.19 | Testes Integração QuoteItems | Média |
| 5.20 | AutoMapper Profiles | Baixa |

---

## 🎯 Ordem Sugerida de Implementação

1. **Fundação (5.1 - 5.3)**: Entidades e DTOs
2. **Validações (5.4 - 5.5)**: Garantir integridade dos dados
3. **Persistência (5.6 - 5.8)**: Camada de dados
4. **Testes Persistência (5.9 - 5.10)**: Validar camada de dados
5. **Serviços (5.11 - 5.13)**: Lógica de negócio
6. **AutoMapper (5.20)**: Mapeamentos necessários para serviços
7. **Testes Serviços (5.14 - 5.15)**: Validar lógica de negócio
8. **Controllers (5.16 - 5.17)**: Camada de API
9. **Testes Integração (5.18 - 5.19)**: Validar API end-to-end

---

## 📝 Observações Importantes

- **Cálculos Automáticos**: O TotalPrice de cada QuoteItem deve ser calculado automaticamente (Quantity × UnitPrice)
- **Cálculo do Total**: O TotalPrice do Quote deve ser a soma de todos os QuoteItems
- **Desconto**: O CashDiscount é aplicado sobre o TotalPrice para calcular o FinalPrice
- **Validação de Desconto**: Desconto não pode ser maior que o total
- **Mínimo de Itens**: Todo orçamento deve ter pelo menos 1 item
- **Autorização**: Usuários só podem modificar/visualizar seus próprios orçamentos ou orçamentos da sua empresa
- **Soft Delete**: Considerar implementação de soft delete para orçamentos (manter histórico)
- **Auditoria**: Campos CreatedAt e UpdatedAt devem ser mantidos automaticamente

---

## 🔄 Features Futuras (Não incluídas neste MVP)

- Exportação em PDF (UC09) - Tarefa separada
- Exportação em Imagem (UC09) - Tarefa separada
- Campos customizáveis configuráveis (UC08) - Tarefa separada
- Status de orçamento (Rascunho, Enviado, Aprovado, Rejeitado)
- Versionamento de orçamentos
- Templates de orçamento
- Duplicação de orçamentos

