using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmiProjektna
{
    public class Day6
    {
        public static int PartOne(string file, int days)
        {
            string input = file.FileByLines().First();

            //Seznam namenjen shranjevanju vseh rib. Mora biti tipa long, ker dosežemo maksimum za int, in rezultat ni pravilen
            List<long> fishFamily = new List<long>();

            foreach (string fish in input.Split(','))
            {
                //za vsako ribo v našem inputu, preberemo njen interval in ga shranimo v naš seznam vseh rib
                int interval = int.Parse(fish);
                fishFamily.Add(interval);
            }

            //Dvojna for-zanka. Prvi del se sprehaja čez dneve, drugi del pa se sprehaja čez vse naše ribe
            for (int i = 0; i < days; i++)
            {
                for (int fish = 0; fish < fishFamily.Count(); fish++)
                {
                    //Preverjam če je riba na tem da ima otroke, če je njen timer postavimo na 6 in dodamo novo ribo z timerjem 9
                    if (fishFamily[fish] == 0)
                    {
                        fishFamily[fish] = 6;
                        fishFamily.Add(9);
                    }
                    //Če timer od ribe še ni enak 0, potem ga zgolj odštejemo
                    else
                    {
                        fishFamily[fish] -= 1;
                    }

                }
            }
            //Vrnemo število elementvo v seznamu
            return fishFamily.Count();
        }

        public static long PartTwo(string file, int days)
        {
            string input = file.FileByLines().First();

            //Pri prvem delu naloge shranjujemo vsako ribo posebaj. Če nastavimo preveliko število dni za katere bomo opazovali, se hitro zgodi da naletimo na težavo OutOfStack
            //Ker nas ne zanima vsaka riba posebaj, ampak zgolj koliko rib je, lahko ribe grupiramo po njihovih timerjih. Posledično lahko samo spreminjamo število timerjev
            long[] timerGroup = new long[9];
            foreach (var fish in input.Split(','))
            {
                //Preberemo timer za posamezno ribo in seštevamo enake timerje.
                int fishTimer = int.Parse(fish);
                timerGroup[fishTimer]++;
            }

            //For-zanka ki se sprehaja čez dni
            for (int currentDay = 0; currentDay < days; currentDay++)
            {
                //Vsak dan v začasno spremenljivko shranimo število rib katerih timer je enak 0
                long babyFishCount = timerGroup[0];
                //V naši tabeli timerjev počistimo število rib z timerjem 0
                timerGroup[0] = 0;
                //For-zanka ki se sprehaja čez čase katere lahko imajo timerji
                for (long timer = 1; timer <= 8; timer++)
                {
                    //Premakne število timerjev za eno polje nazaj
                    timerGroup[timer - 1] = timerGroup[timer];
                }

                //Prištejemo število novih rib k riba katerih timer je trenutno 6. Hkrati pa jih shranimo v tabelo kjer je timer enak 8
                timerGroup[6] += babyFishCount;
                timerGroup[8] = babyFishCount;
            }
            return timerGroup.Sum();
        }
    }
}
