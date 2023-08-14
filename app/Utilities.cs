using System.Net.Mail;

public static class Utiliies
{
    public static bool LogTime { get; set; } = true;

    public static Task SendEmail(string from, string to, string subject, string body)
    {
        var smtpClient = new SmtpClient("127.0.0.1", 1025);
        var email = new MailMessage(from, to, subject, body);
        smtpClient.Send(email);
        return Task.CompletedTask;
    }

    public static Task Log(string message)
    {
        if (!LogTime)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
        var time = DateTimeOffset.Now.ToLocalTime();
        Console.WriteLine(time + ": " + message);
        return Task.CompletedTask;
    }
}