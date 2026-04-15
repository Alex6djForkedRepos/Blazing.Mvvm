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
/// ViewModel for displaying a specific user's post, handling loading, navigation, and error states.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class UserPostViewModel : ViewModelBase
{
    private readonly ILogger<UserPostViewModel> _logger;
    private readonly IMvvmNavigationManager _navigationManager;
    private readonly IUsersService _usersService;
    private readonly IPostsService _postsService;

    /// <summary>
    /// Gets or sets the user whose post is being displayed.
    /// </summary>
    [ObservableProperty]
    private User? _user;

    /// <summary>
    /// Gets or sets the post being displayed.
    /// </summary>
    [ObservableProperty]
    private Post? _post;

    /// <summary>
    /// Gets or sets a value indicating whether the post is currently loading.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Gets or sets a value indicating whether the post was not found.
    /// </summary>
    [ObservableProperty]
    private bool _postNotFound;

    /// <summary>
    /// Gets or sets the user ID parameter from the view.
    /// </summary>
    [ViewParameter]
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the post ID parameter from the view.
    /// </summary>
    [ViewParameter]
    public string? PostId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPostViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging events.</param>
    /// <param name="navigationManager">The navigation manager for view model navigation.</param>
    /// <param name="usersService">The service for retrieving user data.</param>
    /// <param name="postsService">The service for retrieving post data.</param>
    public UserPostViewModel(
        ILogger<UserPostViewModel> logger, 
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
    /// Called when component parameters are set. Loads the post data if user and post IDs are provided.
    /// </summary>
    public override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(PostId))
        {
            await LoadPostDataAsync(UserId, PostId);
        }
    }

    /// <summary>
    /// Loads the user and post data asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to load data for.</param>
    /// <param name="postId">The post ID to load data for.</param>
    private async Task LoadPostDataAsync(string userId, string postId)
    {
        try
        {
            IsLoading = true;
            PostNotFound = false;

            _logger.LogInformation("Loading post {PostId} for user {UserId}", postId, userId);

            // Load user
            User = await _usersService.GetUserByIdAsync(userId);
            
            if (User == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                PostNotFound = true;
                return;
            }

            // Load post
            Post = await _postsService.GetPostByIdAsync(userId, postId);
            
            if (Post == null)
            {
                _logger.LogWarning("Post {PostId} not found for user {UserId}", postId, userId);
                PostNotFound = true;
                return;
            }

            _logger.LogInformation("Loaded post '{PostTitle}' for user {UserName}", Post.Title, User.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post {PostId} for user {UserId}", postId, userId);
            PostNotFound = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the posts list for the current user.
    /// </summary>
    [RelayCommand]
    private void BackToPosts()
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            _logger.LogInformation("Navigating back to posts for user {UserId}", UserId);
            _navigationManager.NavigateTo<UserPostsViewModel>(UserId);
        }
    }

    /// <summary>
    /// Navigates back to the user details view for the current user.
    /// </summary>
    [RelayCommand]
    private void BackToUser()
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            _logger.LogInformation("Navigating back to user {UserId}", UserId);
            _navigationManager.NavigateTo<UserViewModel>(UserId);
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
