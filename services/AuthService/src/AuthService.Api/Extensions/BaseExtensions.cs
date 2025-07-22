using Microsoft.OpenApi.Models;

namespace AuthService.Api.Extensions;

public static class BaseExtensions {

  public static WebApplicationBuilder AddBase (this WebApplicationBuilder builder) {
    
    var config = new ConfigurationBuilder()
     .AddUserSecrets<Program>()
     .Build();
    
    builder.Services.AddControllers ();
    builder.Services.AddEndpointsApiExplorer ();
    builder.Services.AddOpenApi ();

    builder.Services.AddCors(opt => {
      opt.AddPolicy("AllowAll", p => p
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());
    });
    
    return builder;
  }
  
  public static IApplicationBuilder UseBase (this IApplicationBuilder app) {
    
    app.UseDeveloperExceptionPage ();
    app.UseCors("AllowAll");
    return app;
  }

}