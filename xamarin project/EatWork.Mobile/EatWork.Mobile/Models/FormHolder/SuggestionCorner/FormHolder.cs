using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Validations;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.SuggestionCorner
{
    public class FormHolder : RequestHolder
    {
        public FormHolder()
        {
            Suggestions = new ValidatableObject<string>();
            Category = new ValidatableObject<string>();
            Categories = new ObservableCollection<ComboBoxObject>();
            SelectedCategory = new ComboBoxObject();
        }

        private ValidatableObject<string> suggestions_;

        public ValidatableObject<string> Suggestions
        {
            get { return suggestions_; }
            set { suggestions_ = value; RaisePropertyChanged(() => Suggestions); }
        }

        private ValidatableObject<string> category_;

        public ValidatableObject<string> Category
        {
            get { return category_; }
            set { category_ = value; RaisePropertyChanged(() => Category); }
        }

        private ComboBoxObject selectedCategory_;

        public ComboBoxObject SelectedCategory
        {
            get { return selectedCategory_; }
            set { selectedCategory_ = value; RaisePropertyChanged(() => SelectedCategory); }
        }

        private ObservableCollection<ComboBoxObject> categories_;

        public ObservableCollection<ComboBoxObject> Categories
        {
            get { return categories_; }
            set { categories_ = value; RaisePropertyChanged(() => Categories); }
        }

        public bool Isvalid()
        {
            Category.Validations.Clear();
            /*
            Category.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            */

            Suggestions.Validations.Clear();
            Suggestions.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            Category.Validate();
            Suggestions.Validate();

            if (SelectedCategory.Id == 0)
                Category.Errors.Add("");

            return Category.IsValid && Suggestions.IsValid;
        }
    }
}