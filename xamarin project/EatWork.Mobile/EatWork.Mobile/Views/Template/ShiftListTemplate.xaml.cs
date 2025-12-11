using Syncfusion.ListView.XForms;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Template
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShiftListTemplate : StackLayout
    {
        public object ItemsSource
        {
            get
            {
                return ((BindableObject)this).GetValue(SfListView.ItemsSourceProperty);
            }
            set
            {
                ((BindableObject)this).SetValue(SfListView.ItemsSourceProperty, value);
            }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
                                                                        propertyName: nameof(ItemsSource),
                                                                        returnType: typeof(object),
                                                                        declaringType: typeof(ShiftListTemplate),
                                                                        defaultValue: (object)null,
                                                                        defaultBindingMode: BindingMode.TwoWay,
                                                                        propertyChanged: OnItemsSourceChanged);

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ShiftListTemplate)bindable;

            control.ShiftListView.ItemsSource = newValue;
            control.ShiftListView.RefreshView();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
                                                                    propertyName: nameof(Command),
                                                                    returnType: typeof(ICommand),
                                                                    declaringType: typeof(ShiftListTemplate),
                                                                    defaultValue: default(ICommand),
                                                                    defaultBindingMode: BindingMode.TwoWay,
                                                                    propertyChanged: OnCommandChanged);

        private static void OnCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShiftListTemplate control && newValue is ICommand command)
            {
                control.Command = command;
            }
        }

        public ShiftListTemplate()
        {
            InitializeComponent();
        }
    }
}