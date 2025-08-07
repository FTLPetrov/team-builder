# Team Model Pipeline - Git Commit Commands

## Overview
This document provides the git commands to commit all Team-related changes in separate, logical commits following the development pipeline structure.

## 1. Team Models & Database Changes

### Commit 1: Team Data Models
```bash
git add server/Data/TeamBuilder.Data.Models/Team.cs
git add server/Data/TeamBuilder.Data.Models/TeamMember.cs
git add server/Data/TeamBuilder.Data.Models/TeamRole.cs
git commit -m "feat(model): update Team, TeamMember, and TeamRole models

- Enhanced Team entity with new properties and relationships
- Updated TeamMember model for better member management
- Improved TeamRole model with enhanced role definitions
- Added validation attributes and constraints"
```

### Commit 2: Team Repository Layer
```bash
git add server/Data/TeamBuilder.Data/Repositories/Interfaces/ITeamRepository.cs
git add server/Data/TeamBuilder.Data/Repositories/Interfaces/ITeamMemberRepository.cs
git add server/Data/TeamBuilder.Data/Repositories/TeamRepository.cs
git add server/Data/TeamBuilder.Data/Repositories/TeamMemberRepository.cs
git commit -m "feat(repository): implement Team and TeamMember repositories

- Added ITeamRepository interface with comprehensive team operations
- Added ITeamMemberRepository interface for member management
- Implemented TeamRepository with CRUD and query operations
- Implemented TeamMemberRepository with member-specific operations
- Enhanced data access patterns for team functionality"
```

### Commit 3: Database Context Updates
```bash
git add server/Data/TeamBuilder.Data/TeamBuilderDbContext.cs
git add server/Data/TeamBuilder.Data/Migrations/TeamBuilderDbContextModelSnapshot.cs
git commit -m "feat(database): update DbContext for Team model changes

- Updated TeamBuilderDbContext for new Team entity configurations
- Enhanced entity relationships and constraints
- Updated model snapshot for Team-related schema changes"
```

## 2. Team Contracts (DTOs)

### Commit 4: Team Request Contracts
```bash
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/TeamCreateRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/TeamUpdateRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/EventCreateRequest.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/EventUpdateRequest.cs
git commit -m "feat(contracts): update Team request DTOs

- Enhanced TeamCreateRequest with comprehensive validation
- Updated TeamUpdateRequest for team modification scenarios
- Added EventCreateRequest for team event creation
- Added EventUpdateRequest for team event management
- Improved data validation and error handling"
```

### Commit 5: Team Response Contracts
```bash
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamCreateResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamUpdateResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventCreateResponse.cs
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventUpdateResponse.cs
git commit -m "feat(contracts): update Team response DTOs

- Enhanced TeamResponse with complete team information
- Updated team operation response structures
- Added comprehensive event response DTOs
- Improved API response consistency and data serialization
- Added proper error handling responses"
```

## 3. Team Service Layer

### Commit 6: Team Service Interface
```bash
git add server/Services/TeamBuilder.Services.Core/Interfaces/ITeamService.cs
git commit -m "feat(service): update ITeamService interface

- Added comprehensive team management methods
- Enhanced team member management capabilities
- Added team event management methods
- Added team search and filtering operations
- Improved async operation patterns
- Added role-based access control methods"
```

### Commit 7: Team Service Implementation
```bash
git add server/Services/TeamBuilder.Services.Core/TeamService.cs
git commit -m "feat(service): implement enhanced TeamService

- Implemented comprehensive team CRUD operations
- Added team member management functionality
- Implemented team event creation and management
- Added team search and filtering capabilities
- Enhanced validation and error handling
- Implemented role-based access control
- Added team invitation and joining logic
- Improved business logic for team operations"
```

## 4. Team Controller (API Layer)

### Commit 8: Team Controller
```bash
git add server/WebApi/TeamBuilder.WebApi/Controllers/TeamController.cs
git commit -m "feat(controller): update TeamController endpoints

- Enhanced team CRUD API endpoints
- Added team member management endpoints
- Implemented team event management APIs
- Added team search and filtering endpoints
- Improved API documentation and responses
- Enhanced security and authorization
- Added proper error handling and validation
- Implemented team invitation endpoints"
```

## 5. Team Tests

### Commit 9: Team Service Tests
```bash
git add server/Tests/TeamBuilder.Services.Tests/TeamServiceTests.cs
git commit -m "test(team): update TeamService unit tests

- Added comprehensive team CRUD test scenarios
- Enhanced team member management test coverage
- Added team event management test cases
- Implemented team search and filtering tests
- Added role-based access control tests
- Enhanced test data setup and cleanup
- Improved test coverage for edge cases
- Added integration test scenarios"
```

## 6. Supporting Files & Dependencies

### Commit 10: Project Dependencies
```bash
git add server/Services/TeamBuilder.Services.Core/TeamBuilder.Services.Core.csproj
git add server/Data/TeamBuilder.Data.Models/TeamBuilder.Data.Models.csproj
git add server/Data/TeamBuilder.Data/TeamBuilder.Data.csproj
git add server/Tests/TeamBuilder.Services.Tests/TeamBuilder.Services.Tests.csproj
git commit -m "chore(deps): update project dependencies for Team features

- Updated service layer dependencies for team functionality
- Added required packages for team management features
- Updated data layer dependencies for team repositories
- Enhanced test project dependencies for team testing"
```

