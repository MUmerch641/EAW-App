using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.IndividualObjectives;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EatWork.Mobile.Services.TestServices
{
    public class IndividualObjectivesDataService : IIndividualObjectivesDataService
    {
        public long TotalListItem { get; set; }

        public IndividualObjectivesDataService()
        {
        }

        public async Task<ObservableCollection<IndividualObjectivesDto>> GetListAsync(ObservableCollection<IndividualObjectivesDto> list, ListParam args)
        {
            for (int i = 1; i <= 15; i++)
            {
                list.Add(new IndividualObjectivesDto()
                {
                    IndividualOjbectiveId = i,
                    Status = "New",
                    DatePrepared = System.DateTime.UtcNow,
                    EffectiveYear = 2021,
                    ProfileId = 138,
                    Period = "Mid Year",
                    StatusId = 0,
                    Details = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ac tincidunt vitae semper quis lectus nulla.",
                    DatePrepared_String = System.DateTime.UtcNow.ToString(FormHelper.DateFormat),
                    Icon = Application.Current.Resources["StringFlagIcon"].ToString(),
            });
            }

            return list;
        }
    }
}