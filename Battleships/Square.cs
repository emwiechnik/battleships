namespace Battleships
{
  public class Square
  {
    public char Column { get; set; }
    public int Row { get; set; }
    public Square(string squareString)
    {
      if (!string.IsNullOrWhiteSpace(squareString))
      {
        var rowParsed = int.TryParse(squareString.Substring(1), out int row);
        Column = rowParsed ? squareString[0] : '\0';
        Row = rowParsed ? row : 0;
      }
    }
  }
}
