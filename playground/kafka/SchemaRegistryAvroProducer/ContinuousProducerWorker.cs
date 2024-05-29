// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Confluent.SchemaRegistry;

namespace SchemaRegistryAvroProducer;

internal sealed class ContinuousProducerWorker(IProducer<Null, User> producer, ILogger<ContinuousProducerWorker> logger, ISchemaRegistryClient schemaRegistryClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PollSchemaRegistryAsync(stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
        long i = 0;
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var message = new Message<Null, User>
            {
                Value = new()
                {
                    name = $"User{i}",
                    favorite_color = "blue",
                    favorite_number = 42
                }
            };
            producer.Produce("topic", message);
            logger.LogInformation($"{producer.Name} sent message '{message.Value}'");
            i++;
        }
    }

    private async Task PollSchemaRegistryAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await schemaRegistryClient.GetAllSubjectsAsync();
                break;
            }
            catch (HttpRequestException)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
