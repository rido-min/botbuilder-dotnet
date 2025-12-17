# GitHub Copilot Instructions for Bot Framework .NET SDK

## Repository Overview
This repository contains the .NET version of the Microsoft Bot Framework SDK, a comprehensive framework for building enterprise-grade conversational AI experiences. The SDK is written in C# and targets .NET 6.x.

## Project Structure
- `libraries/` - Core SDK libraries and packages
- `tests/` - Unit and integration tests
- `FunctionalTests/` - Functional test suites
- `build/` - Build scripts and configuration
- `doc/` - Documentation
- `.github/` - GitHub configuration and workflows

## Build Requirements
- **.NET SDK**: Version 6.x (see `global.json`)
- **IDE**: Visual Studio 2019 or later
- **Solution File**: `Microsoft.Bot.Builder.sln`
- **Package Output**: Built packages go to `outputPackages/` directory

### Building the Project
```bash
# Open in Visual Studio and build
# OR use command line:
dotnet build Microsoft.Bot.Builder.sln
```

### Running Tests
- Open the solution in Visual Studio
- Use Test Explorer to run tests
- Code coverage settings are in `CodeCoverage.runsettings`

## Coding Standards
- **Language Version**: Latest C# features enabled via `LangVersion` in `Directory.Build.props`
- **Code Analysis**: StyleCop.Analyzers and Microsoft.CodeAnalysis.NetAnalyzers are enforced
- **Ruleset**: See `BotBuilder-DotNet.ruleset` for specific rules
- **Key Rules**:
  - Use ordinal string comparison (CA1309 - Error)
  - StyleCop rules are enforced as errors (SA* rules)
  - Single line comments don't require space (SA1005 - None)
  - Mark members as static is ignored (CA1822 - None)

## Package Management
- Uses Microsoft.Build.CentralPackageVersions for centralized package version management
- NuGet packages follow Microsoft's compliance rules
- All packages are signed in Release builds
- License: MIT

## Contributing Guidelines
- **Legal**: Contributors must sign a CLA (Contributor License Agreement) for changes >15 lines
- **Issue Tracking**: All contributions must have an approved GitHub issue
- **Pull Requests**: Must include a link to the issue being fixed
- **Code of Conduct**: Follows Microsoft Open Source Code of Conduct

## Testing
- Maintain existing test patterns
- Tests should be comprehensive but not redundant
- Run relevant tests before submitting changes
- Ensure code coverage requirements are met

## Security
- Report security issues privately to secure@microsoft.com
- Never commit secrets or sensitive data
- Follow secure coding practices
- All release builds are delay-signed with Microsoft's key

## Dependencies
- When updating dependencies, consider the Azure Artifacts daily feed
- Package versions are managed centrally
- Test thoroughly when updating major dependencies

## Common Tasks
### Adding a New Library
1. Create project in appropriate `libraries/` subdirectory
2. Follow naming convention: `Microsoft.Bot.Builder.*`
3. Add to `Microsoft.Bot.Builder.sln`
4. Configure package metadata in `.csproj`
5. Add appropriate tests

### Bug Fixes
1. Ensure there's a GitHub issue
2. Make minimal changes to fix the issue
3. Add or update tests to cover the fix
4. Run full test suite before submitting
5. Link PR to the issue

### New Features
1. Feature must have approved GitHub issue
2. Follow existing architectural patterns
3. Add comprehensive tests
4. Update documentation if needed
5. Consider backward compatibility

## Integration Points
- **Azure Bot Service**: Primary deployment target
- **Application Insights**: Telemetry integration
- **Azure Storage**: Blobs and Queues adapters
- **LUIS**: Language understanding integration
- **QnA Maker**: Question/answer integration

## Avoided Patterns
- Don't use literal strings in localized contexts (CA1303 disabled but be mindful)
- Avoid problematic synchronous waits when possible
- Don't use `ToLower()` for comparison, use ordinal string comparison
- Avoid URI parameters as strings when appropriate

## Support Channels
- GitHub Issues: Bug reports and feature requests
- Stack Overflow: Tag with `botframework`
- Gitter: Real-time community chat
- Azure Support: For Azure Bot Service issues
