using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;

namespace EatWork.Mobile.Models.FormHolder
{
    public class LoginHolder : ExtendedBindableObject
    {
        public LoginHolder()
        {
            UserModel = new UserModel();
            HasImageSetup = false;
            HasLogo = false;
            HasBranding = false;
            TogglePassVisibility = true;
            Username = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();
            EmailAddress = new ValidatableObject<string>();
            EmployeeNo = new ValidatableObject<string>();
            ConfirmPassword = new ValidatableObject<string>();
        }

        private bool rememberCreds_;

        public bool RememberCredential
        {
            get { return rememberCreds_; }
            set { rememberCreds_ = value; RaisePropertyChanged(() => RememberCredential); }
        }

        private UserModel model;

        public UserModel UserModel
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(() => UserModel); }
        }

        private string sourceImage_;

        public string SourceImage
        {
            get { return sourceImage_; }
            set { sourceImage_ = value; RaisePropertyChanged(() => SourceImage); }
        }

        private bool hasImageSetup_;

        public bool HasImageSetup
        {
            get { return hasImageSetup_; }
            set { hasImageSetup_ = value; RaisePropertyChanged(() => HasImageSetup); }
        }

        private bool hasLogo_;

        public bool HasLogo
        {
            get { return hasLogo_; }
            set { hasLogo_ = value; RaisePropertyChanged(() => HasLogo); }
        }

        private string logoSourceImage_;

        public string LogoSourceImage
        {
            get { return logoSourceImage_; }
            set { logoSourceImage_ = value; RaisePropertyChanged(() => LogoSourceImage); }
        }

        private bool hasBranding_;

        public bool HasBranding
        {
            get { return hasBranding_; }
            set { hasBranding_ = value; RaisePropertyChanged(() => HasBranding); }
        }

        private string brandingSourceImage_;

        public string BrandingSourceImage
        {
            get { return brandingSourceImage_; }
            set { brandingSourceImage_ = value; RaisePropertyChanged(() => BrandingSourceImage); }
        }

        private bool togglePassVisibility_;

        public bool TogglePassVisibility
        {
            get { return togglePassVisibility_; }
            set { togglePassVisibility_ = value; RaisePropertyChanged(() => TogglePassVisibility); }
        }

        private ValidatableObject<string> userName_;

        public ValidatableObject<string> Username
        {
            get { return userName_; }
            set { userName_ = value; RaisePropertyChanged(() => Username); }
        }

        private ValidatableObject<string> password_;

        public ValidatableObject<string> Password
        {
            get { return password_; }
            set { password_ = value; RaisePropertyChanged(() => Password); }
        }

        private ValidatableObject<string> emailAddress_;

        public ValidatableObject<string> EmailAddress
        {
            get { return emailAddress_; }
            set { emailAddress_ = value; RaisePropertyChanged(() => EmailAddress); }
        }

        private ValidatableObject<string> employeeNo_;

        public ValidatableObject<string> EmployeeNo
        {
            get { return employeeNo_; }
            set { employeeNo_ = value; RaisePropertyChanged(() => EmployeeNo); }
        }

        private ValidatableObject<string> confirmPassword_;

        public ValidatableObject<string> ConfirmPassword
        {
            get { return confirmPassword_; }
            set { confirmPassword_ = value; RaisePropertyChanged(() => ConfirmPassword); }
        }

        private bool isSuccess;

        public bool IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; RaisePropertyChanged(() => IsSuccess); }
        }

        public void CopyModel()
        {
            UserModel = new UserModel()
            {
                Username = Username.Value,
                Password = Password.Value,
                EmailAddress = EmailAddress.Value,
                ConfirmPassword = ConfirmPassword.Value,
                EmployeeNo = EmployeeNo.Value,
            };
        }

        public bool IsValidLogin()
        {
            Username.Validations.Clear();
            Password.Validations.Clear();

            Username.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Username is required."
            });

            Password.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Password is required."
            });

            Username.Validate();
            Password.Validate();

            return Password.IsValid && Username.IsValid;
        }

        public bool IsValidForgotPassword()
        {
            Username.Validations.Clear();
            EmailAddress.Validations.Clear();

            Username.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Username is required."
            });

            EmailAddress.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email address is required."
            });

            Username.Validate();
            EmailAddress.Validate();

            if (!string.IsNullOrWhiteSpace(EmailAddress.Value) && (!new ValidationRules().Email(EmailAddress.Value)))
            {
                EmailAddress.Errors.Add("Invalid email address.");
            }

            return Username.IsValid && EmailAddress.IsValid;
        }

        public bool IsValidRegistation()
        {
            Username.Validations.Clear();
            EmployeeNo.Validations.Clear();
            EmailAddress.Validations.Clear();
            Password.Validations.Clear();
            ConfirmPassword.Validations.Clear();

            Username.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Username is required."
            });

            EmployeeNo.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Employee No. is required."
            });

            EmailAddress.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email Address is required."
            });

            Password.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Password is required."
            });

            ConfirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Confirm Password is required."
            });

            Username.Validate();
            EmployeeNo.Validate();
            EmailAddress.Validate();
            Password.Validate();
            ConfirmPassword.Validate();

            if (!string.IsNullOrWhiteSpace(EmailAddress.Value) && (!new ValidationRules().Email(EmailAddress.Value)))
            {
                EmailAddress.Errors.Add("Invalid email address.");
            }

            if (!string.IsNullOrWhiteSpace(ConfirmPassword.Value))
            {
                if (ConfirmPassword.Value != Password.Value)
                    ConfirmPassword.Errors.Add("Password mismatch.");
            }

            return Username.IsValid &&
                    EmployeeNo.IsValid &&
                    EmailAddress.IsValid &&
                    Password.IsValid &&
                    ConfirmPassword.IsValid;
        }
    }
}