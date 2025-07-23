using Serilog;
using Serilog.Events;

namespace QuestionService.Api.Extensions;

public static class SerilogExtensions {

  public static WebApplicationBuilder AddCustomSerilog (this WebApplicationBuilder builder) {
    
    Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Debug()
     .WriteTo.Console()
     .WriteTo.Logger(lc => lc
       .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Error)
       .WriteTo.File("Logs/Errors/error-.log", rollingInterval: RollingInterval.Day))
     .WriteTo.Logger(lc => lc
       .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Warning)
       .WriteTo.File("Logs/Warnings/warning-.log", rollingInterval: RollingInterval.Day))
     .WriteTo.Logger(lc => lc
       .Filter.ByIncludingOnly(le => le.Level == LogEventLevel.Information)
       .WriteTo.File("Logs/Infos/info-.log", rollingInterval: RollingInterval.Day))
     .CreateLogger();

    builder.Host.UseSerilog();
    
    return builder;
  }

}