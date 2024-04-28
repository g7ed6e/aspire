// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Contracts;

public class EchoService : IEchoService
{
    public string Echo(string text)
    {
        return text;
    }
}
