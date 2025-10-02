# 📊 Relational Schema – ManiaDeLimpezaApp MVP

## 🔹 Main Tables

### **Company**
| Field       | Type          | Key   | Notes |
|-------------|--------------|-------|------------|
| Id   | INT PK       | PK    | Unique identifier |
| Name        | VARCHAR(MAX) |       | Company name |
| CNPJ        | VARCHAR(MAX)  |       | Optional |
| CreatedDate | DATETIME     |       | |

---

### **User**
| Field       | Type          | Key   | Notes |
|-------------|--------------|-------|------------|
| Id   | INT PK       | PK    | Unique identifier |
| CompanyId   | INT FK       | FK → Company.Id |
| Name        | VARCHAR(MAX) |       | User name |
| Email       | VARCHAR(MAX) | UQ    | Login |
| PasswordHash| VARBINARY    |       | Encrypted password |
| Profile     | ENUM(Admin, Employee) | | Defines role |
| CreatedDate | DATETIME     |       | |

---

### **Customer**
| Field        | Type          | Key   | Notes |
|--------------|--------------|-------|------------|
| Id    | INT PK       | PK    | Unique identifier |
| CompanyId    | INT FK       | FK → Company.Id |
| Name         | VARCHAR(MAX) |       | |
| Phone        | VARCHAR(MAX)  |       | |
| Email        | VARCHAR(MAX) |       | |
| Address      | VARCHAR(MAX) |       | |
| RegistrationDate | DATETIME     |       | |

---

### **CustomerRelationship**
| Field            | Type          | Key   | Notes |
|------------------|--------------|-------|------------|
| Id | INT PK       | PK    | |
| CustomerId       | INT FK       | FK → Customer.Id |
| Description      | VARCHAR(MAX) |       | Relevant information (e.g.: "has 2 children") |
| RegistrationDate | DATETIME     |       | |

---

### **Quote**
| Field              | Type          | Key   | Notes |
|--------------------|--------------|-------|------------|
| Id        | INT PK       | PK    | |
| CustomerId         | INT FK       | FK → Customer.Id |
| UserId             | INT FK       | FK → User.Id (who created) |
| TotalValue         | DECIMAL(12,2)|       | Sum of items |
| PaymentConditions  | TEXT         |       | |
| CashDiscount       | DECIMAL(12,2)|       | |
| CreatedDate        | DATETIME     |       | |

---

### **QuoteItem**
| Field         | Type          | Key   | Notes |
|---------------|--------------|-------|------------|
| Id        | INT PK       | PK    | |
| QuoteId       | INT FK       | FK → Quote.Id |
| Description   | VARCHAR(MAX) |       | |
| Quantity      | DECIMAL(10,2)| NULL  | Optional |
| UnitPrice     | DECIMAL(12,2)| NULL  | Optional |
| TotalValue    | DECIMAL(12,2)| NOT NULL | Always required |
| ExtraFields   | JSON         |       | Allows future flexibility |

---

## 🔹 Relationships
- **Company → User** = 1:N  
- **Company → Customer** = 1:N  
- **Customer → CustomerRelationship** = 1:N  
- **Customer → Quote** = 1:N  
- **Quote → QuoteItem** = 1:N  

---

## 📐 ER Model (Entity-Relationship) – textual description
```
Company (1) —— (N) User
Company (1) —— (N) Customer —— (N) CustomerRelationship
Customer (1) —— (N) Quote —— (N) QuoteItem
```
