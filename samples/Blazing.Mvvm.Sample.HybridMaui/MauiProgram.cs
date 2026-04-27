using Blazing.Mvvm.Sample.HybridMaui.Data;
using Blazing.Mvvm.Sample.Shared.Data;
using Blazing.Mvvm.Sample.Shared.Services;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Sample.HybridMaui;

public static class MauiProgram
{
#pragma warning disable CA1416
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

        builder.Services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
    
    // Add application services
    builder.Services.AddScoped<IWeatherService, WeatherService>();
    builder.Services.AddScoped<IUsersService, UsersService>();
    builder.Services.AddScoped<IPostsService, PostsService>();

        builder.Services.AddMvvm(options =>
        { 
            options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
            options.HostingModelType = BlazorHostingModelType.HybridMaui;
            
            // Register Shared ViewModels
            options.RegisterViewModelsFromAssembly(typeof(Blazing.Mvvm.Sample.Shared.ViewModels.MainLayoutViewModel).Assembly);
        });            

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
#pragma warning restore CA1416
}