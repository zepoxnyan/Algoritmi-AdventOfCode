using System;
using System.IO;
using System.Reflection;

namespace AlgoritmiProjektna
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Dan 6
            //https://adventofcode.com/2021/day/6
            string day6file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + @"\Data\Day6Input.txt";
            Console.WriteLine($"Day 6 \t| Part one : {Day6.PartOne(day6file, 80)}");
            Console.WriteLine($"Day 6 \t| Part two : {Day6.PartTwo(day6file, 256)}");
           


            //Dan 8
            //https://adventofcode.com/2021/day/8
            string day8file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))+ @"\Data\Day8Input.txt";
            Console.WriteLine($"Day 8 \t| Part one : {Day8.PartOne(day8file)}");
            Console.WriteLine($"Day 8 \t| Part two : {Day8.PartTwo(day8file)}");



            //Dan 9
            //https://adventofcode.com/2021/day/9
            //
            string day9input = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + @"\Data\Day9Input.txt";
            Console.WriteLine($"Day 9 \t| Part one : {Day9.PartOne(day9input)}");
            Console.WriteLine($"Day 9 \t| Part two : {Day9.PartTwo(day9input)}");

            

            //Dan 16
            //https://adventofcode.com/2021/day/16
            //
            string day16input = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + @"\Data\Day16Input.txt";
            Console.WriteLine($"Day 16 \t| Part one : {Day16.PartOne(day16input)}");
            Console.WriteLine($"Day 16 \t| Part two : {Day16.PartTwo(day16input)}");

        }

    }
}
