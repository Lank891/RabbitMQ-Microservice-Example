using MediatR;

namespace API.Multiplication;

/// <summary>
/// Query representing multiplication
/// </summary>
public class MultiplicationQuery : IRequest<CalculationResult>
{
    /// <summary>
    /// First factor of the product
    /// </summary>
    public double FirstFactor { get; set; }
    /// <summary>
    /// Second factor of the product
    /// </summary>
    public double SecondFactor { get; set; }
}
