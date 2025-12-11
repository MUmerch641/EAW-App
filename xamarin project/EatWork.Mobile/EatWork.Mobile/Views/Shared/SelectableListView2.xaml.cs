using Syncfusion.ListView.XForms;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EatWork.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectableListView2 : StackLayout
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
                                                                        declaringType: typeof(SelectableListView2),
                                                                        defaultValue: (object)null,
                                                                        defaultBindingMode: BindingMode.TwoWay,
                                                                        propertyChanged: OnItemsSourceChanged);

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelectableListView2)bindable;

            control.ListView.ItemsSource = newValue;
            control.ListView.RefreshView();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
                                                                    propertyName: nameof(Command),
                                                                    returnType: typeof(ICommand),
                                                                    declaringType: typeof(SelectableListView2),
                                                                    defaultValue: default(ICommand),
                                                                    defaultBindingMode: BindingMode.TwoWay,
                                                                    propertyChanged: OnCommandChanged);

        private static void OnCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SelectableListView2 control && newValue is ICommand command)
            {
                control.Command = command;
            }
        }

        public SelectableListView2()
        {
            InitializeComponent();
        }
    }
}