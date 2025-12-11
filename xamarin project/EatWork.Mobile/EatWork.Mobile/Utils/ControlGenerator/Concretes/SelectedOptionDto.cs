namespace Mobile.Utils.ControlGenerator
{
    public class SelectedOptionDto
    {
        public string ID { get; set; }
        public string DisplayText { get; set; }
        public bool IsChecked { get; set; }
    }

    public class CheckboxListHolder
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }
}