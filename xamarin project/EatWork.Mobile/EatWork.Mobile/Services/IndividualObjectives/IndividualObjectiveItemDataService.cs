using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class IndividualObjectiveItemDataService : IIndividualObjectiveItemDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly IGoalDataService goalService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public long TotalListItem { get; set; }

        public IndividualObjectiveItemDataService()
        {
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            workflowDataService_ = AppContainer.Resolve<IWorkflowDataService>();
            goalService_ = AppContainer.Resolve<IGoalDataService>();
            string_ = new StringHelper();
            TotalListItem = 0;
        }

        public async Task<IndividualObjectiveItemHolder> InitForm(long id)
        {
            var holder = new IndividualObjectiveItemHolder();

            try
            {
                var user = PreferenceHelper.UserInfo();
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                holder.EmployeeName.Value = user.EmployeeName;
                holder.CompanyName.Value = user.Company;
                holder.DepartmentName.Value = user.Department;
                holder.Position.Value = user.Position;

                if (id > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.IndividualObjectives}/{id}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.PerformanceObjectiveResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        var helpers = await InitObjectiveDetailForm(id, response.PerformanceObjectiveHeader.EffectiveYear.GetValueOrDefault(0));

                        var kpis = helpers.KPISource;
                        var measures = helpers.MeasureSource;

                        holder.Status = string.Empty;
                        holder.EmployeeName.Value = response.EmployeeInfo.FullNameMiddleInitialOnly;
                        holder.CompanyName.Value = response.EmployeeInfo.Company;
                        holder.DepartmentName.Value = response.EmployeeInfo.Department;
                        holder.Position.Value = response.EmployeeInfo.Position;
                        holder.PreparedDate = response.PerformanceObjectiveHeader.DatePrepared.GetValueOrDefault();
                        holder.EffectiveYear.Value = response.PerformanceObjectiveHeader.EffectiveYear.ToString();
                        holder.MidYearChecked = (response.PerformanceObjectiveHeader.PeriodType == 2);
                        holder.AnnualChecked = (response.PerformanceObjectiveHeader.PeriodType != 2);

                        var list = response.PerformanceObjectiveHeader.PerformanceObjectiveDetail;

                        TotalListItem = list.Count;

                        if (list.Count > 0)
                        {
                            var items = new ObservableCollection<ObjectiveDetailDto>();
                            var index = 0;
                            foreach (var x in list)
                            {
                                index++;

                                var kpi = kpis.Where(kp => kp.DisplayId == x.KeyPerformanceIndicatorId).FirstOrDefault();
                                var measure = measures.Where(ms => ms.Id == x.Measurement.GetValueOrDefault(0)).FirstOrDefault();

                                var kpiName = string.Empty;
                                var measurename = string.Empty;
                                if (x.KeyPerformanceIndicatorId > 0)
                                {
                                    kpiName = (kpi != null ? kpi.DisplayField : "");
                                    measurename = (kpi != null ? kpi.DisplaySource : "");
                                }
                                else
                                {
                                    kpiName = x.CustomKPI;
                                    measurename = (measure != null ? measure.DisplayText : "");
                                }

                                var baselineIsNumber = decimal.TryParse(x.BaseLine, out decimal baseline);
                                var baselineVal = !baselineIsNumber ? x.BaseLine : baseline.ToString("N0");

                                var parentObjective = Constants.NoOrgGoalsConstant;

                                if (x.OrganizationGoalId != 0)
                                {
                                    parentObjective = !string.IsNullOrWhiteSpace(x.ParentObjective) ? x.ParentObjective : Constants.OrgGoalsConstant;
                                }

                                items.Add(new ObjectiveDetailDto()
                                {
                                    BaseLine = baselineVal,
                                    CustomKPI = x.CustomKPI,
                                    GoalId = x.OrganizationGoalId.GetValueOrDefault(),
                                    KPIId = x.KeyPerformanceIndicatorId.GetValueOrDefault(),
                                    KPIName = kpiName,
                                    MeasureId = x.Measurement.GetValueOrDefault(),
                                    Objectives = x.Objectives,
                                    MeasureName = measurename,
                                    ObjectiveDescription = x.ObjectiveDescription,
                                    PODetailId = x.PerformanceObjectiveDetailId,
                                    POHeaderId = x.PerformanceObjectiveHeaderId.GetValueOrDefault(),
                                    Target = (x.TargetGoal > 0 ? x.TargetGoal.GetValueOrDefault(0).ToString("n2") : ""),
                                    UnitOfMeasure = x.UnitOfMeasure,
                                    ObjectiveHeader = x.ParentObjective,
                                    ObjectiveDetail = x.ObjectiveName,
                                    /*ParentObjective = (!string.IsNullOrWhiteSpace(x.ParentObjective) ? x.ParentObjective : Constants.NoOrgGoalsConstant),*/
                                    ParentObjective = parentObjective,
                                    StatusId = (long)response.PerformanceObjectiveHeader.StatusId,
                                    RateScaleDto = new ObservableCollection<RateScaleDto>(
                                        x.POCustomCriteria.Select(p => new RateScaleDto()
                                        {
                                            Criteria = p.Criteria,
                                            CriteriaId = p.CriteriaId,
                                            Max = p.Max.GetValueOrDefault(0),
                                            Min = p.Min.GetValueOrDefault(0),
                                            PODetailId = p.PODetailId.GetValueOrDefault(0),
                                            Rating = p.TargetScore.GetValueOrDefault(0),
                                            TempId = p.PODetailId.GetValueOrDefault(0),
                                        })
                                    ),
                                    IsRetrievedFromTemplate = x.IsRetrievedFromTemplate.GetValueOrDefault(),
                                    RetrievedCompKPIId = x.RetrievedCompKPIId.GetValueOrDefault(),
                                    RetrievedPATemplateId = x.RetrievedPATemplateId.GetValueOrDefault(),
                                    RetrievedType = x.RetrievedType.GetValueOrDefault(),
                                    ShowLine = (index != list.Count),
                                    /*ShowLine = (index > 0),*/
                                    Weight = x.Weight.GetValueOrDefault(0).ToString("N0"),
                                    ActualWeightVal = x.Weight.GetValueOrDefault(0).ToString("N0"),
                                    /*TargetGoalSetup = $"{x.TargetGoal.GetValueOrDefault(0).ToString("n2")} {x.UnitOfMeasure}",*/
                                    TargetGoalSetup = $"{(measurename == "Amount" ? (x.UnitOfMeasure + " " + x.TargetGoal.GetValueOrDefault(0).ToString("n2")) : (x.TargetGoal.GetValueOrDefault(0).ToString("n2") + " " + x.UnitOfMeasure))}"
                                });
                            }

                            holder.ObjectivesToSave = items;

                            //DISPLAY OBJECTIVES AT ACCORDIONS
                            var groupings = await this.GroupObjectives(items);
                            holder.Objectives = groupings.Objectives;
                            holder.ObjectivesLimited = groupings.ObjectivesLimited;
                            holder.IsExceeded = groupings.IsExceeded;

                            holder.AddedObjectiveCount = holder.Objectives.Count;
                        }

                        holder.IsEditable = false;
                        holder.IsEnabled = false;
                        holder.ShowCancelButton = false;
                        holder.ShowButton = false;

                        if (response.PerformanceObjectiveHeader.StatusId == RequestStatusValue.Draft
                            || response.PerformanceObjectiveHeader.StatusId == 0)
                        {
                            holder.IsEnabled = true;
                            holder.IsEditable = true;
                            holder.ShowButton = true;
                        }

                        if (response.PerformanceObjectiveHeader.StatusId == RequestStatusValue.ForReview
                            || response.PerformanceObjectiveHeader.StatusId == RequestStatusValue.ForApproval)
                        {
                            holder.ShowCancelButton = true;
                            holder.IsEnabled = false;
                            holder.IsEditable = false;
                            holder.ShowButton = true;
                        }

                        holder.Model = new R.Models.PerformanceObjectiveHeader()
                        {
                            DatePrepared = response.PerformanceObjectiveHeader.DatePrepared,
                            StatusId = response.PerformanceObjectiveHeader.StatusId,
                            EffectiveYear = response.PerformanceObjectiveHeader.EffectiveYear,
                            PerformanceObjectiveDetail = list,
                            PerformanceObjectiveHeaderId = response.PerformanceObjectiveHeader.PerformanceObjectiveHeaderId,
                            PeriodType = response.PerformanceObjectiveHeader.PeriodType,
                            ProfileId = response.PerformanceObjectiveHeader.ProfileId,
                            SourceId = response.PerformanceObjectiveHeader.SourceId,
                        };
                    }
                    else
                    {
                        throw new Exception(response.ErrorMessage);
                    }
                }
                else
                {
                    holder.EffectiveYear.Value = DateTime.UtcNow.Year.ToString();
                    holder.IsEnabled = true;

                    var data = await this.RetrieveStandardObjectives(holder.EffectiveYear.Value);
                    holder.Objectives = data.Objectives;
                    holder.ObjectivesLimited = data.ObjectivesLimited;
                    holder.IsExceeded = data.IsExceeded;
                    holder.ObjectivesToSave = data.ObjectivesToSave;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return holder;
        }

        public async Task<IndividualObjectiveItemHolder> SubmitRequest(IndividualObjectiveItemHolder holder)
        {
            if (holder.IsValid())
            {
                var message = (holder.IsSaveOnly ? Messages.Save : Messages.Submit);
                if (await dialogService_.ConfirmDialogAsync(message))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);
                            UriBuilder builder;
                            R.Requests.SubmitPerformanceObjectiveRequest param;
                            SetupModel(holder, url, out builder, out param);

                            if (holder.Model.PerformanceObjectiveHeaderId == 0)
                            {
                                var response = await genericRepository_.PostAsync<R.Requests.SubmitPerformanceObjectiveRequest, R.Responses.BaseResponse<R.Models.PerformanceObjectiveHeader>>(builder.ToString(), param);

                                if (response.Model.PerformanceObjectiveHeaderId > 0)
                                {
                                    holder.Success = true;
                                    FormSession.IsSubmitted = true;
                                }
                            }
                            else
                            {
                                var response = await genericRepository_.PutAsync<R.Requests.SubmitPerformanceObjectiveRequest, R.Responses.BaseResponse>(
                                    builder.ToString() + "/" + param.Data.PerformanceObjectiveHeaderId, param);

                                if (response.IsSuccess)
                                {
                                    holder.Success = true;
                                    FormSession.IsSubmitted = true;
                                }
                            }
                        }
                        catch (HttpRequestExceptionEx ex)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }

            if (holder.AddedObjectiveCount == 0)
                throw new Exception(Messages.NoObjectivesFound);

            return holder;
        }

        public async Task<ObjectiveDetailHolder> InitObjectiveDetailForm(long id, short effectiveYear)
        {
            var holder = new ObjectiveDetailHolder();

            try
            {
                var user = PreferenceHelper.UserInfo();
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}Measure"
                };

                var enumResponse = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (enumResponse.Count > 0)
                {
                    holder.MeasureSource = new ObservableCollection<Models.DataObjects.SelectableListModel>(
                        enumResponse.Select(p => new Models.DataObjects.SelectableListModel()
                        {
                            Id = Convert.ToInt64(p.Value),
                            DisplayText = p.DisplayText,
                        }));
                }

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.IndividualObjectives}/initialize-objectives"
                };

                var param = new R.Requests.InitIndividualObjectiveRequest()
                {
                    CompanyId = user.CompanyId,
                    DepartmentId = user.DepartmentId,
                    JobLevelId = user.JobLevelId,
                    PositionId = user.PositionId,
                    ProfileId = user.ProfileId,
                    EffectiveYear = effectiveYear,
                    Id = id
                };

                var request = string_.CreateUrl<R.Requests.InitIndividualObjectiveRequest>(builder.ToString(), param);
                var response = await genericRepository_.GetAsync<R.Responses.IndividualObjectiveInitResponse>(request);

                if (response != null)
                {
                    holder.KPISource = new ObservableCollection<R.Models.KPIDataDto>(
                        response.KPIDataList.Select(p => new R.Models.KPIDataDto()
                        {
                            Criteria = p.Criteria,
                            DisplayData = p.DisplayData,
                            DisplayField = p.DisplayField,
                            DisplayId = p.DisplayId,
                            DisplaySource = p.DisplaySource,
                        }));

                    holder.ParentObjectiveList = response.ParentObjectiveList;
                    holder.OtherObjectiveList = response.OtherObjectiveList;

                    var baseline = response.FormFieldList.FirstOrDefault(p => p.FormFieldName == "txtBaseLine");
                    if (baseline != null)
                        holder.BaseLineHelper = baseline;

                    var kpi = response.FormFieldList.FirstOrDefault(p => p.FormFieldName == "cmbKeyPerformanceIndicatorId");

                    if (kpi != null && response.ShowOtherKPIFields)
                    {
                        holder.ShowKPI = !kpi.HideTag;
                        holder.KPIHelper = kpi;
                    }

                    holder.ShowWeight = response.WeightComputation;
                }

                holder.DepartmentName = user.Department;
                holder.EffectiveYear = effectiveYear.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<KPISelectionResponse> RetrieveKPICriteria(long id, ObservableCollection<RateScaleDto> list)
        {
            var data = new KPISelectionResponse();

            try
            {
                using (UserDialogs.Instance.Loading())
                {
                    await Task.Delay(500);

                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.IndividualObjectives}/get-kpi-criteria/{id}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.KPISelectionResponse>(builder.ToString());

                    if (response.KPICriterias.Count > 0)
                    {
                        data.RateScales = new ObservableCollection<RateScaleDto>(
                            response.KPICriterias.Select(p => new RateScaleDto()
                            {
                                CriteriaId = p.CriteriaId,
                                Max = p.txtMax,
                                Min = p.txtMin,
                                Rating = p.txtScore,
                                Criteria = p.txtCriteria,
                                DisplayText1 = string.Format("{0} {2} Rating: {1}",
                                    (p.txtMin == p.txtMax ? p.txtMin.ToString() : p.txtMin.ToString() + " - " + p.txtMax.ToString()),
                                    p.txtScore.ToString(),
                                    (!string.IsNullOrWhiteSpace(p.txtCriteria) ? p.txtCriteria : "unit")
                            )
                            }));
                    }

                    data.KPIObjective = response.KPIObjective;
                }

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectiveGroupingResponse> RetrieveStandardObjectives(string effectiveYear)
        {
            var retVal = new ObjectiveGroupingResponse();
            try
            {
                var user = PreferenceHelper.UserInfo();
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.IndividualObjectives}/standard-objectives"
                };

                int year = 0;
                var isNum = int.TryParse(effectiveYear, out year);

                var param = new R.Requests.InitIndividualObjectiveRequest()
                {
                    CompanyId = user.CompanyId,
                    DepartmentId = user.DepartmentId,
                    JobLevelId = user.JobLevelId,
                    PositionId = user.PositionId,
                    ProfileId = user.ProfileId,
                    EffectiveYear = Convert.ToInt16(year),
                    Id = 0
                };

                var request = string_.CreateUrl<R.Requests.InitIndividualObjectiveRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.InitIndividualObjectiveResponse>(request);

                TotalListItem = response.StandardObjectiveList.Count;

                if (TotalListItem > 0)
                {
                    var index = 0;
                    var items = new ObservableCollection<ObjectiveDetailDto>();

                    foreach (var item in response.StandardObjectiveList)
                    {
                        index++;

                        var baselineIsNumber = decimal.TryParse(item.BaseLine, out decimal baseline);
                        var baselineVal = !baselineIsNumber ? item.BaseLine : baseline.ToString("N0");

                        items.Add(new ObjectiveDetailDto()
                        {
                            BaseLine = baselineVal,
                            CustomKPI = item.CustomKPI,
                            GoalId = item.OrganizationGoalId, //to verify
                            KPIId = item.KeyPerformanceIndicatorId,
                            KPIName = (string.IsNullOrWhiteSpace(item.KPI) ? " - " : item.KPI),
                            MeasureId = item.MeasureId,
                            MeasureName = (string.IsNullOrWhiteSpace(item.Measure) ? " - " : item.Measure),
                            ObjectiveDescription = item.Objectives,
                            ObjectiveDetail = item.OrgGoalDescription,
                            ObjectiveHeader = item.OrganizationGoal,
                            Target = item.TargetGoal.ToString("n2"),
                            UnitOfMeasure = item.UnitOfMeasure,
                            PODetailId = item.PerformanceObjectiveDetailId,
                            POHeaderId = item.PerformanceObjectiveHeaderId,
                            TempRowId = item.TempRowId,
                            StandardCustomCriteria = new ObservableCollection<RateScaleDto>(
                                        response.StandardCriteriaList.Select(x => new RateScaleDto()
                                        {
                                            Criteria = x.Criteria,
                                            CriteriaId = x.CriteriaId,
                                            Max = x.Max,
                                            Min = x.Min,
                                            TempId = x.CriteriaId,
                                            Rating = x.Score,
                                            PODetailId = x.PODetailId,
                                            RetrievedCompKPIId = x.RetrievedCompKPIId,
                                        }).Where(p => p.PODetailId == item.TempRowId)
                                    ),
                            RateScaleDto = new ObservableCollection<RateScaleDto>(
                                        response.StandardCriteriaList.Select(x => new RateScaleDto()
                                        {
                                            Criteria = x.Criteria,
                                            CriteriaId = x.CriteriaId,
                                            Max = x.Max,
                                            Min = x.Min,
                                            TempId = x.CriteriaId,
                                            Rating = x.Score,
                                            PODetailId = x.PODetailId,
                                            RetrievedCompKPIId = x.RetrievedCompKPIId,
                                        }).Where(p => p.PODetailId == item.TempRowId)
                                    ),
                            IsRetrievedFromTemplate = item.IsRetrievedFromTemplate,
                            RetrievedType = item.RetrievedType,
                            RetrievedPATemplateId = item.RetrievedPATemplateId,
                            RetrievedCompKPIId = item.RetrievedCompKPIId,
                            ShowLine = (index != response.StandardObjectiveList.Count),
                            /*ShowLine = (index > 0),*/
                            ParentObjective = (string.IsNullOrWhiteSpace(item.ParentGoal) ? Constants.NoOrgGoalsConstant : item.ParentGoal),
                            Weight = item.Weight.ToString("N0"),
                            TargetGoalSetup = $"{(item.Measure == "Amount" ? (item.UnitOfMeasure + " " + item.TargetGoal.ToString("n2")) : (item.TargetGoal.ToString("n2") + " " + item.UnitOfMeasure))}"
                        });
                    }

                    var groupings = await this.GroupObjectives(items);

                    retVal.Objectives = groupings.Objectives;
                    /*retVal.ObjectivesLimited = groupings.ObjectivesLimited;*/
                    retVal.ObjectivesLimited = groupings.Objectives;
                    retVal.IsExceeded = groupings.IsExceeded;
                    retVal.ObjectivesToSave = items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return await Task.FromResult(retVal);
        }

        public async Task<ObjectiveDetailHolder> SetValueObjectiveDetailForm(ObjectiveDetailDto item, ObjectiveDetailHolder holder)
        {
            var goals = await goalService_.InitForm(holder);
            holder.GoalHeaderDetails = goals.GoalHeaderDetails;

            holder.Goal.Value = item.ObjectiveHeader ?? item.ParentObjective;
            /*
            holder.GoalDescription.Value = item.ObjectiveDetail;
            holder.Objectives.Value = item.ObjectiveDescription;
            */
            holder.GoalDescription.Value = item.ObjectiveDescription;
            holder.KPISelectedItem = holder.KPISource.FirstOrDefault(x => x.DisplayId == item.KPIId);
            holder.SelectedKPIDataDto = holder.KPISelectedItem;
            holder.IsCustomKPI = (item.KPIId < 0);
            holder.CustomKPI.Value = item.CustomKPI;
            holder.MeasureSelectedItem = holder.MeasureSource.FirstOrDefault(x => x.Id == item.MeasureId);
            holder.Target.Value = item.Target;
            holder.UnitOfMeasure.Value = (item.UnitOfMeasure.Equals(" - ") ? item.MeasureName : item.UnitOfMeasure);
            holder.Baseline.Value = item.BaseLine;
            holder.PODetailId = item.PODetailId;
            holder.POHeaderId = item.POHeaderId;
            holder.TempRowId = item.TempRowId;
            holder.ParentObjective = item.ParentObjective;
            holder.ObjectiveDetail = item.ObjectiveDetail;
            holder.Weight.Value = item.ActualWeightVal;


            if (holder.GoalHeaderDetails != null && holder.GoalHeaderDetails.Count > 0)
            {
                var details = holder.GoalHeaderDetails.SelectMany(x => x.GoalDetails).ToList();
                var goal = details.FirstOrDefault(x => x.Name.Equals(item.ObjectiveDetail));

                if (goal != null)
                {
                    holder.SelectedGoalDetail = new GoalDetailDto()
                    {
                        Name = goal.Name,
                        Description = goal.Description,
                        DetailId = goal.HeaderDetailId,
                        HeaderDetailId = goal.HeaderDetailId,
                        HeaderDetailName = goal.Name,
                    };
                    holder.GoalId = goal.DetailId;
                }
            }

            var detailId = (item.PODetailId > 0 ? item.PODetailId : item.TempRowId);

            if (item.PODetailId == 0)
            {
                /*
                holder.Goal.Value = item.SelectedGoalDetail.Name;
                holder.GoalDescription.Value = item.SelectedGoalDetail.Description;
                */

                //rate scale values here
                holder.RateScaleSource = new ObservableCollection<RateScaleDto>(
                        item.StandardCustomCriteria
                        .Where(p => p.PODetailId == detailId)
                    );

                holder.RateScaleSourceDisplay = new ObservableCollection<RateScaleDto>(
                        holder.RateScaleSource.Where(x => x.IsDelete == false).ToList()
                    );

                if (item.TempRowId != 0)
                    holder.IsEditable = true;
            }
            else
            {
                holder.IsEditable = (item.StatusId == RequestStatusValue.Draft);

                holder.RateScaleSource = item.RateScaleDto;
                holder.ExistingRateScaleSource = item.RateScaleDto;
                holder.RateScaleSourceDisplay = item.RateScaleDto;

                if (item.StatusId == RequestStatusValue.Draft)
                    holder.IsEditable = true;
            }

            holder.Objectives.Value = item.Objectives ?? item.ObjectiveDetail;

            return await Task.FromResult(holder);
        }

        private void SetupModel(IndividualObjectiveItemHolder holder, string url, out UriBuilder builder, out R.Requests.SubmitPerformanceObjectiveRequest param)
        {
            var user = PreferenceHelper.UserInfo();

            var details = new List<R.Models.PerformanceObjectiveDetail>();

            foreach (var item in holder.ObjectivesToSave)
            {
                details.Add(new R.Models.PerformanceObjectiveDetail()
                {
                    BaseLine = item.BaseLine,
                    CustomKPI = item.CustomKPI,
                    KeyPerformanceIndicatorId = item.KPIId,
                    Measurement = Convert.ToInt16(item.MeasureId),
                    /*Objectives = item.ObjectiveDescription,*/
                    Objectives = item.Objectives,
                    ObjectiveName = item.ObjectiveDetail,
                    ObjectiveDescription = item.ObjectiveDescription,
                    ParentObjective = item.ParentObjective,
                    EmployeeRating = 0,
                    EmployeeReview = string.Empty,
                    Actual = string.Empty,
                    ManagerActual = string.Empty,
                    ManagerReview = string.Empty,
                    ManagerReviewRating = 0,
                    ManagerReviewRemarks = string.Empty,
                    TargetGoal = (!string.IsNullOrWhiteSpace(item.Target) ? Convert.ToDecimal(item.Target) : 0),
                    Weight = Convert.ToDecimal(item.Weight),
                    Rating = 0,
                    UseCustomCriteria = (item.KPIId == -999),
                    IsRetrievedFromTemplate = item.IsRetrievedFromTemplate,
                    RetrievedType = item.RetrievedType,
                    RetrievedPATemplateId = item.RetrievedPATemplateId,
                    RetrievedCompKPIId = item.RetrievedCompKPIId,
                    OrganizationGoalId = item.GoalId,
                    UnitOfMeasure = item.UnitOfMeasure,
                    IsDelete = item.IsDelete,
                    PerformanceObjectiveDetailId = item.PODetailId,
                    PerformanceObjectiveHeaderId = item.POHeaderId,
                    POCustomCriteria = new List<R.Models.POCustomCriteria>(
                            item.RateScaleDto.Select(x => new R.Models.POCustomCriteria()
                            {
                                Criteria = x.Criteria,
                                CriteriaId = (item.PODetailId == 0 ? 0 : x.CriteriaId),
                                Max = x.Max,
                                Min = x.Min,
                                PODetailId = x.PODetailId,
                                TargetScore = x.PODetailId,
                                IsDelete = x.IsDelete,
                            })
                        )
                });
            }

            var statusId = (!holder.IsSaveOnly ? RequestStatusValue.Submitted : RequestStatusValue.Draft);

            holder.Model = new R.Models.PerformanceObjectiveHeader()
            {
                DatePrepared = holder.PreparedDate,
                EffectiveYear = Convert.ToInt16(holder.EffectiveYear.Value),
                PeriodType = Convert.ToInt16(holder.AnnualChecked ? 3 : 2),
                ProfileId = (holder.Model.ProfileId.GetValueOrDefault(0) == 0 ? user.ProfileId : holder.Model.ProfileId),
                /*StatusId = (holder.Model.StatusId.GetValueOrDefault(0) == 0 ? RequestStatusValue.Draft : holder.Model.StatusId),*/
                StatusId = statusId,
                PerformanceObjectiveHeaderId = holder.Model.PerformanceObjectiveHeaderId,
                SourceId = (short)SourceEnum.Mobile,
                PerformanceObjectiveDetail = details,
            };

            builder = new UriBuilder(url)
            {
                Path = ApiConstants.IndividualObjectives
            };

            param = new R.Requests.SubmitPerformanceObjectiveRequest()
            {
                Data = holder.Model,
                IsSaveOnly = holder.IsSaveOnly,
            };
        }

        public async Task<IndividualObjectiveItemHolder> CancelRequest(IndividualObjectiveItemHolder holder)
        {
            holder.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(holder.ActionTypeId, holder.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(
                            holder.Model.PerformanceObjectiveHeaderId, TransactionType.PerformanceObjective);

                        var response = await workflowDataService_.CancelWorkFlowByRecordId(holder.Model.PerformanceObjectiveHeaderId,
                                             TransactionType.PerformanceObjective,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            FormSession.IsSubmitted = response.IsSuccess;
                            holder.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<ObjectiveGroupingResponse> GroupObjectives(ObservableCollection<ObjectiveDetailDto> items)
        {
            var response = new ObjectiveGroupingResponse();
            try
            {
                var grouped = items
                           .GroupBy(g => g.ParentObjective)
                           .Select(p => new MainObjectiveDto()
                           {
                               Header = p.Select(g => g.ParentObjective).FirstOrDefault(),
                               IsOpened = false,
                               ObjectiveDetailHeaderDto = new ObservableCollection<ObjectiveDetailHeaderDto>(
                                        p.GroupBy(g2 => g2.ObjectiveDetail)
                                        .Select(p2 => new ObjectiveDetailHeaderDto()
                                        {
                                            ObjectiveHeader = p2.Select(x => x.ObjectiveDetail).FirstOrDefault(),
                                            ObjectiveDetailDto = new ObservableCollection<ObjectiveDetailDto>(
                                                    p2.Select(x => new ObjectiveDetailDto()
                                                    {
                                                        BaseLine = x.BaseLine,
                                                        CustomKPI = x.CustomKPI,
                                                        GoalId = x.GoalId,
                                                        KPIId = x.KPIId,
                                                        KPIName = (string.IsNullOrWhiteSpace(x.KPIName) ? " - " : x.KPIName),
                                                        MeasureId = x.MeasureId,
                                                        MeasureName = (string.IsNullOrWhiteSpace(x.MeasureName) ? " - " : x.MeasureName),
                                                        ObjectiveDescription = x.ObjectiveDescription, //objective
                                                        ObjectiveDetail = x.ObjectiveDetail, //goal name
                                                        ObjectiveHeader = x.ObjectiveHeader, //parent goal
                                                        ParentObjective = x.ParentObjective, //parent goal
                                                        PODetailId = x.PODetailId,
                                                        POHeaderId = x.POHeaderId,
                                                        Target = x.Target,
                                                        UnitOfMeasure = (string.IsNullOrWhiteSpace(x.UnitOfMeasure) ? " - " : x.UnitOfMeasure),
                                                        TempRowId = x.TempRowId,
                                                        StandardCustomCriteria = x.StandardCustomCriteria,
                                                        RateScaleDto = x.RateScaleDto,
                                                        SelectedGoalDetail = x.SelectedGoalDetail,
                                                        ShowLine = x.ShowLine,
                                                        StatusId = x.StatusId,
                                                        RetrievedCompKPIId = x.RetrievedCompKPIId,
                                                        IsRetrievedFromTemplate = x.IsRetrievedFromTemplate,
                                                        RetrievedPATemplateId = x.RetrievedPATemplateId,
                                                        RetrievedType = x.RetrievedType,
                                                        Weight = $"{x.Weight} %",
                                                        TargetGoalSetup = x.TargetGoalSetup,
                                                        Actual = x.Actual,
                                                        EmployeeRating = x.EmployeeRating,
                                                        EmployeeReview = x.EmployeeReview,
                                                        IsDelete = x.IsDelete,
                                                        KPINameDisplay = x.KPINameDisplay,
                                                        ManagerActual = x.ManagerActual,
                                                        ManagerReview = x.ManagerReview,
                                                        ManagerReviewRating = x.ManagerReviewRating,
                                                        ActualWeightVal = x.ActualWeightVal,
                                                        Objectives = x.Objectives,
                                                    })
                                                ),
                                        })
                                   )
                           });

                response.Objectives = new ObservableCollection<MainObjectiveDto>(grouped);

                response.ObjectivesLimited = new ObservableCollection<MainObjectiveDto>(
                        response.Objectives
                            .Select(x => new MainObjectiveDto()
                            {
                                Header = x.Header,
                                ObjectiveDetailHeaderDto = new ObservableCollection<ObjectiveDetailHeaderDto>(
                                        x.ObjectiveDetailHeaderDto.Select(p => new ObjectiveDetailHeaderDto()
                                        {
                                            ObjectiveHeader = p.ObjectiveHeader,
                                            ObjectiveDetailDto = new ObservableCollection<ObjectiveDetailDto>(p.ObjectiveDetailDto.Take(3))
                                        })
                                    )
                            })
                    );

                response.IsExceeded = items.Count > 3;
                response.ObjectivesToSave = items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }

            return await Task.FromResult(response);
        }
    }
}