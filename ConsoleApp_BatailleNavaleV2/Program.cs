using ConsoleApp_BatailleNavaleV2.Models;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Position = ConsoleApp_BatailleNavaleV2.Models.Position;

namespace ConsoleApp_BatailleNavaleV2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            bool isShowShips = false;//pos navires ennemi

            NavyAsset MyNavyAsset = new NavyAsset();
            NavyAsset EnemyNavyAsset = new NavyAsset();



            Dictionary<char, int> Coordinates = PopulateDictionary();
            PrintHeader();
            for (int h = 0; h < 19; h++)
            {
                Console.Write(" ");
            }

            //Affichage plateau jeu
            PrintMap(MyNavyAsset.FirePositions, MyNavyAsset, EnemyNavyAsset, isShowShips);

            int Game;
            for (Game = 1; Game < 101; Game++)
            {
                MyNavyAsset.StepsTaken++;

                Position position = new Position();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Coordonnées ? ( ex : A1): ");
                string input = Console.ReadLine();
                position = AnalyzeInput(input, Coordinates);

                if (position.x == -1 || position.y == -1)
                {
                    Console.WriteLine("Coordonnées Invalide");
                    Game--;
                    continue;
                }

                if (MyNavyAsset.FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    Console.WriteLine("Coordonnées deja joué");
                    Game--;
                    continue;
                }


                EnemyNavyAsset.Fire();


                var index = MyNavyAsset.FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);

                if (index == -1)
                    MyNavyAsset.FirePositions.Add(position);

                Console.Clear();



                MyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                MyNavyAsset.CheckShipStatus(EnemyNavyAsset.FirePositions);

                EnemyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                EnemyNavyAsset.CheckShipStatus(MyNavyAsset.FirePositions);

                PrintHeader();
                for (int h = 0; h < 19; h++)
                {
                    Console.Write(" ");
                }



                PrintMap(MyNavyAsset.FirePositions, MyNavyAsset, EnemyNavyAsset, isShowShips);

                Commentator(MyNavyAsset, true);
                Commentator(EnemyNavyAsset, false);
                if (EnemyNavyAsset.IsObliteratedAll || MyNavyAsset.IsObliteratedAll) { break; }


            }

            Console.ForegroundColor = ConsoleColor.White;

            if (EnemyNavyAsset.IsObliteratedAll && !MyNavyAsset.IsObliteratedAll)
            {
                Console.WriteLine("FIN. Gagné.");
            }
            else if (!EnemyNavyAsset.IsObliteratedAll && MyNavyAsset.IsObliteratedAll)
            {
                Console.WriteLine("FIN, PERDU.");
            }
           

            Console.WriteLine("NB positions jouée:{0} ", Game);
            Console.ReadLine();


        }

        static void PrintStatistic(int x, int y, NavyAsset navyAsset)
        {
            if (x == 1 && y == 10)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Indicator:    ");
            }


            if (x == 2 && y == 10)
            {
                if (navyAsset.IsCarrierSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Carrier [5]   ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Carrier [5]   ");
                }

            }

            if (x == 3 && y == 10)
            {
                if (navyAsset.IsBattleshipSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Battleship [4]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Battleship [4]");
                }
            }

            if (x == 4 && y == 10)
            {

                if (navyAsset.IsDestroyerSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Destroyer [3] ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Destroyer [3] ");
                }
            }

            if (x == 5 && y == 10)
            {

                if (navyAsset.IsSubmarineSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Submarine [3] ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Submarine [3] ");
                }
            }

            if (x == 6 && y == 10)
            {

                if (navyAsset.IsPatrolBoatSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("PatrolBoat [2]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("PatrolBoat [2]");
                }

            }


            if (x > 6 && y == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Console.Write(" ");
                }
            }

        }

        static void PrintMap(List<Position> positions, NavyAsset MyNavyAsset, NavyAsset EnemyMyNavyAsset, bool showEnemyShips)
        {
            PrintHeader();
            Console.WriteLine();
            if (!showEnemyShips)
                showEnemyShips = MyNavyAsset.IsObliteratedAll;
            //Stock X et Y tirs
            List<Position> SortedLFirePositions = positions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            //Stock X et Y des navires
            List<Position> SortedShipsPositions = EnemyMyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedShipsPositions = SortedShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            int hitCounter = 0;
            int EnemyshipCounter = 0;
            int myShipCounter = 0;
            int enemyHitCounter = 0;

            char row = 'A';
            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool keepGoing = true;

                        #region row indicator
                        if (y == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("[" + row + "]");
                            row++;
                        }
                        #endregion


                        if (SortedLFirePositions.Count != 0 && SortedLFirePositions[hitCounter].x == x && SortedLFirePositions[hitCounter].y == y)
                        {

                            if (SortedLFirePositions.Count - 1 > hitCounter)
                                hitCounter++;

                            if (EnemyMyNavyAsset.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[*]");

                                keepGoing = false;

                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("[X]");

                                keepGoing = false;

                            }

                        }

                        if (keepGoing && showEnemyShips && SortedShipsPositions.Count != 0 && SortedShipsPositions[EnemyshipCounter].x == x && SortedShipsPositions[EnemyshipCounter].y == y)

                        {

                            if (SortedShipsPositions.Count - 1 > EnemyshipCounter)
                                EnemyshipCounter++;

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("[O]");
                            keepGoing = false;
                        }

                        if (keepGoing)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("[~]");
                        }


                        PrintStatistic(x, y, MyNavyAsset);


                        if (y == 10)
                        {
                            Console.Write("      ");
                            // Plateau jeu ennemie ( Ordinateur)
                            PrintMapOfEnemy(x, row, MyNavyAsset, EnemyMyNavyAsset, ref myShipCounter, ref enemyHitCounter);
                        }
                    }

                    Console.WriteLine();
                }

            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static void PrintMapOfEnemy(int x, char row, NavyAsset MyNavyAsset, NavyAsset EnemyNavyAsset, ref int MyshipCounter, ref int EnemyHitCounter)
        {
            List<Position> EnemyFirePositions = new List<Position>();
            row--;
            Random random = new Random();
            List<Position> SortedLFirePositions = EnemyNavyAsset.FirePositions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedLShipsPositions = MyNavyAsset.AllShipsPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            try
            {

                for (int y = 1; y < 11; y++)
                {
                    bool keepGoing = true;

                    #region row indicator
                    if (y == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[" + row + "]");
                        row++;
                    }
                    #endregion


                    if (SortedLFirePositions.Count != 0 && SortedLFirePositions[EnemyHitCounter].x == x && SortedLFirePositions[EnemyHitCounter].y == y)
                    {

                        if (SortedLFirePositions.Count - 1 > EnemyHitCounter)
                            EnemyHitCounter++;

                        if (MyNavyAsset.AllShipsPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                        {

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[*]");

                            keepGoing = false;

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("[X]");

                            keepGoing = false;

                        }

                    }

                    if (keepGoing && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[MyshipCounter].x == x && SortedLShipsPositions[MyshipCounter].y == y)

                    {

                        if (SortedLShipsPositions.Count - 1 > MyshipCounter)
                            MyshipCounter++;

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");

                        keepGoing = false;

                    }

                    if (keepGoing)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[~]");
                    }


                    PrintStatistic(x, y, EnemyNavyAsset);

                }


            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static Position AnalyzeInput(string input, Dictionary<char, int> Coordinates)
        {
            Position pos = new Position();

            char[] inputSplit = input.ToUpper().ToCharArray();


            if (inputSplit.Length < 2 || inputSplit.Length > 4)
            {
                return pos;
            }




            if (Coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }


            if (inputSplit.Length == 3)
            {

                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }

            }


            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }

            return pos;
        }

        static void PrintHeader()
        {
            Console. ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[ ]");
            for (int i = 1; i < 11; i++)
                Console.Write("[" + i + "]");


        }


        static Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };

            return Coordinate;
        }

        static void Commentator(NavyAsset navyAsset, bool isMyShip)
        {

            string title = isMyShip ? "Your" : "Enemy";

            if (navyAsset.CheckPBattleship && navyAsset.IsBattleshipSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(navyAsset.Battleship));
                navyAsset.CheckPBattleship = false;
            }

            if (navyAsset.CheckCarrier && navyAsset.IsCarrierSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(navyAsset.Carrier));
                navyAsset.CheckCarrier = false;
            }

            if (navyAsset.CheckDestroyer && navyAsset.IsDestroyerSunk)
            {
                 Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(navyAsset.Destroyer));
                navyAsset.CheckDestroyer = false;
            }

            if (navyAsset.CheckPatrolBoat && navyAsset.IsPatrolBoatSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(navyAsset.PatrolBoat));
                navyAsset.CheckPatrolBoat = false;
            }

            if (navyAsset.CheckSubmarine && navyAsset.IsSubmarineSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sink", title, nameof(navyAsset.Submarine));
                navyAsset.CheckSubmarine = false;
            }
            // navyAsset.IsBattleshipSunk

        }
    }
}