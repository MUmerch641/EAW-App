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
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class IndividualObjectiveItemDataService : IIndividualObjectiveItemDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly StringHelper string_;

        public long TotalListItem { get; set; }

        public IndividualObjectiveItemDataService()
        {
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            string_ = new StringHelper();
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
                }
                else
                {
                    holder.EffectiveYear.Value = DateTime.UtcNow.Year.ToString();
                    holder.IsEnabled = true;
                }

                //test
                var items = new ObservableCollection<ObjectiveDetailDto>();
                for (int i = 1; i < 3; i++)
                {
                    items.Add(new ObjectiveDetailDto()
                    {
                        ObjectiveDescription = "mauris nunc congue nisi vitae suscipit tellus mauris a diam",
                        KPIName = "Brand#1",
                        MeasureName = "Amount",
                        Target = "Php 5,000.00",
                        BaseLine = "500"
                    });
                }

                //holder.Objectives.Add(new ObjectiveDetailHeaderDto()
                //{
                //    ObjectiveHeader = "Increase Revenue",
                //    ObjectiveDetailDto = items,
                //});

                //holder.Objectives.Add(new ObjectiveDetailHeaderDto()
                //{
                //    ObjectiveHeader = "Action Plan Assensment",
                //    ObjectiveDetailDto = items,
                //});

                /*if (holder.Objectives.Count > 0)*/
                /*holder.ObjectivesLimited = new ObservableCollection<ObjectiveDetailHeaderDto>(holder.Objectives.Take(3));*/
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<IndividualObjectiveItemHolder> SubmitRequest(IndividualObjectiveItemHolder holder)
        {
            if (holder.IsValid())
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);
                        }
                        catch (HttpRequestExceptionEx ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }

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
                    foreach (var item in response.KPIDataList)
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task<KPISelectionResponse> RetrieveKPICriteria(long id, ObservableCollection<RateScaleDto> list)
        {
            throw new NotImplementedException();
        }

        public async Task<ObjectiveGroupingResponse> RetrieveStandardObjectives(string effectiveYear)
        {
            throw new NotImplementedException();
        }

        public async Task<ObjectiveDetailHolder> SetValueObjectiveDetailForm(ObjectiveDetailDto item, ObjectiveDetailHolder holder)
        {
            throw new NotImplementedException();
        }

        public async Task<IndividualObjectiveItemHolder> CancelRequest(IndividualObjectiveItemHolder holder)
        {
            throw new NotImplementedException();
        }

        public async Task<ObjectiveGroupingResponse> GroupObjectives(ObservableCollection<ObjectiveDetailDto> items)
        {
            throw new NotImplementedException();
        }

        /*
        public async Task<ObjectiveDetailHolder> SaveObjectiveDetail(ObjectiveDetailHolder holder)
        {
            return holder;
        }
        */
    }
}