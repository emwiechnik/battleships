using Battleships.Enums;
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
      _board.PlaceShip(ShipType.Battleship, "B2", Axis.X);
      _board.PlaceShip(ShipType.Destroyer, "D3", Axis.Y);
      _board.PlaceShip(ShipType.Destroyer, "G6", Axis.X);
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
