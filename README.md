# 🛡️ URP Backend — Clean Architecture .NET 8

**4 Projects · Each folder = one purpose · Every file = one class**

## Architecture
```
URP.Domain        → Entities, Exceptions, Repository interfaces (zero NuGet deps)
URP.Application   → Services, DTOs, Validators, Mapping, Interfaces
URP.Infrastructure→ EF Core, Repositories, JWT, Password, DataSeeder
URP.API           → Controllers, Middleware, Program.cs
```

## Quick Start

```bash
# 1. Create MySQL database
mysql -u root -p -e "CREATE DATABASE urp_db CHARACTER SET utf8mb4;"

# 2. Update password in src/URP.API/appsettings.json if not root/root

# 3. Install EF tools (once)
dotnet tool install --global dotnet-ef

# 4. Run migrations (creates all tables)
cd src/URP.Infrastructure
dotnet ef database update --startup-project ../URP.API

# 5. Run API
cd ../URP.API
dotnet run
# → http://localhost:5000/swagger
```

## Default Login
- **Email:** `superadmin@urp.local`
- **Password:** `Admin@123`

## Project File Tree
```
URP.sln
src/
├── URP.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs          ← BaseEntity<TKey>, AuditableEntity<TKey>
│   │   └── EpochHelper.cs         ← Epoch ↔ DateTime conversions
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── Permission.cs
│   │   └── JunctionEntities.cs    ← UserRole, RolePermission, RefreshToken
│   ├── Exceptions/
│   │   ├── DomainException.cs     ← Abstract base
│   │   ├── NotFoundException.cs
│   │   ├── ConflictException.cs
│   │   ├── UnauthorizedException.cs
│   │   ├── ForbiddenException.cs
│   │   └── BusinessRuleException.cs
│   └── Repositories/
│       ├── IRepository.cs         ← Generic base interface
│       ├── IUserRepository.cs
│       ├── IRoleRepository.cs
│       ├── IPermissionRepository.cs
│       └── IUnitOfWork.cs
│
├── URP.Application/
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginRequest.cs
│   │   │   └── LoginResponse.cs
│   │   ├── Users/
│   │   │   ├── CreateUserRequest.cs
│   │   │   ├── UpdateUserRequest.cs
│   │   │   └── UserResponse.cs
│   │   ├── Roles/
│   │   │   ├── CreateRoleRequest.cs
│   │   │   ├── AssignRoleRequest.cs
│   │   │   ├── RemoveRoleRequest.cs
│   │   │   └── RoleResponse.cs
│   │   └── Permissions/
│   │       ├── CreatePermissionRequest.cs
│   │       ├── AssignPermissionRequest.cs
│   │       └── PermissionResponse.cs
│   ├── Common/
│   │   ├── ApiResponse.cs
│   │   ├── PaginatedResponse.cs
│   │   └── PaginationQuery.cs
│   ├── Interfaces/
│   │   ├── IUserService.cs
│   │   ├── IRoleService.cs
│   │   ├── IPermissionService.cs
│   │   ├── ITokenService.cs
│   │   └── IPasswordService.cs
│   ├── Services/
│   │   ├── UserService.cs
│   │   ├── RoleService.cs
│   │   └── PermissionService.cs
│   ├── Validators/
│   │   ├── LoginRequestValidator.cs
│   │   ├── CreateUserRequestValidator.cs
│   │   ├── UpdateUserRequestValidator.cs
│   │   ├── CreateRoleRequestValidator.cs
│   │   └── CreatePermissionRequestValidator.cs
│   ├── Mapping/
│   │   ├── UserMappingProfile.cs
│   │   ├── RoleMappingProfile.cs
│   │   └── PermissionMappingProfile.cs
│   └── ApplicationServiceRegistration.cs
│
├── URP.Infrastructure/
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── DataSeeder.cs
│   │   └── Configurations/
│   │       ├── UserConfiguration.cs
│   │       ├── RoleConfiguration.cs
│   │       ├── PermissionConfiguration.cs
│   │       ├── UserRoleConfiguration.cs
│   │       ├── RolePermissionConfiguration.cs
│   │       └── RefreshTokenConfiguration.cs
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   ├── PermissionRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Services/
│   │   ├── JwtTokenService.cs
│   │   └── PasswordService.cs
│   ├── Authorization/
│   │   ├── PermissionRequirement.cs
│   │   └── PermissionAuthorizationHandler.cs
│   └── DependencyInjection/
│       ├── JwtSettings.cs
│       ├── AppClaimTypes.cs
│       ├── PolicyNames.cs
│       └── InfrastructureServiceRegistration.cs
│
└── URP.API/
    ├── Controllers/
    │   ├── UsersController.cs
    │   ├── RolesController.cs
    │   └── PermissionsController.cs
    ├── Middleware/
    │   ├── ExceptionMiddleware.cs
    │   └── RequestLoggingMiddleware.cs
    ├── Extensions/
    │   ├── SwaggerExtensions.cs
    │   └── ClaimsPrincipalExtensions.cs
    ├── Program.cs
    ├── appsettings.json           ← DB: root/root, JWT secret
    └── appsettings.Development.json
```

## Migration Commands
```bash
cd src/URP.Infrastructure

# Apply migrations
dotnet ef database update --startup-project ../URP.API

# Add new migration after entity changes
dotnet ef migrations add MigrationName --startup-project ../URP.API

# Generate SQL script for production
dotnet ef migrations script --idempotent --output migration.sql --startup-project ../URP.API
```

## Timestamps
All DB timestamps are **Unix epoch seconds (BIGINT)**. `EpochHelper` in Domain handles conversions. Frontend converts to IST for display.
