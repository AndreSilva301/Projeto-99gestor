# ManiaDeLimpeza (CRM for Service Providers)

**ManiaDeLimpeza** is a specialized CRM system designed for general service providers such as cleaning services, maintenance, aesthetics, consulting, an5. Open a Pull Request

---

## 📄 License

This is a private project. All rights reserved.

---

## 👨‍💻 Authorlt with clean architecture principles and ASP.NET Core, it helps small and medium-sized service businesses organize leads, manage quotes, track opportunities, and strengthen customer relationships through a professional, mobile-first experience.

> **Note**: "ManiaDeLimpeza" is a temporary name for this CRM solution.

---

## 🎯 Project Vision

This system empowers service providers to transition from manual relationship management (WhatsApp, loose notes, spreadsheets) to an organized, intelligent CRM solution. It's designed specifically for businesses that don't need complex corporate systems but want organization and intelligence in customer relationships.

### Target Users
- **Company Administrators**: Create and manage the company, add collaborators, configure quotes and messages
- **Collaborators**: Use the dashboard to manage clients, quotes, services, and evaluations
- **End Customers**: Interact through received quotes, messages, and evaluation pages (no direct system access)

---

## 🚀 Features

### Current MVP Features
* **Company & User Management**:
  - Company creation and collaborator management
  - Role-based access (Administrator vs. Collaborator)
  - Only administrators can add collaborators

* **Client Management**:
  - Client registration via phone contacts or forms
  - Personal, contact, and address information storage
  - Relationship information tracking (personal interests, family events, conversation history)

* **Quote Management**:
  - Detailed quotes with line items (description, quantity, unit price, total)
  - Automatic calculations for item totals and quote finals
  - Configurable additional fields for quote items
  - Export to PDF and Image formats
  - Payment terms and cash discount support

* **Authentication & Security**:
  - JWT-based authentication with password hashing
  - Role-based authorization system

### Planned Features (Roadmap)
* **Service Scheduling**: Calendar view for approved quote scheduling
* **Service Management**: Track services in progress with completion status
* **Customer Evaluations**: Digital evaluation system with configurable star ratings
* **Proactive CRM**: Automated relationship management with smart contact suggestions

---

## 🏗️ Architecture

Built following Clean Architecture principles with clear separation of concerns:

* **Domain Layer**: Core business entities and rules
* **Application Layer**: Business logic, DTOs, and service interfaces
* **Persistence Layer**: Entity Framework Core with SQL Server
* **Infrastructure Layer**: External concerns and dependency injection
* **API Layer**: ASP.NET Core Web API controllers
* **Testing**: Comprehensive integration and unit tests

---

## 🛠️ Technology Stack

* **.NET 6+** - Core framework
* **ASP.NET Core** - Web API framework
* **Entity Framework Core** - ORM and data access
* **SQL Server** - Database
* **JWT** - Authentication tokens
* **MSTest** - Testing framework
* **Newtonsoft.Json** - JSON serialization

---

## 🛠️ Getting Started

### Prerequisites
- .NET 6 SDK or later
- SQL Server (Local DB or full instance)
- Visual Studio 2022 or VS Code

### 1. Clone and Setup

```powershell
git clone https://github.com/welber91/mania-de-limpeza.git
cd mania-de-limpeza
dotnet restore
```

### 2. Database Setup

Apply Entity Framework migrations to set up the database:
```powershell
dotnet ef database update --project ManiaDeLimpeza.Persistence --startup-project ManiaDeLimpeza
```

### 3. Run the Application

```powershell
dotnet run --project ManiaDeLimpeza
```

The API will be available at `https://localhost:5001`

---

## � Database Schema

The MVP includes the following main entities:
- **Company**: Business information and settings
- **User**: Administrators and collaborators with role-based access
- **Client**: Customer information and relationship data
- **Quote**: Service proposals with line items
- **QuoteItem**: Individual items within quotes with flexible pricing

For detailed schema information, see [Database Schema Documentation](docs/Documentos%20funcionais/Esquema%20do%20banco%20de%20dados%20MVP.md).

---

## 🔐 Authentication

The system uses JWT-based authentication with role-based authorization:

### Registration & Login
- Use `/api/auth/register` to create new accounts
- Use `/api/auth/login` to authenticate
- Include the JWT token in requests: `Authorization: Bearer {your_token}`

### User Roles
- **Administrator**: Full system access, can manage collaborators
- **Collaborator**: Can manage clients and quotes, cannot add users

---

## 📋 Development Roadmap

### Phase 1: MVP (Current)
- ✅ Company and user management
- ✅ Client registration and relationship tracking
- ✅ Quote creation and PDF/image export
- ✅ JWT authentication system

### Phase 2: Service Management
- 📅 Service scheduling with calendar view
- 📊 Service progress tracking and management

### Phase 3: CRM Enhancement
- ⭐ Customer evaluation system
- 🤖 Proactive relationship management
- 📨 Automated communication templates

---

## 🏗️ Project Structure

```
ManiaDeLimpeza/
├── ManiaDeLimpeza/                    # API Layer
│   ├── Controllers/                   # Web API controllers
│   ├── Auth/                         # Authentication services
│   └── Extensions/                   # Service configurations
├── ManiaDeLimpeza.Application/        # Business Logic Layer
│   ├── Services/                     # Application services
│   ├── Dtos/                        # Data transfer objects
│   └── Interfaces/                  # Service contracts
├── ManiaDeLimpeza.Domain/            # Domain Layer
│   ├── Entities/                    # Domain entities
│   └── Dtos/                       # Domain DTOs
├── ManiaDeLimpeza.Persistence/       # Data Access Layer
│   ├── Repositories/                # Data repositories
│   └── Migrations/                  # EF Core migrations
├── ManiaDeLimpeza.Infrastructure/    # Infrastructure Layer
│   └── DependencyInjection/         # IoC container setup
├── Tests/                           # Test Projects
│   ├── Integration/                 # Integration tests
│   └── Unit/                       # Unit tests
└── docs/                           # Project Documentation
    ├── Especificação do projeto/    # Project specifications
    └── Documentos funcionais/       # Functional documents
```

---

## 📚 Documentation

- [Product Vision](docs/Especificação%20do%20projeto/Visão%20de%20produto.md) - Overall product goals and scope
- [Software Requirements](docs/Especificação%20do%20projeto/Especificação%20de%20Requisitos%20de%20Software.md) - Detailed functional requirements
- [Use Cases](docs/Especificação%20do%20projeto/Casos%20de%20Uso.md) - User interaction scenarios
- [Database Schema](docs/Documentos%20funcionais/Esquema%20do%20banco%20de%20dados%20MVP.md) - Data model documentation

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/AmazingFeature`
3. Commit your changes: `git commit -m 'Add some AmazingFeature'`
4. Push to the branch: `git push origin feature/AmazingFeature`
5. Open a Pull Request

---

## 🙏 Acknowledgments

Built with modern development practices focusing on:
- Clean Architecture principles
- SOLID design principles
- Test-driven development
- Mobile-first responsive design
