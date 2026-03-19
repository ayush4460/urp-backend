# 🛡️ URP Backend — User Role Permission Management API

> A production-ready REST API for managing users, roles, and permissions with fine-grained access control.
> Built with **.NET 8**, **Clean Architecture**, **Entity Framework Core**, and **MySQL**.

---

## 📖 What Does This API Do?

This is the **brain** behind the URP system. It handles everything the frontend needs:

- Register and authenticate users with secure JWT tokens
- Store users, roles, and permissions in a MySQL database
- Check if a user is allowed to do something before doing it
- Return clean JSON responses for every action

### Think of it like a company HR + Security system:

| Concept | Real World Example |
|---|---|
| **User** | An employee at the company |
| **Role** | Their job title (SuperAdmin, Admin, Manager, User) |
| **Permission** | A specific thing they can do (`users:read`, `roles:assign`) |
| **JWT Token** | Their signed ID badge — proves who they are on every request |
| **Unit of Work** | One save operation that commits everything at once — like signing a document |

---

## ✨ Features

### 🔐 Authentication
- Register new user account with strong password validation
- Login with email and password — returns a signed JWT access token
- Passwords hashed with **PBKDF2-HMACSHA256** (100,000 iterations) via ASP.NET Identity PasswordHasher
- JWT tokens contain user ID, email, roles, and all permissions as claims
- Token expiry configurable per environment (60 min production, 24 hours dev)

### 👥 User Management
- Get all users with pagination, search, and sorting
- Get own profile (`GET /users/me`) — no special permission needed
- Get any user by ID with full role and permission graph
- Update user details — optionally change password (requires current password)
- Soft-delete users — sets `deleted_at` epoch and `is_active = false`, data preserved

### 🔐 Role Management
- Get all roles with their assigned permissions
- Create, update, delete roles
- Assign or remove roles from users
- Get all users belonging to a specific role

### 🔑 Permission Management
- Get all permissions, optionally filtered by group (`?group=Users`)
- Create new permissions following `resource:action` naming convention
- Assign or remove permissions from roles
- Get all permissions for a specific role

### 🔒 Access Control
- Every protected endpoint requires a valid JWT token
- Each endpoint has a specific permission policy (e.g. `users:read`, `roles:assign`)
- Custom `PermissionAuthorizationHandler` checks JWT claims against required policies
- New users are automatically assigned the default **User** role on registration

### 🗄️ Database
- MySQL 8 with EF Core Code-First migrations
- All timestamps stored as **Unix epoch seconds (BIGINT)** — timezone-neutral
- Soft delete via global query filter (`deleted_at IS NULL`)
- Auto-migrate and auto-seed on startup — no manual setup needed

---

## 🛠️ Tech Stack

| Technology | Version | What It Does |
|---|---|---|
| **.NET 8** | 8.x LTS | The core runtime and framework everything is built on |
| **ASP.NET Core** | 8.x | Handles HTTP requests, routing, middleware pipeline |
| **Entity Framework Core** | 8.x | ORM — maps C# classes to database tables, handles queries |
| **Pomelo MySQL** | 8.x | EF Core provider for MySQL database connection |
| **MySQL 8** | 8.x | Relational database storing all data |
| **JWT Bearer Auth** | — | Validates JWT tokens on every protected request |
| **ASP.NET Identity PasswordHasher** | 2.2 | Securely hashes and verifies passwords (PBKDF2) |
| **AutoMapper** | 12.x | Maps domain entities → DTOs cleanly without manual property copying |
| **FluentValidation** | 11.x | Validates all incoming request DTOs with readable rules |
| **Swashbuckle (Swagger)** | 6.x | Auto-generates interactive API documentation with JWT support |
| **Serilog** | 8.x | Structured logging to console and rolling log files |

---

## 🏗️ Clean Architecture — 4 Layers

This project follows **Clean Architecture** — a way of organising code so business rules never depend on databases or frameworks.

```
┌─────────────────────────────────────────────────────────────┐
│                      URP.API  (Layer 4)                      │
│         Controllers · Middleware · Swagger · Program.cs      │
│         "The waiter — takes HTTP requests, returns JSON"     │
├─────────────────────────────────────────────────────────────┤
│                  URP.Application  (Layer 3)                  │
│         Services · DTOs · Validators · Mapping               │
│         "The chef — executes business use cases"             │
├─────────────────────────────────────────────────────────────┤
│                 URP.Infrastructure  (Layer 2)                 │
│         EF Core · Repositories · JWT · Password Hashing      │
│         "The kitchen equipment — technical tools"            │
├─────────────────────────────────────────────────────────────┤
│                    URP.Domain  (Layer 1)                     │
│         Entities · Exceptions · Repository Interfaces        │
│         "The rulebook — pure C#, zero external dependencies" │
└─────────────────────────────────────────────────────────────┘
```

