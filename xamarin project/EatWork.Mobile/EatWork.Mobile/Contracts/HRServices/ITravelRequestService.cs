using EatWork.Mobile.Models.FormHolder;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ITravelRequestService
    {
        Task<TravelRequestHolder> InitForm(long id);

        Task<TravelRequestHolder> SubmitRecord(TravelRequestHolder holder);

        Task<TravelRequestHolder> RequestCancelRequest(TravelRequestHolder holder);
    }
}