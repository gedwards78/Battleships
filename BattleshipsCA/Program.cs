using Battleship.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipsCA
{
    class Program
    {
        static readonly Random _random = new Random();

        static void Main(String[] args)
        {
            Console.WindowHeight = 40;

            Int32 gameBoardSize = 0;
            while (gameBoardSize < 10 || gameBoardSize > 15)
            {
                Console.Clear();
                RenderTitle();

                Console.WriteLine();
                Console.Write("Enter the size of the game board (between 10 and 15): ");

                String consoleValue = Console.ReadLine();
                if (consoleValue == "")
                {
                    consoleValue = "10";
                }

                if (!Int32.TryParse(consoleValue, out gameBoardSize))
                {
                }
            }

            Int32 totalShips = 0;
            while (totalShips < 3 || totalShips > 5)
            {
                Console.Clear();
                RenderTitle();

                Console.WriteLine();
                Console.Write("Enter the number of ships (between 3 and 5): ");

                String consoleValue = Console.ReadLine();
                if (consoleValue == "")
                {
                    consoleValue = "5";
                }

                if (!Int32.TryParse(consoleValue, out totalShips))
                {
                }
            }

            Dictionary<Int32, Int32> shipLengths = new Dictionary<Int32, Int32>();
            if (totalShips == 3)
            {
                shipLengths.Add(1, 4);
                shipLengths.Add(2, 4);
                shipLengths.Add(3, 5);
            }
            else
            {
                for (Int32 shipLength = 2; shipLength < totalShips + 2; shipLength++)
                {
                    shipLengths.Add(shipLength, _random.Next(4) + 2);
                }
            }

            GameBoard playerGameBoard = new GameBoard(_random, gameBoardSize);
            playerGameBoard.PositionShips(gameBoardSize, shipLengths.Values.ToArray());

            GameBoard opponentGameBoard = new GameBoard(_random, gameBoardSize);
            opponentGameBoard.PositionShips(gameBoardSize, shipLengths.Values.ToArray());


            Boolean isGameRunning = true;
            Boolean playerSunkShip = false;
            Boolean opponentSunkShip = false;

            while (isGameRunning)
            {
                Boolean isInputValid = false;
                Int32 locationX = -1;
                Int32 locationY = -1;

                while (!isInputValid)
                {
                    if (playerSunkShip)
                    {
                        Console.Clear();
                        RenderTitle();
                        RenderYouSunkAShip("Player");
                        playerSunkShip = false;
                        Console.ReadKey();
                    }

                    if (opponentSunkShip)
                    {
                        Console.Clear();
                        RenderTitle();
                        RenderYouSunkAShip("Opponent");
                        opponentSunkShip = false;
                        Console.ReadKey();
                    }

                    Console.Clear();
                    RenderTitle();

                    // Render player board
                    Console.WriteLine();
                    Console.WriteLine("~~ Player's board ~~");
                    RenderBoard(playerGameBoard.Board, gameBoardSize, true);
                    RenderSummary(playerGameBoard.HitLog.Count, playerGameBoard.OpponentSectionsRemaining, playerGameBoard.YourSectionsRemaining);

                    // Render opponent board
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("~~ Opponent's board~~");
                    RenderBoard(opponentGameBoard.Board, gameBoardSize, false);
                    //RenderSummary(opponentGameBoard.HitLog.Count, opponentGameBoard.OpponentSectionsRemaining, opponentGameBoard.YourSectionsRemaining);

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Enter X to Quit or R for Random placement");
                    Console.Write("Enter your next move (e.g. E3): ");

                    String consoleValue = Console.ReadLine().ToUpperInvariant();
                    if (consoleValue == "X")
                    {
                        return;
                    }

                    if (consoleValue == "J")
                    {
                        // Convert data table to JSON
                        String jsonResult = JsonConvert.SerializeObject(playerGameBoard);
                        if (jsonResult != null)
                        {
                            GameBoard gameBoard = JsonConvert.DeserializeObject<GameBoard>(jsonResult);
                            if (gameBoard != null)
                            {

                            }
                        }
                    }

                    if (consoleValue == "R")
                    {
                        while (playerGameBoard.IsLocationBombed(locationX, locationY))
                        {
                            locationX = _random.Next(10);
                            locationY = _random.Next(10);
                        }
                        isInputValid = true;
                    }

                    if (consoleValue.Length == 2)
                    {
                        locationX = consoleValue[0] - 65;
                        locationY = consoleValue[1] - 48;

                        isInputValid = !playerGameBoard.IsLocationBombed(locationX, locationY);
                    }
                }


                // Drop bomb on opponent's board
                Boolean isBombHit = false;
                isBombHit = opponentGameBoard.DropBomb(locationX, locationY, out playerSunkShip);
                playerGameBoard.LogBombHit(locationX, locationY, isBombHit, playerSunkShip);
                if (isBombHit)
                {
                    if (playerGameBoard.OpponentSectionsRemaining == 0)
                    {
                        RenderYouWon(playerGameBoard.HitLog.Count);
                        break;
                    }
                }

                // Calculate opponent move
                (Int32 locationX, Int32 locationY) opponentMove = opponentGameBoard.CalculateNextMove();

                // Drop bomb on player's board
                isBombHit = playerGameBoard.DropBomb(opponentMove.locationX, opponentMove.locationY, out opponentSunkShip);
                opponentGameBoard.LogBombHit(opponentMove.locationX, opponentMove.locationY, isBombHit, opponentSunkShip);
                if (isBombHit)
                {
                    if (opponentGameBoard.OpponentSectionsRemaining == 0)
                    {
                        RenderYouLost(opponentGameBoard.HitLog.Count);
                        break;
                    }
                }
            }

            Console.ReadKey();
        }

        static void RenderTitle()
        {
            Console.WriteLine("#################");
            Console.Write("#  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Battleships");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  #");
            Console.WriteLine("#################");
        }

        static void RenderBoard(String[,] gameBoard, Int32 size, Boolean isYourBoard)
        {
            Console.ForegroundColor = ConsoleColor.White;

            // Horizontial key
            Console.Write("    ");
            for (Int32 y = 0; y < size; y++)
            {
                Console.Write($"{Convert.ToChar(y + 65)} ");
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            for (Int32 x = 0; x < size; x++)
            {
                // Vertical key
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"  {x} ");

                Console.ForegroundColor = ConsoleColor.Gray;
                for (Int32 y = 0; y < size; y++)
                {
                    switch (gameBoard[x, y])
                    {
                        case "~":
                            // Open water
                            Console.Write(". ");
                            break;

                        case "*":
                            // Bomb miss
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("* ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;

                        case "X":
                            // Bomb hit
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("X ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;

                        default:
                            // Battle ship
                            if (isYourBoard)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"{gameBoard[x, y]} ");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                            else
                            {
                                Console.Write(". ");
                            }
                            break;
                    }
                }

                Console.WriteLine();
            }
        }

        static void RenderSummary(Int32 totalBombs, Int32 playerHits, Int32 opponentHits)
        {
            Console.WriteLine($"  You have dropped {totalBombs} bombs, need {playerHits} hits to win or {opponentHits} to lose");
        }

        static void RenderYouSunkAShip(String player)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ************");
            Console.WriteLine("  **  BOOM  **");
            Console.WriteLine("  ************");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine($"  {player} sunk a battleship");
        }

        static void RenderYouWon(Int32 totalBombs)
        {
            Console.Clear();
            RenderTitle();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ***************");
            Console.WriteLine("  **  YOU WON  **");
            Console.WriteLine("  ***************");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine($"  You dropped {totalBombs} bombs and won the war");
        }

        static void RenderYouLost(Int32 totalBombs)
        {
            Console.Clear();
            RenderTitle();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("  ****************");
            Console.WriteLine("  **  YOU LOST  **");
            Console.WriteLine("  ****************");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine($"  You dropped {totalBombs} bombs but luck was not on your side this time");
        }
    }
}
