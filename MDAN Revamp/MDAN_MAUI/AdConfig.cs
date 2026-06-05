using System;

namespace MDAN_MAUI
{
    public static class AdConfig
    {
        public static string BannerAdUnitId
        {
            get
            {
#if DEBUG
                return Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.Android
                    ? "ca-app-pub-3940256099942544/6300978111" // Google Android Test Banner
                    : "ca-app-pub-3940256099942544/2934735716"; // Google iOS Test Banner
#else
                return "ca-app-pub-4484872581755769/6612935997"; // Production Banner
#endif
            }
        }
    }
}
