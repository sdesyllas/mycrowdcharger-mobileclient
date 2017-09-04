using System;
using Android.Content;

namespace MyCrowdCharger.Mobile.Client.Utilities
{
    public static class BatteryManager
    {
        public static int GetCurrentBatteryLevel(Intent batteryIntent)
        {
            int level = batteryIntent.GetIntExtra(Android.OS.BatteryManager.ExtraLevel, -1);
            int scale = batteryIntent.GetIntExtra(Android.OS.BatteryManager.ExtraScale, -1);

            double level_0_to_100 = Math.Floor(level * 100D / scale);
            return (int)level_0_to_100;
        }
    }
}