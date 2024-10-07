﻿using CommunityToolkit.Maui;
using IotHubResources.Handlers;
using MainAppIotHub.ViewModels;
using Microsoft.Extensions.Logging;

namespace MainAppIotHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<IotHubHandler>();

            return builder.Build();
        }
    }
}
