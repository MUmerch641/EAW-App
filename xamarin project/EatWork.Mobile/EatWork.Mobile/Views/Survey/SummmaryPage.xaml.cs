using EatWork.Mobile.Models.FormHolder.Questionnaire;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummmaryPage : ContentView
    {
        public SummmaryPage()
        {
            InitializeComponent();

            //if (answers != null)
            //{
            //BindableLayout.SetItemsSource(SummaryList, answers);
            //}
        }
    }
}