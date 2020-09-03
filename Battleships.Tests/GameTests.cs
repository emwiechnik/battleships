using Battleships.Enums;
using FluentAssertions;
using System;
using Xunit;

namespace Battleships.Tests
{
  public class GameTests
  {
    [Fact]
    public void Computer_Should_Place_1_Battleship_and_2_Destroyers()
    {
      // arrange
      var board = new Board();
      var computer = new Computer(board);

      // act 
      computer.PlaceShips();

      var battleships = board.GetAllShipsOfType(ShipType.Battleship);
      var destroyers = board.GetAllShipsOfType(ShipType.Destroyer);

      // assert
      battleships.Length.Should().Be(1);
      destroyers.Length.Should().Be(2);
    }

    [Theory]
    [InlineData(ShipType.Battleship, Axis.X, 5)]
    [InlineData(ShipType.Battleship, Axis.Y, 5)]
    [InlineData(ShipType.Destroyer, Axis.X, 4)]
    [InlineData(ShipType.Destroyer, Axis.Y, 4)]
    public void Ship_Should_Occupy_Specified_Number_Of_Consecutive_Squares(ShipType shipType, Axis axis, int expectedNumberOfSquares)
    {
      // arrange
      var board = new Board();

      // act
      var ships = board.GetAllShipsOfType(shipType);
      var ship = ships[0];

      // assert
      ship.Length.Should().Be(expectedNumberOfSquares);
      AreSquaresConsecutiveStraightAcrossAxis(ship, axis).Should().BeTrue();
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.X, "A1", "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Battleship, "A2", Axis.X, "A2", "A3", "A4", "A5", "A6")]
    [InlineData(ShipType.Battleship, "B2", Axis.X, "B2", "B3", "B4", "B5", "B6")]
    [InlineData(ShipType.Battleship, "B2", Axis.Y, "B2", "C2", "D2", "E2", "F2")]
    [InlineData(ShipType.Battleship, "H2", Axis.X, "H2", "H3", "H4", "H5", "H6")]
    [InlineData(ShipType.Destroyer, "A1", Axis.X, "A1", "A2", "A3", "A4")]
    [InlineData(ShipType.Destroyer, "A2", Axis.X, "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Destroyer, "B2", Axis.X, "B2", "B3", "B4", "B5")]
    [InlineData(ShipType.Destroyer, "B2", Axis.Y, "B2", "C2", "D2", "E2")]
    [InlineData(ShipType.Destroyer, "H2", Axis.X, "H2", "H3", "H4", "H5")]
    public void Ship_Should_Be_Placed_And_Returned_According_To_Specification(ShipType shipType, string startSquare, Axis axis, params string[] expectedSquares)
    {
      // arrange
      var board = new Board();

      // act 
      board.PlaceShip(shipType, startSquare, axis);

      var ships = board.GetAllShipsOfType(shipType);

      // assert
      ships.Length.Should().Be(1);
      ships[0].Should().BeEquivalentTo(expectedSquares);
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

      for (var i = 0; i < shotSquares.Length - 1; ++i)
      {
        computer.MarkAShot(shotSquares[i]);
      }

      var lastShot = shotSquares[shotSquares.Length - 1];

      // act 
      var actual = computer.MarkAShot(lastShot);

      // assert
      actual.Should().Be(ShotResult.Sink);
    }

    [Fact]
    public void Computer_Should_Inform_That_All_Ships_Have_Sunk_Only_After_All_Of_Them_Have_Sunk()
    {
      // arrange
      var board = new Board();
      board.PlaceShip(ShipType.Battleship, "B2", Axis.X);
      board.PlaceShip(ShipType.Destroyer, "D3", Axis.Y);
      board.PlaceShip(ShipType.Destroyer, "G6", Axis.X);

      var computer = new Computer(board);

      var shots = new[]
      {
        "B2", "B3", "B4", "B5", "B6", "D7", 
        "D3", "E3", "F3", "G3", "D6",
        "G6", "G7", "G8"
      };
      var lastShot = "G9";

      var allSunkBeforeLastShot = false;

      // act
      foreach(var shot in shots)
      {
        computer.MarkAShot(shot);
        if (computer.AllShipsSunk())
        {
          allSunkBeforeLastShot = true;
        }
      }

      computer.MarkAShot(lastShot);
      var allSunkAfterLastShot = computer.AllShipsSunk();

      // assert
      allSunkBeforeLastShot.Should().BeFalse();
      allSunkAfterLastShot.Should().BeTrue();
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
