using Amazon.S3;
using Amazon.S3.Model;

using UserService.Application.Services;

namespace UserService.Infrastructure.Services;

public class AvatarStorageService : IAvatarStorageService {

  private readonly IAmazonS3 _s3Client;
  private readonly string _bucketName;
  private readonly string _avatarsFolder;
  private readonly string _endpoint;

  public AvatarStorageService (
    IAmazonS3 s3Client
    , string bucketName = "code-flow-project-storage"
    , string avatarsFolder = "avatars"
    , string endpoint = "https://storage.yandexcloud.net") {
    _s3Client = s3Client;
    _bucketName = bucketName;
    _avatarsFolder = avatarsFolder.Trim ('/');
    _endpoint = endpoint.TrimEnd ('/');
  }

  public async Task<string> SaveAsync (string fileName, Stream stream, CancellationToken cancellationToken) {
    if (string.IsNullOrWhiteSpace (fileName))
      throw new ArgumentException ("fileName is empty");

    var ext = Path.GetExtension (fileName) ?? ".jpg";
    var uniqueFileName = $"{Guid.NewGuid ():N}{ext.ToLowerInvariant ()}";
    var objectKey = $"{_avatarsFolder}/{uniqueFileName}";

    var putRequest = new PutObjectRequest {
      BucketName = _bucketName
      , Key = objectKey
      , InputStream = stream
      , ContentType = GetContentType (ext)
      , CannedACL = S3CannedACL.PublicRead
    };

    var response = await _s3Client.PutObjectAsync (putRequest, cancellationToken);
    if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
      throw new IOException ($"Ошибка загрузки файла в S3: {response.HttpStatusCode}");

    return $"{_endpoint}/{_bucketName}/{objectKey}";
  }

  public async Task DeleteAsync (string relativeUrl, CancellationToken cancellationToken) {
    if (string.IsNullOrWhiteSpace (relativeUrl))
      return;

    string objectKey;

    if (Uri.TryCreate (relativeUrl, UriKind.Absolute, out var uri)) {
      var pathParts = uri.AbsolutePath.Trim ('/').Split ('/', 2);
      objectKey = pathParts.Length > 1 ? pathParts[1] : pathParts[0];
    }
    else {
      objectKey = relativeUrl.TrimStart ('/');
      if (objectKey.StartsWith (_bucketName + "/", StringComparison.OrdinalIgnoreCase))
        objectKey = objectKey[(_bucketName.Length + 1)..];
    }

    await _s3Client.DeleteObjectAsync (_bucketName, objectKey, cancellationToken);
  }

  private static string GetContentType (string ext) {
    return ext.ToLowerInvariant () switch {
      ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".gif" => "image/gif", ".webp" => "image/webp"
      , _ => "application/octet-stream"
    };
  }

}