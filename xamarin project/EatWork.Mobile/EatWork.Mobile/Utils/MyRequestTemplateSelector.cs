using EatWork.Mobile.Views.Template;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class MyRequestTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate mainTemplate;
        private readonly DataTemplate editTemplate;

        public MyRequestTemplateSelector()
        {
            mainTemplate = new DataTemplate(typeof(MyRequestTemplate));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return mainTemplate;
        }
    }
}