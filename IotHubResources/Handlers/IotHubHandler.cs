using IotHubResources.Models;
using Microsoft.Azure.Devices;
using System.Diagnostics;

namespace IotHubResources.Handlers;

public class IotHubHandler
{
    private readonly string _connectionString = "HostName=OliverA-IoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=6uJ9mNJgNLNBOCMBkS4Ep8gTG+no8cyv5AIoTDHkJFk=";
    private readonly RegistryManager? _registryManager;
    private readonly ServiceClient? _serviceClient;

    public IotHubHandler()
    {
        _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
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
                DeviceType = twin.Properties.Reported["deviceType"].ToString() ?? "",
            };

            // Catches unexpected property name connectionState. Fix!
            bool.TryParse(twin.Properties.Reported["connectionState"].ToString(), out bool connectionState);
            device.ConnectionState = connectionState;

            if (device.ConnectionState)
            {
                bool.TryParse(twin.Properties.Reported["deviceState"].ToString(), out bool deviceState);
                device.DeviceState = deviceState;
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
}
