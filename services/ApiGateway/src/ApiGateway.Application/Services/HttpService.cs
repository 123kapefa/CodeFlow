using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace ApiGateway.Application.Services;

public class HttpService {

  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IConfiguration _configuration;

  public HttpService (IHttpClientFactory httpClientFactory, IConfiguration _configuration) {
    _httpClientFactory = httpClientFactory;
    this._configuration = _configuration;
  }

  public async Task FetchDataAsync (
    string key
    , string path
    , string method
    , object body
    , Dictionary<string, object> results
    , object resultLock) {
    var client = _httpClientFactory.CreateClient ();

    Console.WriteLine($"\n\n{path}");
    
    path = TransformPath(path);
    
    Console.WriteLine($"\n\n{path}");
    
    // Определяем базовый URL для сервиса на основе пути
    string baseUrl = DetermineServiceUrl (path);
    
    Console.WriteLine($"\n\n{baseUrl}");
    
    if (string.IsNullOrEmpty (baseUrl)) {
      lock (resultLock) {
        results[key] = new { error = "Не удалось определить сервис для запроса" };
      }

      return;
    }

    try {
      HttpResponseMessage response;
      string transformedPath = RemoveApiPrefix(path);
      
      Console.WriteLine($"\n\n{transformedPath}");
      
      var requestUrl = $"{baseUrl}{transformedPath}";

      Console.WriteLine($"\n{requestUrl}\n");
      
      // Формируем запрос на основе метода
      if (method.ToUpper () == "GET") {
        response = await client.GetAsync (requestUrl);
      }
      else if (method.ToUpper () == "POST") {
        var content = new StringContent (
          JsonSerializer.Serialize (body), System.Text.Encoding.UTF8, "application/json");
        response = await client.PostAsync (requestUrl, content);
      }
      else {
        lock (resultLock) {
          results[key] = new { error = $"Метод {method} не поддерживается" };
        }

        return;
      }

      var responseContent = await response.Content.ReadAsStringAsync ();

      lock (resultLock) {
        if (response.IsSuccessStatusCode) {
          try {
            var jsonDoc = JsonDocument.Parse (responseContent);
            results[key] = jsonDoc.RootElement.Clone ();
          }
          catch {
            // Если не JSON, просто возвращаем строку
            results[key] = responseContent;
          }
        }
        else {
          results[key] = new { error = $"Ошибка запроса: {response.StatusCode}", content = responseContent };
        }
      }
    }
    catch (Exception ex) {
      lock (resultLock) {
        results[key] = new { error = $"Исключение: {ex.Message}" };
      }
    }
  }

  private string DetermineServiceUrl (string path) {
    var clusters = _configuration.GetSection ("ReverseProxy:Clusters").GetChildren ();

    if (path.StartsWith ("/api/auth/") || path.StartsWith ("/auth/")) {
      return GetServiceUrlFromCluster ("auth-cluster");
    }
    else if (path.StartsWith ("/api/users/") || path.StartsWith ("/users/")) {
      return GetServiceUrlFromCluster ("user-cluster");
    }
    else if (path.StartsWith ("/api/questions") || path.StartsWith ("/questions")) {
      return GetServiceUrlFromCluster ("question-cluster");
    }
    else if (path.StartsWith ("/api/answers/") || path.StartsWith ("/answers/")) {
      return GetServiceUrlFromCluster ("answer-cluster");
    }
    else if (path.StartsWith ("/api/comments/") || path.StartsWith ("/comments/")) {
      return GetServiceUrlFromCluster ("comment-cluster");
    }
    else if (path.StartsWith ("/api/tags/") || path.StartsWith ("/tags/")) {
      return GetServiceUrlFromCluster ("tag-cluster");
    }

    return null;
  }
  
  private string GetServiceUrlFromCluster (string clusterName) {
    var destinations = _configuration.GetSection ($"ReverseProxy:Clusters:{clusterName}:Destinations").GetChildren ();
    foreach (var destination in destinations) {
      var address = destination.GetValue<string> ("Address");
      if (!string.IsNullOrEmpty (address)) {
        return address;
      }
    }

    return null;
  }
  
  private string TransformPath (string path) {
    // Нормализуем путь
    if (!path.StartsWith ("/") && !path.StartsWith ("api/"))
      path = "/api/" + path;
    else if (!path.StartsWith ("/") && path.StartsWith ("api/"))
      path = "/" + path;

    return path;
  }

  private string RemoveApiPrefix (string path) {
    if (path.StartsWith ("/api/"))
      return path.Substring (5);
    else if (path.StartsWith ("api/"))
      return path.Substring (4);
    return path;
  }

}