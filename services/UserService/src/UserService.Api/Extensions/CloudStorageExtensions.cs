using Amazon.S3;

using UserService.Application.Services;
using UserService.Infrastructure.Services;

namespace UserService.Api.Extensions;

public static class CloudStorageExtensions {

  public static WebApplicationBuilder AddCloudStorage (this WebApplicationBuilder builder) {
    
    var envPath = Path.Combine("..", "..", "..", "..", ".env");
    DotNetEnv.Env.Load(envPath);
    
    var configurationBuilder = new ConfigurationBuilder ()
     .SetBasePath (Directory.GetCurrentDirectory ())
     .AddJsonFile ("appsettings.json")
     .AddEnvironmentVariables();
    
    builder.Services.AddSingleton<IAmazonS3>(_ => {
      return new AmazonS3Client(
        Environment.GetEnvironmentVariable("YANDEXCLOUD_ACCESS_KEY"),
        Environment.GetEnvironmentVariable("YANDEXCLOUD_SECRET_KEY"),
        new AmazonS3Config {
          ServiceURL = Environment.GetEnvironmentVariable("YANDEXCLOUD_SERVICE_URL"),
          ForcePathStyle = true
        }
      );
    });
    
    builder.Services.AddSingleton<IAvatarStorageService>(sp =>
      new AvatarStorageService(
        sp.GetRequiredService<IAmazonS3>(),
        Environment.GetEnvironmentVariable("YANDEXCLOUD_BUCKET_NAME") ?? throw new ArgumentNullException(),
        "avatars",
        Environment.GetEnvironmentVariable("YANDEXCLOUD_SERVICE_URL") ?? throw new ArgumentNullException()
      )
    );

    
    return builder;
  }

}