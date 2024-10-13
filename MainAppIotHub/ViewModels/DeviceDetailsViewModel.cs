using Communications.Azure;
using IotHubResources.Handlers;
using IotHubResources.Models;

namespace MainAppIotHub.ViewModels;

public class DeviceDetailsViewModel
{
    private readonly IotHubHandler _iotHubHandler;
    private readonly EmailSender _emailSender;

    public DeviceDetailsViewModel(IotHubHandler iotHubHandler, EmailSender emailSender)
    {
        _iotHubHandler = iotHubHandler;
        _emailSender = emailSender;
    }

    public async Task<IotDevice?> GetDeviceAsync(string deviceId)
    {
        return await _iotHubHandler.GetDeviceAsync(deviceId);
    }

    public async Task RemoveDeviceAsync(string deviceId)
    {
        var success = await _iotHubHandler.RemoveDeviceAsync(deviceId);

        if (success) 
        {
            var toAddress = Preferences.Get("UserEmail", string.Empty);
            _emailSender.SendEmail(toAddress, "Azure IoT Hub Device Deleted", $"<h1>Your Azure IoT Hub device with id: {deviceId} was deleted successfully.</h1>", $"Your Azure IoT Hub device with id: {deviceId} was deleted successfully.");
        }
    }
}
