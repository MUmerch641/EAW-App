namespace Mobile.Utils.ControlGenerator
{
    public static class ControlType
    {
        public const string TextBox = "TextBox";
        public const string CheckBox = "CheckBox";
        public const string CheckboxList = "CheckboxList";
        public const string ComboBox = "ComboBox";
        public const string TextArea = "TextArea";
        public const string DatePicker = "DatePicker";
        public const string TimePicker = "TimePicker";
        public const string NumericTextBox = "NumericTextBox";
        public const string Button = "Button";
        public const string RadioButton = "RadioButton";
    }

    public static class EventName
    {
        public const string Unfocused = "Unfocused";
        public const string TextChanged = "TextChanged";
        public const string SelectionChanged = "SelectionChanged";
        public const string StateChanged = "StateChanged";
        public const string ValueChanged = "ValueChanged";
    }

    public enum ControlTypeEnum
    {
        Textbox = 1,
        MultilineTextbox,
        Dropdown,
        MultiselectDropdown,
        DatePicker,
        DateRangePicker,
        TimePicker,
        TimeRangePicker,
        CheckboxOption,
        RadioOption,
        SwitchOption,
        StarRating
    }
}