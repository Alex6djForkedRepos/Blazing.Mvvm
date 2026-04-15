using Blazing.Mvvm;
using Blazing.SubpathHosting.Server.Components;
using Blazing.SubpathHosting.Server.Data;
using Blazing.Mvvm.Sample.Shared.Data;
using CommunityToolkit.Mvvm.Messaging;
using Blazing.Mvvm.Sample.Shared.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);


// Add application services
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPostsService, PostsService>();

// Add Blazing.Mvvm
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.Server;
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
    
    // Register ViewModels from the Shared assembly
    options.RegisterViewModelsFromAssembly(typeof(Blazing.Mvvm.Sample.Shared.ViewModels.MainLayoutViewModel).Assembly);
});

#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

WebApplication app = builder.Build();

// Configure path base FIRST - before any middleware that generates URLs
app.UsePathBase("/fu/bar");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Blazing.Mvvm.Sample.Shared.Pages.Counter).Assembly);

await app.RunAsync();