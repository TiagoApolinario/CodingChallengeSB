using DepthChartsApp.Application.DepthCharts.Commands;
using DepthChartsApp.Application.DepthCharts.Queries;
using DepthChartsWeb.Controllers.RequestsDto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DepthChartsWeb.Controllers;

/// <summary>
/// TODO: Clean this controller by introducing a "base" controller that will handle the "Result" content
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class DepthChartController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepthChartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetFullDepthChartAsync()
    {
        var result = await _mediator.Send(new GetFullDepthChartQuery());
        if (result.IsFailure) return NotFound();
        return Ok(result.Value);
    }

    [HttpGet("{position}/{player:int}")]
    public async Task<IActionResult> GetBackupsAsync(string position, int player)
    {
        var command = new GetBackupsQuery(position, player);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("player")]
    public async Task<IActionResult> AddPlayerToDepthChartAsync(AddPlayerToDepthChartRequestDto requestDto)
    {
        if (requestDto.Player == null)
            return BadRequest("Player not informed");

        var command = new AddPlayerToDepthChartCommand(
            requestDto.Position,
            requestDto.Player.Number,
            requestDto.Player.Name,
            requestDto.Player.PositionDepth);

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok() : BadRequest(result.Errors);
    }

    [HttpDelete("{position}/{player:int}")]
    public async Task<IActionResult> RemovePlayerFromDepthChartAsync(string position, int player)
    {
        var command = new RemovePlayerFromDepthChartCommand(position, player);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}