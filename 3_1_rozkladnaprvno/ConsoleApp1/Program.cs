using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            List<int> primefactors = new List<int>();
            int n = Convert.ToInt32(Console.ReadLine());
            Console.Write(n + "=");
            for (int i = 2; i < n + 1; i++)
            {
                while ((n % i) == 0)
                {
                    int noven = n / i;
                    n = noven;
                    primefactors.Add(i);
                }
            }        
            Console.Write(String.Join("*", primefactors));

        }
    }
}
