using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace IotHubResources.Models;

public class IotDeviceInstance
{
    public string? ConnectionString { get; set; }
    public Device? Device { get; set; }
    public TwinProperties? Properties { get; set; }
}
