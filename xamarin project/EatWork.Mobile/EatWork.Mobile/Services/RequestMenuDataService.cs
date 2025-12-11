using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services
{
    internal class RequestMenuDataService : IRequestMenuDataService
    {
        public async Task<ObservableCollection<MenuItemModel>> InitMenuList()
        {
            var retValue = new ObservableCollection<MenuItemModel>();

            return retValue;
        }
    }
}