using blecon.BGLocationTracking.Services;
using blecon.BGLocationTracking.ViewModels;
using blecon.BGLocationTracking.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace blecon.BGLocationTracking;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .RegisterServices()
            .RegisterViewsAndViewModels();


#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<IPermissionService, PermissionService>();

        return builder;
    }

    private static MauiAppBuilder RegisterViewsAndViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<MainPage, MainPageViewModel>();

        return builder;
    }
}
