using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmiProjektna
{
    class Day16
    {
        //Spremenljivka za vklop/izklop debugiranja
        private static bool debugFlag = false;

        public abstract class Packet
        {
            //privatne spremenljivke (zasčitene za pisanje samo znotraj objekta)
            //version - verzija paketa
            //typeID - tip paketa
            private int version, typeID;
            protected int packetSize;

            //javna lastnost Version, vrne verzijo paketa
            public int Version
            {
                get { return version; }
            }

            //javna lastnost TypeID, vrne tip paketa
            public int TypeID
            {
                get { return typeID; }
            }

            //javna lastnost PacketSize, vrne velikost/dolžino binarne kode paketa
            public int PacketSize
            {
                get { return packetSize; }
            }

            //Protected metoda ki prebere prve 3 bite glave, in iz nje razbere verzijo paketa
            protected int GetVersion(string packet)
            {
                return Convert.ToInt32(packet.Substring(0, 3), 2);
            }

            //Protected metoda ki prebere naslednje 3 bite glave, in iz nje razbere tip paketa
            protected int GetType(string packet)
            {
                return Convert.ToInt32(packet.Substring(3, 3), 2);
            }

            //Abstraktna metoda za izračun po tipu operatorja
            public abstract long Evaluate();

            //Abstraltna metoda za izračun seštevka verzij
            public abstract int SumVersions();

            //Konstruktor paketa
            public Packet(string packet)
            {
                this.version = GetVersion(packet);
                this.typeID = GetType(packet);
            }

            //Virtualna metoda, namenjena izpisu paketa. Omogoča prepis v pod-razredih
            public virtual void PrintPacket()
            {
                Console.WriteLine($"########PACKET_START#########");
                Console.WriteLine($"Version: {this.Version}");
                Console.WriteLine($"TypeID: {this.TypeID}");
                Console.WriteLine($"########PACKET_END###########");
            }
        }

        public class LiteralPacket : Packet
        {
            //Privatna spremenljivka tipa long, namenjena je literalni vrednosti ki se skriva v literalnePaketu
            private long literalValue;

            //Javna lastnost Value, vrne literalno vrednost paketa
            public long Value
            {
                get { return literalValue; }
            }

            //Privatna metoda za branje binarne kode LiteralPacket-a
            private long ReadPayload(string binaryCode)
            {
                //Za debugiranje si lahko izpišemo binaryCode ki ga prejme paket
                if (debugFlag) { Console.WriteLine($"LiteralPacket - ReadPaylod. Recived msg = {binaryCode}"); }

                //Payload je del binaryCode-a. Lahko bi rekli da je del sporočila, brez glave paketa
                string payload = binaryCode.Substring(6, (binaryCode.Length - 6));

                //Nastavimo dve spremenljivki
                //"convertedPayload" - spremenljivka kamor bomo shranjevali prebran string številk v binarnem formatu
                //"lastGroup" - spremenljivka s katero beležimo ali smo v zadnjem delu števila LiteralPaketa
                string convertedPayload = String.Empty;
                bool lastGroup = false;

                //"packetSize" je spremenljivka ki beleži dolžino binarne kode, ker vemo da je glava LiteralPacketa sestavljena iz verzije (prvi trije biti) in pa tipa paketa (naslednji trije biti) vemo da lahko size takoj nastavimo na 6
                packetSize = 6;

                //Dokler še nismo v zadnjem delu števil izvajamo while zanko
                while (!lastGroup)
                {
                    //Če je se skupina 5 bitov slučajno začne z 0, potem vemo da gre za zadnjo skupino števil
                    if (payload[0] == '0')
                    {
                        lastGroup = true;
                    }

                    //preberemo dvojiški zapis
                    string group = payload.Substring(1, 4);

                    //Prebran string shranimo v convertedPayload
                    convertedPayload += group;

                    //Za payload uzamemo naslednji del stringa
                    payload = payload.Substring(5, (payload.Length - 5));

                    //Za vsako skupino tudi povečamo velikost paketa (s tem prištevamo kontrolni bit skupin, posledično prištejemo število vseh skupin)
                    packetSize += 1;
                }

                //Pretvorimo dvojiško število v desetiško
                literalValue = Convert.ToInt64(convertedPayload, 2);

                //V packetSize prištejemo še dolžino dvojiškega števila.
                packetSize += convertedPayload.Length;

                return literalValue;
            }


            //Override za izračun verzij
            public override int SumVersions()
            {
                //Ker vemo da LiteralPacket nima subpacketov vrnemo samo verzijo paketa
                return this.Version;
            }

            //Override za izračun z uporabo operatorjev
            public override long Evaluate()
            {
                //Ker gre za LiteralPacket vrnemo samo literalno vrednost pri izračunih
                return literalValue;
            }

            //Konstruktor za pod-razred LiteralPacket
            public LiteralPacket(string packet) : base(packet)
            {
                this.literalValue = ReadPayload(packet);
            }

            //Izpis LiteralnegaPaketa
            public override void PrintPacket()
            {
                Console.WriteLine($"########PACKET_START#########");
                Console.WriteLine($"Version: {this.Version}");
                Console.WriteLine($"TypeID: {this.TypeID} (LiteralPacket)");
                Console.WriteLine($"Size: {base.packetSize}");
                Console.WriteLine($"LiteralValue: {this.Value}");
                Console.WriteLine($"########PACKET_END###########");
            }

        }

        public class OperatorPacket : Packet
        {
            //privatna spremenljivka za shranjevanje bita, ki pove kako prebrati/razbrati število vsebovanih paketov
            // lengthTypeID = 0 -> prebermo naslednjih 15 bitov, le ti nam povejo totalLength v obliki številka koliko bitov vsebujejo vsi podpaketi skupaj
            // lengthTypeID = 1 -> prebermo naslednjih 11 bitov, le ti nam povejo totalLength v obliki številka koliko podpaketov vsebuje OperatorPacket
            private int lengthTypeID;

            //privatna spremenljivka za branje števila bitov podpaketov oziroma hranjenje števila podpaketov
            private int totalLength;

            //Javni seznam vseh podpaketov
            public List<Packet> subPackets = new List<Packet>();

            //Javna lasnost za bit lengthTypeID
            public int LengthType
            {
                get { return lengthTypeID; }
            }

            //Javna lastnost za branje števila bitov podpaketov oziroma hranjenje števila podpaketov
            public int TotalLength
            {
                get { return totalLength; }
            }



            //Privatna metoda za branje števila bitov podpaketov
            private int GetSubpacketsLength(string binaryCode)
            {
                //Če je lengthTypeID enak 0 potem preberemo naslednjih 15 bitov in iz njega razberemo totalLength
                if (lengthTypeID == 0)
                {
                    return Convert.ToInt32(binaryCode.Substring(7, 15), 2);
                }
                //Drugače preberemo naslednjih 11 bitov in iz njih razberemo število podpaketov
                else
                {
                    return Convert.ToInt32(binaryCode.Substring(7, 11), 2);
                }
            }

            //Privatna metoda za pridobivanje dolžine binarne kode ki sestavlja OperatorPacket
            private int GetSize()
            {
                //packetSize na začetku nastavimo na 7 (6 bitov sestavlja glavo + 1 bit za lengthTypeID)
                packetSize = 7;

                //Če je lengthTypeID enak 0 potem prištejemo dolžini binarni kode še 15, drugače prištejemo 11
                packetSize += (lengthTypeID == 0 ? 15 : 11);

                //Kličemo foreach zanko ki gre čez vse podpakete in prišteje packetSize
                foreach (Packet packet in subPackets)
                {
                    packetSize += packet.PacketSize;
                }

                return packetSize;
            }

            //Metoda za branje podpaketov
            private void ReadPayload(string binaryCode)
            {
                //Nastavimo si index s katerim bomo razbrali do kje smo že prebrali binarno kodo
                int subPacketIndex;

                //Debug izpis
                if (debugFlag)
                {
                    Console.WriteLine($"OperatorPackage Payload = {binaryCode}");
                    Console.WriteLine($"LengthTypeID = {lengthTypeID}");
                }

                //Preverimo ali gre za lengthTypeID = 0, potem vemo da moramo prebrati 15 bitov da pridobimo dolžino
                if (lengthTypeID == 0)
                {
                    //nastavimo si spremenljivko s katero bomo beležili koliko bitov moramo še prebrati
                    int bitsLeftToRead = totalLength;

                    //nastavimo pri katerem indeksu moramo začeti brati
                    //glava paketa + totalLengthID + 15 ker je lengthTypeID enak 0
                    subPacketIndex = 6 + 1 + 15;

                    //izvajamo while zanko dokler imamo še kaj bitov za prebrat
                    while (bitsLeftToRead > 0)
                    {
                        //Naredimo substring na binarni kodi, začnemo pri indeksu ki si ga nastavimo
                        string binaryCodePart = binaryCode.Substring(subPacketIndex, binaryCode.Length - subPacketIndex);

                        //Preverimo katerega tipa je naslednji paket. Če je tip enak 4 potem ustvarimo LiteralPacket
                        if (GetType(binaryCodePart) == 4)
                        {
                            LiteralPacket packet = new LiteralPacket(binaryCodePart);

                            //Dodamo na novo ustvarjen paket v listo podpaketov
                            subPackets.Add(packet);

                            //Indeks povečamo za dolžino novo ustvarjenega paketa
                            subPacketIndex += packet.PacketSize;

                            //Odštejemo dolžino novega paketa od številka bitov ki jih še moramo prebrati
                            bitsLeftToRead -= packet.PacketSize;

                        }
                        //Če ni enak 4 potem ustvarimo OperatorPacket
                        else
                        {
                            OperatorPacket packet = new OperatorPacket(binaryCodePart);

                            //Dodamo na novo ustvarjeni paket v listo podpaketov
                            subPackets.Add(packet);

                            //Indeks povečamo za dolžino novo ustvarjenega paketa
                            subPacketIndex += packet.PacketSize;

                            //Odštejemo dolžino novega paketa od številka bitov ki jih še moramo prebrati
                            bitsLeftToRead -= packet.PacketSize;
                        }
                    }
                }
                //Preverimo ali gre za lengthTypeID = 1, potem vemo da moramo prebrati 11 bitov da pridobimo število podpaketov
                else if (lengthTypeID == 1)
                {
                    //Nastavimo začasno spremenljivko s katero beležimo koliko paketov še moramo prebrati
                    int numOfPackets = totalLength;

                    //nastavimo pri katerem indeksu moramo začeti brati
                    //glava paketa + totalLengthID + 11 ker je lengthTypeID enak 1
                    subPacketIndex = 6 + 1 + 11;

                    //Izvajamo while zanko dokler imam pakete za prebrati
                    while (numOfPackets > 0)
                    {
                        //Naredimo substring na binarni kodi, začnemo pri indeksu ki si ga nastavimo
                        string binaryCodePart = binaryCode.Substring(subPacketIndex, binaryCode.Length - subPacketIndex);

                        //Debug prikaz
                        if (debugFlag)
                        {
                            Console.WriteLine($"Trying to create new packakage. Recived msg = {binaryCodePart}");
                        }

                        //Preverimo katerega tipa je naslednji paket. Če je tip enak 4 potem ustvarimo LiteralPacket
                        if (GetType(binaryCodePart) == 4)
                        {
                            LiteralPacket packet = new LiteralPacket(binaryCodePart);

                            //Dodamo na novo ustvarjen paket v listo podpaketov
                            subPackets.Add(packet);

                            //Indeks povečamo za dolžino novo ustvarjenega paketa
                            subPacketIndex += packet.PacketSize;

                            //Odštejemo en paket iz števila koliko jih še moramo obdelati
                            numOfPackets--;
                        }

                        //Če ni enak 4 potem ustvarimo OperatorPacket
                        else
                        {
                            OperatorPacket packet = new OperatorPacket(binaryCodePart);

                            //Dodamo na novo ustvarjeni paket v listo podpaketov
                            subPackets.Add(packet);

                            //Indeks povečamo za dolžino novo ustvarjenega paketa
                            subPacketIndex += packet.PacketSize;

                            //Odštejemo en paket iz števila koliko jih še moramo obdelati
                            numOfPackets--;
                        }
                    }
                }
            }

            //Override za izračun verzij
            public override int SumVersions()
            {
                //Ker imajo paketi tipa Operator tudi podpakete, najprej v spremenljivko shranimo verzijo trenutnega paketa
                int versionSum = this.Version;

                //Če OperatorPacket ima podpakete,potem se sprehodimo čez vsak podpaket in prištejemo še njihovo verzijo
                if (subPackets.Count > 0)
                {
                    foreach (Packet subpacket in subPackets)
                    {
                        versionSum += subpacket.SumVersions();
                    }
                }
                return versionSum;
            }

            //Override za izračun z uporabo operatorjev
            public override long Evaluate()
            {
                long result = 0;
                switch (TypeID)
                {
                    case 0:
                        result = 0;
                        foreach (Packet packet in subPackets)
                        {
                            result += packet.Evaluate();
                        }
                        break;

                    case 1:
                        long product = 1;
                        foreach (Packet packet in subPackets)
                        {
                            product *= packet.Evaluate();
                        }
                        result = product;
                        break;

                    case 2:
                        long minimum = int.MaxValue;
                        foreach (Packet packet in subPackets)
                        {
                            if (packet.Evaluate() < minimum)
                            {
                                minimum = packet.Evaluate();
                            }
                        }
                        result = minimum;
                        break;

                    case 3:
                        long maximum = int.MinValue;
                        foreach (Packet packet in subPackets)
                        {
                            if (packet.Evaluate() > maximum)
                            {
                                maximum = packet.Evaluate();
                            }
                        }
                        result = maximum;
                        break;

                    case 5:
                        result = subPackets[0].Evaluate() > subPackets[1].Evaluate() ? 1 : 0;
                        break;

                    case 6:
                        result = subPackets[0].Evaluate() < subPackets[1].Evaluate() ? 1 : 0;
                        break;

                    case 7:
                        result = subPackets[0].Evaluate() == subPackets[1].Evaluate() ? 1 : 0;
                        break;

                    default: break;
                }
                return result;
            }

            //Konstruktor za OperatorPacket
            public OperatorPacket(string binaryCode) : base(binaryCode)
            {
                //Prebermo lengthTypeID iz binarne kode
                lengthTypeID = int.Parse(binaryCode[6].ToString());

                //Pridobimo podatek koliko podpaketov vsebuje paket
                totalLength = GetSubpacketsLength(binaryCode);

                //Preberemo podpakete
                ReadPayload(binaryCode);

                //Preberemo dolžino našega parent paketa
                GetSize();

                //Debug izpis
                if (debugFlag)
                {
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~Creating new OperatorPackage~~~~~~~~~~~~~~~~~~~~~~~~~");
                    PrintSubpackets();
                }
            }

            //Metoda za izpis paketa tipa OperatorPacket
            public override void PrintPacket()
            {
                Console.WriteLine($"########PACKET_START#########");
                Console.WriteLine($"Version: {this.Version}");
                Console.WriteLine($"TypeID: {this.TypeID} (OperatorPacket)");
                Console.WriteLine($"Size: {this.packetSize}");
                Console.WriteLine($"LengthTypeID: {this.lengthTypeID}");
                Console.WriteLine($"MessageLength: {this.totalLength}");
                Console.WriteLine($"########PACKET_END###########");
            }

            //Metoda za izpis vseh podpaketov parent paketa
            public void PrintSubpackets()
            {
                //Zanka ki gre čez vse pakete v seznamu podpaketov
                foreach (Packet p in subPackets)
                {
                    //Kličemo izpis za paket
                    p.PrintPacket();
                }
            }
        }


        //Metoda za branje paketov iz naše vhodnih podatkov
        private static Packet ParsePacket(string binaryCode)
        {
            //Ker vemo da se vsak paket začne z neko glavo, lahko iz te glave takoj pridobimo tip paketa
            int packetType = Convert.ToInt32(binaryCode.Substring(3, 3), 2);

            //Če je TypeID paketa enak 4 potem vemo da gre za LiteralPacket
            if (packetType == 4)
            {
                //Ustavrimo nov objekt tipa LiteralPacket
                LiteralPacket literalPacket = new LiteralPacket(binaryCode);

                if (debugFlag) { literalPacket.PrintPacket(); }

                return literalPacket;
            }
            //Če je TypeID karkoli drugega, potem vemo da gre za OperatorPacket
            else
            {
                //Ustvarimo nov objekt tipa OperatorPacket
                OperatorPacket operatorPacket = new OperatorPacket(binaryCode);

                if (debugFlag)
                {
                    operatorPacket.PrintPacket();
                    Console.WriteLine($"\n\n Contained packages are:");
                    operatorPacket.PrintSubpackets();
                }
                return operatorPacket;
            }
        }

        //Metoda za branje datoteke/vnosa
        private static string ReadInput(string input)
        {
            //Če je vklopljeno debugiranje potem izpiše prejet input
            if (debugFlag) { Console.WriteLine($"Day 16.PartOne input = {input}"); }

            string hexadecimalCode = String.Empty;

            //Preverim če je moj input lokacija datoteke, če je potem datoteko preberem. V primeru da ni potem samo prekopiram input v hexadecimalCode
            if (input.Contains("C:\\Users"))
            {
                if (debugFlag) { Console.WriteLine("This is a file"); }
                hexadecimalCode = input.FileByLines().First();
            }
            else
            {
                hexadecimalCode = input;
            }

            //Pretrvorim iz HEX v BIN zapis.
            return Extensions.HexToBin(hexadecimalCode);
        }


        public static int PartOne(string input)
        {
            //Preberemo input, ga pretvorimo iz HEX v BIN, potem ustvarimo pakete in kličemo metodo SumVersion
            return ParsePacket(ReadInput(input)).SumVersions();
        }

        public static long PartTwo(string input)
        {
            //Preberemo input, ga pretvorimo iz HEX v BIN, potem ustvarimo pakete in kličemo metodo Evaluate
            return ParsePacket(ReadInput(input)).Evaluate();
        }
    }
}
