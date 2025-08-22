using Microsoft.AspNetCore.Authentication.Google;

namespace AuthService.Api;

public static class AuthSchemes {
    public const string ExternalCookie = "External";
    public const string Google = GoogleDefaults.AuthenticationScheme; // "Google"
    public const string GitHub = "GitHub";                            // кастом
}