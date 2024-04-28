// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace ApiService;

public class LoggingMessageInspector(ILogger<LoggingMessageInspector> logger) : IClientMessageInspector
{
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        var builder = new StringBuilder();

        builder.AppendLine(CultureInfo.InvariantCulture, $"from: {reply.Headers.From}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"to: {reply.Headers.From}");

        MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
        var message = buffer.CreateMessage();
        builder.AppendLine(message.ToString());

        logger.LogInformation(builder.ToString());

        reply = buffer.CreateMessage();
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        var builder = new StringBuilder();

        builder.AppendLine(CultureInfo.InvariantCulture, $"from: {request.Headers.From}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"to: {request.Headers.From}");

        MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
        var message = buffer.CreateMessage();
        builder.AppendLine(message.ToString());

        logger.LogInformation(builder.ToString());

        request = buffer.CreateMessage();
        return new object();
    }
}
