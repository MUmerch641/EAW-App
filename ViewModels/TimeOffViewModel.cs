using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class TimeOffViewModel : OfficialBusinessViewModel
{
    public TimeOffViewModel(
        IOfficialBusinessDataService obService,
        NavigationManager navigationManager) 
        : base(obService, navigationManager)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        SetTransactionType(4); // 4 = Time Off (from Xamarin Constants)
    }
}
