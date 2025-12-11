using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.IndividualObjectives
{
    public class KPISelectionResponse
    {
        public KPISelectionResponse()
        {
            RateScales = new ObservableCollection<RateScaleDto>();
        }

        public ObservableCollection<RateScaleDto> RateScales { get; set; }
        public string KPIObjective { get; set; }
    }
}