using MauiHybridApp.Models;
using MauiHybridApp.Models.Attendance;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.Employee;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Models.PerformanceEvaluation;

using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    // This file previously contained multiple DataService implementations.
    // They have been refactored into separate files for better maintainability.
    // 
    // Extracted Services:
    // - MainPageDataService.cs
    // - DashboardDataService.cs
    // - LeaveDataService.cs
    // - OvertimeDataService.cs
    // - ExpenseDataService.cs
    // - OfficialBusinessDataService.cs
    // - TimeEntryDataService.cs
    // - ApprovalDataService.cs
    // - AttendanceDataService.cs
    // - UserDataService.cs
    // - PayrollDataService.cs
    // - PerformanceDataService.cs
    // - EmployeeRelationsDataService.cs
    // - ProfileDataService.cs
    // - NotificationDataService.cs
    // - CommonDataService.cs
    // - SignalRDataService.cs (Moved to Services/SignalR namespace)
    // - PerformanceEvaluationDataService.cs
    // - IndividualObjectivesDataService.cs
    // - IndividualObjectiveItemDataService.cs
    // - SurveyDataService.cs
    // - ScheduleDataService.cs
    // - FinancialDataService.cs
    // - TravelDataService.cs
    // - DocumentDataService.cs
    // - UndertimeDataService.cs
    // - SuggestionDataService.cs
    // - SpecialWorkScheduleDataService.cs
}
