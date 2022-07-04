using MediatR;

namespace API.Addition;

/// <summary>
/// Query representing addition
/// </summary>
public class AdditionQuery : IRequest<CalculationResult>
{
    /// <summary>
    /// First term of the sum
    /// </summary>
    public double FirstTerm { get; set; }
    /// <summary>
    /// Second term of the sum
    /// </summary>
    public double SecondTerm {  get; set; }
}
