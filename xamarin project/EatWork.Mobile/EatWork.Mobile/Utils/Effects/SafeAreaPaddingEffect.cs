using Xamarin.Forms;

[assembly: ResolutionGroupName("EatWork")]

namespace EatWork.Mobile.Utils.Effects
{
    public class SafeAreaPaddingEffect : RoutingEffect
    {
        public SafeAreaPaddingEffect() : base($"EatWork.{nameof(SafeAreaPaddingEffect)}")
        {
        }
    }
}