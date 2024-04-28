using System.ServiceModel;
using System.Text;
using ApiService;
using CoreWCF.ApiService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddChannelFactory<IEchoService>("basic", new BasicHttpBinding(), new EndpointAddress("http://basicHttpService/EchoService.svc"));
builder.Services.AddChannelFactory<IEchoService>("netTcp", new NetTcpBinding(), new EndpointAddress("net.tcp://netTcpService/EchoService/netTcp"));

var app = builder.Build();

app.MapGet("/", async ([FromKeyedServices("basic")]ServiceDiscoveryChannelFactory<IEchoService> basicChannelFactory,
    [FromKeyedServices("netTcp")] ServiceDiscoveryChannelFactory<IEchoService> netTcpChannelFactory
    ) =>
{
    var sb = new StringBuilder();

    var basicHttpChannel = basicChannelFactory.CreateChannel();
    sb.AppendLine(await basicHttpChannel.EchoAsync("Hello BasicHttpBinding!"));

    var netTcpChannel = netTcpChannelFactory.CreateChannel();
    sb.AppendLine(await netTcpChannel.EchoAsync("Hello NetTcpBinding!"));

    return sb.ToString();
});

app.MapGet("/debug", async ([FromServices] ServiceEndpointResolver serviceEndpointResolver) =>
{
    var sb = new StringBuilder();

    var endpoints = await serviceEndpointResolver.GetEndpointsAsync("http://basicHttpService", default);
    sb.AppendLine(endpoints.Endpoints[0].EndPoint.ToString());
    endpoints = await serviceEndpointResolver.GetEndpointsAsync("tcp://netTcpService", default);
    sb.AppendLine(endpoints.Endpoints[0].EndPoint.ToString());

    return sb.ToString();

});

app.Run();
