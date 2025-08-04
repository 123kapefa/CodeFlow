namespace Contracts.Requests.ApiGateway;

public class RequestItem {

  public string Key { get; set; }
  public string Path { get; set; }
  public string Method { get; set; }
  public object Body { get; set; }

}