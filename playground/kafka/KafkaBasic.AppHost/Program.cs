// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
    .WithKafkaUI(kafkaUi => kafkaUi.WithHostPort(8080));

builder.AddProject<Projects.Producer>("producer")
    .WithReference(kafka);

builder.AddProject<Projects.Consumer>("consumer")
    .WithReference(kafka);

var schemaRegistry = builder
    .AddContainer(name: "schema-registry", "cp-schema-registry", "7.6.1")
    .WithImageRegistry("confluentinc")
    .WithEndpoint(name: "primary", targetPort: 8081, port: 8081, isProxied: false, scheme: "http");

var kafka2 = builder.AddKafka("kafka2")
    .WithEnvironment(context =>
    {
        var schemaRegistryEndpoint = schemaRegistry.GetEndpoint("primary");
        var schemaRegistryEndpointReference = ReferenceExpression.Create(
            $"http://{schemaRegistryEndpoint.Property(EndpointProperty.Host)}:{schemaRegistryEndpoint.Property(EndpointProperty.Port)}");
         context.EnvironmentVariables["KAFKA_SCHEMA_REGISTRY_URL"] = schemaRegistryEndpointReference;
    })
    .WithKafkaUI();

schemaRegistry.WithEnvironment(context =>
{
    // Get the broker endpoint
    var brokerEndpoint = kafka2.GetEndpoint("internal");
    var brokerConnectionReference =
         ReferenceExpression.Create($"{brokerEndpoint.ContainerHost}:{brokerEndpoint.Property(EndpointProperty.Port)}");
    context.EnvironmentVariables["SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS"] = brokerConnectionReference;
    context.EnvironmentVariables["SCHEMA_REGISTRY_HOST_NAME"] = "localhost";
});

builder.AddProject<Projects.SchemaRegistryAvroProducer>("avroProducer")
    .WithReference(kafka2);

// This project is only added in playground projects to support development/debugging
// of the dashboard. It is not required in end developer code. Comment out this code
// to test end developer dashboard launch experience. Refer to Directory.Build.props
// for the path to the dashboard binary (defaults to the Aspire.Dashboard bin output
// in the artifacts dir).
builder.AddProject<Projects.Aspire_Dashboard>(KnownResourceNames.AspireDashboard);

builder.Build().Run();
