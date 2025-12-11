using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using Newtonsoft.Json;
using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class EmployeeListDataService : IEmployeeDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;
        private readonly EmployeeFilterSelectionDataAccess employeeConfig_;

        public EmployeeListDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;

            employeeConfig_ = AppContainer.Resolve<EmployeeFilterSelectionDataAccess>();
        }

        public async Task<SfListView> InitListView(SfListView listview)
        {
            var retValue = listview;

            await Task.Run(() =>
            {
                if (retValue.DataSource.GroupDescriptors.Count == 0)
                {
                    //retValue.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                    //{
                    //    PropertyName = "DateFiledDisplay",
                    //    KeySelector = (object obj1) =>
                    //    {
                    //        return (obj1 as MyRequestListModel).DateFiled.ToString("ddd MMM dd yyyy");
                    //    }
                    //});

                    retValue.DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "EmployeeName",
                        Direction = ListSortDirection.Ascending
                    });
                }
            });

            return retValue;
        }

        public async Task<ObservableCollection<EmployeeListModel>> RetrieveEmployeeList(ObservableCollection<Models.EmployeeListModel> list, ListParam obj)
        {
            var retValue = list;

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                var user = PreferenceHelper.UserInfo();
                var config = await employeeConfig_.RetrieveSetup();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.GetEmployeeList
                };

                var param = new R.Requests.GetEmployeeListRequest()
                {
                    Page = (obj.ListCount == 0 ? 1 : ((obj.ListCount + obj.Count) / obj.Count)),
                    Rows = obj.Count,
                    SortOrder = (obj.IsAscending ? 0 : 1),
                    Keyword = obj.KeyWord,
                    BranchId = (config.ByBranch ? user.BranchId : 0),
                    DepartmentId = (config.ByDepartment ? user.DepartmentId : 0),
                    TeamId = (config.ByTeam ? user.TeamId : 0),
                };

                var request = string_.CreateUrl<R.Requests.GetEmployeeListRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.EmployeeInformation>>(request);

                obj.Count = (response.ListData.Count <= obj.Count ? response.ListData.Count : obj.Count);

                var temp = response.ListData;

                if (obj.IsAscending)
                    temp = response.ListData.OrderBy(p => p.EmployeeName).ToList();
                else
                    temp = response.ListData.OrderByDescending(p => p.EmployeeName).ToList();

                if (!string.IsNullOrWhiteSpace(obj.KeyWord))
                {
                    temp = response.ListData
                        .Where(p => p.EmployeeName.Contains(obj.KeyWord)
                        || p.EmployeeNo.Contains(obj.KeyWord)
                        || p.Department.Contains(obj.KeyWord)
                        || p.Position.Contains(obj.KeyWord)
                        ).ToList();
                }

                if (response.TotalPages != 0)
                {
                    try
                    {
                        for (int i = obj.ListCount; i < obj.ListCount + obj.Count; i++)
                        {
                            if (temp.ElementAtOrDefault(i) != null)
                            {
                                var model = new EmployeeListModel();
                                PropertyCopier<R.Models.EmployeeInformation, Models.EmployeeListModel>.Copy(temp[i], model);
                                retValue.Add(model);
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                PreferenceHelper.CRLimitCompany(0);
                PreferenceHelper.CRLimitBranch(0);
                PreferenceHelper.CRLimitDepartment(0);
                PreferenceHelper.CRLimitTeam(0);
            }

            return retValue;
        }
    }
}