using Battleship.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace BattleshipsWeb.Handlers
{
    /// <summary>
    /// Summary description for Preview
    /// </summary>
    public class Preview : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            Random random = new Random();

            context.Response.ContentType = "text/plain";

            String boardSizeValue = context.Request.QueryString["b"];
            if (String.IsNullOrEmpty(boardSizeValue))
            {
                boardSizeValue = "10";
            }

            String totalShipsValue = context.Request.QueryString["s"];
            if (String.IsNullOrEmpty(totalShipsValue))
            {
                totalShipsValue = "3";
            }

            if (!Int32.TryParse(boardSizeValue, out Int32 gameBoardSize))
            {
                gameBoardSize = 10;
            }

            if (!Int32.TryParse(totalShipsValue, out Int32 totalShips))
            {
                totalShips = 10;
            }

            context.Session["gameBoardSize"] = gameBoardSize;
            context.Session["totalShips"] = totalShips;

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
                    shipLengths.Add(shipLength, random.Next(4) + 2);
                }
            }

            GameBoard playerGameBoard = new GameBoard(random, gameBoardSize);
            playerGameBoard.PositionShips(gameBoardSize, shipLengths.Values.ToArray());

            context.Response.Write(RenderBoard(playerGameBoard.Board, gameBoardSize));
        }

        private String RenderBoard(String[,] gameBoard, Int32 size)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<table id='playerBoard'>");

            // Horizontial key
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<th></th>");
            for (Int32 y = 0; y < size; y++)
            {
                stringBuilder.AppendLine($"<th>{Convert.ToChar(y + 65)}</th>");
            }
            stringBuilder.AppendLine("</tr>");

            for (Int32 x = 0; x < size; x++)
            {
                stringBuilder.AppendLine("<tr>");

                // Vertical key
                stringBuilder.AppendLine($"<td class='boardKey'>{Convert.ToChar(x + 65)}</td>");

                for (Int32 y = 0; y < size; y++)
                {
                    switch (gameBoard[x, y])
                    {
                        case "~":
                            // Open water
                            stringBuilder.AppendLine("<td class='openWater'></td>");
                            break;

                        case "*":
                            // Bomb miss
                            stringBuilder.AppendLine("<td class='bombMiss'></td>");
                            break;

                        case "X":
                            // Bomb hit
                            stringBuilder.AppendLine("<td class='bombHit'></td>");
                            break;

                        default:
                            // Battleship
                            stringBuilder.AppendLine($"<td class='battleShip'>{gameBoard[x, y]}</td>");
                            break;
                    }
                }

                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</table>");

            return stringBuilder.ToString();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}