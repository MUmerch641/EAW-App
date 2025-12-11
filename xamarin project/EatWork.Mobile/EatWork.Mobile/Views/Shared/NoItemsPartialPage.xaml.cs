using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoItemsPartialPage : StackLayout
    {
        public string TitleText
        {
            get { return base.GetValue(titleTextProperty).ToString(); }
            set { base.SetValue(titleTextProperty, value); }
        }

        public string DetailsText
        {
            get { return base.GetValue(detailsTextProperty).ToString(); }
            set { base.SetValue(detailsTextProperty, value); }
        }

        public string ImageSource
        {
            get { return base.GetValue(imageProperty).ToString(); }
            set { base.SetValue(imageProperty, value); }
        }

        private static BindableProperty titleTextProperty = BindableProperty.Create(
                                                             propertyName: "TitleText",
                                                             returnType: typeof(string),
                                                             declaringType: typeof(NoItemsPartialPage),
                                                             defaultValue: "NO ITEMS",
                                                             defaultBindingMode: BindingMode.TwoWay,
                                                             propertyChanged: titleTextPropertyChanged);

        private static BindableProperty detailsTextProperty = BindableProperty.Create(
                                                                propertyName: "DetailsText",
                                                                returnType: typeof(string),
                                                                declaringType: typeof(NoItemsPartialPage),
                                                                defaultValue: "",
                                                                defaultBindingMode: BindingMode.TwoWay,
                                                                propertyChanged: detailsTextPropertyChanged);

        private static BindableProperty imageProperty = BindableProperty.Create(
                                                                propertyName: "ErrorImage",
                                                                returnType: typeof(string),
                                                                declaringType: typeof(NoItemsPartialPage),
                                                                defaultValue: "",
                                                                defaultBindingMode: BindingMode.TwoWay,
                                                                propertyChanged: imagePropertyChanged);

        private static void titleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NoItemsPartialPage)bindable;
            control.HeaderText.Text = newValue.ToString();
        }

        private static void detailsTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NoItemsPartialPage)bindable;
            control.ContentText.Text = newValue.ToString();
        }

        private static void imagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (NoItemsPartialPage)bindable;
            control.ErrorImage.Source = newValue.ToString();
        }

        public NoItemsPartialPage()
        {
            InitializeComponent();
        }
    }
}