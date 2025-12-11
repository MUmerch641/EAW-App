using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EatWork.Mobile.Contracts
{
    public interface IReadWritePermission
    {
        Task<PermissionStatus> CheckStatusAsync();

        Task<PermissionStatus> RequestAsync();
    }

    public class ReadWritePermission
    {

    }
}