using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Questionnaire
{
    public class SurveyChartHolder : ExtendedBindableObject
    {
        public SurveyChartHolder()
        {
            BarData = new ObservableCollection<ChartDataModel>();
        }

        private ObservableCollection<ChartDataModel> barData_;

        public ObservableCollection<ChartDataModel> BarData
        {
            get { return barData_; }
            set { barData_ = value; RaisePropertyChanged(() => BarData); }
        }

        private string chartTitle_;

        public string ChartTitle
        {
            get { return chartTitle_; }
            set { chartTitle_ = value; RaisePropertyChanged(() => ChartTitle); }
        }

        private string primaryAxisTitle_;

        public string PrimaryAxisTitle
        {
            get { return primaryAxisTitle_; }
            set { primaryAxisTitle_ = value; RaisePropertyChanged(() => PrimaryAxisTitle); }
        }

        private string secondaryAxisTitle_;

        public string SecondaryAxisTitle
        {
            get { return secondaryAxisTitle_; }
            set { secondaryAxisTitle_ = value; RaisePropertyChanged(() => SecondaryAxisTitle); }
        }
    }

    public class ChartDataModel
    {
        public ChartDataModel(string data, double value, string tooltip)
        {
            DisplayData = data;
            Value = value;
            DisplayData = tooltip;
        }

        public string Name { get; set; }
        public double Value { get; set; }
        public string DisplayData { get; set; }
    }
}