### Commit 11: Database Migrations (Team-specific)
```bash
git add server/Data/TeamBuilder.Data/Migrations/20250803003512_InitialCreate.cs
git add server/Data/TeamBuilder.Data/Migrations/20250803003512_InitialCreate.Designer.cs
git add server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.cs
git add server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.Designer.cs
git commit -m "feat(database): add Team-related database migrations

- Added initial database creation with Team entities
- Updated models for admin functionality affecting teams
- Enhanced team-related database schema
- Added proper foreign key relationships for teams"
```

## Quick Execution Script

If you want to execute all commits at once, you can copy and paste this entire sequence:

```bash
# Team Data Models
git add server/Data/TeamBuilder.Data.Models/Team.cs server/Data/TeamBuilder.Data.Models/TeamMember.cs server/Data/TeamBuilder.Data.Models/TeamRole.cs
git commit -m "feat(model): update Team, TeamMember, and TeamRole models"

# Team Repository Layer
git add server/Data/TeamBuilder.Data/Repositories/Interfaces/ITeamRepository.cs server/Data/TeamBuilder.Data/Repositories/Interfaces/ITeamMemberRepository.cs server/Data/TeamBuilder.Data/Repositories/TeamRepository.cs server/Data/TeamBuilder.Data/Repositories/TeamMemberRepository.cs
git commit -m "feat(repository): implement Team and TeamMember repositories"

# Database Context Updates
git add server/Data/TeamBuilder.Data/TeamBuilderDbContext.cs server/Data/TeamBuilder.Data/Migrations/TeamBuilderDbContextModelSnapshot.cs
git commit -m "feat(database): update DbContext for Team model changes"

# Team Request Contracts
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/TeamCreateRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/TeamUpdateRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/EventCreateRequest.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Requests/EventUpdateRequest.cs
git commit -m "feat(contracts): update Team request DTOs"

# Team Response Contracts
git add server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamCreateResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/TeamUpdateResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventCreateResponse.cs server/Services/TeamBuilder.Services.Core/Contracts/Team/Responses/EventUpdateResponse.cs
git commit -m "feat(contracts): update Team response DTOs"

# Team Service Interface
git add server/Services/TeamBuilder.Services.Core/Interfaces/ITeamService.cs
git commit -m "feat(service): update ITeamService interface"

# Team Service Implementation
git add server/Services/TeamBuilder.Services.Core/TeamService.cs
git commit -m "feat(service): implement enhanced TeamService"

# Team Controller
git add server/WebApi/TeamBuilder.WebApi/Controllers/TeamController.cs
git commit -m "feat(controller): update TeamController endpoints"

# Team Service Tests
git add server/Tests/TeamBuilder.Services.Tests/TeamServiceTests.cs
git commit -m "test(team): update TeamService unit tests"

# Project Dependencies
git add server/Services/TeamBuilder.Services.Core/TeamBuilder.Services.Core.csproj server/Data/TeamBuilder.Data.Models/TeamBuilder.Data.Models.csproj server/Data/TeamBuilder.Data/TeamBuilder.Data.csproj server/Tests/TeamBuilder.Services.Tests/TeamBuilder.Services.Tests.csproj
git commit -m "chore(deps): update project dependencies for Team features"

# Database Migrations
git add server/Data/TeamBuilder.Data/Migrations/20250803003512_InitialCreate.cs server/Data/TeamBuilder.Data/Migrations/20250803003512_InitialCreate.Designer.cs server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.cs server/Data/TeamBuilder.Data/Migrations/20250805075156_UpdateModelsForAdmin.Designer.cs
git commit -m "feat(database): add Team-related database migrations"
```

## Client-Side Team Changes (Bonus)

If you also want to commit client-side Team changes separately:

### Commit 12: Team Frontend Pages
```bash
git add client/src/pages/Teams.jsx
git add client/src/pages/TeamDetail.jsx
git add client/src/pages/CreateTeam.jsx
git commit -m "feat(client): update Team-related pages

- Enhanced Teams listing page
- Improved TeamDetail page functionality
- Updated CreateTeam page with new features"
```

### Commit 13: Team Frontend Services
```bash
git add client/src/services/api/teamService.js
git commit -m "feat(client): update Team API service

- Enhanced team API service methods
- Improved error handling for team operations
- Added new team management endpoints"
```

## Notes

1. **Review before committing**: Always review changes with `git diff --cached`
2. **Dependency order**: Commits follow logical dependency flow
3. **Modular approach**: Each commit focuses on a specific component
4. **Team-Event relationship**: Event-related contracts are included as they're part of team functionality
5. **Repository pattern**: Includes both interfaces and implementations
6. **Testing**: Comprehensive test coverage included
7. **Migration order**: Database migrations are handled separately to maintain schema consistency

## Alternative: Feature Branch Approach

```bash
# Create separate branches for each Team component
git checkout -b feat/team-models
git checkout -b feat/team-repositories  
git checkout -b feat/team-contracts
git checkout -b feat/team-service
git checkout -b feat/team-controller
git checkout -b feat/team-tests
```

This approach allows for independent development and review of each Team component before merging to main.
