using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class IndividualObjectiveFormViewModel : BaseViewModel
{
    private readonly IIndividualObjectiveItemDataService _service;
    private readonly NavigationManager _navigationManager;
    
    private IndividualObjectiveItemHolder _formHolder;

    public IndividualObjectiveFormViewModel(
        IIndividualObjectiveItemDataService service,
        NavigationManager navigationManager)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        
        _formHolder = new IndividualObjectiveItemHolder();
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        CancelRequestCommand = new AsyncRelayCommand(CancelRequestAsync);
        GoBackCommand = new RelayCommand(GoBack);
    }

    #region Properties

    public IndividualObjectiveItemHolder FormHolder
    {
        get => _formHolder;
        set => SetProperty(ref _formHolder, value);
    }

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand SubmitCommand { get; }
    public ICommand CancelRequestCommand { get; }
    public ICommand GoBackCommand { get; }

    #endregion

    #region Methods

    public async Task InitializeAsync(long id)
    {
        await ExecuteBusyAsync(async () =>
        {
            FormHolder = await _service.InitForm(id);
        }, "Loading form...");
    }

    private async Task SaveAsync()
    {
        await SubmitInternalAsync(true);
    }

    private async Task SubmitAsync()
    {
        await SubmitInternalAsync(false);
    }

    private async Task SubmitInternalAsync(bool isSaveOnly)
    {
        try
        {
            FormHolder.IsSaveOnly = isSaveOnly;
            // Validate logic here if needed
            
            await ExecuteBusyAsync(async () =>
            {
                FormHolder = await _service.SubmitRequest(FormHolder);
            }, isSaveOnly ? "Saving..." : "Submitting...");

            if (FormHolder.Success)
            {
                GoBack();
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Unable to {(isSaveOnly ? "save" : "submit")} request.");
        }
    }

    private async Task CancelRequestAsync()
    {
        try
        {
            FormHolder.ActionTypeId = 2; // Cancel
            FormHolder.Msg = "Cancelled by user"; // Should prompt user for reason

            await ExecuteBusyAsync(async () =>
            {
                FormHolder = await _service.CancelRequest(FormHolder);
            }, "Cancelling request...");

            if (FormHolder.Success)
            {
                GoBack();
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, "Unable to cancel request.");
        }
    }

    public async Task AddOrUpdateObjective(ObjectiveDetailDto item)
    {
        var exists = item.PODetailId == 0
            ? FormHolder.ObjectivesToSave.FirstOrDefault(x => x.TempRowId == item.TempRowId)
            : FormHolder.ObjectivesToSave.FirstOrDefault(x => x.PODetailId == item.PODetailId);

        if (exists != null)
        {
            FormHolder.ObjectivesToSave.Remove(exists);
        }

        if (!item.IsDelete)
        {
            FormHolder.ObjectivesToSave.Add(item);
            FormHolder.AddedObjectiveCount++;
        }

        var list = new ObservableCollection<ObjectiveDetailDto>(
                FormHolder.ObjectivesToSave.Where(x => !x.IsDelete).ToList()
            );

        var groupings = await _service.GroupObjectives(list);
        FormHolder.Objectives = groupings.Objectives;
        FormHolder.ObjectivesLimited = groupings.ObjectivesLimited;
        FormHolder.IsExceeded = groupings.IsExceeded;
    }

    private void GoBack()
    {
        _navigationManager.NavigateTo("/individual-objectives");
    }

    #endregion
}
