using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ReputationService.Infrastructure;

namespace ReputationService.Api.Extensions;

public static class DatabaseExtensions {

  public static WebApplicationBuilder AddDatabase (this WebApplicationBuilder builder) {
    builder.Services.AddScoped<ReputationServiceDbContext> (_ =>
      new ReputationServiceDbContext (builder.Configuration.GetConnectionString (nameof(ReputationServiceDbContext))!));

    return builder;
  }

}