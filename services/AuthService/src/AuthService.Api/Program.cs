using AuthService.Infrastructure;

var builder = WebApplication.CreateBuilder (args);

builder.Services.AddControllers ();

builder.Services.AddOpenApi ();
builder.Services.AddScoped<AuthServiceDbContext> (_ =>
  new AuthServiceDbContext (builder.Configuration.GetConnectionString (nameof(AuthServiceDbContext))!));

var app = builder.Build ();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwaggerUI (options => options.SwaggerEndpoint ("/openapi/v1.json", "AuthService"));
}

app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();