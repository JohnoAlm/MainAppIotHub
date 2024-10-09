using IotHubResources.Models;
using Microsoft.Azure.Devices;
using System.Diagnostics;

namespace IotHubResources.Handlers;

public class IotHubHandler
{
    //private readonly string _connectionString = "HostName=OliverA-IoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=6uJ9mNJgNLNBOCMBkS4Ep8gTG+no8cyv5AIoTDHkJFk=";
    private string? _connectionString;
    private  RegistryManager? _registryManager;
    private ServiceClient? _serviceClient;

    //public IotHubHandler()
    //{
    //    _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
    //    _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
    //}

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

    public bool Disconnect()
    {
        try
        {
            _registryManager!.Dispose();
            _serviceClient!.Dispose();

            if (_registryManager == null && _serviceClient == null)
                return true;

            return false;
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

    public async Task SendDirectMethodAsync(string deviceId, string methodName)
    {
        var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(10) };
        var response = await _serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInvocation);
    }

    public async Task<IotDeviceInstance> RegisterDeviceAsync(string deviceId, string deviceName)
    {
        if (string.IsNullOrEmpty(deviceId))
            return null!;

        var iotDeviceInstance = new IotDeviceInstance
        {
            Device = await _registryManager!.GetDeviceAsync(deviceId) ?? await _registryManager.AddDeviceAsync(new Device(deviceId))
        };

        await UpdateDesiredPropertyAsync(iotDeviceInstance.Device, nameof(deviceName), deviceName);

        iotDeviceInstance.ConnectionString = GetDeviceConnectionString(iotDeviceInstance.Device);
        iotDeviceInstance.Properties = (await _registryManager.GetTwinAsync(iotDeviceInstance.Device.Id)).Properties;

        return iotDeviceInstance;
    }

    public string GetDeviceConnectionString(Device device)
    {
        var iotDeviceConnectionString = $"{_connectionString!.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
        return iotDeviceConnectionString ?? null!;
    }

    public async Task<bool> UpdateDesiredPropertyAsync(Device device, string key, string value)
    {
        try
        {
            var twin = await _registryManager!.GetTwinAsync(device.Id);
            twin.Properties.Desired[key] = value;

            await _registryManager.UpdateTwinAsync(device.Id, twin, twin.ETag);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
}
