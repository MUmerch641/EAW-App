namespace EatWork.Mobile.Contracts
{
    public interface INativeHelper
    {
        void CloseApp();
    }

    public interface IStatusBar
    {
        void HideStatusBar();

        void ShowStatusBar();
    }
}