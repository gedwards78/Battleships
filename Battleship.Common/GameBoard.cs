using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Common
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum LocationStatus
    {
        Invalid,
        Unknown,
        Hit,
        Miss,
    }

    public class GameBoard
    {
        #region Private properties

        private readonly Random _random;
        private readonly Dictionary<String, Int32> _ships = new Dictionary<String, Int32>();
        private (Int32 LocationX, Int32 LocationY, Direction Direction) _lastHit = (-1, -1, Direction.None);

        #endregion

        #region Public properties

        public Int32 BoardSize { get; private set; }
        public String[,] Board { get; private set; }

        public Int32 TotalShips { get; private set; } = 0;
        public Int32 TotalShipSections { get; private set; } = 0;
        public Int32 YourSectionsRemaining { get; private set; } = 0;
        public Int32 YourShipsRemaining { get; private set; } = 0;
        public Int32 OpponentSectionsRemaining { get; private set; } = 0;
        public Int32 OpponentShipsRemaining { get; private set; } = 0;

        public Dictionary<String, Boolean> HitLog { get; private set; } = new Dictionary<String, Boolean>();

        public String LastBombLocation { get; set; } = String.Empty;

        #endregion

        #region Private methods

        private void GenerateBoard()
        {
            this.Board = new String[this.BoardSize, this.BoardSize];

            for (Int32 x = 0; x < this.BoardSize; x++)
            {
                for (Int32 y = 0; y < this.BoardSize; y++)
                {
                    //Board[x, y] = x.ToString();
                    this.Board[x, y] = "~";
                }
            }
        }

        #endregion

        #region Public methods

        public void PositionShips(Int32 size, Int32[] shipLengths)
        {
            Int32 shipIndex = 0;
            foreach (Int32 shipLength in shipLengths.OrderByDescending(x => x))
            {
                Boolean isHorizontial = this._random.NextBoolean();
                String shipID = Convert.ToChar(shipIndex + 65).ToString();
                this.PositionShip(size, shipID, shipLength, isHorizontial);

                this._ships.Add(shipID, shipLength);
                this.TotalShips++;
                this.TotalShipSections = this.TotalShipSections + shipLength;
                this.YourSectionsRemaining = this.TotalShipSections;
                this.YourShipsRemaining = this.TotalShips;
                this.OpponentSectionsRemaining = this.TotalShipSections;
                this.OpponentShipsRemaining = this.TotalShips;

                shipIndex++;
            }
        }

        public void PositionShip(Int32 size, String shipID, Int32 shipLength, Boolean isHorizontial)
        {
            Boolean isPositioned = false;
            while (!isPositioned)
            {
                if (isHorizontial)
                {
                    Int32 startPositionX = this._random.Next(size - shipLength);
                    Int32 startPositionY = this._random.Next(size);

                    isPositioned = true;
                    for (Int32 index = 0; index < shipLength; index++)
                    {
                        if (this.Board[startPositionY, startPositionX + index] != "~")
                        {
                            isPositioned = false;
                            break;
                        }
                    }

                    if (isPositioned)
                    {
                        for (Int32 index = 0; index < shipLength; index++)
                        {
                            this.Board[startPositionY, startPositionX + index] = shipID;
                            //Board[startPositionY, startPositionX + index] = "B";
                        }
                    }
                }
                else
                {
                    Int32 startPositionX = this._random.Next(size);
                    Int32 startPositionY = this._random.Next(size - shipLength);

                    isPositioned = true;
                    for (Int32 index = 0; index < shipLength; index++)
                    {
                        if (this.Board[startPositionY + index, startPositionX] != "~")
                        {
                            isPositioned = false;
                            break;
                        }
                    }

                    if (isPositioned)
                    {
                        for (Int32 index = 0; index < shipLength; index++)
                        {
                            this.Board[startPositionY + index, startPositionX] = shipID;
                            //Board[startPositionY + index, startPositionX] = "B";
                        }
                    }
                }
            }
        }

        public Boolean DropBomb(Int32 locationX, Int32 locationY, out Boolean isSunk)
        {
            String square = this.Board[locationY, locationX];
            if (square == "~")
            {
                this.Board[locationY, locationX] = "*";
                isSunk = false;
                return false;
            }
            else
            {
                // Check if the ship has been sunk
                this._ships[this.Board[locationY, locationX]]--;
                isSunk = this._ships[this.Board[locationY, locationX]] == 0;
                if (isSunk)
                {
                    this.YourShipsRemaining--;
                }

                this.YourSectionsRemaining--;

                // Mark as hit on board
                this.Board[locationY, locationX] = "X";
                return true;
            }
        }

        public (Int32 locationX, Int32 locationY) CalculateNextMove()
        {
            if (this._lastHit.Direction == Direction.None)
            {
                Int32 locationX = this._random.Next(this.BoardSize);
                Int32 locationY = this._random.Next(this.BoardSize);
                while (this.IsLocationBombed(locationX, locationY))
                {
                    locationX = this._random.Next(this.BoardSize);
                    locationY = this._random.Next(this.BoardSize);
                }

                return (locationX, locationY);
            }
            else
            {
                Int32 locationX = this._lastHit.LocationX;
                Int32 locationY = this._lastHit.LocationY;
                LocationStatus locationStatus = LocationStatus.Invalid;

                switch (this._lastHit.Direction)
                {
                    case Direction.Up:
                        locationY = locationY - 1;

                        while (locationStatus != LocationStatus.Unknown)
                        {
                            locationStatus = this.GetLocationStatus(locationX, locationY);
                            switch (locationStatus)
                            {
                                case LocationStatus.Miss:
                                case LocationStatus.Invalid:
                                    this._lastHit.Direction = Direction.Down;
                                    return this.CalculateNextMove();

                                case LocationStatus.Unknown:
                                    break;

                                case LocationStatus.Hit:
                                    locationY = locationY - 1;
                                    break;

                                default:
                                    break;
                            }
                        }

                        //if (this.IsLocationBombed(locationX, locationY))
                        //{
                        //    this._lastHit.Direction = Direction.Down;
                        //    return CalculateNextMove();
                        //}
                        break;

                    case Direction.Down:
                        locationY = locationY + 1;

                        while (locationStatus != LocationStatus.Unknown)
                        {
                            locationStatus = this.GetLocationStatus(locationX, locationY);
                            switch (locationStatus)
                            {
                                case LocationStatus.Miss:
                                case LocationStatus.Invalid:
                                    this._lastHit.Direction = Direction.Left;
                                    return this.CalculateNextMove();

                                case LocationStatus.Unknown:
                                    break;

                                case LocationStatus.Hit:
                                    locationY = locationY + 1;
                                    break;

                                default:
                                    break;
                            }
                        }

                        //if (this.IsLocationBombed(locationX, locationY))
                        //{
                        //    this._lastHit.Direction = Direction.Left;
                        //    return CalculateNextMove();
                        //}
                        break;

                    case Direction.Left:
                        locationX = locationX - 1;
                        while (locationStatus != LocationStatus.Unknown)
                        {
                            locationStatus = this.GetLocationStatus(locationX, locationY);
                            switch (locationStatus)
                            {
                                case LocationStatus.Miss:
                                case LocationStatus.Invalid:
                                    this._lastHit.Direction = Direction.Right;
                                    return this.CalculateNextMove();

                                case LocationStatus.Unknown:
                                    break;

                                case LocationStatus.Hit:
                                    locationX = locationX - 1;
                                    break;

                                default:
                                    break;
                            }
                        }


                        //if (this.IsLocationBombed(locationX, locationY))
                        //{
                        //    this._lastHit.Direction = Direction.Right;
                        //    return CalculateNextMove();
                        //}
                        break;

                    case Direction.Right:
                        locationX = locationX + 1;

                        while (locationStatus != LocationStatus.Unknown)
                        {
                            locationStatus = this.GetLocationStatus(locationX, locationY);
                            switch (locationStatus)
                            {
                                case LocationStatus.Miss:
                                case LocationStatus.Invalid:
                                    this._lastHit.Direction = Direction.None;
                                    return this.CalculateNextMove();

                                case LocationStatus.Unknown:
                                    break;

                                case LocationStatus.Hit:
                                    locationX = locationX + 1;
                                    break;

                                default:
                                    break;
                            }
                        }

                        //if (this.IsLocationBombed(locationX, locationY))
                        //{
                        //    this._lastHit.Direction = Direction.None;
                        //    return CalculateNextMove();
                        //}
                        break;

                    default:
                        while (this.IsLocationBombed(locationX, locationY))
                        {
                            locationX = this._random.Next(this.BoardSize);
                            locationY = this._random.Next(this.BoardSize);
                        }
                        break;
                }

                return (locationX, locationY);
            }
        }

        public Boolean IsLocationBombed(Int32 locationX, Int32 locationY)
        {
            if ((locationX < 0) || (locationY < 0))
            {
                return true;
            }

            if ((locationX > this.BoardSize - 1) || (locationY > this.BoardSize - 1))
            {
                return true;
            }

            return this.HitLog.ContainsKey($"{locationX}{locationY}");
        }

        public LocationStatus GetLocationStatus(Int32 locationX, Int32 locationY)
        {
            if ((locationX < 0) || (locationY < 0))
            {
                return LocationStatus.Invalid;
            }

            if ((locationX > this.BoardSize - 1) || (locationY > this.BoardSize - 1))
            {
                return LocationStatus.Invalid;
            }

            if (!this.HitLog.ContainsKey($"{locationX}{locationY}"))
            {
                return LocationStatus.Unknown;
            }

            return this.HitLog[$"{locationX}{locationY}"] ? LocationStatus.Hit : LocationStatus.Miss;
        }

        public void LogBombHit(Int32 locationX, Int32 locationY, Boolean isBombHit, Boolean isSunk)
        {
            this.HitLog.Add($"{locationX}{locationY}", isBombHit);
            if (isBombHit)
            {
                this.OpponentSectionsRemaining--;
                if (this._lastHit.Direction == Direction.None)
                {
                    this._lastHit.LocationX = locationX;
                    this._lastHit.LocationY = locationY;
                    this._lastHit.Direction = Direction.Up;
                }
                else
                {
                    this._lastHit.LocationX = locationX;
                    this._lastHit.LocationY = locationY;
                }
            }
            //else
            //{
            //    switch (this._lastHit.Direction)
            //    {
            //        case Direction.Up:
            //            this._lastHit.Direction = Direction.Down;
            //            break;

            //        case Direction.Down:
            //            this._lastHit.Direction = Direction.Left;
            //            break;

            //        case Direction.Left:
            //            this._lastHit.Direction = Direction.Right;
            //            break;

            //        case Direction.Right:
            //            this._lastHit.Direction = Direction.None;
            //            break;

            //        default:
            //            break;
            //    }
            //}

            if (isSunk)
            {
                this.OpponentShipsRemaining--;
                this._lastHit.Direction = Direction.None;
            }
        }

        public String GetBoardAsJSON()
        {
            // Convert data table to JSON
            String jsonResult = JsonConvert.SerializeObject(this.Board);

            return jsonResult;
        }

        #endregion

        #region Constructors

        public GameBoard()
        {
        }

        public GameBoard(Random random, Int32 boardSize)
        {
            this._random = random;
            this.BoardSize = boardSize;

            this.GenerateBoard();
        }

        #endregion
    }
}
