using DepthChartsApp.Domain;
using FluentAssertions;

namespace DepthChartsAppUnitTests.Domain;

/// <summary>
/// I am not using any test naming conventions here
/// I have used them a lot in the past, but after some readings + experience from real world, I found the approach below the better one
/// That way we can really create small units of tests and it helps to enforce the test of behaviour and not implementation
/// </summary>

public sealed class DepthChartTests
{
    [Test]
    public void New_depth_chart_should_be_successfully_created()
    {
        var depthChartResult = DepthChart.NewDepthChartResult(Guid.NewGuid());
        depthChartResult.IsSuccess.Should().BeTrue();
        depthChartResult.Value.TeamId.Should().NotBeEmpty();
    }

    [Test]
    public void Provided_team_is_invalid()
    {
        var depthChartResult = DepthChart.NewDepthChartResult(Guid.Empty);
        depthChartResult.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void Add_new_position_successfully()
    {
        const string newPositionName = " qb ";
        var depthChartResult = GetDefaultDepthChart();
        depthChartResult.AddPlayerToPosition(newPositionName, 12, "Tom Brady", 1);
        depthChartResult.Positions.Any(x => x.Name == newPositionName.Trim().ToUpper()).Should().BeTrue();
    }

    [Test]
    public void Add_new_player_with_provided_position_depth()
    {
        const int playerNumber = 12;
        const string playerName = " Tom Brady ";
        const int positionDepth = 0;
        var depthChart = GetDefaultDepthChart();

        depthChart.AddPlayerToPosition("QB", playerNumber, playerName, positionDepth);

        GetPlayerFromDepthChart(depthChart, "QB", "Tom Brady").Number.Should().Be(playerNumber);
        GetPlayerFromDepthChart(depthChart, "QB", "Tom Brady").Name.Should().Be(playerName.Trim());
        GetPlayerFromDepthChart(depthChart, "QB", "Tom Brady").PositionDepth.Should().Be(positionDepth);
    }

    [Test]
    public void Add_two_players_in_specific_depth()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;

        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller", 0);
        depthChart.AddPlayerToPosition("LWR", 1, "Jaelon Darden", 1);

        GetPlayerFromDepthChart(depthChart, "LWR", "Scott Miller").PositionDepth.Should().Be(0);
        GetPlayerFromDepthChart(depthChart, "LWR", "Jaelon Darden").PositionDepth.Should().Be(1);
    }

    [Test]
    public void Last_added_player_in_existing_position_gets_priority()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;

        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller", 0);
        depthChart.AddPlayerToPosition("LWR", 1, "Jaelon Darden", 0);
        depthChart.AddPlayerToPosition("LWR", 13, "Mike Evans", 0);

        GetPlayerFromDepthChart(depthChart, "LWR", "Scott Miller").PositionDepth.Should().Be(2);
        GetPlayerFromDepthChart(depthChart, "LWR", "Jaelon Darden").PositionDepth.Should().Be(1);
        GetPlayerFromDepthChart(depthChart, "LWR", "Mike Evans").PositionDepth.Should().Be(0);
    }

    [Test]
    public void Adding_player_without_position_depth_should_place_him_to_end()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;

        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller", 0);
        depthChart.AddPlayerToPosition("LWR", 1, "Jaelon Darden");

        GetPlayerFromDepthChart(depthChart, "LWR", "Scott Miller").PositionDepth.Should().Be(0);
        GetPlayerFromDepthChart(depthChart, "LWR", "Jaelon Darden").PositionDepth.Should().Be(1);
    }

    /// <summary>
    /// Assumption: There can't be gap in between positions
    /// </summary>
    [Test]
    public void Adding_player_skipping_a_position_should_bring_him_next_to_last_added_player()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;

        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller", 0);
        depthChart.AddPlayerToPosition("LWR", 1, "Jaelon Darden", 2);

        GetPlayerFromDepthChart(depthChart, "LWR", "Scott Miller").PositionDepth.Should().Be(0);
        GetPlayerFromDepthChart(depthChart, "LWR", "Jaelon Darden").PositionDepth.Should().Be(1);
    }

    [Test]
    public void Remove_player()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller");
        depthChart.AddPlayerToPosition("LWR", 1, "Jaelon Darden");
        var scottMiller = GetPlayerFromDepthChart(depthChart, "LWR", "Scott Miller");

        depthChart.RemovePlayerFromPosition("LWR", scottMiller.Number);

        depthChart.Positions[0].Players.Count.Should().Be(1);
        depthChart.Positions[0].Players[0].Number.Should().Be(1);
        depthChart.Positions[0].Players[0].Name.Should().Be("Jaelon Darden");
        depthChart.Positions[0].Players[0].PositionDepth.Should().Be(0);
    }

    // Edge cases
    
    [Test]
    public void Trying_to_remove_a_player_from_a_position_that_does_not_exist_should_return_invalid_state()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller");
        var result = depthChart.CanRemovePlayerFromPosition("QB", 10);
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Trying_to_remove_a_player_that_is_not_in_the_position_should_return_invalid_state()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller");
        var result = depthChart.CanRemovePlayerFromPosition("LWR", 1);
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Trying_to_add_the_same_player_to_the_same_position_should_return_validation_error()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        depthChart.AddPlayerToPosition("LWR", 1, "Scott Miller");
        var result = depthChart.CanAddPlayerToPosition("LWR", 1, "Scott Miller");
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Trying_to_remove_a_player_with_invalid_data_input_without_validating_first_should_throw_exception()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        depthChart.AddPlayerToPosition("LWR", 10, "Scott Miller");
        var fn = () => { depthChart.RemovePlayerFromPosition("QB", 1); };
        fn.Should().Throw<InvalidOperationException>();
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Trying_to_add_invalid_position_should_return_validation_error(string position)
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        var result = depthChart.CanAddPlayerToPosition(position, 10, "Scott Miller");
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void Trying_to_add_invalid_player_number_should_return_validation_error(int number)
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        var result = depthChart.CanAddPlayerToPosition("QB", number, "Scott Miller");
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Trying_to_add_invalid_player_name_should_return_validation_error(string name)
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        var result = depthChart.CanAddPlayerToPosition("QB", 1, name);
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    [TestCase(-1)]
    public void Trying_to_add_invalid_player_position_depth_should_return_validation_error(int positionDepth)
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        var result = depthChart.CanAddPlayerToPosition("QB", 1, "Scott Miller", positionDepth);
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public void Calling_AddPlayerToPosition_direct_with_invalid_parameters_should_throw_exception()
    {
        var depthChart = DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
        var fn = () => { depthChart.AddPlayerToPosition(null, 1, "Scott Miller"); };
        fn.Should().Throw<InvalidOperationException>();
    }

    private static Player GetPlayerFromDepthChart(DepthChart depthChart, string positionName, string playerName)
    {
        var position = depthChart.Positions.SingleOrDefault(x => x.Name == positionName);
        return position == null ? null : position.Players.Single(p => p.Name == playerName);
    }

    private static DepthChart GetDefaultDepthChart()
    {
        return DepthChart.NewDepthChartResult(Guid.NewGuid()).Value;
    }
}