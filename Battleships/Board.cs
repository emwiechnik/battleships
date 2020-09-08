using Battleships.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships
{
  public class Board
  {
    private readonly Dictionary<ShipType, IList<Square[]>> _ships;
    private readonly Dictionary<ShipType, int> _shipSizes;

    private const int RowCount = 10;
    private const int ColumnCount = 10;

    public Board()
    {
      _ships = new Dictionary<ShipType, IList<Square[]>>();
      _shipSizes = new Dictionary<ShipType, int>()
      {
        { ShipType.Battleship, 5 },
        { ShipType.Destroyer, 4 }
      };
    }

    public IList<Square[]> GetAllShipsOfType(ShipType shipType)
    {
      return _ships.TryGetValue(shipType, out IList<Square[]> ships) ? ships : new List<Square[]>();
    }

    public Result PlaceShip(ShipType type, Square startSquare, Axis axis)
    {
      if (!_ships.ContainsKey(type))
      {
        _ships.Add(type, new List<Square[]>());
      }

      try
      {
        var ship = GenerateShip(type, startSquare, axis);
        foreach (var shipAlreadyOnBoard in _ships.Values.SelectMany(sh => sh))
        {
          if (shipAlreadyOnBoard.Intersect(ship).Any())
          {
            throw new Exception("Cannot place a ship on another ship!");
          }
        }

        _ships[type].Add(ship);
        return Result.Success;
      }
      catch (Exception)
      {
        return Result.Failure;
      }
    }

    private Square[] GenerateShip(ShipType type, Square startSquare, Axis axis)
    {
      var squares = new List<Square>();

      var column = (int)(startSquare.Column - 'A');
      var row = startSquare.Row;

      for (var i = 0; i < _shipSizes[type]; ++i)
      {
        if (row > RowCount || column + 1 > ColumnCount)
        {
          throw new Exception("Ship will not fit!");
        }

        var square = new Square((char)(column + 'A'), row);
        squares.Add(square);

        if (axis == Axis.X)
        {
          column++;
        }
        else
        {
          row++;
        }
      }

      return squares.ToArray();
    }
  }
}