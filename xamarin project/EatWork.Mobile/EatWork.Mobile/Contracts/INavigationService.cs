using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Contracts
{
    public interface INavigationService
    {
        Task PushPageAsync(Page page);

        Task PopPageAsync();

        Task PopModalAsync();

        Task PushModalAsync(Page page);

        Task PopToRootAsync();
    }
}