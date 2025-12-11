using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.PerformanceEvaluation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class PEListDataService : IPEListDataService
    {
        public long TotalListItem { get; set; }

        public PEListDataService()
        {
        }

        public async Task<ObservableCollection<PEListDto>> GetListAsync(ObservableCollection<PEListDto> list, ListParam args)
        {
            list = new ObservableCollection<PEListDto>()
            {
                new PEListDto()
                {
                    RecordId = 1,
                    EvaluationType = "Annual Evaluation",
                    Status = "Approved",
                    PeriodCovered = "01/01/2021 - 03/01/2021",
                    ScheduledDate = "03/08/2021 - 03/12/2021",
                    DueDate_String = "03/12/2021",
                },
                new PEListDto()
                {
                    RecordId = 2,
                    EvaluationType = "Annual Evaluation",
                    Status = "For Approval",
                    PeriodCovered = "01/01/2021 - 03/01/2021",
                    ScheduledDate = "03/08/2021 - 03/12/2021",
                    DueDate_String = "03/12/2021",
                },
                new PEListDto()
                {
                    RecordId = 3,
                    EvaluationType = "Annual Evaluation",
                    Status = "Reviewed",
                    PeriodCovered = "01/01/2021 - 03/01/2021",
                    ScheduledDate = "03/08/2021 - 03/12/2021",
                    DueDate_String = "03/12/2021",
                },
                new PEListDto()
                {
                    RecordId = 4,
                    EvaluationType = "Annual Evaluation",
                    Status = "For Assessment",
                    PeriodCovered = "01/01/2021 - 03/01/2021",
                    ScheduledDate = "03/08/2021 - 03/12/2021",
                    DueDate_String = "03/12/2021",
                }
            };

            TotalListItem = 4;

            return list;
        }
    }
}