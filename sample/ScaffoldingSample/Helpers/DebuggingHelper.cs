using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScaffoldingSample.Helpers;

public class DebuggingHelper
{
    public static async Task<bool> WaitForDebuggerAsync(TimeSpan? limit = null)
    {
        limit ??= TimeSpan.FromSeconds(30);
        var source = new CancellationTokenSource(limit.Value);
        
        Console.WriteLine($"◉ Waiting {limit.Value.TotalSeconds} secs for debugger (PID: {Environment.ProcessId})...");

        try
        {
            await Task.Run(async () => {
                while (!Debugger.IsAttached) {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), source.Token);
                }
            }, source.Token);
        }
        catch (OperationCanceledException)
        {
            // it's ok
        }

        Console.WriteLine(Debugger.IsAttached 
            ? "✔ Debugger attached" 
            : "✕ Continuing without debugger");

        return Debugger.IsAttached;
    }
    
    public static bool WaitForDebugger(TimeSpan? limit = null)
    {
        limit ??= TimeSpan.FromSeconds(30);
        
        Console.WriteLine($"◉ Waiting {limit.Value.TotalSeconds} secs for debugger (PID: {Environment.ProcessId})...");

        try
        {
            while (!Debugger.IsAttached) {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }
        catch (OperationCanceledException)
        {
            // it's ok
        }

        Console.WriteLine(Debugger.IsAttached 
            ? "✔ Debugger attached" 
            : "✕ Continuing without debugger");

        return Debugger.IsAttached;
    }
}