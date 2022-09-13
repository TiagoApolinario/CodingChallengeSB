using System.Reflection;
using DepthChartsApp.Application.Common.Interfaces;
using DepthChartsApp.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DepthChartsApp;

public static class ConfigureServices
{
    public static IServiceCollection SetupApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("ApplicationDbC"));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }
}