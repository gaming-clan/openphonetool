using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenPhoneTool.Core
{
    /// <summary>
    /// Provides Android unlocking operations (ADB/Fastboot).
    /// </summary>
    public class AndroidUnlockService
    {
        /// <summary>
        /// Attempts to remove Android screen lock using ADB (requires USB debugging ON).
        /// </summary>
        public async Task<UnlockResult> RemoveScreenLockAsync(DeviceInfo device)
        {
            // Only works if USB debugging is enabled and device is authorized
            string[] keyFiles = {
                "/data/system/gesture.key",
                "/data/system/password.key",
                "/data/system/locksettings.db",
                "/data/system/locksettings.db-wal",
                "/data/system/locksettings.db-shm"
            };
            foreach (var file in keyFiles)
            {
                var rm = await RunAdbAsync(device.Id, $"shell rm {file}");
                if (rm.Contains("No such file") || rm == "") continue;
            }
            await RunAdbAsync(device.Id, "reboot");
            return new UnlockResult
            {
                Success = true,
                Message = "Screen lock files deleted (if present). Device is rebooting."
            };
        }

        /// <summary>
        /// Attempts a factory reset via Fastboot (erases all data).
        /// </summary>
        public async Task<UnlockResult> FastbootFactoryResetAsync(DeviceInfo device)
        {
            // User must manually boot device into Fastboot mode!
            var res = await RunProcessAsync("fastboot", "-w");
            return new UnlockResult
            {
                Success = true,
                Message = "Fastboot -w command sent. If successful, device will be wiped."
            };
        }

        /// <summary>
        /// Attempts FRP bypass via public method if supported, else returns instructions.
        /// </summary>
        public async Task<UnlockResult> FRPBypassAsync(DeviceInfo device)
        {
            // Check for known public exploits (none for modern devices)
            // Here, only manual instructions are returned
            string instructions =
@"No universal automated FRP bypass is available for your device.
For some older models, you may try:
- Connect to WiFi, return to Welcome screen
- Tap Accessibility or Emergency Call, trigger any browser or YouTube loophole
- Use official Google Account recovery: https://accounts.google.com/signin/recovery

Otherwise, seek device-specific instructions on XDA or YouTube.";
            return new UnlockResult
            {
                Success = false,
                Message = "Manual FRP bypass required.",
                ManualInstructions = instructions
            };
        }

        private async Task<string> RunAdbAsync(string id, string args)
        {
            return await RunProcessAsync("adb", $"-s {id} {args}");
        }

        private async Task<string> RunProcessAsync(string exe, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            string output = await p.StandardOutput.ReadToEndAsync();
            string error = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();
            return output.Trim();
        }
    }

    public class UnlockResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ManualInstructions { get; set; }
    }
}