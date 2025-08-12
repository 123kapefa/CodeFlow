using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;

namespace ApiGateway.Application.Extensions;

public static class HeaderPropagation
{
  public static void CopyAuthAndTrace(HttpClient http, HttpContext ctx)
  {
    if (ctx.Request.Headers.TryGetValue("Authorization", out var auth))
      http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth!);

    if (ctx.Request.Headers.TryGetValue("traceparent", out var tp))
      http.DefaultRequestHeaders.TryAddWithoutValidation("traceparent", tp.ToString());
    if (ctx.Request.Headers.TryGetValue("x-correlation-id", out var cid))
      http.DefaultRequestHeaders.TryAddWithoutValidation("x-correlation-id", cid.ToString());
  }
}
