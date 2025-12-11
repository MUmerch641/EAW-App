using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ISignalRDataService
    {
        Task StartConnectionAsync();

        Task StopConnectionAsync();

        void ShowLocalNotification(string title, string message);
    }
}