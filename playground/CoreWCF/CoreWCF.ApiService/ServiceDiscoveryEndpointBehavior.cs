// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.ServiceDiscovery.Http;

namespace ApiService;

public class ServiceDiscoveryEndpointBehavior(IServiceDiscoveryHttpMessageHandlerFactory serviceDiscoveryHttpMessageHandlerFactory) : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(GetHttpMessageHandler));
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }

    private HttpMessageHandler GetHttpMessageHandler(HttpClientHandler httpClientHandler)
        => serviceDiscoveryHttpMessageHandlerFactory.CreateHandler(httpClientHandler);
}
