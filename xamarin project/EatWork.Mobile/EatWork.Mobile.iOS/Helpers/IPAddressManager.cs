using EatWork.Mobile.Contracts;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Xamarin.Forms;

[assembly: Dependency(typeof(EatWork.Mobile.iOS.Helpers.IPAddressManager))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class IPAddressManager : IIPAddressManager
    {
        public string GetIPAddress()
        {
            String ipAddress = "";
            try
            {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = addrInfo.Address.ToString();
                            }
                        }
                    }
                }

                /*return ipAddress;*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
            }
            finally
            {
            }

            return ipAddress;
        }
    }
}