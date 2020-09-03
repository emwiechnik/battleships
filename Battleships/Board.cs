﻿using Battleships.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships
{
  public class Board
  {
    private readonly Dictionary<ShipType, List<string[]>> _ships;
    private readonly Dictionary<ShipType, int> _shipSizes;

    private const int RowCount = 10;
    private const int ColumnCount = 10;

    public Board()
    {
      _ships = new Dictionary<ShipType, List<string[]>>();
      _shipSizes = new Dictionary<ShipType, int>()
      {
        { ShipType.Battleship, 5 },
        { ShipType.Destroyer, 4 }
      };
    }

    public string[][] GetAllShipsOfType(ShipType shipType)
    {
      return _ships.TryGetValue(shipType, out List<string[]> ships) ? ships.ToArray() : new List<string[]>().ToArray();
    }

    public Result PlaceShip(ShipType type, string startSquare, Axis axis)
    {
      if (!_ships.ContainsKey(type))
      {
        _ships.Add(type, new List<string[]>());
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

    private string[] GenerateShip(ShipType type, string startSquare, Axis axis)
    {
      var squares = new List<string>();

      var start = startSquare.ToUpper();

      var column = (int)(start[0] - 'A');
      var row = int.Parse(start.Substring(1));

      for (var i = 0; i < _shipSizes[type]; ++i)
      {
        if (row > RowCount || column + 1 > ColumnCount)
        {
          throw new Exception("Ship will not fit!");
        }

        var square = $"{(char)(column + 'A')}{row}";
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