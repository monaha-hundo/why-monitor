using static Utiliies;

var tasks = new List<YMonitorTask>()
{
    /// Simple heartbeat.
    new YMonitorTask("Hello World", TimeSpan.Zero, TimeSpan.Zero, async (task)=>{
        await Log("Hello World ");
        await Log("This task is started with no delay and won't run again.");
    }),

    /// Simple heartbeat.
    new YMonitorTask("Heartbeat", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), async (task)=>{
        await Log("This task repeat every 30 seconds after 30 seconds.");
    }),

    /// Check if emails are being sent.
    new YMonitorTask("Exception Thrower", TimeSpan.FromHours(1), async (task)=>{
        await Log("Starting basic email automation test.");
        await SendEmail(
            "automations@localhost",
            "someone@localhost",
            "Test",
            "Hello from a ymonitor task");
    })
};

var run = true;
void HandleCtrlC(object? sender, ConsoleCancelEventArgs e)
{
    run = false;
    foreach (var task in tasks)
    {
        task?.Dispose();
    }
}
Console.CancelKeyPress += HandleCtrlC;

while (run)
{
    foreach (var task in tasks){
        Console.WriteLine($"Task: \t{task.Name} \tPeriod: {task.Period}");
    }
    await Task.Delay(1000);
}