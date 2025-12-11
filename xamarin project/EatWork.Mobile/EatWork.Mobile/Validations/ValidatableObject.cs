using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EatWork.Mobile.Validations
{
    public class ValidatableObject<T> : ExtendedBindableObject, IValidity
    {
        private readonly List<IValidationRule<T>> _validations;
        private List<string> _errors;
        private T _value;

        public List<IValidationRule<T>> Validations => _validations;

        public List<string> Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
                RaisePropertyChanged(() => IsValid);
                RaisePropertyChanged(() => Message);
            }
        }

        public bool IsValid => !Errors.Any();
        public string Message => Errors.FirstOrDefault();

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public ValidatableObject()
        {
            _errors = new List<string>();
            _validations = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            IEnumerable<string> errors = _validations
                .Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();

            return IsValid;
        }
    }
}
