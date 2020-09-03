using Battleships.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public ShotResult MarkAShot(string shotSquare)
    {
      var ships = _board.GetAllShipsOfType(ShipType.Battleship).ToList()
                        .Union(_board.GetAllShipsOfType(ShipType.Destroyer));
      foreach(var ship in ships)
      {
        if (ship.Contains(shotSquare))
        {
          return ShotResult.Hit;
        }
      }

      return ShotResult.Miss;
    }

    public bool AllShipsSunk()
    {
      throw new NotImplementedException();
    }
  }
}
