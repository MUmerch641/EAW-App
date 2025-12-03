# Project Structure Standards

## 📁 **Folder Organization**

### **Root Level**
```
EAT-App/
├── App.xaml                    # Application definition
├── App.xaml.cs                 # Application code-behind
├── MainPage.xaml               # Main page XAML
├── MainPage.xaml.cs            # Main page code-behind
├── MauiProgram.cs              # DI container & app configuration
├── MauiHybridApp.csproj        # Project file
├── MauiHybridApp.sln           # Solution file
├── README.md                   # Project documentation
└── my-release-key.keystore     # Android signing key
```

---

## 🏗️ **Architecture Folders**

### **1. ViewModels/** (Business Logic)
**Purpose:** MVVM ViewModels containing business logic and state management

**Naming Convention:** `[PageName]ViewModel.cs`

**Structure:**
```
ViewModels/
├── BaseViewModel.cs              # Base class for all ViewModels
├── DashboardViewModel.cs         # Dashboard page logic
├── ProfileViewModel.cs           # Profile page logic
├── LeaveViewModel.cs             # Leave page logic
├── TimeEntryViewModel.cs         # Time entry page logic
├── OvertimeViewModel.cs          # Overtime page logic
├── OfficialBusinessViewModel.cs  # Official business page logic
├── PayslipsViewModel.cs          # Payslips page logic
├── AttendanceViewModel.cs        # Attendance page logic
└── ApprovalsViewModel.cs         # Approvals page logic
```

**Rules:**
- ✅ Each Razor page MUST have a corresponding ViewModel
- ✅ Inherit from `BaseViewModel`
- ✅ Use `INotifyPropertyChanged` for data binding
- ✅ No direct UI code (no `StateHasChanged()`)
- ✅ Register in `MauiProgram.cs` DI container
- ❌ NO code-behind `.razor.cs` files

**Example Template:**
```csharp
using MauiHybridApp.ViewModels;
using MauiHybridApp.Commands;

namespace MauiHybridApp.ViewModels
{
    public class [PageName]ViewModel : BaseViewModel
    {
        private readonly I[Service]DataService _service;
        
        public [PageName]ViewModel(I[Service]DataService service)
        {
            _service = service;
            Initialize();
        }
        
        // Properties with INotifyPropertyChanged
        private string _title = "[Page Name]";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        // Commands
        public RelayCommand LoadDataCommand => new RelayCommand(async () => await LoadDataAsync());
        
        // Methods
        private void Initialize() { }
        
        public override async Task InitializeAsync()
        {
            await LoadDataAsync();
        }
        
        private async Task LoadDataAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                // Your logic here
            });
        }
    }
}
```

---

### **2. Commands/** (ICommand Implementations)
**Purpose:** Reusable command pattern implementations

**Structure:**
```
Commands/
├── RelayCommand.cs           # Synchronous command
└── AsyncRelayCommand.cs      # Asynchronous command
```

**Rules:**
- ✅ Use `RelayCommand` for sync operations
- ✅ Use `AsyncRelayCommand` for async operations
- ✅ Import: `using MauiHybridApp.Commands;`
- ❌ Don't create custom commands unless absolutely needed

---

### **3. Components/** (UI Components)
**Purpose:** Blazor Razor components and pages

**Structure:**
```
Components/
├── _Imports.razor              # Global imports
├── Routes.razor                # Route definitions
├── Layout/                     # Layout components
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── Pages/                      # Page components (NO .cs files)
│   ├── Dashboard.razor
│   ├── Profile.razor
│   ├── Leave.razor
│   ├── TimeEntry.razor
│   ├── TimeEntry.razor.css     # Component-specific CSS (optional)
│   ├── Overtime.razor
│   ├── OfficialBusiness.razor
│   ├── Payslips.razor
│   ├── Attendance.razor
│   ├── Approvals.razor
│   ├── Login.razor
│   ├── Register.razor
│   └── ForgotPassword.razor
└── Shared/                     # Reusable components
    ├── PageHeader.razor
    ├── StatusCard.razor
    ├── ActionButton.razor
    ├── LoadingIndicator.razor
    ├── FormGroup.razor
    ├── AlertMessage.razor
    ├── EmptyState.razor
    ├── LoadingSpinner.razor
    └── ErrorBoundary.razor
```

