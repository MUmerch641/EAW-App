using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        #region commands

        public ICommand LoadItemsCommand { get; set; }
        public ICommand ViewItemCommand { get; set; }
        public ICommand OpenSearchCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand ResetSearchCommand { get; set; }
        public ICommand OpenFilterCommand { get; set; }
        public ICommand FilterTypeCommand { get; set; }
        public ICommand SelectedTransactionTypeCommand { get; set; }
        public ICommand ResetFilteredTypeCommand { get; set; }
        public ICommand EditCommand { get; set; }

        #endregion commands

        private SfListView listView_;

        public SfListView ListView
        {
            get { return listView_; }
            set { listView_ = value; RaisePropertyChanged(() => ListView); }
        }

        private bool showSearchField_;

        public bool ShowSearchField
        {
            get { return showSearchField_; }
            set { showSearchField_ = value; RaisePropertyChanged(() => ShowSearchField); }
        }

        private bool ascending_;

        public bool Ascending
        {
            get { return ascending_; }
            set { ascending_ = value; RaisePropertyChanged(() => Ascending); }
        }

        private string keyWord_;

        public string KeyWord
        {
            get { return keyWord_; }
            set { keyWord_ = value; RaisePropertyChanged(() => KeyWord); }
        }

        private bool showList_;

        public bool ShowList
        {
            get { return showList_; }
            set { showList_ = value; RaisePropertyChanged(() => ShowList); }
        }

        private bool noItems_;

        public bool NoItems
        {
            get { return noItems_; }
            set { noItems_ = value; RaisePropertyChanged(() => NoItems); }
        }

        private bool displayFilter_;

        public bool DisplayFilter
        {
            get { return displayFilter_; }
            set { displayFilter_ = value; RaisePropertyChanged(() => DisplayFilter); }
        }

        private bool isEditMode_;

        public bool IsEditMode
        {
            get { return isEditMode_; }
            set { isEditMode_ = value; RaisePropertyChanged(() => IsEditMode); }
        }

        private ObservableCollection<SelectableListModel> selectableListItemSource_;

        public ObservableCollection<SelectableListModel> SelectableListItemSource
        {
            get { return selectableListItemSource_; }
            set { selectableListItemSource_ = value; RaisePropertyChanged(() => SelectableListItemSource); }
        }

        private ObservableCollection<SelectableListModel> selectedTransactionTypes_;

        public ObservableCollection<SelectableListModel> SelectedTransactionTypes
        {
            get { return selectedTransactionTypes_; }
            set { selectedTransactionTypes_ = value; RaisePropertyChanged(() => SelectedTransactionTypes); }
        }

        private long totalListItem_;

        public long TotalListItem
        {
            get { return totalListItem_; }
            set { totalListItem_ = value; RaisePropertyChanged(() => TotalListItem); }
        }

        public int TotalItems = 10;

        public KeyboardHelper Keyboard;
        public RequestType RequestType;      

        public ListViewModel()
        {
            Init();
        }

        private void Init()
        {
            RequestType = new RequestType();
            Keyboard = new KeyboardHelper();

            ShowSearchField = false;
            Ascending = false;
            NoItems = false;
            DisplayFilter = false;
            IsEditMode = false;
            TotalListItem = 0;

            SelectedTransactionTypes = new ObservableCollection<SelectableListModel>();

            OpenSearchCommand = new Command<StackLayout>(OpenSeach);

            /*
            OpenSearchCommand = new Command(() =>
            {
                ShowSearchField = !ShowSearchField;
                if (!ShowSearchField)
                    Keyboard.Dismiss();
            });
            */

            OpenFilterCommand = new Command(() => DisplayFilter = !DisplayFilter);

            SelectedTransactionTypeCommand = new Command<SelectableListModel>(AddSelectedTransactionType);
        }

        public virtual void AddSelectedTransactionType(SelectableListModel obj)
        {
            if (obj != null)
            {
                if (!obj.IsChecked)
                    SelectedTransactionTypes.Remove(obj);
                else
                    SelectedTransactionTypes.Add(obj);

                SelectableListItemSource = SelectableListItemSource;
            }
        }

        public virtual void OpenSeach(StackLayout obj)
        {
            ShowSearchField = !ShowSearchField;

            if (ShowSearchField)
            {
                var expandAnimation = new Animation(
                property =>
                {
                    obj.WidthRequest = property;
                    var opacity = property / obj.Width;
                    obj.Opacity = opacity;
                }, 0, obj.Width, Easing.Linear);

                expandAnimation.Commit(obj, "Expand", 16, 250, Easing.Linear);
            }

            if (!ShowSearchField)
                Keyboard.Dismiss();
        }
    }
}