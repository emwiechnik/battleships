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
      var game = new Game();

      // act 
      game.Start();

      var battleship = game.GetBattleship();
      var destroyers = game.GetDestroyers();

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
      var game = new Game();

      // act 
      game.Start();
      var battleship = game.GetBattleship();

      // assert
      var expectedBattleshipLength = 5;

      battleship.Length.Should().Be(expectedBattleshipLength);
      var areSquaresConsecutive = AreSquaresConsecutiveStraightAcrossAxis(battleship, Axis.X) || AreSquaresConsecutiveStraightAcrossAxis(battleship, Axis.Y);
      areSquaresConsecutive.Should().BeTrue();
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

    public enum Axis
    {
      Y = 0,
      X = 1
    }
  }
}
