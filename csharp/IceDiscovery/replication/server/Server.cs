// Copyright (c) ZeroC, Inc. All rights reserved.

using Demo;
using System;
using ZeroC.Ice;

// using statement - communicator is automatically destroyed at the end of this statement.
await using var communicator = new Communicator(ref args);

// Destroy the communicator on Ctrl+C or Ctrl+Break
Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
        _ = communicator.ShutdownAsync();
    };

if (args.Length > 0)
{
    throw new ArgumentException("too many arguments");
}

ObjectAdapter adapter = communicator.CreateObjectAdapter("Hello");
adapter.Add("hello", new Hello(communicator.GetProperty("Ice.ProgramName")!));
await adapter.ActivateAsync();
await communicator.WaitForShutdownAsync();
