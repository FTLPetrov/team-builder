# User Model Pipeline - Git Commit Commands

## Overview
This document provides the git commands to commit all User-related changes in separate, logical commits following the development pipeline structure.

## 1. User Model & Database Changes

### Commit 1: User Model Updates
```bash
git add server/Data/TeamBuilder.Data.Models/User.cs
git commit -m "feat(model): update User model structure

- Updated User entity properties
- Enhanced data model for new features
- Improved model validation attributes"
```

### Commit 2: Database Context & Migrations
```bash
git add server/Data/TeamBuilder.Data/TeamBuilderDbContext.cs
git add server/Data/TeamBuilder.Data/Migrations/TeamBuilderDbContextModelSnapshot.cs
git add server/Data/TeamBuilder.Data/Migrations/20250804224809_AddEmailVerificationFields.Designer.cs
git add server/Data/TeamBuilder.Data/Migrations/20250804224809_AddEmailVerificationFields.cs
git add server/Data/TeamBuilder.Data/Migrations/20250804230910_RemoveEmailVerificationFields.Designer.cs
git add server/Data/TeamBuilder.Data/Migrations/20250804230910_RemoveEmailVerificationFields.cs
git add server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.Designer.cs
git add server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.cs
git commit -m "feat(database): add User-related database migrations

- Add email verification fields migration
- Remove email verification fields migration  
- Update models for admin functionality
- Update database context for User model changes"
```

## 2. User Contracts (DTOs)

### Commit 3: User Request Contracts
```bash
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserCreateRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserLoginRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserUpdateRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/AssignRoleRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ConfirmEmailRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ForgotPasswordRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ResetPasswordRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/CreateWarningRequest.cs
git commit -m "feat(contracts): update User request DTOs

- Enhanced UserCreateRequest with new validation
- Updated UserLoginRequest structure
- Improved UserUpdateRequest properties
- Added role assignment request
- Added email confirmation request
- Added password reset functionality requests
- Added warning creation request"
```

### Commit 4: User Response Contracts
```bash
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserCreateResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserLoginResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserUpdateResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/AssignRoleResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ConfirmEmailResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ForgotPasswordResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ResetPasswordResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/WarningResponse.cs
git commit -m "feat(contracts): update User response DTOs

- Enhanced UserResponse with additional properties
- Updated authentication response structures
- Added role assignment response
- Added email confirmation response
- Added password reset responses
- Added warning response for user management"
```

## 3. User Service Layer

### Commit 5: User Service Interface
```bash
git add server/Services/TeamBuilder.Services.Core/Interfaces/IUserService.cs
git commit -m "feat(service): update IUserService interface

- Added new user management methods
- Enhanced authentication methods
- Added role management capabilities
- Added email verification methods
- Added password reset functionality"
```

### Commit 6: User Service Implementation
```bash
git add server/Services/TeamBuilder.Services.Core/UserService.cs
git commit -m "feat(service): implement enhanced UserService

- Implemented new user management features
- Enhanced authentication logic
- Added role assignment functionality
- Implemented email verification system
- Added password reset capabilities
- Improved error handling and validation"
```

## 4. User Controller (API Layer)

### Commit 7: User Controller
```bash
git add server/WebApi/TeamBuilder.WebApi/Controllers/UserController.cs
git commit -m "feat(controller): update UserController endpoints

- Enhanced user CRUD operations
- Added authentication endpoints
- Implemented role management endpoints
- Added email verification endpoints
- Added password reset endpoints
- Improved API response handling
- Enhanced security and validation"
```

## 5. User Tests

### Commit 8: User Service Tests
```bash
git add server/Tests/TeamBuilder.Services.Tests/UserServiceTests.cs
git commit -m "test(user): update UserService unit tests

- Added tests for new user management features
- Enhanced authentication test coverage
- Added role assignment test cases
- Added email verification tests
- Added password reset test scenarios
- Improved test data setup and cleanup"
```

## 6. Supporting Files & Dependencies

### Commit 9: Project Dependencies
```bash
git add server/Services/TeamBuilder.Services.Core/TeamBuilder.Services.Core.csproj
git add server/Data/TeamBuilder.Data.Models/TeamBuilder.Data.Models.csproj
git add server/Data/TeamBuilder.Data/TeamBuilder.Data.csproj
git commit -m "chore(deps): update project dependencies for User features

- Updated service layer dependencies
- Added required packages for user management
- Updated data layer dependencies"
```

