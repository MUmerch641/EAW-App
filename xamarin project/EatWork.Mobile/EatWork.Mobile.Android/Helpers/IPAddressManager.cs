using EatWork.Mobile.Contracts;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(EatWork.Mobile.Droid.Helpers.IPAddressManager))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class IPAddressManager : IIPAddressManager
    {
        public string GetIPAddress()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}