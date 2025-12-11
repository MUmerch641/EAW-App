using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Utils
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormModal : PopupPage
    {
        protected IPopupNavigation _popupNavigation;

        protected Action OnApearing;

        public static readonly BindableProperty HeaderProperty
           = BindableProperty.Create(nameof(Header), typeof(View), typeof(FormModal));

        public View Header
        {
            get { return (View)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly BindableProperty BodyProperty
           = BindableProperty.Create(nameof(Body), typeof(View), typeof(FormModal));

        public View Body
        {
            get { return (View)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public FormModal()
        {
            InitializeComponent();
            _popupNavigation = PopupNavigation.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnApearing?.Invoke();
        }
    }
}