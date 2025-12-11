using MauiHybridApp.Models;
using MauiHybridApp.Models.IndividualObjectives;
using MauiHybridApp.Utils;
using MauiHybridApp.Services.Data;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class IndividualObjectiveItemDataService : IIndividualObjectiveItemDataService
    {
        private readonly IGenericRepository _repository;

        public IndividualObjectiveItemDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public async Task<IndividualObjectiveItemHolder> InitForm(long id)
        {
            var holder = new IndividualObjectiveItemHolder();
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                
                if (id > 0)
                {
                    var url = $"{ApiEndpoints.IndividualObjectives}/{id}";
                    var response = await _repository.GetAsync<PerformanceObjectiveResponse>(url);

                    if (response != null && response.IsSuccess)
                    {
                        var helpers = await InitObjectiveDetailForm(id, (short)response.PerformanceObjectiveHeader.EffectiveYear.GetValueOrDefault());
                        
                        holder.EffectiveYear.Value = response.PerformanceObjectiveHeader.EffectiveYear.ToString();
                        
                        var list = response.PerformanceObjectiveHeader.PerformanceObjectiveDetail;
                        
                        if (list != null && list.Count > 0)
                        {
                            var items = new ObservableCollection<ObjectiveDetailDto>();
                            foreach (var x in list)
                            {
                                 items.Add(new ObjectiveDetailDto()
                                    {
                                        BaseLine = x.BaseLine,
                                        CustomKPI = x.CustomKPI,
                                        GoalId = x.OrganizationGoalId.GetValueOrDefault(),
                                        KPIId = x.KeyPerformanceIndicatorId.GetValueOrDefault(),
                                        MeasureId = x.Measurement.GetValueOrDefault(),
                                        Objectives = x.Objectives,
                                        ObjectiveDescription = x.ObjectiveDescription,
                                        PODetailId = x.PerformanceObjectiveDetailId,
                                        POHeaderId = x.PerformanceObjectiveHeaderId.GetValueOrDefault(),
                                        Target = (x.TargetGoal > 0 ? x.TargetGoal.GetValueOrDefault(0).ToString("n2") : ""),
                                        UnitOfMeasure = x.UnitOfMeasure,
                                        ObjectiveHeader = x.ParentObjective,
                                        ObjectiveDetail = x.ObjectiveName,
                                        ParentObjective = x.ParentObjective ?? "No Organization Goal",
                                        StatusId = (long)response.PerformanceObjectiveHeader.StatusId,
                                        RateScaleDto = new ObservableCollection<RateScaleDto>(
                                            x.POCustomCriteria.Select(p => new RateScaleDto()
                                            {
                                                Criteria = p.Criteria,
                                                CriteriaId = p.CriteriaId,
                                                Max = p.Max.GetValueOrDefault(0),
                                                Min = p.Min.GetValueOrDefault(0),
                                                Rating = p.TargetScore.GetValueOrDefault(0),
                                                TempId = p.CriteriaId,
                                            })
                                        ),
                                        IsRetrievedFromTemplate = x.IsRetrievedFromTemplate.GetValueOrDefault(),
                                        RetrievedCompKPIId = x.RetrievedCompKPIId.GetValueOrDefault(),
                                        RetrievedPATemplateId = x.RetrievedPATemplateId.GetValueOrDefault(),
                                        RetrievedType = (short)x.RetrievedType.GetValueOrDefault(),
                                        Weight = x.Weight.GetValueOrDefault(0).ToString("N0"),
                                        ActualWeightVal = x.Weight.GetValueOrDefault(0).ToString("N0"),
                                    });
                            }
                            
                            holder.ObjectivesToSave = items;
                            var groupings = await GroupObjectives(items);
                            holder.Objectives = groupings.Objectives;
                            holder.ObjectivesLimited = groupings.ObjectivesLimited;
                            holder.IsExceeded = groupings.IsExceeded;
                            holder.AddedObjectiveCount = holder.Objectives.Count;
                        }
                    }
                }
                else
                {
                    holder.EffectiveYear.Value = DateTime.Now.Year.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitForm Error: {ex.Message}");
            }
            return holder;
        }

        public async Task<IndividualObjectiveItemHolder> SubmitRequest(IndividualObjectiveItemHolder holder)
        {
            try 
            {
                // var url = ApiEndpoints.IndividualObjectives;
                // Construct payload logic would go here
                // For now, returning holder as placeholder
                await Task.CompletedTask;
                return holder;
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"SubmitRequest Error: {ex.Message}");
                 throw;
            }
        }

        public async Task<ObjectiveDetailHolder> InitObjectiveDetailForm(long id, short effectiveYear)
        {
            var holder = new ObjectiveDetailHolder();
            try
            {
                 var profileIdStr = await SecureStorage.GetAsync("profile_id");
                 long.TryParse(profileIdStr, out long pid);
                 
                 var url = $"{ApiEndpoints.IndividualObjectives}/initialize-objectives?ProfileId={pid}&EffectiveYear={effectiveYear}&Id={id}";
                 var response = await _repository.GetAsync<IndividualObjectiveInitResponse>(url);
                 
                 if (response != null)
                 {
                     // Map response to holder
                     holder.EffectiveYear = effectiveYear.ToString();
                     // ... mapping logic
                 }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitObjectiveDetailForm Error: {ex.Message}");
            }
            return holder;
        }

        public async Task<ObjectiveDetailHolder> SetValueObjectiveDetailForm(ObjectiveDetailDto item, ObjectiveDetailHolder holder)
        {
            // Implement SetValue logic
            return await Task.FromResult(holder);
        }

        public async Task<KPISelectionResponse> RetrieveKPICriteria(long id, ObservableCollection<RateScaleDto> list)
        {
            try
            {
                var url = $"{ApiEndpoints.IndividualObjectives}/get-kpi-criteria/{id}";
                var response = await _repository.GetAsync<KPISelectionResponseApi>(url);
                
                var result = new KPISelectionResponse();
                if (response != null)
                {
                    result.KPIObjective = response.KPIObjective;
                    result.RateScales = new ObservableCollection<RateScaleDto>(
                        response.KPICriterias.Select(p => new RateScaleDto
                        {
                             CriteriaId = p.CriteriaId,
                             Max = p.txtMax,
                             Min = p.txtMin,
                             Rating = p.txtScore,
                             Criteria = p.txtCriteria
                        }));
                }
                return result;
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"RetrieveKPICriteria Error: {ex.Message}");
                 return new KPISelectionResponse();
            }
        }

        public async Task<ObjectiveGroupingResponse> GroupObjectives(ObservableCollection<ObjectiveDetailDto> items)
        {
            var response = new ObjectiveGroupingResponse();
             var grouped = items
                       .GroupBy(g => g.ParentObjective)
                       .Select(p => new MainObjectiveDto()
                       {
                           Header = p.Key,
                           IsOpened = false,
                           ObjectiveDetailHeaderDto = new ObservableCollection<ObjectiveDetailHeaderDto>(
                                    p.GroupBy(g2 => g2.ObjectiveDetail)
                                    .Select(p2 => new ObjectiveDetailHeaderDto()
                                    {
                                        ObjectiveHeader = p2.Key,
                                        ObjectiveDetailDto = new ObservableCollection<ObjectiveDetailDto>(p2.ToList())
                                    })
                               )
                       }).ToList();
                       
            response.Objectives = new ObservableCollection<MainObjectiveDto>(grouped);
            response.ObjectivesLimited = response.Objectives; 
            return await Task.FromResult(response);
        }

        public async Task<IndividualObjectiveItemHolder> CancelRequest(IndividualObjectiveItemHolder holder)
        {
            // Implement Cancel logic
            return await Task.FromResult(holder);
        }
    }
}
