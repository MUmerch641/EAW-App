using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpandableLabel : ContentView, INotifyPropertyChanged
    {
        public ExpandableLabel()
        {
            InitializeComponent();
        }

        #region Text

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(ExpandableLabel),
            typeof(string),
            typeof(ContentView),
            default(string),
            BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (newValue != null && bindable is ExpandableLabel control)
                {
                    var actualNewValue = (string)newValue;
                    control.SmallLabel.Text = actualNewValue;
                    control.FullSpanText.Text = actualNewValue;
                    control.SmallSpanText.Text = actualNewValue;
                    var len = actualNewValue.Length;

                    if ((int)len > 75)
                    {
                        control.SmallSpanText.Text = actualNewValue.Substring(0, 75);
                        control.SmallLabel.IsVisible = false;
                        control.SmallLabelSeeMore.IsVisible = true;
                        /*//control.SmallSpanText.IsVisible = true;*/
                        control.Checker.Text = "1";
                    }
                    else
                    {
                        control.SmallLabel.IsVisible = true;
                        control.SmallLabelSeeMore.IsVisible = false;
                    }
                }
            });

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(nameof(Text));
            }
        }

        #endregion Text

        private void ShowMoreTapped(object sender, System.EventArgs e)
        {
            SmallLabel.IsVisible = false;
            SmallLabelSeeMore.IsVisible = false;
            FullLabel.IsVisible = true;
        }

        private void ShowLessTapped(object sender, System.EventArgs e)
        {
            if (Checker.Text == "1")
                SmallLabelSeeMore.IsVisible = true;
            else
                SmallLabel.IsVisible = true;

            FullLabel.IsVisible = false;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
    }
}