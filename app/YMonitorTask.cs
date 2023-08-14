using System.Diagnostics;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class YMonitorTask : IDisposable
{
    public Func<YMonitorTask, Task> Action { get; set; }
    public string Name { get; set; }
    public TimeSpan Period { get; set; }
    public Timer Timer { get; set; }
    private TextWriter _log { get; set; }
    public bool TimeInLog { get; set; } = true;

    public YMonitorTask(string name, TimeSpan period, Func<YMonitorTask, Task> action)
    {
        _log = new StreamWriter(new MemoryStream());
        Action = action;
        Name = name;
        Period = period;
        var callback = new TimerCallback(Callback);
        Timer = new Timer(callback, this, TimeSpan.Zero, period);
    }

    public YMonitorTask(string name, TimeSpan delay, TimeSpan period, Func<YMonitorTask, Task> action)
    {
        Action = action;
        Name = name;
        Period = period;
        var callback = new TimerCallback(Callback);
        Timer = new Timer(callback, this, delay, period);
    }

    private async void Callback(object? state)
    {
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await Log($"🦿 Running task '{Name}'.");
            await Action.Invoke(this);
            await Log($"🦾 Ran task '{Name}' in {stopwatch.Elapsed}s.");
        }
        catch (Exception ex)
        {
            await Log($"🚫 Task '{Name}' failed with an exception:");
            await Log(JsonConvert.SerializeObject(ex, Formatting.Indented));
        }
    }

    private Task Log(string message, ConsoleColor? color = null)
    {
        var oldColor = Console.ForegroundColor;
        if (color.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        if (!TimeInLog)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
        var time = DateTimeOffset.Now.ToLocalTime();
        Console.WriteLine(time + ": " + message);
        if (color.HasValue)
        {
            Console.ForegroundColor = oldColor;
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _log?.Dispose();
    }
}