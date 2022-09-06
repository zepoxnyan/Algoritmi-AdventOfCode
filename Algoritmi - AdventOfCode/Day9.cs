using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmiProjektna
{
    public class Day9
    {
        private static int[][] cavesMap;
        private static int mapWidth;
        private static int mapHeight;
        private static List<Point> lowPoints = new List<Point>();

        private class Point
        {
            private int x, y, value;

            public int X
            {
                get { return x; }
                set { }
            }
            public int Y
            {
                get { return y; }
                set { }
            }
            public int Value
            {
                get { return value; }
                set { }
            }

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
                this.value = cavesMap[y][x];
            }

            override public string ToString()
            {
                return ($"({x},{y})");
            }

        }

        private static void InitializeData(string input)
        {
            //Nastavimo dve spremenljivke za beleženje stolpca in vrstice v naših podatkih
            int rowNumber = 0, lineNumber = 0;

            //Nastavimo 2D "Jagged" tabelo, kjer mu podamo samo število vrstic s pomočjo funkcije ki gre čez celo datoteko in vrne število vrstic
            cavesMap = new int[Extensions.ReturnLineCount(input)][];

            foreach (string line in input.FileByLines())
            {
                //Vstopimo v prvo loop, le ta gre čez vse vrstice v naši datoteki
                //Za vsako vrstico nato definiramo drugi del 2D tabele, kjer mu podamo dolžino vrstice
                rowNumber = 0;
                cavesMap[lineNumber] = new int[line.Length];
                foreach (char peak in line)
                {
                    //V našo 2D array nato pričnemo shranjevati podatke iz datoteke
                    cavesMap[lineNumber][rowNumber] = int.Parse(peak.ToString());
                    rowNumber++;
                }
                lineNumber++;
            }
            //Ko imam prebrane cele podatke iz input-a pa nastavimo še obe spremenljivke za dimezijo naše 2D tabele
            mapHeight = cavesMap.Length;
            mapWidth = cavesMap[0].Length;
        }

        private static void PrintMap()
        {
            //Izpiše 2D tabelo
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Console.Write($"{cavesMap[y][x]}");
                }
                Console.WriteLine();
            }
        }

        private static List<Point> GetNeighbours(Point selectedPoint)
        {
            //Nov seznam tipa Point
            List<Point> neighbours = new List<Point>();
            //Robni pogoji za določanje koliko sosedov ima točka v odvisnosti na položaj v mapi
            if (selectedPoint.Y > 0)
            {
                neighbours.Add(new Point(selectedPoint.X, selectedPoint.Y - 1));
            }
            if (selectedPoint.X < mapWidth - 1)
            {
                neighbours.Add(new Point(selectedPoint.X + 1, selectedPoint.Y));
            }

            if (selectedPoint.Y < mapHeight - 1)
            {
                neighbours.Add(new Point(selectedPoint.X, selectedPoint.Y + 1));
            }
            if (selectedPoint.X > 0)
            {
                neighbours.Add(new Point(selectedPoint.X - 1, selectedPoint.Y));
            }
            return neighbours;
        }

        private static bool IsSmaller(Point selectedPoint, List<Point> neighbours)
        {
            //Bool spremenljivka ali je točka manjša od sosedov
            bool isSmaller = true;

            //Foreach zanka, le ta primerja vrednosti izbrane točke, z vsemi vrednostmi sosedov točke
            foreach (Point neighbourPoint in neighbours)
            {
                //Če je ima katerikoli sosed manjšo vrednost potem točka ni "LowPoint"
                if (selectedPoint.Value >= neighbourPoint.Value)
                {
                    isSmaller = false;
                }
            }
            return isSmaller;
        }

        public static void PrintLowPoints()
        {
            foreach (Point point in lowPoints)
            {
                Console.WriteLine($"Low point at {point.ToString()} with value {point.Value}");
            }
        }

        private static void GetBasins(Point selectedPoint, HashSet<string> existingPoints, List<List<string>> basin)
        {
            //GetBasins je rekurzivna funkcija. Najprej pridobi vse sosede izbrane točke
            List<Point> neighbourPoints = GetNeighbours(selectedPoint);
            foreach (Point point in neighbourPoints)
            {
                //Foreach zanka se sprehaja čez vse sosede točke
                //If zanka preverja ali je vrednost točke 9, če je se ne smatra za del kotline in če se lahko doda v HashSet obstoječih točk (HashSet.add vedno sprejme samo unikatne vnose)
                if (point.Value != 9 && existingPoints.Add(point.ToString()))
                {
                    //Spremenljivka basin je tip gnezdenega seznam, zato je potrebno pa uporabimo Last(), ki nam vrne zadnji element v seznamu. V ta element damo točko ki pripada gledani kotlini
                    basin.Last().Add(point.ToString());
                    //Kličemo rekurzijo
                    GetBasins(point, existingPoints, basin);
                }
            }
        }

        public static int PartOne(string inputFile)
        {
            //Nastavimo spremeljivko za izračun risk stopnje in preberemo datoteko
            int riskLevel = 0;
            InitializeData(inputFile);

            //Dve for zanki ki se sprehodita čez celo našo 2D tabelo in preverjata vsako točko
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Point point = new Point(x, y);
                    List<Point> neighbourPoints = GetNeighbours(point);

                    //Če je vrednost točke manjša potem prištejemo njeno vrednost plus 1 v spremenljivko riskLevel
                    if (IsSmaller(point, neighbourPoints))
                    {
                        riskLevel += (point.Value + 1);
                        lowPoints.Add(point);
                    }
                }
            }
            return riskLevel;
        }

        public static int PartTwo(string inputFile)
        {
            InitializeData(inputFile);

            //Gnezdeni seznam, razlog je da
            // List Basin:
            //  - Prva kotlina:
            //      +Ena izmed točk ki je vsebovana v kotlini
            //      +Ena izmed točk ki je vsebovana v kotlini
            //  - Druga kotlina:
            //      +Ena izmed točk ki je vsebovana v kotlini
            //      +Ena izmed točk ki je vsebovana v kotlini
            List<List<string>> basins = new List<List<string>>();

            //Zanko začenmo na najnižji točki kotline
            foreach (Point point in lowPoints)
            {
                //Spremenljivka basinPointCordinates beleži že zaznane točke v neki kotlini
                HashSet<string> existingPoints = new HashSet<string>() { point.ToString() };

                //Kreiramo podseznam za točke neke kotline, dodamo našo najnižjo točko, nato pa seznam dodamo seznamu vseh kotlin
                List<string> basinPoints = new List<string>() { point.ToString() };
                basins.Add(basinPoints);

                //Kličemo rekurzijo
                GetBasins(point, existingPoints, basins);
            }
            //Sortiramo seznam vseh kotlin glede na dolžino podseznamov (največji proti najmanjšemu)
            basins.Sort((a, b) => b.Count - a.Count);

            //Zmnožžimo prve 3 kotline
            return basins[0].Count * basins[1].Count * basins[2].Count;
        }
    }
}
