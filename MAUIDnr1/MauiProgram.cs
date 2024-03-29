﻿global using Plugin.Maui.Audio;
global using MAUIDnr1.Models;
global using MAUIDnr1.Services;
global using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace MAUIDnr1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<ApiService>();
        return builder.Build();
	}
}
