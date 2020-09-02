using FluentAssertions;
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
    }
  }
}
