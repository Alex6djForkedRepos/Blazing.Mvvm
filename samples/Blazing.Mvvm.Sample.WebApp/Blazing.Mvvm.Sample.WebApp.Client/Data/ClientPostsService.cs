using System.Net.Http.Json;
using Blazing.Mvvm.Sample.Shared.Models;
using Blazing.Mvvm.Sample.Shared.Services;

namespace Blazing.Mvvm.Sample.WebApp.Client.Data;

/// <summary>
/// Client-side implementation of the posts service that calls the server API.
/// </summary>
public class ClientPostsService : IPostsService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientPostsService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public ClientPostsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
    {
        var posts = await _httpClient.GetFromJsonAsync<IEnumerable<Post>>($"/api/users/{userId}/posts");
        return posts ?? [];
    }

    /// <inheritdoc/>
    public Task<Post?> GetPostByIdAsync(string userId, string postId)
        => _httpClient.GetFromJsonAsync<Post>($"/api/users/{userId}/posts/{postId}");

    /// <inheritdoc/>
    public async Task<bool> PostExistsAsync(string userId, string postId)
    {
        var post = await GetPostByIdAsync(userId, postId);
        return post != null;
    }
}
