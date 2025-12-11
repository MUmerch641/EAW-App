using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class IndividualObjectivesViewModel : BaseViewModel
{
    private readonly IIndividualObjectivesDataService _service;
    private readonly NavigationManager _navigationManager;
    
    private ObservableCollection<IndividualObjectivesDto> _objectives;
    private IndividualObjectivesDto? _selectedObjective;

    public IndividualObjectivesViewModel(
        IIndividualObjectivesDataService service,
        NavigationManager navigationManager)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _objectives = new ObservableCollection<IndividualObjectivesDto>();
        
        ViewDetailCommand = new AsyncRelayCommand<IndividualObjectivesDto>(ViewDetailAsync);
        CreateNewCommand = new RelayCommand(CreateNew);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public ObservableCollection<IndividualObjectivesDto> Objectives
    {
        get => _objectives;
        private set => SetProperty(ref _objectives, value);
    }

    public IndividualObjectivesDto? SelectedObjective
    {
        get => _selectedObjective;
        private set => SetProperty(ref _selectedObjective, value);
    }

    public bool HasObjectives => Objectives.Any();
    public bool ShowEmptyState => !IsBusy && !HasObjectives;

    #endregion

    #region Commands

    public ICommand ViewDetailCommand { get; }
    public ICommand CreateNewCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public override async Task InitializeAsync()
    {
        await ExecuteBusyAsync(LoadObjectivesAsync, "Loading objectives...");
    }

    private async Task LoadObjectivesAsync()
    {
        try
        {
            var param = new ListParam
            {
                ListCount = 0,
                Count = 50,
                IsAscending = false,
                KeyWord = "",
                FilterTypes = "",
                StartDate = "",
                EndDate = "",
                Status = ""
            };

            var result = await _service.GetListAsync(new ObservableCollection<IndividualObjectivesDto>(), param);
            
            if (result != null && result.ListData != null)
            {
                Objectives = new ObservableCollection<IndividualObjectivesDto>(result.ListData);
            }
            else
            {
                Objectives = new ObservableCollection<IndividualObjectivesDto>();
            }
            
            ClearError();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to load individual objectives.");
        }
    }

    private async Task ViewDetailAsync(IndividualObjectivesDto? objective)
    {
        if (objective == null) return;
        
        // Navigate to detail page
        _navigationManager.NavigateTo($"/individual-objectives/form/{objective.IndividualOjbectiveId}");
    }

    private void CreateNew()
    {
        // Navigate to new form
        _navigationManager.NavigateTo("/individual-objectives/form/0");
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/dashboard");
    }

    #endregion
}
