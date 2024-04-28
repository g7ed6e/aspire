// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace CoreWCF.BasicHttpService;

public class LoggingServiceBehavior(IServiceProvider serviceProvider) : IServiceBehavior
{
    public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
    {
        
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
     
    }

    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
        for (int i = 0; i < serviceHostBase.ChannelDispatchers.Count; i++)
        {
            ChannelDispatcher? channelDispatcher = serviceHostBase.ChannelDispatchers[i] as ChannelDispatcher;
            if (channelDispatcher != null)
            {
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    LoggingMessageInspector inspector = new(serviceProvider.GetRequiredService<ILogger<LoggingMessageInspector>>());
                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
                }
        }
    }
}

    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
        
    }
}
