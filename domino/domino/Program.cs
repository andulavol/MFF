using System;
using System.Collections.Generic;
using System.Linq;

namespace domino
{
    class Program
    {
        public static List<int> domino(List<List<int>> kostky, int delkasekvence, List<int> vysledek )
        {
            int a = kostky[0][0];
            int b = kostky[0][1];
            int delka = kostky[0][2];
            if (kostky.Count() == 1)
            {
                vysledek.Add(delkasekvence + delka);
                return vysledek;
            }
            kostky.RemoveAt(0);

            for (int l = 0; l < kostky.Count(); l++)
            {
                if ((kostky[l][0] == a || kostky[l][1] == b) || (kostky[l][0] == b || kostky[l][1] == a))
                {
                    domino(kostky, delka, vysledek);
                }

            }
            return vysledek;


        }
        static void Main(string[] args)
        {
            int pocetkostek = -1;

            string vstupcisla = Console.ReadLine();
            string[] stringcisla = vstupcisla.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<int> vstup = new List<int>();

            int hodnota;
            if (int.TryParse(stringcisla[0], out hodnota))
            {
                pocetkostek = (Int32.Parse(stringcisla[0]));
            }
            if (stringcisla.Length > 1)
            {
                for (int i = 1; i < stringcisla.Length; i++)
                {
                    if (int.TryParse(stringcisla[i], out hodnota))
                    {
                        vstup.Add(Int32.Parse(stringcisla[i]));
                    }
                }
            }


            while (vstup.Count() != pocetkostek * 2)
            {
                string line = Console.ReadLine();
                string[] stringline = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < stringline.Length; i++)
                {
                    if (int.TryParse(stringline[i], out hodnota))
                    {
                        vstup.Add(Int32.Parse(stringline[i]));
                    }
                }

            }
            List<List<int>> kostky = new List<List<int>>();
            List<int> pouzitacisla = new List<int>();

            for (int i = 0; i < vstup.Count(); i = i + 2)
            {
                List<int> cisla = new List<int>();
                int x = vstup[i];
                int y = vstup[i+1];
                if (i == 0)
                {
                    cisla.Add(x);
                    cisla.Add(y);
                    cisla.Add(1);
                    pouzitacisla.Add(x);
                    pouzitacisla.Add(y);
                    kostky.Add(cisla);
                }
                else
                {
                    if (pouzitacisla.Contains(x) && pouzitacisla.Contains(y))
                    {
                        foreach (List<int> list in kostky)
                        {
                            if ((list[0] == x && list[1] == y) || (list[0] == y && list[1] == x))
                                list[2] = list[2] + 1;
                        }
                    }
                    else
                    {
                        cisla.Add(x);
                        cisla.Add(y);
                        cisla.Add(1);
                        pouzitacisla.Add(x);
                        pouzitacisla.Add(y);
                    }

                }
            }
            List<int> listik = new List<int>();
            List<int> vysledek = domino(kostky, 0, listik );
            int max = vysledek.Max();
            Console.WriteLine(max.ToString());
            
        }

    }
}
