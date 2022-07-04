using API.Addition;
using API.Multiplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for calculator
/// </summary>
[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mediator"></param>
    public CalculatorController(ILogger<CalculatorController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentException(null, nameof(logger));
        _mediator = mediator ?? throw new ArgumentException(null, nameof(mediator));
    }

    /// <summary>
    /// Adds two numbers together
    /// </summary>
    /// <param name="term1">First number to add</param>
    /// <param name="term2">Second number to add</param>
    /// <returns>Sum of two numbers</returns>
    [HttpGet("Add/{term1}/{term2}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CalculationResult), 200)]
    public async Task<IActionResult> GetAddition([FromRoute] double term1, [FromRoute] double term2)
    {
        _logger.LogInformation("Received request to add {term1} to {term2}", term1, term2);

        AdditionQuery query = new();
        query.FirstTerm = term1;
        query.SecondTerm = term2;

        CalculationResult result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Multiplies two numbers together
    /// </summary>
    /// <param name="factor1">First number to multiply</param>
    /// <param name="factor2">Second number to multiply</param>
    /// <returns>Sum of two numbers</returns>
    [HttpGet("Multiply/{factor1}/{factor2}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CalculationResult), 200)]
    public async Task<IActionResult> GetMultiplication([FromRoute] double factor1, [FromRoute] double factor2)
    {
        _logger.LogInformation("Received request to multiply {factor1} to {factor2}", factor1, factor2);

        MultiplicationQuery query = new();
        query.FirstFactor = factor1;
        query.SecondFactor = factor2;

        CalculationResult result = await _mediator.Send(query);

        return Ok(result);
    }
}
