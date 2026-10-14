using Microsoft.AspNetCore.Mvc;
using UnitConverterApi.Models;
using UnitConverterApi.Services;

namespace UnitConverterApi.Controllers;

[ApiController]
[Route("api/conversion")]
public sealed class UnitController : ControllerBase
{
    private readonly IUnitConverterService _converterService;

    public UnitController(IUnitConverterService converterService)
    {
        _converterService = converterService;
    }

    [HttpPost("convert")]
    [ProducesResponseType<ConvertResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponse> Convert([FromBody] ConvertRequest request)
    {
        try
        {
            var result = _converterService.Convert(
                request.Value!.Value,
                request.FromUnit,
                request.ToUnit);

            var response = new ConvertResponse
            {
                OriginalValue = request.Value.Value,
                FromUnit = request.FromUnit.Trim(),
                ToUnit = request.ToUnit.Trim(),
                ConvertedValue = Math.Round(result.Value, 6, MidpointRounding.AwayFromZero),
                Category = result.Category
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
