using System;

namespace EatWork.Mobile.Contracts
{
    /// <summary>
    /// https://theconfuzedsourcecode.wordpress.com/2015/05/16/how-to-easily-get-device-ip-address-in-xamarin-forms-using-dependencyservice/
    /// </summary>
    public interface IIPAddressManager
    {
        String GetIPAddress();
    }
}