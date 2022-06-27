using System;

namespace desetinnamista
{
    class Program
    {
        static string Trimafter6(string cislo, double cislo2)
        {
            char prvek;
            prvek = cislo[0];

            int index = 0;
            string hotovystring;

            while (index < cislo.Length && prvek != ',')
            {
                prvek = cislo[index];
                index++;
            }

            if (index == 0)
            {
                hotovystring = cislo2.ToString("N6");
                hotovystring = hotovystring.Replace(",", ".");
                return hotovystring;
            }

            if (cislo.Length < index + 5)
            {
                hotovystring = cislo2.ToString("N6");
                hotovystring = hotovystring.Replace(",", ".");
                return hotovystring;
            }
            hotovystring = cislo.Substring(0, index + 6);
            hotovystring = hotovystring.Replace(",", ".");
            return hotovystring;


        }
        static void Main(string[] args)
        {
            string line1 = Console.ReadLine();
            line1 = line1.Replace(".", ",");
            string line2 = Console.ReadLine();
            line2 = line2.Replace(".", ",");
            double cislo1 = Convert.ToDouble(line1);
            double cislo2 = Convert.ToDouble(line2);
            double mydouble = (double)cislo1 / (double)cislo2;
            string vysledek;
            vysledek = mydouble.ToString();
            Console.WriteLine(Trimafter6(vysledek, mydouble));

        }
    }
}
