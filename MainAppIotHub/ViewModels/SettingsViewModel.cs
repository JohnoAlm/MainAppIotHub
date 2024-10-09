using IotHubResources.Handlers;

namespace MainAppIotHub.ViewModels;

public class SettingsViewModel
{
    private readonly IotHubHandler _iotHubHandler;

    public string? ConnectionString { get; set; }

    public SettingsViewModel(IotHubHandler iotHubHandler)
    {
        _iotHubHandler = iotHubHandler;
    }
    public bool InitializeIotHubHandler()
    {
        return _iotHubHandler.Initialize(ConnectionString!);
    }
}
