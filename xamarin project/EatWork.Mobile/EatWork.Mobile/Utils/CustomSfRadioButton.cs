using Syncfusion.XForms.Buttons;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class CustomSfRadioButton : SfRadioButton
    {
        public static readonly BindableProperty UniqueIdProperty = BindableProperty.Create(
            nameof(UniqueId),
            typeof(string),
            typeof(CustomSfRadioButton),
            string.Empty,
            BindingMode.TwoWay,
            null,
            OnUniqueIdPropertyChanged);

        public static readonly BindableProperty ControlTypeIdProperty = BindableProperty.Create(
            nameof(ControlTypeId),
            typeof(int),
            typeof(CustomSfRadioButton),
            0,
            BindingMode.TwoWay,
            null,
            OnControlTypeIdPropertyChanged);

        /// <summary>
        /// Gets or sets the UniqueId.
        /// </summary>
        public string UniqueId
        {
            get { return (string)GetValue(UniqueIdProperty); }
            set { this.SetValue(UniqueIdProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ControlTypeId.
        /// </summary>
        public int ControlTypeId
        {
            get { return (int)GetValue(ControlTypeIdProperty); }
            set { this.SetValue(ControlTypeIdProperty, value); }
        }

        private static void OnUniqueIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CustomSfRadioButton;
            var newText = (string)newValue;

            if (!string.IsNullOrEmpty(newText))
            {
                view.UniqueId = newText;
            }
        }

        private static void OnControlTypeIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as CustomSfRadioButton;
            var newText = (int)newValue;

            if (newText != 0)
            {
                view.ControlTypeId = newText;
            }
        }
    }
}