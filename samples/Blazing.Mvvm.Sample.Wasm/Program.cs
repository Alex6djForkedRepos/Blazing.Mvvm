using Blazing.Mvvm;
using Blazing.Mvvm.Sample.Wasm;
using Blazing.Mvvm.Sample.Wasm.Data;
using Blazing.Mvvm.Sample.Shared.Data;
using Blazing.Mvvm.Sample.Shared.Services;
using Blazing.Mvvm.Sample.Shared.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

// Add application services
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPostsService, PostsService>();

// Add Blazing.Mvvm
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.WebAssembly;
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
    
    // Register Shared ViewModels
    options.RegisterViewModelsFromAssembly(typeof(MainLayoutViewModel).Assembly);
});

#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

await builder.Build().RunAsync();
