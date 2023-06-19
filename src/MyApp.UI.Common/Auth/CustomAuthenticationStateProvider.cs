using Blazored.LocalStorage;
using Corelibs.Blazor.UIComponents.Common;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyApp.UI.Common.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
{
    private Task<AuthenticationState> _authenticationStateTask;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAntiforgery _xsrf;
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(
        IHttpContextAccessor httpContextAccessor,
        IAntiforgery xsrf,
        ILocalStorageService localStorage)
    {
        _httpContextAccessor = httpContextAccessor;
        _xsrf = xsrf;

        AuthenticationStateChanged += CustomAuthenticationStateProvider_AuthenticationStateChanged;
        _localStorage = localStorage;
    }

    private async void CustomAuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        //var tokens = new TokenProvider
        //{
        //    AccessToken = await httpContext.GetTokenAsync("access_token"),
        //    RefreshToken = await httpContext.GetTokenAsync("refresh_token"),
        //    XsrfToken = _xsrf.GetAndStoreTokens(httpContext).RequestToken
        //};
        //if (tokenValue is not null)
        //{
        //    var dataProtector = _dataProtectionProvider.CreateProtector(typeof(CookieAuthenticationMiddleware).FullName, OpenIdConnectDefaults.AuthenticationScheme, "v2");

        //    UTF8Encoding specialUtf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        //    byte[] protectedBytes = Base64UrlTextEncoder.Decode(tokenValue);
        //    byte[] plainBytes = dataProtector.Unprotect(protectedBytes);
        //    string plainText = specialUtf8Encoding.GetString(plainBytes);

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var jwtToken = tokenHandler.ReadJwtToken(plainText);
        //}

        //tokens.WritePublicFields();
        //Console.WriteLine($"{nameof(tokens.AccessToken)}: {tokens.AccessToken}");
        //Console.WriteLine($"{nameof(tokens.RefreshToken)}: {tokens.RefreshToken}");
        //Console.WriteLine($"{nameof(tokens.XsrfToken)}: {tokens.XsrfToken}");
    }

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_authenticationStateTask is null)
            throw new InvalidOperationException($"{nameof(GetAuthenticationStateAsync)} was called before {nameof(SetAuthenticationState)}.");

        return await _authenticationStateTask;
    }
    
    /// <inheritdoc />
    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask ?? throw new ArgumentNullException(nameof(authenticationStateTask));
        NotifyAuthenticationStateChanged(_authenticationStateTask);
    }

    public async Task<bool> SetAuthenticationStateFromLocalStorage()
    {
        if (await _localStorage.ContainKeyAsync("access_token") is false)
            return false;

        var accessToken = await _localStorage.GetItemAsStringAsync("access_token");

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);

        var expiration = token.ValidTo;
        if (expiration < DateTime.UtcNow)
        {
            await _localStorage.RemoveItemAsync("access_token");
            return false;
        }

        var claims = token.Claims;

        var identity = new ClaimsIdentity(claims, "Custom authentication");
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        SetAuthenticationState(Task.FromResult(state));

        return true;
    }

    public async Task SaveTokensFromHttp()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (accessToken.IsOk())
            await _localStorage.SetItemAsStringAsync("access_token", accessToken);

        var refreshToken = await httpContext.GetTokenAsync("refresh_token");
        if (refreshToken.IsOk())
            await _localStorage.SetItemAsStringAsync("refresh_token", accessToken);
    }

    public async Task ClearTokens()
    {
        await _localStorage.RemoveItemAsync("access_token");
        await _localStorage.RemoveItemAsync("refresh_token");
    }

    public void Dispose()
    {
        AuthenticationStateChanged -= CustomAuthenticationStateProvider_AuthenticationStateChanged;
    }
}
