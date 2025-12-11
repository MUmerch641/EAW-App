using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Services.Navigation;
using System.Threading.Tasks;

namespace MauiHybridApp.ViewModels;

public partial class PayslipDetailViewModel : BaseViewModel
{
    private readonly IPayrollDataService _dataService;
    private readonly INavigationService _navigationService;

    public PayslipDetailViewModel(
        IPayrollDataService dataService,
        INavigationService navigationService)
    {
        _dataService = dataService;
        _navigationService = navigationService;
    }

    private PayslipDetailModel _detail;
    public PayslipDetailModel Detail
    {
        get => _detail;
        set => SetProperty(ref _detail, value);
    }

    private long _payslipId;

    public override async Task InitializeAsync()
    {
        var parameters = _navigationService.GetAndClearParameters();
        if (parameters != null && parameters.ContainsKey("PayslipId"))
        {
            _payslipId = long.Parse(parameters["PayslipId"].ToString());
        }

        if (_payslipId > 0)
        {
            await ExecuteBusyAsync(LoadDetailAsync, "Loading payslip details...");
        }
    }

    private async Task LoadDetailAsync()
    {
        var result = await _dataService.GetPayslipDetailAsync(_payslipId);
        if (result is PayslipDetailModel model)
        {
            Detail = model;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}
