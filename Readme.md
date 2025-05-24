# ManiaDeLimpeza

ManiaDeLimpeza is a clean architecture-based ASP.NET Core application for managing cleaning service operations. It supports user authentication, client management, quoting, and search functionalities. Built using modern development practices and SOLID principles, it emphasizes maintainability, testability, and separation of concerns.

---

## 🚀 Features

* **Authentication**: JWT-based login and registration with password hashing.
* **Client Management**:

  * Create, read, update, and delete clients.
  * Full-text search on name and phone fields.
* **Quote Management** (in progress):

  * Quotes have line items, total price, creator, timestamps.
  * Support for payment method (enum) and optional cash discounts.
* **Database**:

  * EF Core with SQL Server.
  * Indexed search fields for performance.
* **Testing**:

  * Integration tests with real database using `WebApplicationFactory`.
  * Repository tests for CRUD operations.

---

## 🧱 Architecture

* **Domain**: Core entities.
* **Application**: DTOs and business services.
* **Persistence**: EF Core `DbContext` and repositories.
* **Infrastructure**: Dependency injection via marker interfaces.
* **API**: ASP.NET Core Web API with controller endpoints.
* **Tests**: Integration tests simulating real-world usage.

---

## 🧪 Technologies

* .NET 6+
* ASP.NET Core
* Entity Framework Core
* SQL Server
* MSTest for Integration Testing
* Newtonsoft.Json

---

## 🛠️ Getting Started

### 1. Clone and Restore Packages

```bash
git clone https://github.com/your-org/ManiaDeLimpeza.git
cd ManiaDeLimpeza
dotnet restore
```

### 2. Apply Migrations and Setup DB

```bash
dotnet ef database update --project ManiaDeLimpeza.Persistence --startup-project ManiaDeLimpeza
```

### 3. Run the API

```bash
dotnet run --project ManiaDeLimpeza
```

API should be available at `https://localhost:5001`

---

## 🔐 Authentication

Use the `/api/auth/register` and `/api/auth/login` endpoints to get a JWT. Pass the token as a Bearer header to access authenticated routes:

```http
Authorization: Bearer {your_token}
```

---

## 🤝 Contributing

1. Fork this repo
2. Create your feature branch: `git checkout -b feature/YourFeature`
3. Commit your changes
4. Push to the branch: `git push origin feature/YourFeature`
5. Create a pull request

---

## 📁 Folder Structure

```
ManiaDeLimpeza/
├── Api/
├── Application/
├── Domain/
├── Infrastructure/
├── Persistence/
├── Tests/
└── README.md
```

---

## 📃 License

[MIT](LICENSE)

---

## 👨‍💻 Author

Developed by the Welber Reis.
