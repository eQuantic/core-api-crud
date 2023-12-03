using System.Reflection;
using System.Security.Claims;
using eQuantic.Core.Application;

namespace eQuantic.Core.Api.Sample;

public class ApplicationContext : IApplicationContext<int>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        LastUpdate = GetLastUpdate();
        Version = GetVersion();
    }
    
    /// <summary>
    /// Gets the last update.
    /// </summary>
    /// <value>
    /// The last update.
    /// </value>
    public DateTime LastUpdate { get; }
    
    /// <summary>
    /// Gets the version.
    /// </summary>
    /// <value>
    /// The version.
    /// </value>
    public string? Version { get; }

    /// <summary>
    /// Gets the local path.
    /// </summary>
    /// <value>
    /// The local path.
    /// </value>
    public string? LocalPath => Path.GetDirectoryName(typeof(ApplicationContext).Assembly.Location);
    
    /// <summary>
    /// Gets the last update
    /// </summary>
    /// <returns>The date time</returns>
    public static DateTime GetLastUpdate()
    {
        var assembly = typeof(Program).Assembly;
        return System.IO.File.GetLastWriteTime(assembly.Location);
    }
        
    /// <summary>
    /// Gets the version
    /// </summary>
    /// <returns>The string</returns>
    public static string? GetVersion()
    {
        var assembly = typeof(Program).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
    public Task<int> GetCurrentUserIdAsync()
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Task.FromResult<int>(default);
        }
        return Task.FromResult(userId);
    }
}