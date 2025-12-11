using EatWork.Mobile.Models.FormHolder;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface IAuthenticationDataService
    {
        Task<LoginHolder> Authenticate(LoginHolder form);

        Task<LoginHolder> ForgotPassword(LoginHolder form);

        Task<LoginHolder> Registration(LoginHolder form);

        Task<LoginHolder> RetreiveLoginCredential();

        Task<ConnectionHolder> RetrieveClientSetup(ConnectionHolder form);

        Task<bool> HasClientSetup();
    }
}