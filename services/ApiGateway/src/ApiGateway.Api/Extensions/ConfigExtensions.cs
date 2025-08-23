namespace ApiGateway.Api.Extensions;

public static class ConfigExtensions {

  public static WebApplicationBuilder AddConfig (this WebApplicationBuilder builder) {
    
    if (builder.Environment.IsDevelopment()) {
      DotNetEnv.Env.TraversePath().Load();
    }
    
    var configurationBuilder = new ConfigurationBuilder ()
     .SetBasePath (Directory.GetCurrentDirectory ())
     .AddJsonFile ("appsettings.json")
     .AddEnvironmentVariables();

    var configuration = configurationBuilder.Build();
    
    builder.Services.AddReverseProxy()
     .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    
    return builder;
  }

}