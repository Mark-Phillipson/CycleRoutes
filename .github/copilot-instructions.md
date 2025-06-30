# CycleRoutes Project Guide

## Core Commands

### Build & Run
- **Build entire solution**: 
```pwsh
dotnet build CycleRoutes.slnx
```
- **Run Web application**: 
```pwsh
dotnet run --project CycleRoutes/CycleRoutes.Web
dotnet watch run --project CycleRoutes/CycleRoutes.Web
```

- **Run MAUI app**: 
```pwsh
dotnet run --project CycleRoutes/CycleRoutes --framework net9.0-windows10.0.19041.0
```

- **Build specific project**: 
```pwsh
dotnet build CycleRoutes/{ProjectName}
```
- **Clean solution**: 
```pwsh
dotnet clean CycleRoutes.slnx
```
- **Restore packages**: 
```pwsh
dotnet restore CycleRoutes.slnx
```

### Development
- **Web dev server**: Runs on https://localhost:7148 (HTTPS) or http://localhost:5178 (HTTP)
- **Hot reload**: Enabled by default in development mode
- **Debug MAUI**: Use "Windows Machine" profile for native debugging

## High-Level Architecture

### Project Structure
- **CycleRoutes.Shared**: Core Blazor components and shared business logic
  - Razor components, services, layouts
  - Cross-platform UI components
  - Abstract service interfaces (`IFormFactor`)
- **CycleRoutes** (MAUI): Cross-platform mobile/desktop app
  - .NET MAUI with Blazor WebView
  - Targets: Android, iOS, macOS, Windows
  - Platform-specific service implementations
- **CycleRoutes.Web**: Server-side Blazor web application
  - Interactive Server + WebAssembly render modes
  - Static file hosting, HTTPS redirection
- **CycleRoutes.Web.Client**: Blazor WebAssembly client
  - Client-side execution in browser
  - Web-specific service implementations

### Technology Stack
- **.NET 9.0**: Target framework
- **Blazor**: UI framework (Server, WebAssembly, Hybrid)
- **MAUI**: Cross-platform app framework
- **Bootstrap**: CSS framework (via wwwroot/bootstrap/)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

### Key Services
- **IFormFactor**: Abstraction for platform detection
  - `GetFormFactor()`: Returns device form factor
  - `GetPlatform()`: Returns platform identifier
  - Platform-specific implementations in each project

### Configuration
- **Launch profiles**: Defined in `Properties/launchSettings.json`
- **MAUI platforms**: Android 24.0+, iOS 15.0+, Windows 10.0.17763.0+
- **Package management**: Central package references, implicit usings enabled

## Style Rules & Conventions

### C# Coding Standards
- **Nullable reference types**: Enabled across all projects (`<Nullable>enable</Nullable>`)
- **Implicit usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Namespace strategy**: File-scoped namespaces preferred
- **Service registration**: Use appropriate lifetime (Singleton for stateless services)

### Blazor Component Guidelines
- **Page routing**: Use `@page` directive with clean URLs
- **Dependency injection**: Use `@inject` for service dependencies
- **Component lifecycle**: Prefer `@code` blocks over code-behind files
- **CSS isolation**: Use `.razor.css` files for component-specific styles

### Project References
- Shared project referenced by all platform-specific projects
- Web.Client referenced by Web server project
- Avoid circular dependencies between projects

### File Organization
- **Services**: Platform-specific implementations in `/Services/`
- **Components**: Shared UI in `CycleRoutes.Shared`
- **Platform assets**: In respective `/Resources/` directories
- **Static assets**: In `/wwwroot/` directories

## Development Guidelines

### Adding New Features
1. Define interfaces in `CycleRoutes.Shared/Services/`
2. Implement platform-specific versions in each project
3. Register services in respective `Program.cs` or `MauiProgram.cs`
4. Create shared Blazor components in `CycleRoutes.Shared`

### Platform Targeting
- Use conditional compilation for platform-specific code
- Test across all target platforms (Web, Android, iOS, Windows, macOS)
- Respect minimum OS versions defined in project files

### Performance Considerations
- Blazor WebAssembly: Minimize payload size, use lazy loading
- MAUI: Optimize startup time, manage memory for mobile devices
- Server-side: Consider SignalR connection limits and state management

### Debugging
- Web: Use browser dev tools and Blazor debugging
- MAUI: Platform-specific debugging tools (Android Studio, Xcode)
- Enable `BlazorWebViewDeveloperTools` in debug builds only
