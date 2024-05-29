// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using SchemaRegistryAvroProducer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<ISchemaRegistryClient>(provider =>
{
    var schemaRegistryConfig = new SchemaRegistryConfig
    {
        Url = "http://localhost:8081"
    };

    return new CachedSchemaRegistryClient(schemaRegistryConfig);
});
builder.AddKafkaProducer<Null, User>("kafka2", configureBuilder: (provider, producerBuilder) =>
{
    var schemaRegistry = provider.GetRequiredService<ISchemaRegistryClient>();
    producerBuilder.SetValueSerializer(new AvroSerializer<User>(schemaRegistry).AsSyncOverAsync());
});

builder.Services.AddHostedService<ContinuousProducerWorker>();

builder.Build().Run();
