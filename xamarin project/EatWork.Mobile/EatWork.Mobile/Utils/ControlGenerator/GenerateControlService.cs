using EatWork.Mobile.Models.Questionnaire;
using EatWork.Mobile.Utils;
using Plugin.Iconize;
using Syncfusion.SfRating.XForms;
using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.ComboBox;
using Syncfusion.XForms.TextInputLayout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Utils.ControlGenerator
{
    public class GenerateControlService : IGenerateControlService
    {
        public GenerateControlService()
        {
            Counter = 1;
        }

        public int Counter { get; set; }

        public async Task<Button> CreateButton(BaseQControlDto model, ICommand command = null)
        {
            var control = new Button()
            {
                Text = model.QuestionName,
                BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"],
            };

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateComboBox(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var source = new ObservableCollection<DataSourceDto>();
            if (!string.IsNullOrWhiteSpace(model.BaseQuestion.Options))
            {
                var ctr = 1;
                foreach (var item in model.BaseQuestion.Options.Split('|').ToList())
                {
                    source.Add(new DataSourceDto() { ID = ctr, DisplayText = item });
                    ctr++;
                }
            }

            if (!isBlank)
            {
                var selected = source.Where(x => x.DisplayText == model.Answer).FirstOrDefault();
                inputHelper.SelectedItem = selected;
            }

            /*
            var source = new ObservableCollection<DataSourceDto>
            {
                new DataSourceDto(){DisplayText = "Option 1", ID = 1 },
                new DataSourceDto(){DisplayText = "Option 2", ID = 2 },
                new DataSourceDto(){DisplayText = "Option 3", ID = 3 },
            };
            */

            var input = new SfComboBox()
            {
                Style = (Style)Application.Current.Resources["ComboBoxStyle"],
                DataSource = source,
                DisplayMemberPath = "DisplayText",
                SelectedValuePath = "ID",
                SelectedItem = new DataSourceDto(),
                IsEnabled = isBlank,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(SfComboBox.SelectedItemProperty, new Binding("SelectedItem", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Please select",
                ContainerType = ContainerType.Outlined,
                InputView = input,
                EnableFloating = false,
                Style = (Style)Application.Current.Resources["inputLayoutStyle"],
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.SelectionChanged,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateMultiSelect(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var source = new ObservableCollection<DataSourceDto>();
            if (!string.IsNullOrWhiteSpace(model.BaseQuestion.Options))
            {
                var ctr = 1;
                foreach (var item in model.BaseQuestion.Options.Split('|').ToList())
                {
                    source.Add(new DataSourceDto() { ID = ctr, DisplayText = item });
                    ctr++;
                }
            }

            if (!isBlank)
            {
                foreach (var item in model.Answer.Split(',').ToList())
                {
                    var answer = source.Where(x => x.DisplayText == item).FirstOrDefault();
                    inputHelper.MultiSelectOptions.Add(answer);
                }
            }

            var input = new SfComboBox()
            {
                Style = (Style)Application.Current.Resources["ComboBoxStyle"],
                DataSource = source,
                DisplayMemberPath = "DisplayText",
                MultiSelectMode = MultiSelectMode.Token,
                SelectedItem = new ObservableCollection<object>(),
                DropDownItemHeight = 50,
                EnableAutoSize = true,
                IsEnabled = isBlank,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(SfComboBox.SelectedItemProperty, new Binding("MultiSelectOptions", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Please select",
                ContainerType = ContainerType.Outlined,
                InputView = input,
                EnableFloating = false,
                Style = (Style)Application.Current.Resources["InputLayoutStyleEditor"],
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.SelectionChanged,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateTextArea(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                Value = (!isBlank ? model.Answer : ""),
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var input = new Editor()
            {
                Style = (Style)Application.Current.Resources["EditorStyle"],
                IsEnabled = isBlank,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(Entry.TextProperty, new Binding("Value", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Type here",
                ContainerType = ContainerType.Outlined,
                EnableFloating = false,
                Style = (Style)Application.Current.Resources["InputLayoutStyleEditor"],
                InputView = input
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateTextBox(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                Value = (!isBlank ? model.Answer : ""),
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var input = new Entry()
            {
                Style = (Style)Application.Current.Resources["TextBoxStyle"],
                Text = model.Answer,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(Entry.TextProperty, new Binding("Value", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Type here",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["inputLayoutStyle"],
                InputView = input,
                IsEnabled = isBlank,
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateTimePicker(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            TimeSpan? ts = null;
            if (!isBlank)
                ts = DateTime.ParseExact(model.Answer, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                TimeValue = ts,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var input = new NullableTimePicker()
            {
                Style = (Style)Application.Current.Resources["TimePickerStyle"],
                NullableTime = ts,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(NullableTimePicker.NullableTimeProperty, new Binding("TimeValue", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Select time",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["TimeIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
                IsEnabled = isBlank,
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateDatePicker(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            DateTime? dt = null;
            if (!isBlank)
                dt = DateTime.Parse(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                DateValue = dt,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var input = new NullableDatePicker()
            {
                Style = (Style)Application.Current.Resources["DatePickerStyle"],
                NullableDate = dt,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(NullableDatePicker.NullableDateProperty, new Binding("DateValue", source: inputHelper));

            var control = new SfTextInputLayout
            {
                Hint = "Select date",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["DateIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
                IsEnabled = isBlank,
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfTextInputLayout> CreateNumericTextBox(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            decimal answer = 0;
            if (!isBlank)
                answer = Convert.ToDecimal(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                Value = (!isBlank ? answer.ToString() : ""),
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var input = new Entry()
            {
                Style = (Style)Application.Current.Resources["TextBoxStyle"],
                Keyboard = Keyboard.Numeric,
            };

            input.BindingContext = inputHelper;
            input.SetBinding(Entry.TextProperty, new Binding("Value", source: inputHelper));

            var control = new SfTextInputLayout
            {
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["inputLayoutStyle"],
                InputView = input,
                IsEnabled = isBlank,
            };

            control.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<SfCheckBox> CreateCheckBox(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            bool answer = false;
            if (!isBlank)
                answer = Convert.ToBoolean(model.Answer);

            var control = new SfCheckBox()
            {
                Text = model.Question,
                IsChecked = answer,
                IsEnabled = isBlank,
            };

            return await Task.FromResult(control);
        }

        public async Task<SfRadioButton> CreateRadioButton(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            bool answer = false;
            if (!isBlank)
                answer = Convert.ToBoolean(model.Answer);

            var control = new SfRadioButton()
            {
                Text = model.Question,
                IsEnabled = isBlank,
                IsChecked = answer,
            };

            return await Task.FromResult(control);
        }

        public async Task<FlexLayout> CreateRadioButtons(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var group = new SfRadioGroupKey();
            var layout = new FlexLayout();
            var stack = new StackLayout();

            if (!string.IsNullOrWhiteSpace(model.BaseQuestion.Options))
            {
                var list = model.BaseQuestion.Options.Split('|').ToList();

                foreach (var item in list)
                {
                    var isAnswer = (item == model.Answer);
                    var control = new CustomSfRadioButton() { Text = item, GroupKey = group, UniqueId = model.ID, IsEnabled = isBlank, IsChecked = isAnswer };
                    control.SetBinding(CustomSfRadioButton.UniqueIdProperty, new Binding("ID", source: inputHelper));
                    control.SetBinding(CustomSfRadioButton.ControlTypeIdProperty, new Binding("ControlTypeId", source: inputHelper));

                    var evnt = new EventToCommandBehavior()
                    {
                        EventName = EventName.StateChanged,
                        Command = command,
                        CommandParameter = control,
                    };

                    control.Behaviors.Add(evnt);

                    stack.Children.Add(control);
                }
            }

            layout.Children.Add(stack);

            return await Task.FromResult(layout);
        }

        public async Task<StackLayout> CreateCheckboxList(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var answers = new List<string>();

            if (!isBlank)
                answers = model.Answer.Split(',').ToList();

            try
            {
                var source = new ObservableCollection<DataSourceQuestionDto>();
                if (!string.IsNullOrWhiteSpace(model.BaseQuestion.Options))
                {
                    var ctr = 1;
                    foreach (var item in model.BaseQuestion.Options.Split('|').ToList())
                    {
                        var ischecked = false;

                        foreach (var ans in answers)
                            if (ans == item) ischecked = true;

                        source.Add(new DataSourceQuestionDto()
                        {
                            ID = ctr,
                            DisplayText = item,
                            IsChecked = ischecked,
                            DisplayId = model.ID,
                            ControlTypeId = model.BaseQuestion.ControlTypeId,
                            Name = model.ID,
                            Label = model.Question,
                            BaseQuestion = model.BaseQuestion,
                            FormQuestionId = model.BaseQuestion.FormQuestionId,
                        });
                        ctr++;
                    }
                }

                var layout = new StackLayout();
                var template = new DataTemplate(() =>
                {
                    var control = new SfCheckBox();
                    var grid = new Grid();

                    control.SetBinding(SfCheckBox.TextProperty, "DisplayText");
                    control.SetBinding(SfCheckBox.IsCheckedProperty, "IsChecked");

                    control.IsEnabled = isBlank;

                    var evnt = new EventToCommandBehavior()
                    {
                        EventName = EventName.StateChanged,
                        Command = command,
                        /*CommandParameter = new ControlDto(),*/
                    };
                    evnt.SetBinding(EventToCommandBehavior.CommandParameterProperty, ".");
                    control.Behaviors.Add(evnt);
                    grid.Children.Add(control);

                    return grid;
                });

                BindableLayout.SetItemsSource(layout, source);
                BindableLayout.SetItemTemplate(layout, template);
                return await Task.FromResult(layout);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
                throw ex;
            }
        }

        public async Task<SfRating> CreateRateControl(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            int val = 0;
            if (!isBlank)
                val = Convert.ToInt16(model.Answer);

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                Value = (!isBlank ? val.ToString() : "0"),
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var control = new SfRating()
            {
                Precision = Precision.Standard,
                TooltipPlacement = TooltipPlacement.BottomRight,
                ItemSpacing = 10,
                ItemSize = 37,
                IsEnabled = isBlank,
            };

            control.BindingContext = inputHelper;
            control.SetBinding(SfRating.ValueProperty, new Binding("Value", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.ValueChanged,
                Command = command,
                CommandParameter = inputHelper,
            };

            control.Behaviors.Add(evnt);

            return await Task.FromResult(control);
        }

        public async Task<Grid> CreateDateRange(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            DateTime? dt1 = null;
            DateTime? dt2 = null;
            if (!isBlank)
            {
                var dates = model.Answer.Split(',');
                dt1 = Convert.ToDateTime(dates[0]);
                dt2 = Convert.ToDateTime(dates[1]);
            }

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                StartDateValue = dt1,
                EndDateValue = dt2,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var layout = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(){ Height = GridLength.Star }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(){ Width = GridLength.Star }
                }
            };

            #region START DATE

            var input1 = new NullableDatePicker()
            {
                Style = (Style)Application.Current.Resources["DatePickerStyle"],
                IsEnabled = isBlank,
            };

            input1.BindingContext = inputHelper;
            input1.SetBinding(NullableDatePicker.NullableDateProperty, new Binding("StartDateValue", source: inputHelper));
            input1.SetBinding(NullableDatePicker.DateProperty, new Binding("StartDateValue", source: inputHelper));

            var stack1 = new StackLayout();
            var helper = new SfTextInputLayout
            {
                Hint = "Start date",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input1,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["DateIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
            };

            helper.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            stack1.Children.Add(helper);
            layout.Children.Add(stack1);

            #endregion START DATE

            #region END DATE

            var input2 = new NullableDatePicker()
            {
                Style = (Style)Application.Current.Resources["DatePickerStyle"],
                IsEnabled = isBlank,
            };

            input2.BindingContext = inputHelper;
            input2.SetBinding(NullableDatePicker.NullableDateProperty, new Binding("EndDateValue", source: inputHelper));
            input2.SetBinding(NullableDatePicker.DateProperty, new Binding("EndDateValue", source: inputHelper));

            var stack2 = new StackLayout();
            var helper2 = new SfTextInputLayout
            {
                Hint = "End date",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input2,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["DateIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
            };

            helper2.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            stack2.Children.Add(helper2);
            layout.Children.Add(stack2, 1, 0);

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input2.Behaviors.Add(evnt);

            #endregion END DATE

            return await Task.FromResult(layout);
        }

        public async Task<Grid> CreateTimeRange(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            TimeSpan? ts1 = null;
            TimeSpan? ts2 = null;
            if (!isBlank)
            {
                var dates = model.Answer.Split(',');
                ts1 = DateTime.ParseExact(dates[0], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                ts2 = DateTime.ParseExact(dates[1], "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            }

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                Question = model.BaseQuestion,
                StartTimeValue = ts1,
                EndTimeValue = ts2,
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var layout = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(){ Height = GridLength.Star }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(){ Width = GridLength.Star }
                }
            };

            #region START DATE

            var input1 = new NullableTimePicker()
            {
                Style = (Style)Application.Current.Resources["TimePickerStyle"],
                IsEnabled = isBlank,
            };

            input1.BindingContext = inputHelper;
            input1.SetBinding(NullableTimePicker.NullableTimeProperty, new Binding("StartTimeValue", source: inputHelper));

            var stack1 = new StackLayout();
            var helper = new SfTextInputLayout
            {
                Hint = "Start time",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input1,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["TimeIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
            };

            helper.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            stack1.Children.Add(helper);

            layout.Children.Add(stack1);

            #endregion START DATE

            #region END DATE

            var input2 = new NullableTimePicker()
            {
                Style = (Style)Application.Current.Resources["TimePickerStyle"],
                IsEnabled = isBlank,
            };

            input2.BindingContext = inputHelper;
            input2.SetBinding(NullableTimePicker.NullableTimeProperty, new Binding("EndTimeValue", source: inputHelper));

            var stack2 = new StackLayout();
            var helper2 = new SfTextInputLayout
            {
                Hint = "End time",
                EnableFloating = false,
                ContainerType = ContainerType.Outlined,
                Style = (Style)Application.Current.Resources["InputLayoutStyleWithIcon"],
                InputView = input2,
                LeadingView = new IconLabel()
                {
                    Text = Application.Current.Resources["TimeIcon"].ToString(),
                    Style = (Style)Application.Current.Resources["IconizeIconStyle"],
                },
                LeadingViewPosition = ViewPosition.Inside,
            };

            helper2.SetBinding(SfTextInputLayout.HasErrorProperty, new Binding("HasError", source: inputHelper));

            stack2.Children.Add(helper2);
            layout.Children.Add(stack2, 1, 0);

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.Unfocused,
                Command = command,
                CommandParameter = inputHelper,
            };

            input2.Behaviors.Add(evnt);

            #endregion END DATE

            return await Task.FromResult(layout);
        }

        public async Task<Grid> CreateSwitchButton(BaseQControlDto model, ICommand command = null)
        {
            bool isBlank = string.IsNullOrWhiteSpace(model.Answer);

            var layout = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(){ Width = GridLength.Auto }
                }
            };

            var inputHelper = new ControlDto()
            {
                Name = model.ID,
                ID = model.ID,
                Label = model.Question,
                ControlTypeId = model.BaseQuestion.ControlTypeId,
                IsChecked = (!isBlank && model.Answer == "1"),
                FormQuestionId = model.BaseQuestion.FormQuestionId,
            };

            var control = new SfSwitch
            {
                BindingContext = inputHelper,
                VisualType = VisualType.Cupertino,
                IsEnabled = isBlank,
            };

            control.SetBinding(SfSwitch.IsOnProperty, new Binding("IsChecked", source: inputHelper));

            var evnt = new EventToCommandBehavior()
            {
                EventName = EventName.StateChanged,
                Command = command,
                CommandParameter = inputHelper,
            };

            var stack = new StackLayout();
            stack.Children.Add(control);
            control.Behaviors.Add(evnt);

            layout.Children.Add(stack);

            return await Task.FromResult(layout);
        }

        public async Task<Xamarin.Forms.View> ProcessControl(BaseQControlDto model, ICommand command = null)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                Xamarin.Forms.View value = null;

                if (string.IsNullOrWhiteSpace(model.ID))
                    model.ID = $"Question{Counter}";

                var controlType = (ControlTypeEnum)model.BaseQuestion.ControlTypeId;

                switch (controlType)
                {
                    case ControlTypeEnum.Textbox:
                        value = await this.CreateTextBox(model, command);
                        break;

                    case ControlTypeEnum.MultilineTextbox:
                        value = await this.CreateTextArea(model, command);
                        break;

                    case ControlTypeEnum.Dropdown:
                        value = await this.CreateComboBox(model, command);
                        break;

                    case ControlTypeEnum.MultiselectDropdown:
                        value = await this.CreateMultiSelect(model, command);
                        break;

                    case ControlTypeEnum.DatePicker:
                        value = await this.CreateDatePicker(model, command);
                        break;

                    case ControlTypeEnum.DateRangePicker:
                        value = await this.CreateDateRange(model, command);
                        break;

                    case ControlTypeEnum.TimePicker:
                        value = await this.CreateTimePicker(model, command);
                        break;

                    case ControlTypeEnum.TimeRangePicker:
                        value = await this.CreateTimeRange(model, command);
                        break;

                    case ControlTypeEnum.CheckboxOption:
                        value = await this.CreateCheckboxList(model, command);
                        break;

                    case ControlTypeEnum.RadioOption:
                        value = await this.CreateRadioButtons(model, command);
                        break;

                    case ControlTypeEnum.SwitchOption:
                        value = await this.CreateSwitchButton(model, command);
                        break;

                    case ControlTypeEnum.StarRating:
                        value = await this.CreateRateControl(model, command);
                        break;

                    default:
                        break;
                }

                Counter++;

                return value;
            }
            else
                return null;
        }
    }
}