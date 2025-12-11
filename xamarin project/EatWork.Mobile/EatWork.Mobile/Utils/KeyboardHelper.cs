using EatWork.Mobile.Contracts;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class KeyboardHelper
    {
        private IKeyboardHelper keyboard_;

        public KeyboardHelper()
        {
            keyboard_ = DependencyService.Get<IKeyboardHelper>();
        }

        public void Dismiss()
        {
            keyboard_.HideKeyboard();
        }
    }
}