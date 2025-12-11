using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models;
using MauiHybridApp.Models.PerformanceEvaluation;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class PerformanceEvaluationViewModel : BaseViewModel
{
    private readonly IPerformanceEvaluationDataService _peService;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<PEListDto> _evaluations;
    private PEListDto? _selectedEvaluation;

    public PerformanceEvaluationViewModel(
        IPerformanceEvaluationDataService peService,
        NavigationManager navigationManager)
    {
        _peService = peService ?? throw new ArgumentNullException(nameof(peService));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _evaluations = new ObservableCollection<PEListDto>();
        
        ViewDetailCommand = new AsyncRelayCommand<PEListDto>(ViewDetailAsync);
        CreateNewCommand = new RelayCommand(CreateNew);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public ObservableCollection<PEListDto> Evaluations
    {
        get => _evaluations;
        private set => SetProperty(ref _evaluations, value);
    }

    public PEListDto? SelectedEvaluation
    {
        get => _selectedEvaluation;
        private set => SetProperty(ref _selectedEvaluation, value);
    }

    public bool HasEvaluations => Evaluations.Any();
    public bool ShowEmptyState => !IsBusy && !HasEvaluations;

    #endregion

    #region Commands

    public ICommand ViewDetailCommand { get; }
    public ICommand CreateNewCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(LoadEvaluationsAsync, "Loading evaluations...");
    }

    private async Task LoadEvaluationsAsync()
    {
        try
        {
            var result = await _peService.GetListAsync();
            Evaluations = new ObservableCollection<PEListDto>(result);
            ClearError();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load performance evaluations.");
        }
    }

    private async Task ViewDetailAsync(PEListDto? evaluation)
    {
        if (evaluation == null) return;
        
        // Navigate to detail page
        _navigationManager.NavigateTo($"/performance-evaluation/form/{evaluation.RecordId}");
    }

    private void CreateNew()
    {
        // Navigate to new form
        _navigationManager.NavigateTo("/performance-evaluation/form/new");
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    #endregion
}
