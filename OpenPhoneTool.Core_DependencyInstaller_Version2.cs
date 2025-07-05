using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenPhoneTool.Core
{
    /// <summary>
    /// Manages automatic installation of dependencies like ADB, Fastboot, and libimobiledevice.
    /// </summary>
    public class DependencyInstaller
    {
        private string toolsDir = "tools";

        public async Task EnsureDependenciesAsync()
        {
            Directory.CreateDirectory(toolsDir);
            await EnsureAdbAsync();
            await EnsureFastbootAsync();
            await EnsureLibimobiledeviceAsync();
        }

        public async Task<bool> EnsureAdbAsync()
        {
            if (!File.Exists(Path.Combine(toolsDir, "adb.exe")))
            {
                // Download minimal ADB package (user must accept terms!)
                // Example URL: https://dl.google.com/android/repository/platform-tools-latest-windows.zip
                // For this example, prompt user to download manually.
                System.Windows.MessageBox.Show("ADB not found. Please download and extract platform-tools from Google, then copy adb.exe and AdbWinApi.dll to the tools folder.", "Dependency Missing");
                return false;
            }
            return true;
        }

        public async Task<bool> EnsureFastbootAsync()
        {
            if (!File.Exists(Path.Combine(toolsDir, "fastboot.exe")))
            {
                System.Windows.MessageBox.Show("Fastboot not found. Please download and extract platform-tools from Google, then copy fastboot.exe to the tools folder.", "Dependency Missing");
                return false;
            }
            return true;
        }

        public async Task<bool> EnsureLibimobiledeviceAsync()
        {
            if (!File.Exists(Path.Combine(toolsDir, "idevice_id.exe")))
            {
                // Instruct user to download precompiled binaries for Windows
                System.Windows.MessageBox.Show("libimobiledevice tools not found. Please download Windows binaries and copy idevice_id.exe and idevicebackup2.exe to the tools folder.", "Dependency Missing");
                return false;
            }
            return true;
        }
    }
}