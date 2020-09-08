using FluentAssertions;
using Xunit;

namespace Battleships.Tests
{
  public class SquareTests
  {
    [Theory]
    [InlineData("A1", 'A', 1)]
    [InlineData("D2", 'D', 2)]
    [InlineData("b2", 'B', 2)]
    [InlineData("", '\0', 0)]
    [InlineData("AB", '\0', 0)]
    [InlineData("A1c", '\0', 0)]
    public void Square_Should_Correctly_Initialize_From_SquareString(string squareString, char expectedColumn, int expectedRow)
    {
      // act
      var square = new Square(squareString);

      // assert
      square.Column.Should().Be(expectedColumn);
      square.Row.Should().Be(expectedRow);
    }

    [Theory]
    [InlineData('A', 1, 'A', 1)]
    [InlineData('D', 2, 'D', 2)]
    [InlineData('g', 2, 'G', 2)]
    [InlineData('\0', 0, '\0', 0)]
    public void Square_Should_Correctly_Initialize_With_Provided_Column_And_Row(char column, int row, char expectedColumn, int expectedRow)
    {
      // act
      var square = new Square(column, row);

      // assert
      square.Column.Should().Be(expectedColumn);
      square.Row.Should().Be(expectedRow);
    }
  }
}
