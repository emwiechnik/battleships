using System;

namespace Battleships
{
  public class Computer
  {
    private readonly Board _board;

    public Computer(Board board)
    {
      _board = board;
    }

    public void PlaceShips()
    {
      throw new NotImplementedException();
    }

    public object MarkAShot(string shotSquare)
    {
      throw new NotImplementedException();
    }

    public bool AllShipsSunk()
    {
      throw new NotImplementedException();
    }
  }
}
