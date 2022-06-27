using System;

namespace factorial
{
    class Program
    {
        public static ulong Factorial(ulong f)
        {
            if (f == 0)
                return 1;
            else
                return f * Factorial(f - 1);
        }

        static void Main(string[] args)
        {
            ulong cislo = Convert.ToUInt64(Console.ReadLine());
            ulong vysledek = Factorial(cislo);
            Console.WriteLine(vysledek);

        }
    }
}
