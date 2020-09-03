using Battleships.Enums;
using System;

namespace Battleships
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Battleships!");

      var board = new Board();
      var computer = new Computer(board);

      computer.PlaceShips();

      var finished = false;
      do
      {
        Console.Write("\nEnter the coordinates to shoot: ");
        var coordinates = Console.ReadLine();

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
