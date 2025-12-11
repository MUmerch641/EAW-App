using EatWork.Mobile.Models.Questionnaire;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Utils.ControlGenerator
{
    public interface IGenerateControlService
    {
        int Counter { get; set; }

        Task<Xamarin.Forms.Button> CreateButton(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateTextArea(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateTextBox(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateComboBox(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateDatePicker(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateTimePicker(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.TextInputLayout.SfTextInputLayout> CreateNumericTextBox(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.Buttons.SfCheckBox> CreateCheckBox(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.XForms.Buttons.SfRadioButton> CreateRadioButton(BaseQControlDto model, ICommand command = null);

        Task<FlexLayout> CreateRadioButtons(BaseQControlDto model, ICommand command = null);

        Task<StackLayout> CreateCheckboxList(BaseQControlDto model, ICommand command = null);

        Task<Grid> CreateDateRange(BaseQControlDto model, ICommand command = null);

        Task<Grid> CreateTimeRange(BaseQControlDto model, ICommand command = null);

        Task<Syncfusion.SfRating.XForms.SfRating> CreateRateControl(BaseQControlDto model, ICommand command = null);

        Task<Grid> CreateSwitchButton(BaseQControlDto model, ICommand command = null);

        Task<Xamarin.Forms.View> ProcessControl(BaseQControlDto model, ICommand command = null);
    }
}