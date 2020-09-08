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
    private static readonly Random _random = new Random();

    public Computer(Board board)
    {
      _board = board;
    }

    public void PlaceShips()
    {
      PlaceShipRandomly(ShipType.Battleship);
      PlaceShipRandomly(ShipType.Destroyer);
      PlaceShipRandomly(ShipType.Destroyer);
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

    private void PlaceShipRandomly(ShipType shipType)
    {
      bool placed;
      do
      {
        var column = (char)(_random.Next(0, 9) + 'A');
        var row = _random.Next(1, 10);
        var axis = _random.Next(0, 100) >= 50 ? Axis.Y : Axis.X;
        placed = _board.PlaceShip(shipType, $"{column}{row}", axis) == Result.Success;
      } while (!placed);
    }

    private IEnumerable<string[]> GetAllShips()
    {
      return _board.GetAllShipsOfType(ShipType.Battleship)
                   .Union(_board.GetAllShipsOfType(ShipType.Destroyer));
    }
  }
}
