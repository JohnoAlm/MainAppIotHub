using IotHubResources.Handlers;
using IotHubResources.Models;
using System.Diagnostics;

namespace MainAppIotHub.ViewModels;

public class HomeViewModel
{
    private readonly IotHubHandler _iotHubHandler;

    public Timer? Timer { get; set; }
    public int TimerInterval { get; set; } = 4000;

    public HomeViewModel(IotHubHandler iotHubHandler)
    {
        _iotHubHandler = iotHubHandler;
    }

    public bool EnsureInitialized()
    {
        return _iotHubHandler.EnsureInitialized();
    }

    public async Task<IEnumerable<IotDevice>> GetDevicesAsync()
    {
        return await _iotHubHandler.GetDevicesAsync();    
    }

    public async Task OnDeviceStateChanged(IotDevice iotDevice)
    {
        Timer?.Change(Timeout.Infinite, Timeout.Infinite);
        await SendDirectMethodAsync(iotDevice);
        Timer?.Change(TimerInterval, TimerInterval);
    }

    public async Task SendDirectMethodAsync(IotDevice device)
    {
        var methodName = device.DeviceState ? "off" : "on";
        await _iotHubHandler.SendDirectMethodAsync(device.DeviceId, methodName);
    }
}
