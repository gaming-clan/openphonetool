using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OpenPhoneTool.Core
{
    /// <summary>
    /// Manages device detection and communication for Android and iOS devices.
    /// </summary>
    public class DeviceManager
    {
        /// <summary>
        /// Raised when the list of devices changes (device connected/disconnected).
        /// </summary>
        public event Action<List<DeviceInfo>> DevicesChanged;

        /// <summary>
        /// List of currently detected devices.
        /// </summary>
        public List<DeviceInfo> Devices { get; private set; } = new();

        /// <summary>
        /// Starts device detection for Android and iOS (polling every 3s).
        /// </summary>
        public void StartDetection()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var devices = new List<DeviceInfo>();
                    devices.AddRange(await DetectAndroidDevices());
                    devices.AddRange(await DetectIOSDevices());

                    if (DevicesChanged != null)
                        DevicesChanged(devices);

                    Devices = devices;
                    await Task.Delay(3000);
                }
            });
        }

        private async Task<List<DeviceInfo>> DetectAndroidDevices()
        {
            var list = new List<DeviceInfo>();
            try
            {
                var result = await RunProcessAsync("adb", "devices -l");
                var lines = result.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("device") && !line.StartsWith("List"))
                    {
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0)
                        {
                            var id = parts[0];
                            var model = "";
                            foreach (var p in parts)
                                if (p.StartsWith("model:")) model = p.Replace("model:", "");
                            list.Add(new DeviceInfo
                            {
                                Id = id,
                                Manufacturer = "Android",
                                Model = model,
                                Type = DeviceType.Android
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        private async Task<List<DeviceInfo>> DetectIOSDevices()
        {
            var list = new List<DeviceInfo>();
            try
            {
                var result = await RunProcessAsync("idevice_id", "-l");
                var lines = result.Split('\n');
                foreach (var line in lines)
                {
                    var id = line.Trim();
                    if (!string.IsNullOrEmpty(id))
                    {
                        var model = "iPhone/iPad";
                        try
                        {
                            var info = await RunProcessAsync("ideviceinfo", $"-u {id} -k ProductType");
                            if (!string.IsNullOrWhiteSpace(info))
                                model = info.Trim();
                        }
                        catch { }
                        list.Add(new DeviceInfo
                        {
                            Id = id,
                            Manufacturer = "Apple",
                            Model = model,
                            Type = DeviceType.IOS
                        });
                    }
                }
            }
            catch { }
            return list;
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

    /// <summary>
    /// Represents a mobile device.
    /// </summary>
    public class DeviceInfo
    {
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public DeviceType Type { get; set; }
        public string AndroidVersion { get; set; }
    }

    public enum DeviceType
    {
        Android,
        IOS
    }
}