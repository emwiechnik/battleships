using System;

namespace Battleships
{
  public class Square
  {
    public char Column { get { return _column; } set { _column = char.ToUpper(value); } }
    public int Row { get; set; }

    private char _column;

    public Square(string squareString)
    {
      if (!string.IsNullOrWhiteSpace(squareString))
      {
        var rowParsed = int.TryParse(squareString.Substring(1), out int row);
        Column = rowParsed ? squareString[0] : '\0';
        Row = rowParsed ? row : 0;
      }
    }

    public Square(char column, int row)
    {
      Column = column;
      Row = row;
    }

    public override bool Equals(object obj)
    {
      if ((obj == null) || !this.GetType().Equals(obj.GetType()))
      {
        return false;
      }
      else
      {
        var s = (Square)obj;
        return (Column == s.Column) && (Row == s.Row);
      }
    }

    public override int GetHashCode()
    {
      int columnHash = Column.GetHashCode();
      int rowHash = Row.GetHashCode();

      return columnHash ^ rowHash;
    }
  }
}
