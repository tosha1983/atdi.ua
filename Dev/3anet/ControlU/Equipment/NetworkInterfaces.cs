using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace ControlU.Equipment
{
    public class NetworkInterfaces
    {
        public void GetIpAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)//ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
                {
                    //System.Windows.MessageBox.Show(ni.Name);
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            
                            System.Windows.MessageBox.Show(ip.Address.ToString() + "\r\n" + ip.IPv4Mask.ToString());
                        }
                    }
                }
            }
        }
    }
}
