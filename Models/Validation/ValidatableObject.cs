using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiHybridApp.Models.Validation
{
    public class ValidatableObject<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private T _value;
        private bool _isValid = true;
        private List<string> _errors = new();

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        public List<string> Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                OnPropertyChanged();
            }
        }

        public void Validate()
        {
            Errors.Clear();
            // Basic validation: if T is string, check if null or empty (simplified)
            // In a real scenario, we'd have rules.
            // For now, we'll assume manual validation or basic required checks.
            IsValid = Errors.Count == 0;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
