// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel;
using System.ServiceModel.Channels;
using ApiService;
using Microsoft.Extensions.ServiceDiscovery;
using Microsoft.Extensions.ServiceDiscovery.Http;

namespace CoreWCF.ApiService;

public static class SystemServiceModelServiceCollectionExtensions
{
    public static IServiceCollection AddChannelFactory<TChannel>(this IServiceCollection services, string connectionName, Binding binding, EndpointAddress remoteAddress)
    {
        services.AddKeyedSingleton<ServiceDiscoveryChannelFactory<TChannel>>(connectionName, (provider, key) => {
            var serviceEndpointResolver = provider.GetRequiredService<ServiceEndpointResolver>();
            var channelFactory = new ServiceDiscoveryChannelFactory<TChannel>(binding, remoteAddress, serviceEndpointResolver);
            var logger = provider.GetRequiredService<ILogger<LoggingMessageInspector>>();
            var loggingEndpointBehavior = new LoggingEndpointBehavior(logger);
            channelFactory.Endpoint.EndpointBehaviors.Add(loggingEndpointBehavior);

            if (binding.Scheme is "http" or "https")
            {
                var serviceDiscoveryHttpMessageHandlerFactory = provider.GetRequiredService<IServiceDiscoveryHttpMessageHandlerFactory>();
                var endpointBehavior = new ServiceDiscoveryEndpointBehavior(serviceDiscoveryHttpMessageHandlerFactory);
                channelFactory.Endpoint.EndpointBehaviors.Add(endpointBehavior);
            }
            
            return channelFactory;
        });
        return services;
    }
}
