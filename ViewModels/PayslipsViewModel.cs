using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

/// <summary>
/// ViewModel for Payslips page
/// </summary>
public class PayslipsViewModel : BaseViewModel
{
    private readonly IPayrollDataService _payrollService;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<MyPayslipListModel> _payslips;
    private PayslipDetailModel? _selectedPayslipDetail;
    private MyPayslipListModel? _selectedPayslip;
    private bool _isDetailModalOpen;

    public PayslipsViewModel(
        IPayrollDataService payrollService,
        NavigationManager navigationManager)
    {
        _payrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _payslips = new ObservableCollection<MyPayslipListModel>();
        
        ViewDetailCommand = new AsyncRelayCommand<MyPayslipListModel>(ViewPayslipDetailAsync);
        CloseModalCommand = new RelayCommand(CloseModal);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public ObservableCollection<MyPayslipListModel> Payslips
    {
        get => _payslips;
        private set => SetProperty(ref _payslips, value);
    }

    public MyPayslipListModel? SelectedPayslip
    {
        get => _selectedPayslip;
        private set => SetProperty(ref _selectedPayslip, value);
    }

    public PayslipDetailModel? SelectedPayslipDetail
    {
        get => _selectedPayslipDetail;
        private set => SetProperty(ref _selectedPayslipDetail, value);
    }

    public bool IsDetailModalOpen
    {
        get => _isDetailModalOpen;
        private set => SetProperty(ref _isDetailModalOpen, value);
    }

    public bool HasPayslips => Payslips.Any();
    public bool ShowEmptyState => !IsBusy && !HasPayslips;

    #endregion

    #region Commands

    public ICommand ViewDetailCommand { get; }
    public ICommand CloseModalCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        await ExecuteBusyAsync(LoadPayslipsAsync, "Loading payslips...");
    }

    private async Task LoadPayslipsAsync()
    {
        try
        {
            var result = await _payrollService.GetPayslipsAsync();
            var payslipList = result.Cast<MyPayslipListModel>().ToList();
            Payslips = new ObservableCollection<MyPayslipListModel>(payslipList);
            ClearError();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load payslips. Please try again.");
        }
    }

    private async Task ViewPayslipDetailAsync(MyPayslipListModel? payslip)
    {
        if (payslip == null) return;

        try
        {
            SelectedPayslip = payslip;
            IsDetailModalOpen = true;
            
            // Load detail
            var result = await _payrollService.GetPayslipDetailAsync(payslip.PaysheetHeaderDetailId);
            SelectedPayslipDetail = result as PayslipDetailModel;
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load payslip details.");
            CloseModal();
        }
    }

    private void CloseModal()
    {
        IsDetailModalOpen = false;
        SelectedPayslip = null;
        SelectedPayslipDetail = null;
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    #endregion
}
