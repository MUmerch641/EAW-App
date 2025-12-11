using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.Utils
{
    /// <summary>
    /// https://stackoverflow.com/questions/43102069/xamarin-forms-listview-load-more
    /// This class extends the behavior of the SfListView control to filter the ListViewItem based on text.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class CustomListView : ListView
    {
        private Delegate eventHandler;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("LoadMoreCommand", typeof(ICommand), typeof(CustomListView), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("LoadMoreCommandParameter", typeof(object), typeof(CustomListView), null);

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object LoadMoreCommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public CustomListView()
        {
            ItemAppearing += ListView_ItemAppearing;
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = ItemsSource as IList;

            if (items != null && e.Item == items[items.Count - 1])
            {
                if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                    LoadMoreCommand.Execute(null);
            }
        }
    }
}