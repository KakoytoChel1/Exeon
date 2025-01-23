using System.Management;
using System;

namespace Exeon.Models.Tools
{
    public static class SystemBrightnessController
    {
        // change brightness using WMI
        public static void SetBrightness(double brightnessLevel)
        {
            if (brightnessLevel < 0 || brightnessLevel > 100)
                throw new ArgumentOutOfRangeException(nameof(brightnessLevel), "Brightness must be between 0 and 100.");

            using (var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM WmiMonitorBrightnessMethods"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    // Устанавливаем яркость
                    obj.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, (byte)brightnessLevel });
                }
            }
        }
    }
}
