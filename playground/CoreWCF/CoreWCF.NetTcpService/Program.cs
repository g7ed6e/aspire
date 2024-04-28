// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Contracts;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.NetTcpService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

const int NetTcpPort = 10123;
builder.WebHost.UseNetTcp(System.Net.IPAddress.Any, NetTcpPort);
builder.Services.AddServiceModelServices();
builder.Services.AddTransient<EchoService>();
builder.Services.AddSingleton<IServiceBehavior, LoggingServiceBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<EchoService>(options =>
    {
        options.BaseAddresses.Clear();
        options.BaseAddresses.Add(new Uri($"net.tcp://localhost:{NetTcpPort}/EchoService"));
    });
    serviceBuilder.AddServiceEndpoint<EchoService, IEchoService>(new NetTcpBinding(), "netTcp");
});

app.Run();
