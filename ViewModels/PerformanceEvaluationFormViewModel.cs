using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.PerformanceEvaluation;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class PerformanceEvaluationFormViewModel : BaseViewModel
{
    private readonly IPerformanceEvaluationDataService _peService;
    private readonly NavigationManager _navigationManager;
    
    private PEFormHolder _formHolder;
    private bool _isEditMode;

    public PerformanceEvaluationFormViewModel(
        IPerformanceEvaluationDataService peService,
        NavigationManager navigationManager)
    {
        _peService = peService ?? throw new ArgumentNullException(nameof(peService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _formHolder = new PEFormHolder();
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public PEFormHolder FormHolder
    {
        get => _formHolder;
        private set => SetProperty(ref _formHolder, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand SubmitCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public async Task InitializeAsync(long id)
    {
        if (id == 0)
        {
            // New form logic if applicable, or just empty
            IsEditMode = true;
        }
        else
        {
            await ExecuteBusyAsync(async () => await LoadFormAsync(id), "Loading form...");
        }
    }

    private async Task LoadFormAsync(long id)
    {
        try
        {
            var result = await _peService.InitFormAsync(id);
            if (result != null)
            {
                FormHolder = result;
                IsEditMode = FormHolder.CanSave;
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load performance evaluation form.");
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;
            var result = await _peService.SavePODetailsAsync(FormHolder);
            if (result != null)
            {
                FormHolder = result;
                // Show success message
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to save form.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SubmitAsync()
    {
        // Implementation for submit
        await SaveAsync();
        // Additional submit logic
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/performance-evaluation");
    }

    #endregion
}
