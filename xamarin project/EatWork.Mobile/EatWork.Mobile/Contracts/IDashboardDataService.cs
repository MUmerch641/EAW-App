using EatWork.Mobile.Models.FormHolder;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IDashboardDataService
    {
        Task<DashboardFormHolder> GetDashboardDefault();
    }
}