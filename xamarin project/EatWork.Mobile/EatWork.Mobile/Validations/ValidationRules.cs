using System.Text.RegularExpressions;

namespace EatWork.Mobile.Validations
{
    public class ValidationRules
    {
        public ValidationRules()
        {
        }

        public bool Email(string value)
        {
            if (value == null)
            {
                return false;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(value);

            return match.Success;
        }
    }
}