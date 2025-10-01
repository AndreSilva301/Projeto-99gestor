# 📊 Esquema Relacional – MVP do ManiaDeLimpezaApp

## 🔹 Tabelas principais

### **Empresa**
| Campo       | Tipo          | Chave | Observação |
|-------------|--------------|-------|------------|
| EmpresaId   | INT PK       | PK    | Identificador único |
| Nome        | VARCHAR(150) |       | Nome da empresa |
| CNPJ        | VARCHAR(20)  |       | Opcional |
| DataCriacao | DATETIME     |       | |

---

### **Usuario**
| Campo       | Tipo          | Chave | Observação |
|-------------|--------------|-------|------------|
| UsuarioId   | INT PK       | PK    | Identificador único |
| EmpresaId   | INT FK       | FK → Empresa.EmpresaId |
| Nome        | VARCHAR(150) |       | Nome do usuário |
| Email       | VARCHAR(150) | UQ    | Login |
| SenhaHash   | VARBINARY    |       | Senha criptografada |
| Perfil      | ENUM(Admin, Colaborador) | | Define papel |
| DataCriacao | DATETIME     |       | |

---

### **Cliente**
| Campo        | Tipo          | Chave | Observação |
|--------------|--------------|-------|------------|
| ClienteId    | INT PK       | PK    | Identificador único |
| EmpresaId    | INT FK       | FK → Empresa.EmpresaId |
| Nome         | VARCHAR(150) |       | |
| Telefone     | VARCHAR(20)  |       | |
| Email        | VARCHAR(150) |       | |
| Endereco     | VARCHAR(255) |       | |
| DataCadastro | DATETIME     |       | |

---

### **ClienteRelacionamento**
| Campo            | Tipo          | Chave | Observação |
|------------------|--------------|-------|------------|
| RelacionamentoId | INT PK       | PK    | |
| ClienteId        | INT FK       | FK → Cliente.ClienteId |
| Descricao        | VARCHAR(255) |       | Informação relevante (ex: "tem 2 filhos") |
| DataCadastro     | DATETIME     |       | |

---

### **Orcamento**
| Campo              | Tipo          | Chave | Observação |
|--------------------|--------------|-------|------------|
| OrcamentoId        | INT PK       | PK    | |
| ClienteId          | INT FK       | FK → Cliente.ClienteId |
| UsuarioId          | INT FK       | FK → Usuario.UsuarioId (quem criou) |
| ValorTotal         | DECIMAL(12,2)|       | Soma dos itens |
| CondicoesPagamento | TEXT         |       | |
| DescontoAVista     | DECIMAL(12,2)|       | |
| DataCriacao        | DATETIME     |       | |

---

### **OrcamentoItem**
| Campo         | Tipo          | Chave | Observação |
|---------------|--------------|-------|------------|
| ItemId        | INT PK       | PK    | |
| OrcamentoId   | INT FK       | FK → Orcamento.OrcamentoId |
| Descricao     | VARCHAR(255) |       | |
| Quantidade    | DECIMAL(10,2)| NULL  | Opcional |
| ValorUnitario | DECIMAL(12,2)| NULL  | Opcional |
| ValorTotal    | DECIMAL(12,2)| NOT NULL | Sempre obrigatório |
| CamposExtras  | JSON         |       | Permite flexibilidade futura |

---

## 🔹 Relacionamentos
- **Empresa → Usuario** = 1:N  
- **Empresa → Cliente** = 1:N  
- **Cliente → ClienteRelacionamento** = 1:N  
- **Cliente → Orcamento** = 1:N  
- **Orcamento → OrcamentoItem** = 1:N  

---

## 📐 Modelo ER (Entidade-Relacionamento) – descrição textual
```
Empresa (1) —— (N) Usuario
Empresa (1) —— (N) Cliente —— (N) ClienteRelacionamento
Cliente (1) —— (N) Orcamento —— (N) OrcamentoItem
```
