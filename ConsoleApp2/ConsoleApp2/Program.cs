using System;
using System.Collections.Generic;
using System.Linq;

namespace Rozsireni
{
    public static class MojeTrida
    {
        public static T[] Podseznam<T>(this T[] data, int index, int delka)
        {
            T[] vysledek = new T[delka];
            Array.Copy(data, index, vysledek, 0, delka);
            return vysledek;
        }
    }
}

namespace Programek
{
    using Rozsireni;
    class Program
    {
        

        public static void rozdeleni(int z, int p, int[] seznam, List<int> mince)
        {
            if (z == 0)
            {
                //int[] subseznam = seznam.Podseznam(1,p);
                Console.WriteLine(String.Join(" ", seznam.Podseznam(1, p-1)));
            }
            else
            {
                for (int k = Math.Min(z, seznam[p-1]); k >= 0; k--)
                {
                    if (mince.Contains(k))
                    {
                        seznam[p] = k;
                        rozdeleni(z - k, p + 1, seznam, mince);
                    }
                }
            }
            
        }

        static void Main(string[] args)
        {
            int pocetminci = Convert.ToInt32(Console.ReadLine());

            string vstuptypyminci = Console.ReadLine();
            string[] stringmince = vstuptypyminci.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<int> typyminci = new List<int>();
            int hodnota;
            for (int i = 0; i < pocetminci; i++)
            {
                if (int.TryParse(stringmince[i], out hodnota))
                {
                    typyminci.Add(Int32.Parse(stringmince[i]));
                }
            }

            int suma = Convert.ToInt32(Console.ReadLine());

            int[] a = new int[suma+1];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = suma;
            }

            rozdeleni(suma, 1, a, typyminci);

        }
    }
}
