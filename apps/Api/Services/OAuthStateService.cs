using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Services;

public class OAuthStateService : IOAuthStateService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<OAuthStateService> _logger;
    private readonly TimeSpan _stateExpiration = TimeSpan.FromMinutes(10);

    public OAuthStateService(IMemoryCache cache, ILogger<OAuthStateService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public string GenerateState()
    {
        // Generate cryptographically secure random state
        using var rng = RandomNumberGenerator.Create();
        var stateBytes = new byte[32];
        rng.GetBytes(stateBytes);
        
        var state = Convert.ToBase64String(stateBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
        
        // Store state with expiration
        _cache.Set($"oauth_state_{state}", DateTime.UtcNow, _stateExpiration);
        
        _logger.LogDebug("Generated OAuth state: {State}", state);
        return state;
    }

    public bool ValidateState(string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            _logger.LogWarning("OAuth state validation failed: empty state");
            return false;
        }

        var cacheKey = $"oauth_state_{state}";
        if (!_cache.TryGetValue(cacheKey, out DateTime _))
        {
            _logger.LogWarning("OAuth state validation failed: state not found or expired");
            return false;
        }

        // Remove state after successful validation (one-time use)
        _cache.Remove(cacheKey);
        _logger.LogDebug("OAuth state validated successfully: {State}", state);
        return true;
    }

    public void CleanupExpiredStates()
    {
        // MemoryCache handles expiration automatically, but this could be used
        // for additional cleanup if using a different storage mechanism
        _logger.LogDebug("OAuth state cleanup completed");
    }
}
