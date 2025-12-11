using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.SuggestionCorner;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Contracts
{
    public interface ISuggestionFormDataService
    {
        Task<ObservableCollection<ComboBoxObject>> GetCategories();

        Task<FormHolder> SaveRecord(FormHolder holder);

        Task<R.Models.SuggestionCategory> SaveCategory(string category);
    }
}