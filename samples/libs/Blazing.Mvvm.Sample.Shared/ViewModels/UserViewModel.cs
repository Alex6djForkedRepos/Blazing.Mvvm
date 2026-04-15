using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Blazing.Mvvm.Sample.Shared.Models;
using Blazing.Mvvm.Sample.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable BL0007 // Component parameter should be auto property - ViewModel pattern requires manual implementation

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for displaying a user's details and posts, handling loading, navigation, and error states.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class UserViewModel : ViewModelBase
{
    private readonly ILogger<UserViewModel> _logger;
    private readonly IMvvmNavigationManager _navigationManager;
    private readonly IUsersService _usersService;
    private readonly IPostsService _postsService;

    /// <summary>
    /// Gets or sets the user whose details are being displayed.
    /// </summary>
    [ObservableProperty]
    private User? _user;

    /// <summary>
    /// Gets or sets the list of posts for the user.
    /// </summary>
    [ObservableProperty]
    private List<Post> _posts = [];

    /// <summary>
    /// Gets or sets a value indicating whether the data is currently loading.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Gets or sets a value indicating whether the user was not found.
    /// </summary>
    [ObservableProperty]
    private bool _userNotFound;

    /// <summary>
    /// Gets or sets the user ID parameter from the view.
    /// </summary>
    [ViewParameter]
    public string? UserId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging events.</param>
    /// <param name="navigationManager">The navigation manager for view model navigation.</param>
    /// <param name="usersService">The service for retrieving user data.</param>
    /// <param name="postsService">The service for retrieving post data.</param>
    public UserViewModel(
        ILogger<UserViewModel> logger, 
        IMvvmNavigationManager navigationManager,
        IUsersService usersService,
        IPostsService postsService)
    {
        _logger = logger;
        _navigationManager = navigationManager;
        _usersService = usersService;
        _postsService = postsService;
    }

    /// <summary>
    /// Called when component parameters are set. Loads the user data if a user ID is provided.
    /// </summary>
    public override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            await LoadUserDataAsync(UserId);
        }
    }

    /// <summary>
    /// Loads the user and their posts asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to load data for.</param>
    private async Task LoadUserDataAsync(string userId)
    {
        try
        {
            IsLoading = true;
            UserNotFound = false;

            _logger.LogInformation("Loading data for user {UserId}", userId);

            // Load user
            User = await _usersService.GetUserByIdAsync(userId);
            
            if (User == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                UserNotFound = true;
                Posts = [];
                return;
            }

            // Load user's posts
            var posts = await _postsService.GetPostsByUserIdAsync(userId);
            Posts = posts.ToList();

            _logger.LogInformation("Loaded user {UserName} with {PostCount} posts", User.Name, Posts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user data for {UserId}", userId);
            UserNotFound = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates to the posts list for the current user.
    /// </summary>
    [RelayCommand]
    private void ViewUserPosts()
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            _logger.LogInformation("Navigating to posts for user {UserId}", UserId);
            _navigationManager.NavigateTo<UserPostsViewModel>(UserId);
        }
    }

    /// <summary>
    /// Navigates to the details of a specific post for the current user.
    /// </summary>
    /// <param name="postId">The ID of the post to view.</param>
    [RelayCommand]
    private void ViewPost(string postId)
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            _logger.LogInformation("Navigating to post {PostId} for user {UserId}", postId, UserId);
            _navigationManager.NavigateTo<UserPostViewModel>($"{UserId}/{postId}");
        }
    }

    /// <summary>
    /// Navigates back to the users list view.
    /// </summary>
    [RelayCommand]
    private void BackToUsers()
    {
        _logger.LogInformation("Navigating back to users list");
        _navigationManager.NavigateTo<UsersViewModel>();
    }
}
