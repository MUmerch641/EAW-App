using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.Dashboard;
using EatWork.Mobile.Models.FormHolder;
using EatWork.Mobile.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class DashboardDataService : IDashboardDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;

        public DashboardDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
        }

        public async Task<DashboardFormHolder> GetDashboardDefault()
        {
            var retValue = new DashboardFormHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.GetDashboardDefault, userInfo.ProfileId)
                };

                var response = await genericRepository_.GetAsync<R.Responses.DashboardResponse>(builder.ToString());

                if (response != null)
                {
                    retValue.AbsencesMTD = new DashboardModel()
                    {
                        InfoboxDetail = response.AbsencesMTD.InfoboxDetail,
                        InfoboxValue = response.AbsencesMTD.InfoboxValue
                    };

                    retValue.AbsencesWTD = new DashboardModel()
                    {
                        InfoboxDetail = response.AbsencesWTD.InfoboxDetail,
                        InfoboxValue = response.AbsencesWTD.InfoboxValue
                    };

                    retValue.AbsencesYTD = new DashboardModel()
                    {
                        InfoboxDetail = response.AbsencesYTD.InfoboxDetail,
                        InfoboxValue = response.AbsencesYTD.InfoboxValue
                    };

                    retValue.TardinessMTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TardinessMTD.InfoboxDetail,
                        InfoboxValue = response.TardinessMTD.InfoboxValue
                    };

                    retValue.TardinessWTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TardinessWTD.InfoboxDetail,
                        InfoboxValue = response.TardinessWTD.InfoboxValue
                    };

                    retValue.TardinessYTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TardinessYTD.InfoboxDetail,
                        InfoboxValue = response.TardinessYTD.InfoboxValue
                    };

                    retValue.TotalOvertimeMTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TotalOvertimeMTD.InfoboxDetail,
                        InfoboxValue = response.TotalOvertimeMTD.InfoboxValue
                    };

                    retValue.TotalOvertimeWTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TotalOvertimeWTD.InfoboxDetail,
                        InfoboxValue = response.TotalOvertimeWTD.InfoboxValue
                    };

                    retValue.TotalOvertimeYTD = new DashboardModel()
                    {
                        InfoboxDetail = response.TotalOvertimeYTD.InfoboxDetail,
                        InfoboxValue = response.TotalOvertimeYTD.InfoboxValue
                    };

                    retValue.VacationLeaveBalance = new DashboardModel()
                    {
                        InfoboxDetail = response.VacationLeaveBalance.InfoboxDetail,
                        InfoboxValue = response.VacationLeaveBalance.InfoboxValue
                    };

                    retValue.SickLeaveBalance = new DashboardModel()
                    {
                        InfoboxDetail = response.SickLeaveBalance.InfoboxDetail,
                        InfoboxValue = response.SickLeaveBalance.InfoboxValue
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name + " : " + e.Message}");
                throw;
            }

            return retValue;
        }
    }
}