### Commit 10: JWT & Authentication Supporting Files
```bash
git add server/Services/TeamBuilder.Services.Core/JwtService.cs
git add server/Services/TeamBuilder.Services.Core/Interfaces/IJwtService.cs
git add server/WebApi/TeamBuilder.WebApi/Middleware/JwtMiddleware.cs
git commit -m "feat(auth): update JWT and authentication middleware

- Enhanced JWT service functionality
- Updated JWT middleware for new user features
- Improved token validation and security"
```

## Quick Execution Script

If you want to execute all commits at once, you can copy and paste this entire sequence:

```bash
# User Model Updates
git add server/Data/TeamBuilder.Data.Models/User.cs
git commit -m "feat(model): update User model structure"

# Database Context & Migrations
git add server/Data/TeamBuilder.Data/TeamBuilderDbContext.cs server/Data/TeamBuilder.Data/Migrations/TeamBuilderDbContextModelSnapshot.cs server/Data/TeamBuilder.Data/Migrations/20250804224809_AddEmailVerificationFields.Designer.cs server/Data/TeamBuilder.Data/Migrations/20250804224809_AddEmailVerificationFields.cs server/Data/TeamBuilder.Data/Migrations/20250804230910_RemoveEmailVerificationFields.Designer.cs server/Data/TeamBuilder.Data/Migrations/20250804230910_RemoveEmailVerificationFields.cs server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.Designer.cs server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.cs
git commit -m "feat(database): add User-related database migrations"

# User Request Contracts
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserCreateRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserLoginRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/UserUpdateRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/AssignRoleRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ConfirmEmailRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ForgotPasswordRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/ResetPasswordRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Requests/CreateWarningRequest.cs
git commit -m "feat(contracts): update User request DTOs"

# User Response Contracts
git add server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserCreateResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserLoginResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserUpdateResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/UserResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/AssignRoleResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ConfirmEmailResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ForgotPasswordResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/ResetPasswordResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/User/Responses/WarningResponse.cs
git commit -m "feat(contracts): update User response DTOs"

# User Service Interface
git add server/Services/TeamBuilder.Services.Core/Interfaces/IUserService.cs
git commit -m "feat(service): update IUserService interface"

# User Service Implementation
git add server/Services/TeamBuilder.Services.Core/UserService.cs
git commit -m "feat(service): implement enhanced UserService"

# User Controller
git add server/WebApi/TeamBuilder.WebApi/Controllers/UserController.cs
git commit -m "feat(controller): update UserController endpoints"

# User Service Tests
git add server/Tests/TeamBuilder.Services.Tests/UserServiceTests.cs
git commit -m "test(user): update UserService unit tests"

# Project Dependencies
git add server/Services/TeamBuilder.Services.Core/TeamBuilder.Services.Core.csproj server/Data/TeamBuilder.Data.Models/TeamBuilder.Data.Models.csproj server/Data/TeamBuilder.Data/TeamBuilder.Data.csproj
git commit -m "chore(deps): update project dependencies for User features"

# JWT & Authentication Supporting Files
git add server/Services/TeamBuilder.Services.Core/JwtService.cs server/Services/TeamBuilder.Services.Core/Interfaces/IJwtService.cs server/WebApi/TeamBuilder.WebApi/Middleware/JwtMiddleware.cs
git commit -m "feat(auth): update JWT and authentication middleware"
```

## Notes

1. **Review before committing**: Always review the changes using `git diff --cached` before each commit
2. **Adjust commit messages**: Feel free to make the commit messages more specific based on your actual changes
3. **Order matters**: The commits are ordered to respect dependencies (model → contracts → service → controller → tests)
4. **Push after all commits**: After all commits are done, push with `git push origin main`
5. **Check status**: Use `git status` between commits to ensure you're only committing the intended files

## Alternative: Single Branch Approach

If you prefer to create separate branches for each component:

```bash
# Create and switch to user-model branch
git checkout -b feat/user-model
git add server/Data/TeamBuilder.Data.Models/User.cs
git commit -m "feat(model): update User model structure"
git push origin feat/user-model

# Create and switch to user-contracts branch  
git checkout main
git checkout -b feat/user-contracts
# ... add contract files and commit
git push origin feat/user-contracts

# Continue for each component...
```
