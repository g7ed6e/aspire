// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CoreWCF.ApiService;

public class ServiceDiscoveryChannelFactory<TChannel>(Binding binding, EndpointAddress remoteAddress,
    Microsoft.Extensions.ServiceDiscovery.ServiceEndpointResolver serviceEndpointResolver)
    : ChannelFactory<TChannel>(binding, remoteAddress)
{
    public override TChannel CreateChannel(EndpointAddress address, Uri via)
    {
        if (address.Uri.Scheme is "net.tcp")
        {
#pragma warning disable CA2012 // Use ValueTasks correctly
            var getEndpointsTask = serviceEndpointResolver.GetEndpointsAsync($"tcp://{address.Uri.Host}", CancellationToken.None);
#pragma warning restore CA2012 // Use ValueTasks correctly
            var result = getEndpointsTask.IsCompleted
                ? getEndpointsTask.Result
                : getEndpointsTask.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            var netTcpEndpointUri = new Uri(result.Endpoints[0].ToString()!, UriKind.Absolute);
            EndpointAddressBuilder endpointAddressBuilder = new EndpointAddressBuilder(address);
            UriBuilder uriBuilder = new UriBuilder(address.Uri);
            uriBuilder.Host = netTcpEndpointUri.Host;
            uriBuilder.Port = netTcpEndpointUri.Port;
            endpointAddressBuilder.Uri = uriBuilder.Uri;
            address = endpointAddressBuilder.ToEndpointAddress();
        }

        return base.CreateChannel(address, via);
    }
}
