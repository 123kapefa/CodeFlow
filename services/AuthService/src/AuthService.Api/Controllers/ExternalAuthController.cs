using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System;
using AuthService.Domain.Entities;
using Contracts.Publishers.AuthService;
using System.Threading;

namespace AuthService.Api.Controllers;


[ApiController]
[Route("auth/external")]
public class ExternalAuthController : ControllerBase {

    private readonly IExternalTokenService _tokens;
    private readonly IUserDataRepository _users;
    private readonly IOptions<JwtSettings> _jwt;
    private readonly IConfiguration _cfg;
    private readonly IPublishEndpoint _bus;
    private readonly IUserDataRepository _userDataRepository;

    public ExternalAuthController( IExternalTokenService tokens, IUserDataRepository users, IOptions<JwtSettings> jwt, IConfiguration cfg, IPublishEndpoint bus, IUserDataRepository userDataRepository ) {
        _tokens = tokens;
        _users = users;
        _jwt = jwt;
        _cfg = cfg;
        _bus = bus;
        _userDataRepository = userDataRepository;
    }

    [HttpGet("challenge/{provider}")]
    [AllowAnonymous]
    public IActionResult ChallengeExternal( string provider, string? returnUrl = "/" ) {
        var scheme = Request.Scheme;
        var host = Request.Host.Value; // будет localhost:5000 через RequestHeaderOriginalHost

        var redirect = $"{scheme}://{host}/api/auth/external/callback" +
                       $"?provider={Uri.EscapeDataString(provider)}" +
                       $"&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}";

        var props = new AuthenticationProperties { RedirectUri = redirect };
        var schemeName = provider.Equals("GitHub", StringComparison.OrdinalIgnoreCase)
            ? AuthSchemes.GitHub
            : AuthSchemes.Google;

        return Challenge(props, schemeName);
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback( [FromQuery] string provider, [FromQuery] string? returnUrl = "/" ) {

        // 1) Внешняя cookie
        var ext = await HttpContext.AuthenticateAsync(AuthSchemes.ExternalCookie);
        if(!ext.Succeeded || ext.Principal is null)
            return BadRequest("External authentication failed");

        var principal = ext.Principal;

        // 2) Email/Name из разных клеймов (Google/GitHub)
        var email =
            principal.FindFirstValue(ClaimTypes.Email) ??
            principal.FindFirst("email")?.Value ??
            principal.FindFirst("urn:github:email")?.Value;

        var fullName =
            principal.FindFirstValue(ClaimTypes.Name) ??
            principal.FindFirst("name")?.Value ??
            principal.FindFirst("urn:github:name")?.Value;

        if(string.IsNullOrWhiteSpace(email))
            return BadRequest("Email not provided by external provider. Ensure 'email' scope.");

        // 3) Найти/создать пользователя
        var userRes = await _users.GetByEmailAsync(email);

        if(!userRes.IsSuccess || userRes.Value is null) {
            var user = string.IsNullOrWhiteSpace(fullName)
                ? UserData.Create(email)
                : UserData.Create(fullName, email);

            user.EmailConfirmed = true;
            user.IsExternal = true;

            var createRes = await _users.CreateAsync(user, password: null);
            if(!createRes.IsSuccess)
                return StatusCode(500, "Cannot create local user from external login");

            await _users.SaveChangesAsync(HttpContext.RequestAborted);

            // повторная загрузка (и повторная проверка)
            userRes = await _users.GetByEmailAsync(email);
            if(!userRes.IsSuccess || userRes.Value is null)
                return StatusCode(500, "Local user lookup failed after creation");
        }

        var userEntity = userRes.Value;

        // 4) Выпустить токены (и проверить, что не null/пустые)
        var (jwt, refresh) = await _tokens.IssueAsync(userEntity.Id, HttpContext.RequestAborted);
        if(string.IsNullOrWhiteSpace(jwt))
            return StatusCode(500, "Token service returned empty access token");
        if(string.IsNullOrWhiteSpace(refresh))
            return StatusCode(500, "Token service returned empty refresh token");


        var userNameForEvent = !string.IsNullOrWhiteSpace(fullName)
       ? fullName
       : email.Split('@')[0]; // дефолт из почты

        await _bus.Publish(new UserRegistered(userEntity.Id, userNameForEvent), HttpContext.RequestAborted);
        await _userDataRepository.SaveChangesAsync(new CancellationToken(false));


        // 5) HttpOnly refresh cookie (домен фронта)
        var frontBase = _cfg["Auth:FrontendBaseUrl"] ?? "https://codeflow-project.ru";
        string cookieDomain;
        try {
            cookieDomain = new Uri(frontBase).Host;
        }
        catch {
            // если внезапно неверный URL в конфиге
            cookieDomain = "localhost";
        }

        Response.Cookies.Append("refresh_token", refresh, new CookieOptions {
            HttpOnly = true,
            Secure = bool.TryParse(_cfg["JWT_REQUIRE_HTTPS"], out var https) ? https : true,
            SameSite = SameSiteMode.Lax,          // Strict может резать редиректы
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Domain = cookieDomain.Equals("localhost", StringComparison.OrdinalIgnoreCase) ? null : cookieDomain,
            Path = "/"
        });

        // 6) Гасим внешнюю cookie
        await HttpContext.SignOutAsync(AuthSchemes.ExternalCookie);

        // 7) Нормализуем returnUrl (только относительный путь)
        if(string.IsNullOrWhiteSpace(returnUrl) || !Uri.TryCreate(returnUrl, UriKind.Relative, out _))
            returnUrl = "/";

        // 8) Редирект на SPA-колбэк
        var frontCb = _cfg["Auth:FrontendCallback"] ?? "https://codeflow-project.ru/auth/callback";
        var url = $"{frontCb}?jwt={Uri.EscapeDataString(jwt)}&returnUrl={Uri.EscapeDataString(returnUrl)}";
        return Redirect(url);
    }
}