**Rules:**
- ✅ **Pages/** contains route-level pages
- ✅ **Shared/** contains reusable UI components
- ✅ Use `@inject` for ViewModels: `@inject DashboardViewModel ViewModel`
- ✅ Component-specific CSS allowed (e.g., `TimeEntry.razor.css`)
- ❌ **NO** `.razor.cs` code-behind files
- ❌ **NO** `.backup`, `.old`, `.new` files

**Page Template:**
```razor
@page "/dashboard"
@inject DashboardViewModel ViewModel

<PageHeader Title="@ViewModel.Title" />

@if (ViewModel.IsBusy)
{
    <LoadingIndicator />
}
else if (ViewModel.HasError)
{
    <AlertMessage Type="error" Message="@ViewModel.ErrorMessage" />
}
else
{
    <div class="page-content">
        <!-- Your content here -->
    </div>
}

@code {
    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
    }
}
```

---

### **4. Models/** (Data Models)
**Purpose:** Data transfer objects and domain models

**Structure:**
```
Models/
├── UserModel.cs
├── DashboardResponse.cs
├── TimeEntryModels.cs
├── PayslipModels.cs
├── EmployeeProfileResponse.cs
├── Attendance/
│   └── TimeEntryLogModel.cs
├── Dashboard/
│   └── DashboardModel.cs
├── Employee/
│   └── ProfileModel.cs
├── Leave/
│   ├── LeaveRequestModel.cs
│   └── LeaveApiResponse.cs
├── Schedule/
│   ├── OvertimeModel.cs
│   └── OfficialBusinessModel.cs
├── Workflow/
│   ├── MyApprovalListModel.cs
│   └── NotificationModel.cs
├── DataAccess/
│   └── SQLiteModels.cs
└── DataObjects/
    ├── ComboBoxObject.cs
    ├── FileUploadResponse.cs
    ├── MenuItemModel.cs
    └── SelectableListModel.cs
```

**Naming Convention:**
- `[Entity]Model.cs` - For domain models
- `[Entity]Response.cs` - For API responses
- `[Feature]Models.cs` - For feature-specific models

**Rules:**
- ✅ Group related models in subfolders
- ✅ Use clear, descriptive names
- ✅ Keep models simple (POCOs)
- ❌ No business logic in models

---

### **5. Services/** (Business Services)
**Purpose:** Data access, API calls, and business services

**Structure:**
```
Services/
├── IGenericRepository.cs
├── IPreferenceHelper.cs
├── FileUploadService.cs
├── Authentication/
│   └── (auth services)
├── Common/
│   ├── BaseService.cs
│   ├── StateServices.cs
│   └── LoggingService.cs
├── Data/
│   ├── IDataServices.cs
│   ├── DataServiceImplementations.cs
│   └── SQLiteDataService.cs
├── Navigation/
│   └── INavigationService.cs
├── Performance/
│   ├── CacheService.cs
│   └── PerformanceService.cs
├── Platform/
│   ├── ISQLite.cs
│   ├── IFileService.cs
│   ├── IDeviceService.cs
│   └── IAppCenterService.cs
├── SignalR/
│   ├── ISignalRDataService.cs
│   └── SignalRDataService.cs
└── State/
    ├── IStateService.cs
    └── StateService.cs
```

**Naming Convention:**
- `I[Service]Service.cs` - Interface
- `[Service]Service.cs` - Implementation
- Group by responsibility in subfolders

**Rules:**
- ✅ Use interface-based services
- ✅ Register in `MauiProgram.cs` DI
- ✅ Keep services focused (Single Responsibility)
- ✅ Use async/await patterns

---

### **6. wwwroot/** (Static Assets)
**Purpose:** Static files (CSS, JS, images)

**Structure:**
```
wwwroot/
├── index.html
├── css/
│   ├── app.css           # Main app styles
│   ├── global.css        # Global utilities
│   ├── components.css    # Component styles
│   ├── pages.css         # Page-specific styles
│   └── bootstrap/        # Bootstrap files
├── js/
│   └── site.js
└── images/
    └── (image files)
```

**CSS Organization:**
- `global.css` - Variables, utilities, resets
- `components.css` - Shared component styles
- `pages.css` - Page-specific styles
- `app.css` - Main entry point

---

### **7. Utils/** (Utilities)
**Purpose:** Helper classes and constants

**Structure:**
```
Utils/
├── Constants.cs
├── FormSession.cs
└── Keystore/
```

---

### **8. Platforms/** (Platform-Specific Code)
**Purpose:** Platform-specific implementations

**Structure:**
```
Platforms/
├── Android/
│   ├── AndroidManifest.xml
│   ├── MainActivity.cs
│   └── MainApplication.cs
└── iOS/
    ├── AppDelegate.cs
    ├── Info.plist
    └── Program.cs
```

---

## 📋 **File Naming Standards**

### **DO's ✅**
- `DashboardViewModel.cs` - PascalCase for classes
- `IDashboardService.cs` - Interface prefix `I`
- `dashboard.service.ts` - kebab-case for non-C# files
- `TimeEntry.razor` - PascalCase for components
- `TimeEntry.razor.css` - Component-scoped CSS

### **DON'Ts ❌**
- ❌ `Dashboard.razor.cs` - No code-behind files
- ❌ `Dashboard.razor.backup` - No backup files
- ❌ `Dashboard.old.razor` - No old files
- ❌ `Dashboard_new.razor` - No temp files
- ❌ `dashboardViewModel.cs` - Wrong casing

---

## 🔧 **Adding New Features**

### **When Creating a New Page:**

1. **Create ViewModel** (`ViewModels/[PageName]ViewModel.cs`)
```csharp
public class MyNewPageViewModel : BaseViewModel
{
    // Implementation
}
```

2. **Register in DI** (`MauiProgram.cs`)
```csharp
services.AddTransient<MauiHybridApp.ViewModels.MyNewPageViewModel>();
```

3. **Create Razor Page** (`Components/Pages/MyNewPage.razor`)
```razor
@page "/mynewpage"
@inject MyNewPageViewModel ViewModel

<PageHeader Title="@ViewModel.Title" />
<!-- Content -->
```

4. **Add Route** (if needed in `Routes.razor`)

5. **Add Navigation** (if needed in `NavMenu.razor`)

---

### **When Creating a New Service:**

1. **Create Interface** (`Services/[Category]/I[Service]Service.cs`)
```csharp
public interface IMyService
{
    Task<Result> DoSomethingAsync();
}
```

2. **Create Implementation** (`Services/[Category]/[Service]Service.cs`)
```csharp
public class MyService : IMyService
{
    public async Task<Result> DoSomethingAsync() { }
}
```

3. **Register in DI** (`MauiProgram.cs`)
```csharp
services.AddSingleton<IMyService, MyService>();
```

---

### **When Creating a New Reusable Component:**

1. **Create Component** (`Components/Shared/[ComponentName].razor`)
```razor
@* Component markup *@

@code {
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public EventCallback OnClick { get; set; }
}
```

2. **Add Styles** (in `wwwroot/css/components.css`)
```css
.my-component {
    /* Styles */
}
```

3. **Use in Pages:**
```razor
<MyComponent Title="Hello" OnClick="HandleClick" />
```

---

## 🚫 **What NOT to Create**

### **Never Create:**
- ❌ `.razor.cs` code-behind files (use ViewModels instead)
- ❌ `.backup`, `.old`, `.new`, `.tmp` files
- ❌ Duplicate documentation files
- ❌ Test files in production folders
- ❌ Random utility classes without clear purpose

### **Before Creating a File, Ask:**
1. Does this belong in an existing folder?
2. Is there already a similar file I can use?
3. Does this follow the naming convention?
4. Will this be registered properly in DI?

---

## ✅ **Quality Checklist**

Before committing code:
- [ ] No `.razor.cs` code-behind files
- [ ] All ViewModels registered in DI
- [ ] No backup/temp files (`.backup`, `.old`, `.new`)
- [ ] Proper naming conventions followed
- [ ] Files in correct folders
- [ ] No duplicate code
- [ ] Build succeeds with 0 errors
- [ ] Components reused where possible

---

## 📊 **Current Project Statistics**

- **ViewModels:** 9 + 1 base = 10 files
- **Commands:** 2 files
- **Pages:** 11 Razor files (no .cs files)
- **Shared Components:** 9 reusable components
- **Services:** 20+ organized in subfolders
- **Models:** 28+ organized by domain
- **CSS Files:** 4 organized files
- **Total Code Reduction:** 50% (4,545 → 2,267 lines)

---

## 🎯 **Summary**

**Your project follows professional .NET MAUI Blazor MVVM standards:**
- ✅ Clear separation of concerns
- ✅ MVVM architecture properly implemented
- ✅ No code duplication
- ✅ Reusable components
- ✅ Organized file structure
- ✅ Dependency injection throughout

**When adding new features, follow these patterns and your codebase will remain clean and maintainable!**
