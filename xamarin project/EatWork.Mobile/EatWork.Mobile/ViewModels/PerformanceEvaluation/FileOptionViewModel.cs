using EatWork.Mobile.Models.DataObjects;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels.PerformanceEvaluation
{
    public class FileOptionViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand SelectedOptionCommand { get; set; }

        private ObservableCollection<SelectableListModel> _options;

        public ObservableCollection<SelectableListModel> Options
        {
            get { return _options; }
            set { _options = value; RaisePropertyChanged(() => Options); }
        }

        public FileOptionViewModel()
        {
            Init();
        }

        private void Init()
        {
            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            SelectedOptionCommand = new Command<SelectableListModel>(ExecuteSelectedOptionCommand);

            Options = new ObservableCollection<SelectableListModel>()
            {
                //new SelectableListModel(){ Id = 1, DisplayText = "Open File", Icon = Xamarin.Forms.Application.Current.Resources["InfoIcon"].ToString()},
                new SelectableListModel(){ Id = 2, DisplayText = "Delete", Icon = Xamarin.Forms.Application.Current.Resources["DeleteIcon"].ToString()},
            };
        }

        private async void ExecuteSelectedOptionCommand(SelectableListModel val)
        {
            if (val != null)
            {
                await PopupNavigation.Instance.PopAsync(true);
                MessagingCenter.Send(this, "FileOptionSelectedValue", val.Id);
            }
        }
    }
}