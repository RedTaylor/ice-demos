// Copyright (c) ZeroC, Inc. All rights reserved.

using Demo;
using System;
using System.Configuration;
using System.Threading;
using ZeroC.Ice;

try
{
    await using var communicator = new Communicator(ref args, ConfigurationManager.AppSettings);
    await communicator.ActivateAsync();

    if (args.Length > 0)
    {
        Console.Error.WriteLine("too many arguments");
        return 1;
    }

    // Shutdown the communicator on Ctrl+C or Ctrl+Break (shutdown always with Cancel = true)
    Console.CancelKeyPress += (eventSender, eventArgs) =>
        {
            Console.Write("shutdown communicator...");
            eventArgs.Cancel = true;
            _ = communicator.ShutdownAsync();
            Console.WriteLine("ok");
        };

    ObjectAdapter adapter = communicator.CreateObjectAdapter("Callback.Server");
    var sender = new CallbackSender();
    adapter.Add("sender", sender);
    await adapter.ActivateAsync();

    var t = new Thread(new ThreadStart(sender.Run));
    t.Start();

    try
    {
        communicator.WaitForShutdown();
    }
    finally
    {
        Console.WriteLine("Destroy sender");
        sender.Destroy();
        t.Join();
        Console.WriteLine("Destroy joined");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}
return 0;
