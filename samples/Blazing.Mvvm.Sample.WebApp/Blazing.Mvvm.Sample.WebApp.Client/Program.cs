using Blazing.Mvvm;
using Blazing.Mvvm.Sample.Shared.Data;
using Blazing.Mvvm.Sample.Shared.Services;
using Blazing.Mvvm.Sample.WebApp.Client.Data;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IWeatherService, ClientWeatherService>();
builder.Services.AddSingleton<IUsersService, ClientUsersService>();
builder.Services.AddSingleton<IPostsService, ClientPostsService>();
builder.Services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

// Add Blazing.Mvvm
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.WebApp;
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
    
    // Register Shared ViewModels
    options.RegisterViewModelsFromAssembly(typeof(Blazing.Mvvm.Sample.Shared.ViewModels.MainLayoutViewModel).Assembly);
});

await builder.Build().RunAsync();
