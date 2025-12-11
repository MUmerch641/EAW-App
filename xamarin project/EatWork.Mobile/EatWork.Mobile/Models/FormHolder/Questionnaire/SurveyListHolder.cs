using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;
using APIM = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.Questionnaire
{
    public class SurveyListHolder : ExtendedBindableObject
    {
        public SurveyListHolder()
        {
            ItemSource = new ObservableCollection<APIM.Models.PulseSurveyList>();
        }

        private ObservableCollection<APIM.Models.PulseSurveyList> itemSource_;

        public ObservableCollection<APIM.Models.PulseSurveyList> ItemSource
        {
            get { return itemSource_; }
            set { itemSource_ = value; RaisePropertyChanged(() => ItemSource); }
        }
    }
}