### Dependency Rule (never break this)
```
API        →  Application  →  Domain   ✅
API        →  Infrastructure →  Domain  ✅
Infrastructure → Application → Domain  ✅
Application  →  Infrastructure         ❌ NEVER
Domain       →  anything               ❌ NEVER
```

---

## 📁 Project File Tree

Every file has **one class** and **one responsibility**.

```
URP.sln
src/
│
├── URP.Domain/                          ← Layer 1: Zero NuGet dependencies
│   ├── Common/
│   │   ├── BaseEntity.cs               ← Base class with Id, CreatedAt, UpdatedAt (epoch)
│   │   └── EpochHelper.cs              ← Unix epoch ↔ DateTime conversion helpers
│   ├── Entities/
│   │   ├── User.cs                     ← User entity with factory method + domain logic
│   │   ├── Role.cs                     ← Role entity with factory method
│   │   ├── Permission.cs               ← Permission entity (resource:action format)
│   │   └── JunctionEntities.cs         ← UserRole, RolePermission, RefreshToken
│   ├── Exceptions/
│   │   ├── DomainException.cs          ← Abstract base exception
│   │   ├── NotFoundException.cs        ← Resource not found → HTTP 404
│   │   ├── ConflictException.cs        ← Duplicate data → HTTP 409
│   │   ├── UnauthorizedException.cs    ← Bad credentials → HTTP 401
│   │   ├── ForbiddenException.cs       ← No permission → HTTP 403
│   │   └── BusinessRuleException.cs    ← Rule violation → HTTP 400
│   └── Repositories/
│       ├── IRepository.cs              ← Generic base: GetById, Add, Update, Remove
│       ├── IUserRepository.cs          ← User-specific queries
│       ├── IRoleRepository.cs          ← Role-specific queries + GetByNameAsync
│       ├── IPermissionRepository.cs    ← Permission queries + role assignment
│       └── IUnitOfWork.cs              ← Wraps all repos + SaveChangesAsync
│
├── URP.Application/                     ← Layer 2: Business use cases
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginRequest.cs         ← { email, password }
│   │   │   └── LoginResponse.cs        ← { accessToken, refreshToken, expiresIn, user }
│   │   ├── Users/
│   │   │   ├── CreateUserRequest.cs    ← Register form data
│   │   │   ├── UpdateUserRequest.cs    ← Edit profile / change password
│   │   │   └── UserResponse.cs         ← User data sent to frontend (with epoch timestamps)
│   │   ├── Roles/
│   │   │   ├── CreateRoleRequest.cs    ← { name, description }
│   │   │   ├── AssignRoleRequest.cs    ← { userId, roleId }
│   │   │   ├── RemoveRoleRequest.cs    ← { userId, roleId }
│   │   │   └── RoleResponse.cs         ← Role + its permissions
│   │   └── Permissions/
│   │       ├── CreatePermissionRequest.cs  ← { name, group, description }
│   │       ├── AssignPermissionRequest.cs  ← { roleId, permissionId }
│   │       └── PermissionResponse.cs       ← Permission data
│   ├── Common/
│   │   ├── ApiResponse.cs              ← Standard envelope: { success, message, data, errors }
│   │   ├── PaginatedResponse.cs        ← { items, totalCount, page, pageSize, totalPages }
│   │   └── PaginationQuery.cs          ← { page, pageSize, search, sortBy, sortDescending }
│   ├── Interfaces/
│   │   ├── IUserService.cs             ← Register, Login, GetById, GetAll, Update, Delete
│   │   ├── IRoleService.cs             ← GetAll, Create, Update, Delete, Assign, Remove
│   │   ├── IPermissionService.cs       ← GetAll, Create, Assign, Remove, GetByRole
│   │   ├── ITokenService.cs            ← GenerateToken (implemented in Infrastructure)
│   │   └── IPasswordService.cs         ← Hash, Verify (implemented in Infrastructure)
│   ├── Services/
│   │   ├── UserService.cs              ← All user use cases + auto-assign User role on register
│   │   ├── RoleService.cs              ← All role use cases
│   │   └── PermissionService.cs        ← All permission use cases
│   ├── Validators/
│   │   ├── LoginRequestValidator.cs             ← Email + password required
│   │   ├── CreateUserRequestValidator.cs        ← Strong password, unique username/email
│   │   ├── UpdateUserRequestValidator.cs        ← Optional password change validation
│   │   ├── CreateRoleRequestValidator.cs        ← Name 2-50 chars
│   │   └── CreatePermissionRequestValidator.cs  ← Must match resource:action format
│   ├── Mapping/
│   │   ├── UserMappingProfile.cs       ← User entity → UserResponse (flattens roles+permissions)
│   │   ├── RoleMappingProfile.cs       ← Role entity → RoleResponse
│   │   └── PermissionMappingProfile.cs ← Permission entity → PermissionResponse
│   └── ApplicationServiceRegistration.cs  ← Registers AutoMapper, FluentValidation, Services
│
├── URP.Infrastructure/                  ← Layer 3: Technical implementation
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs     ← EF Core DbContext + global soft-delete filter
│   │   ├── DataSeeder.cs               ← Seeds 4 roles, 12 permissions, SuperAdmin user
│   │   └── Configurations/             ← One EF Fluent API config file per entity
│   │       ├── UserConfiguration.cs    ← Column names, indexes, unique constraints
│   │       ├── RoleConfiguration.cs
│   │       ├── PermissionConfiguration.cs
│   │       ├── UserRoleConfiguration.cs      ← Composite PK + explicit navigation
│   │       ├── RolePermissionConfiguration.cs← Composite PK + explicit navigation
│   │       └── RefreshTokenConfiguration.cs
│   ├── Repositories/
│   │   ├── BaseRepository.cs           ← Generic GetById, Add, Update, Remove
│   │   ├── UserRepository.cs           ← Full graph loads, pagination, search, sort
│   │   ├── RoleRepository.cs           ← Loads with permissions, GetByName
│   │   ├── PermissionRepository.cs     ← Group queries, role assignment queries
│   │   └── UnitOfWork.cs               ← Wraps all repos, single SaveChangesAsync
│   ├── Services/
│   │   ├── JwtTokenService.cs          ← Generates HS256 JWT with role + permission claims
│   │   └── PasswordService.cs          ← PBKDF2 hash and verify via Identity PasswordHasher
│   ├── Authorization/
│   │   ├── PermissionRequirement.cs          ← IAuthorizationRequirement with permission string
│   │   └── PermissionAuthorizationHandler.cs ← Checks JWT "permission" claims
│   └── DependencyInjection/
│       ├── JwtSettings.cs                     ← POCO bound from appsettings.json
│       ├── AppClaimTypes.cs                   ← Claim key constants (uid, permission, fullname)
│       ├── PolicyNames.cs                     ← All 12 policy name constants
│       └── InfrastructureServiceRegistration.cs ← Registers DB, repos, JWT, auth policies
│
└── URP.API/                             ← Layer 4: HTTP delivery
    ├── Controllers/
    │   ├── UsersController.cs          ← register, login, getAll, getById, getMe, update, delete
    │   ├── RolesController.cs          ← getAll, getById, create, update, delete, assign, remove
    │   └── PermissionsController.cs    ← getAll, getById, create, assign, remove, getByRole
    ├── Middleware/
    │   ├── ExceptionMiddleware.cs      ← Catches domain exceptions → correct HTTP status + JSON
    │   └── RequestLoggingMiddleware.cs ← Logs every request with method, path, status, elapsed ms
    ├── Extensions/
    │   ├── SwaggerExtensions.cs        ← Swagger with JWT Bearer UI support
    │   └── ClaimsPrincipalExtensions.cs← GetUserId(), HasPermission() helpers
    ├── Program.cs                      ← App entry point: registers all layers, builds pipeline
    ├── appsettings.json                ← DB connection string, JWT config, CORS
    └── appsettings.Development.json    ← Dev overrides: debug logging, 24hr token expiry
```

