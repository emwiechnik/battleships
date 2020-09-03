using Battleships.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void Computer_Should_Place_Ships_In_Random_Places_On_The_Board()
    {
      // arrange
      const int attemptsCount = 5;
      var eachAttemptResults = new List<List<string>>();
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
    [InlineData(ShipType.Battleship, Axis.X, 5)]
    [InlineData(ShipType.Battleship, Axis.Y, 5)]
    [InlineData(ShipType.Destroyer, Axis.X, 4)]
    [InlineData(ShipType.Destroyer, Axis.Y, 4)]
    public void Ship_Should_Occupy_Specified_Number_Of_Consecutive_Squares(ShipType shipType, Axis axis, int expectedNumberOfSquares)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType, "A1", axis);

      // act
      var ships = board.GetAllShipsOfType(shipType);
      var ship = ships[0];

      // assert
      ship.Length.Should().Be(expectedNumberOfSquares);
      AreSquaresConsecutiveStraightAcrossAxis(ship, axis).Should().BeTrue();
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.Y, "A1", "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Battleship, "A2", Axis.Y, "A2", "A3", "A4", "A5", "A6")]
    [InlineData(ShipType.Battleship, "B2", Axis.Y, "B2", "B3", "B4", "B5", "B6")]
    [InlineData(ShipType.Battleship, "B2", Axis.X, "B2", "C2", "D2", "E2", "F2")]
    [InlineData(ShipType.Battleship, "H2", Axis.Y, "H2", "H3", "H4", "H5", "H6")]
    [InlineData(ShipType.Destroyer, "A1", Axis.Y, "A1", "A2", "A3", "A4")]
    [InlineData(ShipType.Destroyer, "A2", Axis.Y, "A2", "A3", "A4", "A5")]
    [InlineData(ShipType.Destroyer, "B2", Axis.Y, "B2", "B3", "B4", "B5")]
    [InlineData(ShipType.Destroyer, "B2", Axis.X, "B2", "C2", "D2", "E2")]
    [InlineData(ShipType.Destroyer, "H2", Axis.Y, "H2", "H3", "H4", "H5")]
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
    [InlineData(ShipType.Battleship, "A1", Axis.Y, true)]
    [InlineData(ShipType.Battleship, "A8", Axis.Y, false)]
    [InlineData(ShipType.Battleship, "A10", Axis.Y, false)]
    [InlineData(ShipType.Battleship, "B2", Axis.X, true)]
    [InlineData(ShipType.Battleship, "H2", Axis.X, false)]
    [InlineData(ShipType.Destroyer, "G9", Axis.Y, false)]
    public void Placing_A_Ship_Should_Be_Possible_Only_Within_The_Size_Of_The_Board(ShipType shipType, string startSquare, Axis axis, bool expected)
    {
      // arrange
      var board = new Board();

      // act 
      var actual = board.PlaceShip(shipType, startSquare, axis);

      // assert
      actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.X, ShipType.Battleship, "D1", Axis.Y)]
    [InlineData(ShipType.Battleship, "B1", Axis.Y, ShipType.Battleship, "A3", Axis.X)]
    [InlineData(ShipType.Battleship, "F4", Axis.Y, ShipType.Destroyer, "F7", Axis.X)]
    public void Placing_A_Ship_On_Another_Ship_Should_Not_Be_Possible(ShipType shipType1, string startSquare1, Axis axis1, ShipType shipType2, string startSquare2, Axis axis2)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType1, startSquare1, axis1);

      // act 
      var wasPlaced = board.PlaceShip(shipType2, startSquare2, axis2);

      // assert
      wasPlaced.Should().BeFalse();
    }

    [Theory]
    [InlineData(ShipType.Battleship, "A1", Axis.X, ShipType.Battleship, "D2", Axis.Y)]
    [InlineData(ShipType.Battleship, "B1", Axis.Y, ShipType.Battleship, "C2", Axis.X)]
    [InlineData(ShipType.Battleship, "F4", Axis.Y, ShipType.Destroyer, "G4", Axis.X)]
    public void Placing_A_Ship_Next_To_Another_Ship_Should_Be_Possible(ShipType shipType1, string startSquare1, Axis axis1, ShipType shipType2, string startSquare2, Axis axis2)
    {
      // arrange
      var board = new Board();
      board.PlaceShip(shipType1, startSquare1, axis1);

      // act 
      var wasPlaced = board.PlaceShip(shipType2, startSquare2, axis2);

      // assert
      wasPlaced.Should().BeTrue();
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
      board.PlaceShip(shipType, startSquare, axis);

      var computer = new Computer(board);

      // act 
      var actual = computer.MarkAShot(shotSquare);

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
      board.PlaceShip(ShipType.Battleship, "B2", Axis.Y);
      board.PlaceShip(ShipType.Destroyer, "D3", Axis.X);
      board.PlaceShip(ShipType.Destroyer, "G6", Axis.Y);

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
    [InlineData(true, Axis.Y, "A1", "A2", "A3")]
    [InlineData(true, Axis.Y, "A3", "A2", "A1")]
    [InlineData(false, Axis.Y, "A1", "A2", "A4")]
    [InlineData(false, Axis.Y, "A1", "B2", "A3")]
    [InlineData(true, Axis.X, "A1", "B1", "C1")]
    [InlineData(true, Axis.X, "C1", "B1", "A1")]
    [InlineData(false, Axis.X, "A1", "B1", "D1")]
    [InlineData(false, Axis.X, "A1", "B2", "C1")]
    [InlineData(false, Axis.X, "A1", "B2", "C3")]
    [InlineData(false, Axis.X, "C3", "B2", "A1")]
    [InlineData(true, Axis.X, "G2", "H2", "I2", "J2")]
    [InlineData(false, Axis.X, "G2", "H2", "I2", "J3")]
    [InlineData(true, Axis.Y, "B7", "B8", "B9", "B10")]
    [InlineData(false, Axis.X, "B7", "B8", "B9", "C10")]
    [InlineData(false, Axis.X, "B7", "B7", "B9")]
    public void AreSquaresConsecutiveAcrossAxis_Should_Return_Correct_Result(bool expected, Axis axis, params string[] gridSquares)
    {
      // act
      var actual = AreSquaresConsecutiveStraightAcrossAxis(gridSquares, axis);

      // assert
      actual.Should().Be(expected);
    }

    private bool AreSquaresConsecutiveStraightAcrossAxis(string[] gridSquares, Axis axis)
    {
      for (var i = 0; i < gridSquares.Length - 1; ++i)
      {
        var column1 = (int)(char.ToUpper(gridSquares[i][0]) - 'A');
        var column2 = (int)(char.ToUpper(gridSquares[i + 1][0]) - 'A');
        var row1 = int.Parse(gridSquares[i].Substring(1));
        var row2 = int.Parse(gridSquares[i + 1].Substring(1));

        var diffOnColumns = Math.Abs(column1 - column2);
        var diffOnRows = Math.Abs(row1 - row2);

        if ((axis == Axis.X && (diffOnColumns != 1 || diffOnRows != 0)) ||
            (axis == Axis.Y && (diffOnColumns != 0 || diffOnRows != 1)))
        {
          return false;
        }
      }

      return true;
    }
  }
}