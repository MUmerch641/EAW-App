using EatWork.Mobile.Validations;
using Rg.Plugins.Popup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class SpecialNoteRequestViewModel : BaseViewModel
    {
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddNoteRequestCommand { get; set; }

        private ValidatableObject<string> note_;

        public ValidatableObject<string> Note
        {
            get { return note_; }
            set { note_ = value; RaisePropertyChanged(() => Note); }
        }

        public SpecialNoteRequestViewModel()
        {
        }

        public void Init(string note)
        {
            Note = new ValidatableObject<string>();
            CloseModalCommand = new Command(async () => await PopupNavigation.Instance.PopAsync(true));
            AddNoteRequestCommand = new Command(ExecuteAddNoteRequestCommand);

            if (!string.IsNullOrWhiteSpace(note))
                Note.Value = note;
        }

        private async void ExecuteAddNoteRequestCommand()
        {
            if (IsValid())
            {
                MessagingCenter.Send(this, "specialNoteRequestCompleted", Note.Value);
                await PopupNavigation.Instance.PopAsync(true);
            }
        }

        private bool IsValid()
        {
            Note.Validations.Clear();
            Note.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            Note.Validate();

            return Note.IsValid;
        }
    }
}