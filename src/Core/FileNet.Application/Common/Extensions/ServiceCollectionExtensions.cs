﻿using Microsoft.Extensions.DependencyInjection;

namespace FileNet.Application.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
