using Battleships.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships
{
  public class Computer
  {
    private readonly Board _board;
    private readonly List<string> _hits = new List<string>();

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
      var ships = GetAllShips();
      foreach (var ship in ships)
      {
        if (ship.Contains(shotSquare))
        {
          _hits.Add(shotSquare);
          if (_hits.Intersect(ship).Count() == ship.Length)
          {
            return ShotResult.Sink;
          }
          return ShotResult.Hit;
        }
      }

      return ShotResult.Miss;
    }

    public bool AllShipsSunk()
    {
      var ships = GetAllShips().ToList();
      var sunkShipsCount = 0;
      foreach (var ship in ships)
      {
        if (_hits.Intersect(ship).Count() == ship.Length)
        {
          sunkShipsCount++;
        }
      }

      return ships.Count == sunkShipsCount;
    }

    private IEnumerable<string[]> GetAllShips()
    {
      return _board.GetAllShipsOfType(ShipType.Battleship).ToList()
                              .Union(_board.GetAllShipsOfType(ShipType.Destroyer));
    }
  }
}
