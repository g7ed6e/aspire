// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Contracts;

[CoreWCF.ServiceContract]
public interface IEchoService
{
    [CoreWCF.OperationContract]
    string Echo(string text);
}
