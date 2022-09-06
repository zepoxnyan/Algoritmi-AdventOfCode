using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgoritmiProjektna
{
    public static class Extensions
    {
        public static IEnumerable<string> FileByLines(this string path)
        {
            if (path == null)
            {
                yield break;
            }

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }


        public static int ReturnLineCount(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int lineCount = 0;
                    while ((sr.ReadLine()) != null)
                    {
                        lineCount++;
                    }
                    return lineCount;
                }
            }
        }

        public static string HexToBin(string input)
        {
            string covertedInput = "";
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '0':
                        covertedInput += "0000";
                        break;
                    case '1':
                        covertedInput += "0001";
                        break;
                    case '2':
                        covertedInput += "0010";
                        break;
                    case '3':
                        covertedInput += "0011";
                        break;
                    case '4':
                        covertedInput += "0100";
                        break;
                    case '5':
                        covertedInput += "0101";
                        break;
                    case '6':
                        covertedInput += "0110";
                        break;
                    case '7':
                        covertedInput += "0111";
                        break;
                    case '8':
                        covertedInput += "1000";
                        break;
                    case '9':
                        covertedInput += "1001";
                        break;
                    case 'A':
                        covertedInput += "1010";
                        break;
                    case 'B':
                        covertedInput += "1011";
                        break;
                    case 'C':
                        covertedInput += "1100";
                        break;
                    case 'D':
                        covertedInput += "1101";
                        break;
                    case 'E':
                        covertedInput += "1110";
                        break;
                    case 'F':
                        covertedInput += "1111";
                        break;
                    default:
                        Console.WriteLine($"There was a error converting {input[i]}");
                        break;
                }
            }
            return covertedInput;
        }

        public static bool HaveSameLetters(string a, string b)
        {
            string min, max;
            if (a.Length > b.Length)
            {
                max = a;
                min = b;
            }
            else
            {
                max = b;
                min = a;
            }
            int sameLetters = 0;
            for (int i = 0; i < max.Length; i++)
            {
                for (int j = 0; j < min.Length; j++)
                {
                    if (max[i] == min[j])
                    {
                        sameLetters++;
                    }
                }
            }
            if (sameLetters == min.Length)
            {
                return true;
            }

            return false;
        }
    }
}
