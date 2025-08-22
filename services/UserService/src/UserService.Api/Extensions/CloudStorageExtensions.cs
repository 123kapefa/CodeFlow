using Amazon.S3;

using UserService.Application.Services;
using UserService.Infrastructure.Services;

namespace UserService.Api.Extensions;

public static class CloudStorageExtensions {

  public static WebApplicationBuilder AddCloudStorage (this WebApplicationBuilder builder) {

        var cfg = builder.Configuration;

        builder.Services.AddSingleton<IAmazonS3>(_ =>
            new AmazonS3Client(
                cfg["YANDEXCLOUD_ACCESS_KEY"],
                cfg["YANDEXCLOUD_SECRET_KEY"],
                new AmazonS3Config {
                    ServiceURL = cfg["YANDEXCLOUD_SERVICE_URL"],
                    ForcePathStyle = true
                }
            )
        );

        builder.Services.AddSingleton<IAvatarStorageService>(sp =>
           new AvatarStorageService(
               sp.GetRequiredService<IAmazonS3>(),
               cfg["YANDEXCLOUD_BUCKET_NAME"] ?? throw new ArgumentNullException("YANDEXCLOUD_BUCKET_NAME"),
               "avatars",
               cfg["YANDEXCLOUD_SERVICE_URL"] ?? throw new ArgumentNullException("YANDEXCLOUD_SERVICE_URL")
           )
       );
    
    return builder;
  }

}