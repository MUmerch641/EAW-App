namespace EatWork.Mobile.Models.DataObjects
{
    public class SelectableListModel
    {
        public bool IsChecked { get; set; }
        public long Id { get; set; }
        public string DisplayText { get; set; }
        public string DisplayData { get; set; }
        public string Icon { get; set; }
    }
}