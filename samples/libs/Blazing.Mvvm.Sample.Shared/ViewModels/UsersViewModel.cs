using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Blazing.Mvvm.Sample.Shared.Models;
using Blazing.Mvvm.Sample.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for displaying and managing a list of users, including loading and navigation logic.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class UsersViewModel : ViewModelBase
{
    private readonly ILogger<UsersViewModel> _logger;
    private readonly IMvvmNavigationManager _navigationManager;
    private readonly IUsersService _usersService;

    /// <summary>
    /// Gets or sets the list of users.
    /// </summary>
    [ObservableProperty]
    private List<User> _users = [];

    /// <summary>
    /// Gets or sets a value indicating whether the users are currently loading.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging events.</param>
    /// <param name="navigationManager">The navigation manager for view model navigation.</param>
    /// <param name="usersService">The service for retrieving user data.</param>
    public UsersViewModel(
        ILogger<UsersViewModel> logger,
        IMvvmNavigationManager navigationManager,
        IUsersService usersService)
    {
        _logger = logger;
        _navigationManager = navigationManager;
        _usersService = usersService;
    }

    /// <summary>
    /// Called when the ViewModel is initialized. Loads the list of users.
    /// </summary>
    public override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    /// <summary>
    /// Loads the list of users asynchronously.
    /// </summary>
    private async Task LoadUsersAsync()
    {
        try
        {
            IsLoading = true;
            var users = await _usersService.GetAllUsersAsync();
            Users = users.ToList();
            _logger.LogInformation("Loaded {Count} users", Users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates to the details view for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to view details for.</param>
    [RelayCommand]
    private void ViewUserDetails(string userId)
    {
        _logger.LogInformation("Navigating to user {UserId}", userId);
        _navigationManager.NavigateTo<UserViewModel>(userId);
    }
}
