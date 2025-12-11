using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiHybridApp.Commands;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Services.Data;
using Microsoft.AspNetCore.Components;

namespace MauiHybridApp.ViewModels;

public class ObjectiveDetailViewModel : BaseViewModel
{
    private readonly IIndividualObjectiveItemDataService _service;
    
    private ObjectiveDetailHolder _holder;
    private bool _isEditMode;

    public ObjectiveDetailViewModel(IIndividualObjectiveItemDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _holder = new ObjectiveDetailHolder();
        
        RetrieveKPICommand = new AsyncRelayCommand(RetrieveKPIAsync);
    }

    #region Properties

    public ObjectiveDetailHolder Holder
    {
        get => _holder;
        set => SetProperty(ref _holder, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    #endregion

    #region Commands

    public ICommand RetrieveKPICommand { get; }

    #endregion

    #region Methods

    public async Task InitializeAsync(long id, short effectiveYear, ObjectiveDetailDto? item = null)
    {
        await ExecuteBusyAsync(async () =>
        {
            Holder = await _service.InitObjectiveDetailForm(id, effectiveYear);
            
            if (item != null)
            {
                IsEditMode = true;
                Holder = await _service.SetValueObjectiveDetailForm(item, Holder);
            }
            else
            {
                IsEditMode = false;
            }
        }, "Loading details...");
    }

    private async Task RetrieveKPIAsync()
    {
        if (Holder.KPI.Value > 0)
        {
            await ExecuteBusyAsync(async () =>
            {
                var result = await _service.RetrieveKPICriteria(Holder.KPI.Value, Holder.RateScaleSource);
                Holder.RateScaleSource = result.RateScales;
                // Update other properties if needed
            }, "Retrieving KPI...");
        }
    }

    public ObjectiveDetailDto GetResult()
    {
        // Construct ObjectiveDetailDto from Holder
        // This logic mimics ConsolidateObjectives in Xamarin
        return new ObjectiveDetailDto
        {
            // Map properties
            ObjectiveHeader = Holder.Goal.Value,
            ObjectiveDescription = Holder.GoalDescription.Value,
            KPIId = Holder.KPI.Value,
            CustomKPI = Holder.CustomKPI.Value,
            MeasureId = Holder.Measure.Value,
            Target = Holder.Target.Value,
            UnitOfMeasure = Holder.UnitOfMeasure.Value,
            BaseLine = Holder.Baseline.Value,
            Weight = Holder.Weight.Value,
            // ... map others
            RateScaleDto = Holder.RateScaleSource
        };
    }

    #endregion
}
