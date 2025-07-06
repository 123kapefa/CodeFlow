namespace AuthService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder UseBase (this WebApplicationBuilder builder) {
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();

    return builder;
  }

}