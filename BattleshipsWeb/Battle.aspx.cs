using Battleship.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace BattleshipsWeb
{
    public partial class Battle : System.Web.UI.Page
    {
        static readonly Random _random = new Random();

        public static String BoardPlayer = "BoardPlayer";
        public static String BoardOpponent = "BoardOpponent";

        private Int32 SessionToInt(String key, Int32 defaultValue)
        {
            if (this.Session[key] == null)
            {
                return defaultValue;
            }

            String stringValue = this.Session[key].ToString();
            if (String.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            if (!Int32.TryParse(stringValue, out Int32 value))
            {
                value = defaultValue;
            }

            return value;
        }

        protected void Page_Load(Object sender, EventArgs e)
        {
            GameBoard playerGameBoard;
            GameBoard opponentGameBoard;

            Int32 gameBoardSize = SessionToInt("gameBoardSize", 10);
            Int32 totalShips = SessionToInt("totalShips", 3);

            if ((this.Session[BoardPlayer] == null) || (this.Session[BoardOpponent] == null))
            {
                this.InitialiseGame(gameBoardSize, totalShips);
            }

            playerGameBoard = (GameBoard)this.Session[BoardPlayer];
            opponentGameBoard = (GameBoard)this.Session[BoardOpponent];

            String locationValue = this.Request.Form["bombLocation"];
            //String locationValue = Request.QueryString["Location"];
            if (!String.IsNullOrEmpty(locationValue))
            {
                if (playerGameBoard.LastBombLocation != locationValue)
                {
                    if (locationValue.Length == 2)
                    {
                        this.strikeLog.Controls.Add(new LiteralControl(this.DropBomb(locationValue, playerGameBoard, opponentGameBoard)));
                    }
                }
            }

            Boolean isGameOver = playerGameBoard.YourShipsRemaining == 0 || playerGameBoard.OpponentShipsRemaining == 0;

            // Render player board
            Console.WriteLine();
            Console.WriteLine("~~ Player's board ~~");
            this.RenderBoard(this.playerBoardArea, playerGameBoard.Board, gameBoardSize, true, isGameOver);
            //RenderSummary(playerGameBoard.HitLog.Count, playerGameBoard.OpponentSectionsRemaining, playerGameBoard.YourSectionsRemaining);

            // Render computer board
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("~~ Opponent's board~~");
            this.RenderBoard(this.opponentBoardArea, opponentGameBoard.Board, gameBoardSize, false, isGameOver);
            //RenderSummary(computerGameBoard.HitLog.Count, computerGameBoard.OpponentSectionsRemaining, computerGameBoard.YourSectionsRemaining);


            //GetBoardAsJSON()
        }

        private void InitialiseGame(Int32 gameBoardSize, Int32 totalShips)
        {
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
            this.Session[BoardPlayer] = playerGameBoard;

            GameBoard opponentGameBoard = new GameBoard(_random, gameBoardSize);
            opponentGameBoard.PositionShips(gameBoardSize, shipLengths.Values.ToArray());
            this.Session[BoardOpponent] = opponentGameBoard;
        }

        private String DropBomb(String locationValue, GameBoard playerGameBoard, GameBoard opponentGameBoard)
        {
            List<String> strikeLog = new List<String>();

            Int32 locationX = locationValue[0] - 65;
            Int32 locationY = locationValue[1] - 65;
            //Int32 locationY = locationValue[1] - 48;

            if (playerGameBoard.IsLocationBombed(locationX, locationY))
            {
                playerGameBoard.HitLog.Remove($"{locationX}{locationY}");
                //return $"<span>You have already dropped a bomb on <b>{locationValue}</b></span>";
            }

            strikeLog.Add($"<span>You dropped a bomb on <b>{locationValue}</b></span>");

            // Drop bomb on opponent's board
            Boolean isBombHit = false;
            isBombHit = opponentGameBoard.DropBomb(locationX, locationY, out Boolean playerSunkShip);
            playerGameBoard.LogBombHit(locationX, locationY, isBombHit, playerSunkShip);
            playerGameBoard.LastBombLocation = locationValue;

            if (isBombHit)
            {
                if (playerSunkShip)
                {
                    strikeLog.Add("<span class='sunk'>You sunk a battleship</span>");
                }
                else
                {
                    strikeLog.Add("<span class='hit'>You hit a battleship</span>");
                }

                if (playerGameBoard.OpponentSectionsRemaining == 0)
                {
                    return "<span class='youWon'>YOU WON<br /><br />All your opponents battleships are at the bottom of the sea</span><br /><br />" +
                        "<a class='btn btn-primary btn-lg' href='/config'>Play again &raquo;</a>";
                    // Redirect you won
                    //RenderYouWon(playerGameBoard.HitLog.Count);
                    //break;
                }
            }
            else
            {
                strikeLog.Add("<span>You missed, try harder</span>");
            }

            strikeLog.Add("");

            // Calculate opponent move
            (Int32 locationX, Int32 locationY) opponentMove = opponentGameBoard.CalculateNextMove();
            strikeLog.Add($"<span>Your opponent dropped a bomb on <b>{Convert.ToChar(opponentMove.locationY + 65)}{opponentMove.locationX}</b></span>");

            // Drop bomb on player's board
            isBombHit = playerGameBoard.DropBomb(opponentMove.locationX, opponentMove.locationY, out Boolean opponentSunkShip);
            opponentGameBoard.LogBombHit(opponentMove.locationX, opponentMove.locationY, isBombHit, opponentSunkShip);
            if (isBombHit)
            {
                if (opponentSunkShip)
                {
                    strikeLog.Add("<span class='sunk'>Your battleship has been sunk</span>");
                }
                else
                {
                    strikeLog.Add("<span class='hit'>Your battleship was hit</span>");
                }

                if (opponentGameBoard.OpponentSectionsRemaining == 0)
                {
                    return "<span class='youLost'>GAME OVER<br /><br />All your battleships have been sunk</span><br /><br />" +
                        "<a class='btn btn-primary btn-lg' href='/config'>Play again &raquo;</a>";

                    // Redirect you lost
                    //RenderYouLost(opponentGameBoard.HitLog.Count);
                    //break;
                }
            }
            else
            {
                strikeLog.Add("<span>You were lucky, nothing hit</span>");
            }

            strikeLog.Add("");
            strikeLog.Add($"<span>You have dropped <b>{playerGameBoard.HitLog.Count}</b> bombs</span>");
            strikeLog.Add($"<span>You need <b>{playerGameBoard.OpponentSectionsRemaining}</b> hits on <b>{playerGameBoard.OpponentShipsRemaining}</b> ships to win</span>");
            strikeLog.Add($"<span>Your opponent needs <b>{playerGameBoard.YourSectionsRemaining}</b> hits on <b>{playerGameBoard.YourShipsRemaining}</b> ships to win</span>");

            return String.Join("<br />", strikeLog);
        }

        private void RenderBoard(Control control, String[,] gameBoard, Int32 size, Boolean isYourBoard, Boolean isGameOver)
        {
            //control.Controls.Add(new LiteralControl($"<table id='{(isYourBoard ? "player" : "opponent")}Board' width='450'>"));
            control.Controls.Add(new LiteralControl($"<table id='{(isYourBoard ? "player" : "opponent")}Board'>"));

            // Horizontial key
            control.Controls.Add(new LiteralControl("<tr>"));
            control.Controls.Add(new LiteralControl("<th></th>"));
            for (Int32 y = 0; y < size; y++)
            {
                control.Controls.Add(new LiteralControl($"<th>{Convert.ToChar(y + 65)}</th>"));
            }
            control.Controls.Add(new LiteralControl("</tr>"));

            for (Int32 x = 0; x < size; x++)
            {
                control.Controls.Add(new LiteralControl("<tr>"));

                // Vertical key
                control.Controls.Add(new LiteralControl($"<td class='boardKey'>{Convert.ToChar(x + 65)}</td>"));

                for (Int32 y = 0; y < size; y++)
                {
                    switch (gameBoard[x, y])
                    {
                        case "~":
                            // Open water
                            if (isGameOver)
                            {
                                control.Controls.Add(new LiteralControl("<td class='openWater'></td>"));
                            }
                            else
                            {
                                control.Controls.Add(new LiteralControl($"<td class='openWater' data-location='{Convert.ToChar(y + 65)}{Convert.ToChar(x + 65)}'></td>"));
                            }
                            break;

                        case "*":
                            // Bomb miss
                            control.Controls.Add(new LiteralControl("<td class='bombMiss'></td>"));
                            break;

                        case "X":
                            // Bomb hit
                            control.Controls.Add(new LiteralControl("<td class='bombHit'></td>"));
                            break;

                        default:
                            // Battleship
                            if (isYourBoard || isGameOver)
                            {
                                control.Controls.Add(new LiteralControl($"<td class='battleShip'>{gameBoard[x, y]}</td>"));
                            }
                            else
                            {
                                control.Controls.Add(new LiteralControl($"<td class='openWater' data-location='{Convert.ToChar(y + 65)}{Convert.ToChar(x + 65)}'></td>"));
                            }
                            break;
                    }
                }

                control.Controls.Add(new LiteralControl("</tr>"));
            }

            control.Controls.Add(new LiteralControl("</table>"));
        }
    }
}