# Everything at Work - .NET MAUI Hybrid Application

This is the .NET MAUI Hybrid version of the Everything at Work mobile application, migrated from Xamarin.Forms.

## Project Structure

```
MauiHybridApp/
├── Components/              # Blazor components
│   ├── Layout/             # Layout components (MainLayout, NavMenu)
│   ├── Pages/              # Page components (Login, Dashboard, etc.)
│   ├── Shared/             # Shared/reusable components
│   └── Routes.razor        # Routing configuration
├── Models/                 # Data models and DTOs
├── Services/               # Business logic and data services
│   ├── Authentication/     # Authentication services
│   ├── Common/            # Common/shared services
│   ├── Data/              # Data access services
│   ├── Navigation/        # Navigation service
│   └── Platform/          # Platform-specific services
├── Utils/                  # Utility classes and helpers
├── Platforms/             # Platform-specific code
│   ├── Android/           # Android-specific implementations
│   └── iOS/               # iOS-specific implementations
├── Resources/             # Application resources
│   ├── Fonts/            # Custom fonts
│   ├── Images/           # Images and icons
│   └── Styles/           # XAML styles and colors
├── wwwroot/              # Web assets for Blazor
│   ├── css/              # CSS stylesheets
│   ├── js/               # JavaScript files
│   └── index.html        # Blazor host page
├── App.xaml              # Application XAML
├── App.xaml.cs           # Application code-behind
├── MainPage.xaml         # Main page with BlazorWebView
├── MauiProgram.cs        # Application startup and DI configuration
└── MauiHybridApp.csproj  # Project file
```

## Technology Stack

- **.NET 9.0** - Latest .NET framework
- **.NET MAUI** - Multi-platform app UI framework
- **Blazor Hybrid** - Web UI in native apps
- **SQLite** - Local database
- **SignalR** - Real-time communication
- **Polly** - Resilience and transient-fault-handling
- **Newtonsoft.Json** - JSON serialization
- **AppCenter** - Analytics and crash reporting

## Key Features Implemented (Phase 1)

### ✅ Project Foundation
- [x] .NET MAUI Hybrid project structure
- [x] Dependency injection setup (Microsoft.Extensions.DependencyInjection)
- [x] Blazor component infrastructure
- [x] Platform-specific implementations (Android, iOS)
- [x] Resource files (styles, colors)

### ✅ Core Services
- [x] Generic HTTP repository with Polly retry policies
- [x] Authentication state management
- [x] Navigation service for Blazor
- [x] Platform services (SQLite, File, Device, AppCenter)
- [x] Preference/settings management
- [x] SignalR service for real-time notifications

### ✅ UI Components
- [x] Main layout with navigation menu
- [x] Login page
- [x] Dashboard page
- [x] Responsive design

### ✅ Data Services (Stub Implementations)
- [x] Authentication data service
- [x] Dashboard data service
- [x] Leave data service
- [x] Overtime data service
- [x] Official business data service
- [x] Time entry data service
- [x] Approval data service
- [x] Attendance data service
- [x] Payroll data service
- [x] Performance data service
- [x] Employee relations data service
- [x] Profile data service
- [x] Notification data service

## Building and Running

### Prerequisites
- Visual Studio 2022 (17.8 or later) or Visual Studio Code
- .NET 9.0 SDK
- Android SDK (for Android development)
- Xcode (for iOS development on macOS)

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build for Android
dotnet build -f net9.0-android

# Build for iOS
dotnet build -f net9.0-ios

# Run on Android
dotnet build -f net9.0-android -t:Run

# Run on iOS
dotnet build -f net9.0-ios -t:Run
```

### Visual Studio
1. Open `MauiHybridApp.csproj` in Visual Studio 2022
2. Select target platform (Android or iOS)
3. Press F5 to build and run

## Configuration

### API Endpoint
Update the base URL in `Utils/Constants.cs`:
```csharp
public const string BaseUrl = "https://api.everythingatwork.com/";
```

### AppCenter Keys
Update AppCenter secrets in `Services/Platform/IAppCenterService.cs`:
```csharp
var appCenterSecret = "ios=YOUR_IOS_KEY;android=YOUR_ANDROID_KEY";
```

## Migration Status

### Phase 1: Project Foundation ✅ COMPLETE
- Project structure created
- Core services implemented
- Basic UI components created
- Platform-specific code set up

### Phase 2: Feature Migration (Pending)
- [ ] Complete all XAML to Blazor page conversions
- [ ] Implement all data service methods
- [ ] Migrate all models and DTOs
- [ ] Implement custom handlers for platform-specific features
- [ ] Migrate all business logic

### Phase 3: Testing & Validation (Pending)
- [ ] Unit tests
- [ ] Integration tests
- [ ] UI tests
- [ ] Platform-specific testing

## Known Limitations (Phase 1)

1. **Stub Implementations**: Most data services return placeholder data
2. **Authentication**: Uses mock authentication for demonstration
3. **API Integration**: Not fully connected to backend APIs
4. **Models**: Only core models migrated, many DTOs pending
5. **UI Pages**: Only Login and Dashboard pages created
6. **Custom Renderers**: Not yet migrated to MAUI handlers

## Next Steps

1. **Complete Model Migration**: Copy all remaining models from Xamarin project
2. **Implement Data Services**: Connect services to actual API endpoints
3. **Create Remaining Pages**: Convert all 60+ XAML pages to Blazor components
4. **Platform Handlers**: Migrate custom renderers to MAUI handlers
5. **Testing**: Comprehensive testing on both platforms

## Notes

- The original Xamarin project remains untouched in the parent directory
- This is a completely separate, self-contained project
- All dependencies are .NET 9 compatible
- Blazor Hybrid provides a modern web-based UI approach while maintaining native performance

## Support

For issues or questions about the migration, please refer to:
- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [Blazor Hybrid Documentation](https://docs.microsoft.com/aspnet/core/blazor/hybrid/)

# EAW-App
