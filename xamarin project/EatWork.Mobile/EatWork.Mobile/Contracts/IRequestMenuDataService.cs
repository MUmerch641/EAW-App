using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IRequestMenuDataService
    {
        Task<ObservableCollection<MenuItemModel>> InitMenuList();
    }
}