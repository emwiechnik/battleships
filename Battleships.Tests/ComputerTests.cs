using Battleships.Enums;
using Battleships.Logic;
using Battleships.ValueObjects;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Battleships.Tests
{
  public class ComputerTests
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
      battleships.Count.Should().Be(1);
      destroyers.Count.Should().Be(2);
    }

    [Fact]
    public void Computer_Should_Place_Ships_In_Random_Places_On_The_Board()
    {
      // arrange
      const int attemptsCount = 5;
      var eachAttemptResults = new List<List<Square>>();
      var sameResultsCount = 0;

      var acceptableSameResultsCount = attemptsCount - 1;

      // act 
      for (int i = 0; i < attemptsCount; ++i)
      {
        var board = new Board();
        var computer = new Computer(board);

        computer.PlaceShips();

        var battleships = board.GetAllShipsOfType(ShipType.Battleship);
        var destroyers = board.GetAllShipsOfType(ShipType.Destroyer);

        var allShipSquares = battleships.Union(destroyers).SelectMany(sh => sh).ToList();

        if (eachAttemptResults.Any(result => result.Intersect(allShipSquares).Count() == allShipSquares.Count))
        {
          sameResultsCount++;
        }

        eachAttemptResults.Add(allShipSquares);
      }

      // assert
      sameResultsCount.Should().BeLessThan(acceptableSameResultsCount);
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.Y, "A3", ShotResult.Hit)]
    [InlineData(ShipType.Battleship, "A1", Axis.Y, "B1", ShotResult.Miss)]
    [InlineData(ShipType.Destroyer, "G6", Axis.Y, "G8", ShotResult.Hit)]
    [InlineData(ShipType.Destroyer, "G6", Axis.Y, "D7", ShotResult.Miss)]
    public void Computer_Should_Inform_That_There_Was_A_Hit_Or_Miss(ShipType shipType, string startSquare, Axis axis, string shotSquare, ShotResult expected)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType, new Square(startSquare), axis);

      var computer = new Computer(board);

      // act 
      var actual = computer.MarkAShot(new Square(shotSquare));

      // assert
      actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.Y, "A1", "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Battleship, "A1", Axis.Y, "A5", "A4", "A3", "A2", "A1")]
    [InlineData(ShipType.Battleship, "C3", Axis.X, "C3", "D3", "E3", "F3", "G3")]
    [InlineData(ShipType.Battleship, "C3", Axis.X, "G3", "E3", "F3", "C3", "D3")]
    [InlineData(ShipType.Destroyer, "C3", Axis.X, "C3", "D3", "E3", "F3")]
    [InlineData(ShipType.Destroyer, "C3", Axis.X, "E3", "D3", "C3", "F3")]
    public void Computer_Should_Inform_That_There_Was_A_Sink_After_Shooting_Last_Remaining_Square_Of_A_Ship(ShipType shipType, string startSquare, Axis axis, params string[] shotSquares)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType, new Square(startSquare), axis);

      var computer = new Computer(board);

      for (var i = 0; i < shotSquares.Length - 1; ++i)
      {
        computer.MarkAShot(new Square(shotSquares[i]));
      }

      var lastShot = shotSquares[shotSquares.Length - 1];

      // act 
      var actual = computer.MarkAShot(new Square(lastShot));

      // assert
      actual.Should().Be(ShotResult.Sink);
    }

    [Fact]
    public void Computer_Should_Inform_That_All_Ships_Have_Sunk_Only_After_All_Of_Them_Have_Sunk()
    {
      // arrange
      var board = new Board();
      board.PlaceShip(ShipType.Battleship, new Square("B2"), Axis.Y);
      board.PlaceShip(ShipType.Destroyer, new Square("D3"), Axis.X);
      board.PlaceShip(ShipType.Destroyer, new Square("G6"), Axis.Y);

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
      foreach (var shot in shots)
      {
        computer.MarkAShot(new Square(shot));
        if (computer.AllShipsSunk())
        {
          allSunkBeforeLastShot = true;
        }
      }

      computer.MarkAShot(new Square(lastShot));
      var allSunkAfterLastShot = computer.AllShipsSunk();

      // assert
      allSunkBeforeLastShot.Should().BeFalse();
      allSunkAfterLastShot.Should().BeTrue();
    }
  }
}
