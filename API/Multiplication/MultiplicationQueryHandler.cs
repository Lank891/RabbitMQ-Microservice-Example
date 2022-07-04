using API.Rabbit;
using Common;
using Common.Messages;
using MediatR;

namespace API.Multiplication;

/// <summary>
/// Handler for addition query
/// </summary>
public class MultiplicationQueryHandler : IRequestHandler<MultiplicationQuery, CalculationResult>
{
    private readonly IRabbitQueue rabbitQueue;

    /// <summary>
    /// Creates handler with rabbit queue interface
    /// </summary>
    /// <param name="_rabbitQueue"></param>
    public MultiplicationQueryHandler(IRabbitQueue _rabbitQueue)
    {
        rabbitQueue = _rabbitQueue;
    }

    /// <inheritdoc/>
    public Task<CalculationResult> Handle(MultiplicationQuery request, CancellationToken cancellationToken)
    {
        MultiplicationMessage multiplicationMessage = new(request.FirstFactor, request.SecondFactor);

        CalculationResult res = new();
        res.Result = multiplicationMessage.Result;

        rabbitQueue.SendMessage(multiplicationMessage);

        return Task.FromResult(res);
    }
}