---

## ⏱️ Timestamps — Unix Epoch Seconds

All database timestamps (`created_at`, `updated_at`, `deleted_at`, `last_login_at`) are stored as **BIGINT (Unix epoch seconds)** — a plain integer.

```sql
-- What you see in MySQL
SELECT id, email, created_at FROM users;
-- 1 | superadmin@urp.local | 1735689600
```

The `EpochHelper` class in `URP.Domain/Common/EpochHelper.cs` handles conversions:

```csharp
EpochHelper.NowSeconds()           // Current time as epoch
EpochHelper.ToEpoch(dateTime)      // DateTime → long
EpochHelper.FromEpoch(1735689600)  // long → DateTime (UTC)
```

The **frontend** converts epoch → IST (Indian Standard Time) for display using `epochToISTDateTime()`.

**Why epoch?**
- No timezone confusion in the database
- Simple integer — fast to compare and sort
- Works identically in .NET, TypeScript, Python, SQL

---

## 🌱 Seeded Default Data

On first startup the API automatically seeds:

| Data | Details |
|---|---|
| **4 Roles** | SuperAdmin, Admin, Manager, User |
| **12 Permissions** | users:read/create/update/delete, roles:read/create/update/delete/assign, permissions:read/create/assign |
| **Role-Permission mapping** | SuperAdmin gets all 12, Admin gets 7, Manager gets 3, User gets 0 |
| **1 Default User** | `superadmin@urp.local` / `Admin@123` with SuperAdmin role |

