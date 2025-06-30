using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure;


// УДАЛИТЬ !!!!!!!! ????????
public static class DependencyInjection {
    //public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
    //    services.AddDbContext<UserServiceDbContext> (options =>
    //    options.UseNpgsql (configuration.GetConnectionString ("DefaultConnection")));

    //    services.AddScoped<IUserInfoService, UserInfoService> ();

    //    return services;
    //}
}
