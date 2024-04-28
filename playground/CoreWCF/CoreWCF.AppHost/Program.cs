// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var basicHttpService = builder.AddProject<CoreWCF_BasicHttpService>("basicHttpService", launchProfileName: "http");

var netTcpService = builder.AddProject<CoreWCF_NetTcpService>("netTcpService", launchProfileName: null)
    .WithEndpoint(name: "tcp", port: 10123, targetPort: 10123, scheme: "tcp", isProxied: false);

builder.AddProject<CoreWCF_ApiService>("api")
    .WithReference(basicHttpService)
    .WithReference(netTcpService);

// This project is only added in playground projects to support development/debugging
// of the dashboard. It is not required in end developer code. Comment out this code
// to test end developer dashboard launch experience. Refer to Directory.Build.props
// for the path to the dashboard binary (defaults to the Aspire.Dashboard bin output
// in the artifacts dir).
builder.AddProject<Projects.Aspire_Dashboard>(KnownResourceNames.AspireDashboard);

builder.Build().Run();
