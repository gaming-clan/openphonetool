using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OpenPhoneTool.Core
{
    /// <summary>
    /// Provides WhatsApp data backup and restore for Android and iOS.
    /// </summary>
    public class WhatsAppTransferService
    {
        /// <summary>
        /// Backs up WhatsApp data from Android via ADB.
        /// </summary>
        public async Task<WhatsAppResult> BackupWhatsAppAndroidAsync(DeviceInfo device, string backupDir)
        {
            string localPath = Path.Combine(backupDir, $"whatsapp_{device.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.ab");
            var backup = await RunAdbAsync(device.Id, $"backup -f \"{localPath}\" -apk -shared com.whatsapp");
            if (File.Exists(localPath))
                return new WhatsAppResult { Success = true, Message = $"Backup saved to {localPath}" };
            else
                return new WhatsAppResult { Success = false, Message = "Backup failed. User may need to unlock device to confirm." };
        }

        /// <summary>
        /// Restores WhatsApp data to Android via ADB.
        /// </summary>
        public async Task<WhatsAppResult> RestoreWhatsAppAndroidAsync(DeviceInfo device, string backupFile)
        {
            if (!File.Exists(backupFile))
                return new WhatsAppResult { Success = false, Message = "Backup file does not exist." };
            var res = await RunAdbAsync(device.Id, $"restore \"{backupFile}\"");
            return new WhatsAppResult { Success = true, Message = "Restore command sent. Watch device for prompts." };
        }

        /// <summary>
        /// Backs up WhatsApp from iOS device via iTunes backup dir or idevicebackup2.
        /// </summary>
        public async Task<WhatsAppResult> BackupWhatsAppIOSAsync(DeviceInfo device, string backupDir)
        {
            var backupPath = Path.Combine(backupDir, $"ios_{device.Id}_{DateTime.Now:yyyyMMdd_HHmmss}");
            var res = await RunProcessAsync("idevicebackup2", $"backup \"{backupPath}\"");
            if (Directory.Exists(backupPath))
                return new WhatsAppResult { Success = true, Message = $"Backup saved to {backupPath}" };
            else
                return new WhatsAppResult { Success = false, Message = "Backup failed. Is device unlocked and trusted?" };
        }

        /// <summary>
        /// Restores WhatsApp to iOS using idevicebackup2.
        /// </summary>
        public async Task<WhatsAppResult> RestoreWhatsAppIOSAsync(DeviceInfo device, string backupFolder)
        {
            if (!Directory.Exists(backupFolder))
                return new WhatsAppResult { Success = false, Message = "Backup folder does not exist." };
            var res = await RunProcessAsync("idevicebackup2", $"restore \"{backupFolder}\"");
            return new WhatsAppResult { Success = true, Message = "Restore command sent. Watch device for prompts." };
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

    public class WhatsAppResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}