﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Behaviours;
using TravelInspiration.API.Shared.Networking;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDestinationSearchApiClient, DestinationSearchApiClient>();
        services.RegisterSlices();
        var currentAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(currentAssembly)
                .RegisterServicesFromAssemblies(currentAssembly)
                .AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
                
        });
        return services;
    }

    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<TravelInspirationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TravelInspirationDbConnection")));
        return services;
    }
}
