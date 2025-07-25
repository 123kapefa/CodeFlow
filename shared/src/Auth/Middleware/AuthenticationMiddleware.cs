using Microsoft.AspNetCore.Http;

namespace Auth.Middleware;

public class AuthenticationMiddleware : IMiddleware {

  public async Task InvokeAsync (HttpContext context, RequestDelegate next) {
    // Проверка токена, логирование, возможно IP rate-limiting
    await next (context);
  }

}