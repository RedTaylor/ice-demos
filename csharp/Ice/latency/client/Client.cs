// Copyright (c) ZeroC, Inc. All rights reserved.

using Demo;
using System;
using System.Configuration;
using System.Diagnostics;
using ZeroC.Ice;

await using var communicator = new Communicator(ref args, ConfigurationManager.AppSettings);
// Activates the communicator. In a simple demo like this one, this activation typically does nothing. It is however
// recommended to always activate a communicator.
await communicator.ActivateAsync();

// Calls DisposeAsync on Ctrl+C or Ctrl+Break, but does not wait until DisposeAsync completes.
Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
        _ = communicator.DestroyAsync();
    };

if (args.Length > 0)
{
    throw new ArgumentException("too many arguments");
}

IPingPrx ping = communicator.GetPropertyAsProxy("Ping.Proxy", IPingPrx.Factory) ??
    throw new ArgumentException("invalid proxy");

// A method needs to be invoked thousands of times before the JIT compiler will convert it to native code.
// To ensure an accurate latency measurement, we need to "warm up" the JIT compiler.

Console.Error.Write("warming up the JIT compiler...");
Console.Error.Flush();
for (int i = 0; i < 20000; i++)
{
    await ping.IcePingAsync();
}
Console.Error.WriteLine("ok");

var watch = new Stopwatch();
watch.Start();
double repetitions = 100000;
Console.Out.WriteLine($"pinging server {repetitions} times (this may take a while)");
for (int i = 0; i < repetitions; i++)
{
    await ping.IcePingAsync();
}
watch.Stop();

Console.Out.WriteLine($"time for {repetitions} pings: {watch.ElapsedMilliseconds}ms");
Console.Out.WriteLine($"time per ping: {watch.ElapsedMilliseconds / repetitions}ms");