---

## 🌐 API Endpoints

**Base URL:** `http://localhost:5000/api/v1`

### 🔓 Public (no auth needed)
| Method | Endpoint | Description |
|---|---|---|
| POST | `/users/register` | Register new user account |
| POST | `/users/login` | Login → returns JWT access token |

### 👤 Users (JWT required)
| Method | Endpoint | Permission | Description |
|---|---|---|---|
| GET | `/users` | `users:read` | Paginated list with search + sort |
| GET | `/users/me` | any auth | Own profile |
| GET | `/users/{id}` | `users:read` | User by ID with roles + permissions |
| PUT | `/users/{id}` | `users:update` | Update user, optional password change |
| DELETE | `/users/{id}` | `users:delete` | Soft-delete |

### 🔐 Roles (JWT required)
| Method | Endpoint | Permission | Description |
|---|---|---|---|
| GET | `/roles` | `roles:read` | All roles with permissions |
| GET | `/roles/{id}` | `roles:read` | Role by ID |
| POST | `/roles` | `roles:create` | Create role |
| PUT | `/roles/{id}` | `roles:update` | Update role |
| DELETE | `/roles/{id}` | `roles:delete` | Delete role |
| POST | `/roles/assign` | `roles:assign` | Assign role to user |
| DELETE | `/roles/remove` | `roles:assign` | Remove role from user |
| GET | `/roles/{id}/users` | `roles:read` | All users in a role |

### 🔑 Permissions (JWT required)
| Method | Endpoint | Permission | Description |
|---|---|---|---|
| GET | `/permissions` | `permissions:read` | All permissions (filter by ?group=) |
| GET | `/permissions/{id}` | `permissions:read` | Permission by ID |
| POST | `/permissions` | `permissions:create` | Create permission |
| POST | `/permissions/assign` | `permissions:assign` | Assign permission to role |
| DELETE | `/permissions/remove` | `permissions:assign` | Remove permission from role |
| GET | `/permissions/role/{id}` | `permissions:read` | All permissions for a role |

---

## 🚀 Quick Start

### Prerequisites
- **.NET 8 SDK** → https://dotnet.microsoft.com/download/dotnet/8.0
- **MySQL 8** → https://dev.mysql.com/downloads/mysql/
- **Visual Studio 2022** or **JetBrains Rider**
- **EF Core CLI Tools** → `dotnet tool install --global dotnet-ef`

### Step 1 — Create the database

```sql
-- Run in MySQL Workbench or terminal
CREATE DATABASE urp_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### Step 2 — Update connection string

Open `src/URP.API/appsettings.json` and set your MySQL password:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=urp_db;Uid=root;Pwd=YOUR_PASSWORD;CharSet=utf8mb4;AllowPublicKeyRetrieval=true;SslMode=None;"
}
```

### Step 3 — Run migrations

```bash
cd src/URP.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../URP.API
dotnet ef database update --startup-project ../URP.API
```

### Step 4 — Start the API

```bash
cd ../URP.API
dotnet run
# → http://localhost:5000/swagger
```

Or press **F5** in Visual Studio (set `URP.API` as startup project).

### Default Login
```
Email:    superadmin@urp.local
Password: Admin@123
```

---

## 📋 Migration Commands

All commands run from `src/URP.Infrastructure`:

```bash
# Create migration after changing any entity
dotnet ef migrations add MigrationName --startup-project ../URP.API

# Apply all pending migrations
dotnet ef database update --startup-project ../URP.API

# List all migrations and their status
dotnet ef migrations list --startup-project ../URP.API

# Rollback to a previous migration
dotnet ef database update PreviousMigrationName --startup-project ../URP.API

# Remove the last unapplied migration
dotnet ef migrations remove --startup-project ../URP.API

# Drop and recreate database (⚠️ dev only)
dotnet ef database drop --force --startup-project ../URP.API
dotnet ef database update --startup-project ../URP.API

# Generate SQL script for production review
dotnet ef migrations script --idempotent --output migration.sql --startup-project ../URP.API
```

---

## 🔗 Related

- **Frontend Repository** → [urp-frontend](#) — React 18 + TypeScript dashboard
- **Live API Docs** → `http://localhost:5000/swagger` (when running)

---

*Built with ❤️ using .NET 8, Clean Architecture, and industry best practices.*
