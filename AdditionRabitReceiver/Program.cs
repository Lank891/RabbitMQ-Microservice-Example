using Common;
using Common.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using IHost host = Utility.CreateHostBuilder(args).Build();

ILogger<RabbitListener>? rabbitLogger = host.Services.GetRequiredService<ILogger<RabbitListener>>();
ILogger<Program>? mainLogger = host.Services.GetRequiredService<ILogger<Program>>();

RabbitListener listener = new (GlobalSettings.rabbitHost, new string[] { Message.QueueNameForMessageType[Message.Type.Addition] }, rabbitLogger);

CancellationTokenSource? cancellationTokenSource = new();
listener.StartAsync(cancellationTokenSource.Token);

while(true)
{
    var message = listener.WaitAndGetNextMessage();

    if (message == null)
        continue;

    AdditionMessage? additionMessage = Message.CreateFromRabbitBody<AdditionMessage, Program> (message, mainLogger);

    if (additionMessage == null)
        continue;

    Console.WriteLine($"[{DateTime.Now:G}]: Addition {additionMessage.Term1} + {additionMessage.Term2} = {additionMessage.Result}");
}

public class Program { };