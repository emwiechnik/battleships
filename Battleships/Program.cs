using Battleships.Enums;
using System;
using System.Linq;

namespace Battleships
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("The Game of Battleships! (one-sided version)");

      var board = new Board();
      var computer = new Computer(board);

      computer.PlaceShips();

      var finished = false;
      do
      {
        Console.Write("\nEnter the coordinates to shoot: ");
        var coordinates = Console.ReadLine();

        if (coordinates.Equals("show the board", StringComparison.InvariantCultureIgnoreCase))
        {
          PrintTheBoard(board);
          continue;
        }
        var shotResult = computer.MarkAShot(coordinates);

        var message = GetMessageBasedOnResult(shotResult);

        Console.WriteLine(message);

        if (computer.AllShipsSunk())
        {
          Console.WriteLine("\nAll the computer's ships have sunk!");
          finished = true;
        }
      } while (!finished);
    }

    private static void PrintTheBoard(Board board)
    {
      var battleshipSquares = board.GetAllShipsOfType(ShipType.Battleship).SelectMany(sh => sh).ToList();
      var destroyerSquares = board.GetAllShipsOfType(ShipType.Destroyer).SelectMany(sh => sh).ToList();

      const int columnCount = 10;
      const int rowCount = 10;

      for (var row = 0; row <= rowCount; ++row)
      {
        Console.Write((row == 0 ? string.Empty : $"{row}").PadRight(2));
        for (var column = 0; column < columnCount; ++column)
        {
          var col = (char)(column + 'A');
          var square = $"{col}{row}";

          if (row == 0)
          {
            Console.Write(col);
            continue;
          }

          var fieldToPrint = "~";
          if (battleshipSquares.Contains(square))
          {
            fieldToPrint = "B";
          }
          else if (destroyerSquares.Contains(square))
          {
            fieldToPrint = "D";
          }

          Console.Write(fieldToPrint);
        }
        Console.WriteLine();
      }
    }

    private static string GetMessageBasedOnResult(ShotResult shotResult)
    {
      switch (shotResult)
      {
        case ShotResult.Hit: return "A ship was hit!";
        case ShotResult.Miss: return "That was a miss";
        case ShotResult.Sink: return "A ship has sunk!";
      }

      return "No idea about the result..";
    }
  }
}
