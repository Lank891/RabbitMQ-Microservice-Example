using Common;
using Common.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using IHost host = Utility.CreateHostBuilder(args).Build();

ILogger<RabbitListener>? rabbitLogger = host.Services.GetRequiredService<ILogger<RabbitListener>>();
ILogger<Program>? mainLogger = host.Services.GetRequiredService<ILogger<Program>>();

RabbitListener listener = new(GlobalSettings.rabbitHost, new string[] { Message.QueueNameForMessageType[Message.Type.Multiplication] }, rabbitLogger);

CancellationTokenSource? cancellationTokenSource = new();
listener.StartAsync(cancellationTokenSource.Token);

while (true)
{
    var message = listener.WaitAndGetNextMessage();

    if (message == null)
        continue;

    MultiplicationMessage? multiplicationMessage = Message.CreateFromRabbitBody<MultiplicationMessage, Program>(message, mainLogger);

    if (multiplicationMessage == null)
        continue;

    Console.WriteLine($"[{DateTime.Now:G}]: Multiplication {multiplicationMessage.Factor1} * {multiplicationMessage.Factor2} = {multiplicationMessage.Result}");
}

public class Program { };