namespace DepthChartsWeb.Controllers.RequestsDto;

public sealed class AddPlayerToDepthChartRequestDto
{
    public string? Position { get; set; }
    public PlayerDto? Player { get; set; }
}