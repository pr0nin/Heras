using Rssdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signus0x539.Heras
{
    public class HeosSSDP
    {
        public List<SsdpRootDevice> Devices = new List<SsdpRootDevice>();

        public string GetMainIp()
        {
            string mainIp = string.Empty;

            if (Devices.Any())
            {
                var firstDevice = Devices.FirstOrDefault();

                if(firstDevice != null && firstDevice.Location != null)
                {
                    mainIp = firstDevice.Location.DnsSafeHost;
                }
            }

            return mainIp;
        }

        public async Task LocateHeosDevices()
        {
            using (var deviceLocator = new SsdpDeviceLocator())
            {
                var foundDevices = await deviceLocator.SearchAsync("urn:schemas-denon-com:device:ACT-Denon:1");
                // Can pass search arguments here (device type, uuid). No arguments means all devices.

                foreach (var foundDevice in foundDevices)
                {
                    // Device data returned only contains basic device details and location ]
                    // of full device description.

                    // Can retrieve the full device description easily though.
                    var fullDevice = await foundDevice.GetDeviceInfo() as SsdpRootDevice;

                    Devices.Add(fullDevice);
                }
            }
        }
    }
}
