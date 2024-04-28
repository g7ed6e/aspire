// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel;

namespace ApiService;

[ServiceContract]
public interface IEchoService
{
    [OperationContract(Name = "Echo")]
    Task<string> EchoAsync(string text);
}
