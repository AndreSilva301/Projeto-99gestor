# 📊 Relational Schema – ManiaDeLimpezaApp MVP

## 🔹 Main Tables

### **Company**
| Field       | Type          | Key   | Notes |
|-------------|--------------|-------|------------|
| Id   | INT PK       | PK    | Unique identifier |
| Name        | VARCHAR(150) |       | Company name |
| CNPJ        | VARCHAR(20)  |       | Optional |
| CreatedDate | DATETIME     |       | |

---

### **User**
| Field       | Type          | Key   | Notes |
|-------------|--------------|-------|------------|
| Id   | INT PK       | PK    | Unique identifier |
| CompanyId   | INT FK       | FK → Company.Id |
| Name        | VARCHAR(150) |       | User name |
| Email       | VARCHAR(150) | UQ    | Login |
| PasswordHash| VARBINARY    |       | Encrypted password |
| Profile     | ENUM(Admin, Employee) | | Defines role |
| CreatedDate | DATETIME     |       | |

---

### **Customer**
| Field        | Type          | Key   | Notes |
|--------------|--------------|-------|------------|
| Id    | INT PK       | PK    | Unique identifier |
| CompanyId    | INT FK       | FK → Company.Id |
| Name         | VARCHAR(150) |       | |
| Phone        | VARCHAR(20)  |       | |
| Email        | VARCHAR(150) |       | |
| Address      | VARCHAR(255) |       | |
| RegistrationDate | DATETIME     |       | |

---

### **CustomerRelationship**
| Field            | Type          | Key   | Notes |
|------------------|--------------|-------|------------|
| Id | INT PK       | PK    | |
| CustomerId       | INT FK       | FK → Customer.Id |
| Description      | VARCHAR(255) |       | Relevant information (e.g.: "has 2 children") |
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
| Description   | VARCHAR(255) |       | |
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
