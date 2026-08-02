using System.Net.Http.Json;
using Blazing.Mvvm.Sample.Shared.Models;
using Blazing.Mvvm.Sample.Shared.Services;

namespace Blazing.Mvvm.Sample.WebApp.Client.Data;

/// <summary>
/// Client-side implementation of the users service that calls the server API.
/// </summary>
public class ClientUsersService : IUsersService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientUsersService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public ClientUsersService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await _httpClient.GetFromJsonAsync<IEnumerable<User>>("/api/users");
        return users ?? [];
    }

    /// <inheritdoc/>
    public Task<User?> GetUserByIdAsync(string userId)
        => _httpClient.GetFromJsonAsync<User>($"/api/users/{userId}");

    /// <inheritdoc/>
    public async Task<bool> UserExistsAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        return user != null;
    }
}
