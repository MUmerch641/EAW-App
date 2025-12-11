using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IGoalDataService
    {
        long TotalListItem { get; set; }

        Task<GoalsHolder> InitForm(ObjectiveDetailHolder holder);
    }
}