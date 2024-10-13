using IotHubResources.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;

namespace IotHubResources.Handlers;

public class IotHubHandler
{
    private string? _connectionString;
    private  RegistryManager? _registryManager;
    private ServiceClient? _serviceClient;

    public bool EnsureInitialized()
    {
        if (_registryManager == null && _serviceClient == null)
            return false;

        return true;
    }

    public bool Initialize(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            _connectionString = connectionString;

            return true;
        }
        catch (Exception ex) 
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<IEnumerable<IotDevice>> GetDevicesAsync()
    {      
        var query = _registryManager!.CreateQuery("select * from devices");
        var devices = new List<IotDevice>();

        foreach (var twin in await query.GetNextAsTwinAsync())
        {
            var device = new IotDevice
            {
                DeviceId = twin.DeviceId,
            };

            try { device.DeviceName = twin.Properties?.Reported["deviceName"]?.ToString(); }
            catch { device.DeviceName = "Unknown"; }

            try { device.DeviceType = twin?.Properties?.Reported["deviceType"]?.ToString(); }
            catch { device.DeviceType = "Unknown"; }

            try
            {
                bool.TryParse(twin?.Properties?.Reported["connectionState"]?.ToString(), out bool connectionState);
                device.ConnectionState = connectionState;
            }
            catch { device.ConnectionState = false; }

            if (device.ConnectionState)
            {
                try
                {
                    bool.TryParse(twin?.Properties?.Reported["deviceState"]?.ToString(), out bool deviceState);
                    device.DeviceState = deviceState;
                }
                catch { device.DeviceState = false; }
            }
            else
            {
                device.DeviceState = false;
            }

            devices.Add(device);
        }

        return devices;    
    }

    public async Task<IotDevice?> GetDeviceAsync(string deviceId)
    {
        try
        {
            var twin = await _registryManager!.GetTwinAsync(deviceId);

            var device = new IotDevice
            {
                DeviceId = twin.DeviceId
            };

            try { device.DeviceName = twin.Properties?.Reported["deviceName"]?.ToString(); }
            catch { device.DeviceName = "Unknown"; }

            try { device.DeviceType = twin?.Properties?.Reported["deviceType"]?.ToString(); }
            catch { device.DeviceType = "Unknown"; }

            try
            {
                bool.TryParse(twin?.Properties?.Reported["connectionState"]?.ToString(), out bool connectionState);
                device.ConnectionState = connectionState;
            }
            catch { device.ConnectionState = false; }

            if (device.ConnectionState)
            {
                try
                {
                    bool.TryParse(twin?.Properties?.Reported["deviceState"]?.ToString(), out bool deviceState);
                    device.DeviceState = deviceState;
                }
                catch { device.DeviceState = false; }
            }
            else
            {
                device.DeviceState = false;
            }

            return device;  
        }
        catch (Exception ex) 
        { 
            Debug.WriteLine($"There was an error retrieving the device with id: {deviceId} :: {ex.Message}");
            return null;
        }
    }

    public async Task SendDirectMethodAsync(string deviceId, string methodName)
    {
        var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(10) };
        var response = await _serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInvocation);
    }

    public async Task<bool> RemoveDeviceAsync(string deviceId)
    {
        try
        {
            await _registryManager!.RemoveDeviceAsync(deviceId);
            return true;
        }
        catch (Exception ex) 
        { 
            Debug.WriteLine($"There was an error in removing the device :: {ex.Message}");
            return false;
        }
    }
}
