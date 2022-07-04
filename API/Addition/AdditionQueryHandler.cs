using API.Rabbit;
using Common;
using Common.Messages;
using MediatR;

namespace API.Addition;

/// <summary>
/// Handler for addition query
/// </summary>
public class AdditionQueryHandler : IRequestHandler<AdditionQuery, CalculationResult>
{
    private readonly IRabbitQueue rabbitQueue;

    /// <summary>
    /// Creates handler with rabbit queue interface
    /// </summary>
    /// <param name="_rabbitQueue"></param>
    public AdditionQueryHandler(IRabbitQueue _rabbitQueue)
    {
        rabbitQueue = _rabbitQueue;
    }

    /// <inheritdoc/>
    public Task<CalculationResult> Handle(AdditionQuery request, CancellationToken cancellationToken)
    {
        AdditionMessage additionMessage = new(request.FirstTerm, request.SecondTerm);

        CalculationResult res = new();
        res.Result = additionMessage.Result;

        rabbitQueue.SendMessage(additionMessage);

        return Task.FromResult(res);
    }
}
