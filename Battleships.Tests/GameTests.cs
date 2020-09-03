using Battleships.Enums;
using FluentAssertions;
using System;
using Xunit;

namespace Battleships.Tests
{
  public class GameTests
  {
    [Fact]
    public void Game_Should_Initialize_With_1_Battleship_and_2_Destroyers()
    {
      // arrange
      var board = new Board();
      var computer = new Computer(board);

      // act 
      computer.Start();

      var battleship = board.GetBattleship();
      var destroyers = board.GetDestroyers();

      // assert
      var expectedBattleshipLength = 5;
      var expectedDestroyerLength = 4;
      
      battleship?.Length.Should().Be(expectedBattleshipLength);

      destroyers?.Length.Should().Be(2);
      destroyers[0].Length.Should().Be(expectedDestroyerLength);
      destroyers[1].Length.Should().Be(expectedDestroyerLength);
    }

    [Fact]
    public void Battleship_Should_Occupy_5_Consecutive_Squares()
    {
      // arrange
      var board = new Board();
      var computer = new Computer(board);

      // act 
      computer.Start();

      var battleship = board.GetBattleship();

      // assert
      var expectedBattleshipLength = 5;

      battleship.Length.Should().Be(expectedBattleshipLength);
      var areSquaresConsecutive = AreSquaresConsecutiveStraightAcrossAxis(battleship, Axis.X) || AreSquaresConsecutiveStraightAcrossAxis(battleship, Axis.Y);
      areSquaresConsecutive.Should().BeTrue();
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.X, "A3", ShotResult.Hit)]
    [InlineData(ShipType.Battleship, "A1", Axis.X, "B1", ShotResult.Miss)]
    public void Computer_Should_Inform_That_There_Was_A_Hit_Or_Miss(ShipType shipType, string startSquare, Axis axis, string shotSquare, ShotResult expected)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType, startSquare, axis);

      var computer = new Computer(board);
      computer.Start();

      // act 
      var actual = computer.MarkAShot(shotSquare);

      // assert
      actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.X, "A1", "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Battleship, "A1", Axis.X, "A5", "A4", "A3", "A2", "A1")]
    [InlineData(ShipType.Battleship, "C3", Axis.Y, "C3", "D3", "E3", "F3", "G6")]
    [InlineData(ShipType.Battleship, "C3", Axis.Y, "G3", "E3", "F3", "C3", "D6")]
    [InlineData(ShipType.Destroyer, "C3", Axis.Y, "C3", "D3", "E3", "F3")]
    public void Computer_Should_Inform_That_There_Was_A_Sink_After_Shooting_Last_Remaining_Square_Of_A_Ship(ShipType shipType, string startSquare, Axis axis, params string[] shotSquares)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType, startSquare, axis);

      var computer = new Computer(board);
      computer.Start();

      for(var i = 0; i < shotSquares.Length - 1; ++i)
      {
        computer.MarkAShot(shotSquares[i]);
      }
      
      var lastShot = shotSquares[shotSquares.Length - 1];

      // act 
      var actual = computer.MarkAShot(lastShot);

      // assert
      actual.Should().Be(ShotResult.Sink);
    }

    [Theory]
    [InlineData("A1", Axis.X, "A1", "A2", "A3", "A4", "A5")]
    [InlineData("A2", Axis.X, "A2", "A3", "A4", "A5", "A6")]
    [InlineData("B2", Axis.X, "B2", "B3", "B4", "B5", "B6")]
    [InlineData("B2", Axis.Y, "B2", "C2", "D2", "E2", "F2")]
    [InlineData("H2", Axis.X, "H2", "H3", "H4", "H5", "H6")]
    public void Game_Should_Place_Ship_Where_Specified(string startSquare, Axis axis, params string[] expectedSquares)
    {
      // arrange
      var board = new Board();

      // act 
      board.PlaceShip(ShipType.Battleship, startSquare, axis);
      
      var battleship = board.GetBattleship();

      // assert
      battleship.Should().BeEquivalentTo(expectedSquares);
    }

    [Theory]
    [InlineData("A1", Axis.X, "A1", "A2", "A3", "A4")]
    [InlineData("A2", Axis.X, "A2", "A3", "A4", "A5")]
    [InlineData("B2", Axis.X, "B2", "B3", "B4", "B5")]
    [InlineData("B2", Axis.Y, "B2", "C2", "D2", "E2")]
    [InlineData("H2", Axis.X, "H2", "H3", "H4", "H5")]
    public void Game_Should_Place_Destroyer_Where_Specified(string startSquare, Axis axis, params string[] expectedSquares)
    {
      // arrange
      var game = new Board();

      // act 
      game.PlaceShip(ShipType.Destroyer, startSquare, axis);

      var destroyers = game.GetDestroyers();

      // assert
      destroyers.Length.Should().Be(1);
      destroyers[0].Should().BeEquivalentTo(expectedSquares);
    }

    [Theory]
    [InlineData(true, Axis.X, "A1", "A2", "A3")]
    [InlineData(true, Axis.X, "A3", "A2", "A1")]
    [InlineData(false, Axis.X, "A1", "A2", "A4")]
    [InlineData(false, Axis.X, "A1", "B2", "A3")]
    [InlineData(true, Axis.Y, "A1", "B1", "C1")]
    [InlineData(true, Axis.Y, "C1", "B1", "A1")]
    [InlineData(false, Axis.Y, "A1", "B1", "D1")]
    [InlineData(false, Axis.Y, "A1", "B2", "C1")]
    [InlineData(false, Axis.Y, "A1", "B2", "C3")]
    [InlineData(false, Axis.Y, "C3", "B2", "A1")]
    public void AreSquaresConsecutiveAcrossAxis_Should_Return_Correct_Result(bool expected, Axis axis, params string[] gridSquares)
    {
      // act
      var actual = AreSquaresConsecutiveStraightAcrossAxis(gridSquares, axis);

      // assert
      actual.Should().Be(expected);
    }

    private bool AreSquaresConsecutiveStraightAcrossAxis(string[] gridSquares, Axis axis)
    {
      var axisValue = (int)axis;
      var otherAxisValue = (int)(axis == Axis.X ? Axis.Y : Axis.X);

      for (var i = 0; i < gridSquares.Length - 1; ++i)
      {
        var diffOnAxis = Math.Abs((int)gridSquares[i][axisValue] - (int)gridSquares[i + 1][axisValue]);
        var diffOnOtherAxis = Math.Abs((int)gridSquares[i][otherAxisValue] - (int)gridSquares[i + 1][otherAxisValue]);
        if (diffOnAxis > 1 || diffOnOtherAxis > 0)
        {
          return false;
        }
      }

      return true;
    }
  }
}
