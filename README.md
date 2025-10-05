# TodoList API - Technical Test .NET

RESTful API for task management (To-Do List) developed with .NET 9, implementing Clean Architecture, JWT Authentication, and Entity Framework Core.
> **Disclaimer:** This README was formatted using AI tools for structure and clarity, but all content has been personally written by hand.

## 📋 Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technical Decisions](#technical-decisions)
- [Prerequisites](#prerequisites)
- [Project Setup](#project-setup)
- [How to Run the Project](#how-to-run-the-project)
- [How to Run Tests](#how-to-run-tests)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Test Users](#test-users)

---

## ✨ Features

- ✅ **JWT Authentication**: Secure login with JWT tokens
- ✅ **Task Management**: Complete CRUD (Create, Read, Update, Delete)
- ✅ **Filter by Status**: Filter tasks by completed/pending
- ✅ **Dashboard with Metrics**: Task statistics
- ✅ **Data Validation**: FluentValidation on all inputs
- ✅ **Centralized Error Handling**: Exception middleware
- ✅ **Logging**: Request and error logging
- ✅ **Swagger/OpenAPI**: Interactive API documentation
- ✅ **Unit Tests**: 32+ tests with xUnit, Moq, and FluentAssertions
- ✅ **Clean Architecture**: Clear separation of concerns

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles with the following layers:

```
TodoList.Solution/
├── src/
│   ├── TodoList.Domain/              # Entities, Interfaces, Exceptions
│   ├── TodoList.Application/         # Use Cases, DTOs, Services, Validators
│   ├── TodoList.Infrastructure/      # EF Core, Repositories, Security
│   └── TodoList.API/                 # Controllers, Middleware, Configuration
└── tests/
    ├── TodoList.Domain.Tests/
    ├── TodoList.Application.Tests/
    └── TodoList.API.Tests/
```

### Dependency Flow

```
API → Application → Domain
  ↓
Infrastructure → Application → Domain
```

**Dependency Inversion Principle**: Inner layers (Domain) don't know about outer layers (Infrastructure, API).

---

## 🎯 Technical Decisions

### 1. **Clean Architecture**
**Reason**: Clear separation of concerns, testability, maintainability, and scalability. Allows changing technologies (database, framework) without affecting business logic.

### 2. **Entity Framework Core with In-Memory Database (default)**
**Reason**: Facilitates project execution and testing without SQL Server installation. Ideal for quick evaluation. Includes option to switch to SQL Server with a simple configuration change.

### 3. **JWT (JSON Web Tokens) for Authentication**
**Reason**: Industry standard for stateless RESTful APIs. Enables horizontal scalability and easy integration with frontend applications.

### 4. **FluentValidation**
**Reason**: Clear, readable, and testable validation. Separates validation logic from domain models.

### 5. **AutoMapper 15**
**Reason**: Automatic mapping between entities and DTOs, reducing boilerplate code and manual mapping errors.

### 6. **Repository Pattern**
**Reason**: Data access abstraction, facilitates testing through mocking, and allows changing persistence implementation.

### 7. **Result Pattern**
**Reason**: Explicit error handling without using exceptions for business flows. Improves readability and error control.

### 8. **Custom Middleware**
- **ExceptionHandlingMiddleware**: Centralized exception handling with appropriate HTTP responses.
- **RequestLoggingMiddleware**: Logging of all HTTP requests with response times.

### 9. **xUnit + Moq + FluentAssertions**
**Reason**: Modern and expressive testing stack. xUnit is Microsoft's recommended framework, Moq facilitates mocking, and FluentAssertions makes assertions more readable.

### 10. **Swagger/OpenAPI**
**Reason**: Automatic and interactive API documentation. Allows testing endpoints directly from the browser.

### 11. **BCrypt for Password Hashing**
**Reason**: Robust algorithm specifically designed for passwords, with automatic salt and protection against brute force attacks.

### 12. **Configured CORS**
**Reason**: Allows integration with Angular frontend running on localhost:4200.

### 13. **Seed Data**
**Reason**: Pre-created users and sample tasks for immediate testing without manual setup.

---

## 📦 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- (Optional) [SQL Server](https://www.microsoft.com/sql-server) if using SQL Server instead of In-Memory DB

---

## ⚙️ Project Setup

### Clone the Repository

```bash
git clone https://github.com/jperezo3/todolist-api.git
cd todolist-api
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Solution

```bash
dotnet build
```

---

## 🚀 How to Run the Project

### Option 1: Using In-Memory Database (Default - Recommended)

**No additional configuration needed**. The project is configured to use In-Memory Database by default.

```bash
dotnet run --project src/TodoList.API/TodoList.API.csproj
```

The API will start on:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

### Option 2: Using SQL Server

1. **Update appsettings.json**:

```json
{
  "DatabaseSettings": {
    "UseInMemoryDatabase": false,
    "ConnectionString": "Server=localhost;Database=TodoListDb;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

2. **Create Database Migration**:

```bash
dotnet ef migrations add InitialCreate --project src/TodoList.Infrastructure --startup-project src/TodoList.API
```

3. **Apply Migration (Create Database)**:

```bash
dotnet ef database update --project src/TodoList.Infrastructure --startup-project src/TodoList.API
```

4. **Run the Project**:

```bash
dotnet run --project src/TodoList.API/TodoList.API.csproj
```

### Access Swagger Documentation

Open your browser and navigate to:

```
http://localhost:5000
```

or

```
https://localhost:5001
```

You'll see the interactive Swagger UI with all available endpoints.

---

## 🧪 How to Run Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests by Project

```bash
# Domain tests
dotnet test tests/TodoList.Domain.Tests/TodoList.Domain.Tests.csproj

# Application tests
dotnet test tests/TodoList.Application.Tests/TodoList.Application.Tests.csproj

# API tests
dotnet test tests/TodoList.API.Tests/TodoList.API.Tests.csproj
```

### Run Tests with Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Tests with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Test Summary

- **Domain Tests**: 4 tests
- **Application Tests**: 14 tests
- **API Tests**: 14 tests
- **Total**: 32 tests ✅

---

## 🔐 API Endpoints

### Authentication

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| POST | `/api/Auth/login` | User login | No |

### Tasks

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/TodoTasks` | Get all tasks | Yes |
| GET | `/api/TodoTasks?status=0` | Get pending tasks | Yes |
| GET | `/api/TodoTasks?status=1` | Get completed tasks | Yes |
| GET | `/api/TodoTasks/{id}` | Get task by ID | Yes |
| POST | `/api/TodoTasks` | Create new task | Yes |
| PUT | `/api/TodoTasks/{id}` | Update task | Yes |
| PATCH | `/api/TodoTasks/{id}/toggle-status` | Toggle task status | Yes |
| DELETE | `/api/TodoTasks/{id}` | Delete task | Yes |

### Dashboard

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/api/Dashboard/metrics` | Get task metrics | Yes |

---

## 👥 Test Users

The application comes with pre-seeded test users:

### Admin User
```json
{
  "email": "admin@todolist.com",
  "password": "Admin123!"
}
```

### Regular User
```json
{
  "email": "user@todolist.com",
  "password": "User123!"
}
```

---

## 🗄️ Database

### In-Memory Database (Default)

- ✅ No installation required
- ✅ Data resets on application restart
- ✅ Ideal for development and testing
- ✅ Pre-seeded with test users and sample tasks

### SQL Server (Optional)

To use SQL Server:

1. Install SQL Server
2. Update connection string in `appsettings.json`
3. Set `UseInMemoryDatabase` to `false`
4. Run migrations (see [Option 2](#option-2-using-sql-server))

---

## 📝 Usage Example with Swagger

### Step 1: Login

1. Navigate to `/api/Auth/login` endpoint
2. Click **"Try it out"**
3. Use test credentials:
```json
{
  "email": "admin@todolist.com",
  "password": "Admin123!"
}
```
4. Click **Execute**
5. Copy the `token` from the response

### Step 2: Authorize

1. Click the **"Authorize"** button (🔒) at the top right
2. Enter: `Bearer YOUR_TOKEN_HERE`
3. Click **Authorize**
4. Click **Close**

### Step 3: Test Endpoints

Now you can test all protected endpoints!

**Example - Create a Task:**

1. Go to `POST /api/TodoTasks`
2. Click **"Try it out"**
3. Enter:
```json
{
  "title": "My first task",
  "description": "Testing the API"
}
```
4. Click **Execute**

**Example - Get Dashboard Metrics:**

1. Go to `GET /api/Dashboard/metrics`
2. Click **"Try it out"**
3. Click **Execute**
4. View your task statistics

---

## 🛠️ Technologies Used

### Backend
- .NET 9
- Entity Framework Core 9
- AutoMapper 15
- FluentValidation 11
- BCrypt.Net-Next 4
- Swashbuckle (Swagger) 6
- JWT Authentication

### Testing
- xUnit 2.9
- Moq 4.20
- FluentAssertions 6.12

---

## 📂 Project Structure

```
TodoList.Solution/
├── src/
│   ├── TodoList.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Interfaces/
│   │   ├── Exceptions/
│   │   └── Common/
│   ├── TodoList.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Mappings/
│   │   └── Validators/
│   ├── TodoList.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── Security/
│   └── TodoList.API/
│       ├── Controllers/
│       ├── Middleware/
│       └── Extensions/
└── tests/
    ├── TodoList.Domain.Tests/
    ├── TodoList.Application.Tests/
    └── TodoList.API.Tests/
```

---

## 🔒 Security

- ✅ Passwords are hashed using BCrypt
- ✅ JWT tokens with configurable expiration
- ✅ HTTPS enforced in production
- ✅ Input validation on all endpoints
- ✅ Authorization required for protected endpoints

---

## 🎯 Key Features Implemented

### Functional Requirements ✅
- [x] Login screen with authentication
- [x] View list of tasks
- [x] Add, edit, and delete tasks
- [x] Mark tasks as completed/pending
- [x] Filter tasks by status
- [x] Notifications on actions (via API responses)
- [x] Dashboard with metrics

### Technical Requirements ✅
- [x] Angular CLI project structure
- [x] Modularization
- [x] State management (NgRx/Services)
- [x] Responsive design
- [x] Unit tests
- [x] Lazy Loading
- [x] Performance optimization (trackBy)
- [x] RESTful API with .NET 9
- [x] JWT authentication
- [x] Entity Framework Core
- [x] Data validation
- [x] Unit tests for controllers and services

### Optional Features ✅
- [x] Error handling
- [x] Interceptors for tokens
- [x] Swagger documentation
- [x] Centralized logging
- [x] Centralized error handling

---
## AI Assistance

This document was structured and formatted using AI-based tools to ensure consistency and readability.

---

## 📧 Contact

For questions or issues, please contact: jsebastianperez72@gmail.com

---

## 📄 License

This project is developed as a technical test and is available for evaluation purposes.

---

**Developed with ❤️ using Clean Architecture and .NET 9**
