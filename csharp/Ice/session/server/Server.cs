// Copyright (c) ZeroC, Inc. All rights reserved.

using Demo;
using System;
using System.Configuration;
using ZeroC.Ice;

// using statement - communicator is automatically destroyed at the end of this statement
await using var communicator = new Communicator(ref args, ConfigurationManager.AppSettings);

// Destroy the communicator on Ctrl+C or Ctrl+Break
Console.CancelKeyPress += (sender, eventArgs) => communicator.ShutdownAsync();

if (args.Length > 0)
{
    throw new ArgumentException("too many arguments");
}

ObjectAdapter adapter = communicator.CreateObjectAdapter("SessionFactory");
adapter.Add("SessionFactory", new SessionFactory());
adapter.Activate();
await communicator.WaitForShutdownAsync();
