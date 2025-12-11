using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Views.Requests;
using EatWork.Mobile.Views.TravelRequest;
using Syncfusion.DataSource.Extensions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    internal class RequestMenuDataService : IRequestMenuDataService
    {
        public async Task<ObservableCollection<MenuItemModel>> InitMenuList()
        {
            var retValue = new ObservableCollection<MenuItemModel>();
            var configForm = MenuHelper.Forms();

            var menus = new ObservableCollection<MenuItemModel>()
            {
                new MenuItemModel { Icon="fas-umbrella-beach", Title="Leave", TargetType = typeof(LeaveRequestPage), Id = MenuItemType.Leave },
                new MenuItemModel { Icon="md-alarm", Title="Overtime", TargetType = typeof(OvertimeRequestPage), Id = MenuItemType.Overtime },
                new MenuItemModel { Icon="fas-business-time", Title="Official Business", TargetType = typeof(OfficialBusinessRequestPage), Id = MenuItemType.OfficialBusiness },
                new MenuItemModel { Icon="far-clock", Title="Undertime",TargetType = typeof(UndertimeRequestPage) , Id = MenuItemType.Undertime},
                new MenuItemModel { Icon="fas-user-clock", Title="Time Entry", TargetType = typeof(TimeEntryRequestPage) , Id = MenuItemType.TimeEntryLog},
                new MenuItemModel { Icon="md-timer-off", Title="Time Off", TargetType = typeof(TimeOffRequestPage), Id = MenuItemType.TimeOff },
                new MenuItemModel { Icon="far-calendar-alt", Title="Change Work Schedule", TargetType = typeof(ChangeWorkSchedulePage), Id = MenuItemType.ChangeWorkSchedule },
                new MenuItemModel { Icon="fas-bed", Title="Change Restday", TargetType = typeof(ChangeRestdayPage), Id = MenuItemType.ChangeRestday  },
                new MenuItemModel { Icon="fas-calendar-plus", Title="Special Work Schedule", TargetType = typeof(SpecialWorkSchedulePage), Id = MenuItemType.SpecialWorkSchedule },
                new MenuItemModel { Icon="fas-hand-holding-usd", Title="Loan", TargetType = typeof(LoanRequestPage), Id = MenuItemType.Loan  },
                new MenuItemModel { Icon="fas-file-alt", Title="Document", TargetType = typeof(DocumentRequestPage), Id = MenuItemType.Document  },
                /*new MenuItemModel { Icon="fas-wallet", Title="Cash Advance", TargetType = typeof(CashAdvanceRequestPage) },*/
                new MenuItemModel { Icon="fas-route", Title="Travel Request", TargetType = typeof(TravelRequestFormPage), Id = MenuItemType.TravelRequest },
            };

            foreach (var item in menus.ToList())
            {
                var eType = (MenuItemType)item.Id;
                var cMenu = configForm.FirstOrDefault(x => x.FormCode == eType.ToString());

#if CLIENT_DEBUG
                retValue.Add(item);
#else
                if (cMenu != null)
                    retValue.Add(item);
#endif
            }

            return await Task.FromResult(retValue); ;
        }
    }
}