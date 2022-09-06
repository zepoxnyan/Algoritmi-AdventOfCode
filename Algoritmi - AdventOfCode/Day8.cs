using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace AlgoritmiProjektna
{
    public class Day8
    {
        public static int PartOne(string file)
        {
            int sumOfLights = 0;

            foreach (string line in file.FileByLines())
            {
                //Delimo vrstico na kodo lučk in pa sam signal lučk
                string[] delimitedLine = line.Split(" | ");
                string[] ledLine = delimitedLine[1].Split(" ");

                //Foreach zanka ki se sprekodi čez vse nize lučk
                foreach (var segment in ledLine)
                {
                    //Ker vemo sledeče:
                    //  2 lučki = številka 1
                    //  3 lučke = številka 7
                    //  4 lučke = številka 4
                    //  7 lučk = številka 8
                    //Tako lahko preprosto pogledamo dolžino posameznega niza
                    if (segment.Length == 2 || segment.Length == 3 || segment.Length == 4 || segment.Length == 7)
                    {
                        sumOfLights++;
                    }
                }
            }
            return sumOfLights;
        }

        public static int PartTwoDecoder(string input)
        {
            //Razdelimo vrstico na ziče in pa same lučke
            string[] delimitedLine = input.Split(" | ");
            string[] ledWires = delimitedLine[0].Split(" ");
            string[] ledLine = delimitedLine[1].Split(" ");

            //Ustvarimo še tabelo poznanih številk, v katero bomo shranjevali nize za katere vemo katero število predstavljajo
            string[] knownNumbers = new string[10];

            //Pričnemo z iskanjem najlažjih števil (1,4,7,8)
            knownNumbers[1] = ledWires.First(x => x.Length == 2);
            knownNumbers[4] = ledWires.First(x => x.Length == 4);
            knownNumbers[7] = ledWires.First(x => x.Length == 3);
            knownNumbers[8] = ledWires.First(x => x.Length == 7);

            //Števila ki so sestavljena iz 6 lučk so lahko sledeča 6,9,0
            //Za število 6 vemo da ne vsebuje vseh lučke ki jih vsebuje število 4, in ne vsebuje vseh lučk s katero lahko prikažemo število 1
            //Za število 0 vemo da ne vsebuje vseh lučke ki jih vsebuje število 4, vsebuje pa vsaj eno lučko s katero lahko prikažemo število 1
            //Za število 9 pa vemo da mora vsebovati vsaj iste lučke kot število 4
            knownNumbers[6] = ledWires.First(x => x.Length == 6 && !Extensions.HaveSameLetters(x, knownNumbers[4]) && !Extensions.HaveSameLetters(x, knownNumbers[1]));
            knownNumbers[0] = ledWires.First(x => x.Length == 6 && !Extensions.HaveSameLetters(x, knownNumbers[4]) && Extensions.HaveSameLetters(x, knownNumbers[1]));
            knownNumbers[9] = ledWires.First(x => x.Length == 6 && Extensions.HaveSameLetters(x, knownNumbers[4]));

            //Števila ki so sestavljena iz 5 lučk so lahko sledeča 2,3,5
            //Za število 2 vemo da ne vsebuje vseh lučke ki jih vsebuje število 1, in ne vsebuje vseh lučk s katero lahko prikažemo število 6
            //Za število 5 vemo da ne vsebuje vseh lučke ki jih vsebuje število 1, vsebuje pa vsaj eno lučko s katero lahko prikažemo število 6
            //Za število 3 pa vemo da mora vsebovati vsaj iste lučke kot število 1
            knownNumbers[2] = ledWires.First(x => x.Length == 5 && !Extensions.HaveSameLetters(x, knownNumbers[1]) && !Extensions.HaveSameLetters(x, knownNumbers[6]));
            knownNumbers[5] = ledWires.First(x => x.Length == 5 && !Extensions.HaveSameLetters(x, knownNumbers[1]) && Extensions.HaveSameLetters(x, knownNumbers[6]));
            knownNumbers[3] = ledWires.First(x => x.Length == 5 && Extensions.HaveSameLetters(x, knownNumbers[1]));

            string decodedNumber = String.Empty;

            //For-zanka ki se sprehodi čez vse dele prikazanih lučk
            foreach (string segment in ledLine)
            {
                //Najprej po abecedi uredimo niz številk iz prikazanih številk
                string sortedSegment = String.Concat(segment.OrderBy(c => c));

                //For zanka ki se sprehaja čez tabelo znanih števil
                for (int number = 0; number < knownNumbers.Length; number++)
                {
                    //Sortiramo še sam niz poznane številke po abecedi
                    string sortedNumber = String.Concat(knownNumbers[number].OrderBy(c => c));

                    //Če se niza ujemata, potem niz prištejemo v dekodirni niz
                    if (sortedSegment.Equals(sortedNumber))
                    {
                        decodedNumber += number;
                    }
                }

            }
            //Vrnemo niz ki ga pretvorimo v število
            return int.Parse(decodedNumber);
        }

        public static int PartTwo(string file)
        {
            //Spremenljivka ki hrani število ki ga želimo prebrati iz kodiranega niza lučk
            int sumOfLights = 0;

            foreach (string line in file.FileByLines())
            {
                //Za vsako vrstico v datoteki kličemo naš dekodirnik
                sumOfLights += PartTwoDecoder(line);
            }
            return sumOfLights;
        }
    }

}
