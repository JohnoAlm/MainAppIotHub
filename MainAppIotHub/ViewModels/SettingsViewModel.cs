﻿using IotHubResources.Handlers;
using System.Diagnostics;

namespace MainAppIotHub.ViewModels;

public class SettingsViewModel
{
    private readonly IotHubHandler _iotHubHandler;

    public bool IsInitialized { get;  set; }
    public string? ConnectionString { get; set; }
    public string? Email { get; set; }

    public string? EmailFeedbackMessage { get; set; }

    public SettingsViewModel(IotHubHandler iotHubHandler)
    {
        _iotHubHandler = iotHubHandler;
    }

    public void EnsureInitialized()
    {
        var response = _iotHubHandler.EnsureInitialized();

        if (!response.Succeeded)
            IsInitialized = false;
        else
        {
            IsInitialized = true;
            ConnectionString = response.Content;
        }
    }

    public bool InitializeIotHubHandler()
    {
        return _iotHubHandler.Initialize(ConnectionString!);
    }

    public void SaveEmail()
    {
        Preferences.Set("UserEmail", Email);
        EmailFeedbackMessage = "Email saved successfully.";
    }

    public void LoadEmail()
    {
        Email = Preferences.Get("UserEmail", string.Empty);

        if (!string.IsNullOrWhiteSpace(Email))
            EmailFeedbackMessage = "Email loaded successfully";
        else
            EmailFeedbackMessage = string.Empty;
    }
}
