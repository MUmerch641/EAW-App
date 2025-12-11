using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IMainPageDataService
    {
        Task<ObservableCollection<MenuItemModel>> InitMenuList();

        Task<ObservableCollection<Boarding>> GetBoardingList();

        Task<MimeTypes> GetMimeTypeList();

        Task<ThemeConfigModel> GetThemeSetup(long setupId);

        Task<int> GetMaxFileSize();

        Task SaveDeviceInfo();

        Task SaveUserDeviceInfo(long userId);

        Task CheckLatestVersion();

        Task GetDateFormat(string url);

        Task RetrievePackageSetup(string clientCode);
    }
}