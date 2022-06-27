using System;
using System.Collections.Generic;

using System.Linq;


namespace evenandodd
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> suda = new List<int>();
            List<int> licha = new List<int>();
            var listofints = Console.ReadLine().Split(' ')
                        .Select(input =>
                        {
                            int? output = null;
                            if (int.TryParse(input, out var parsed))
                            {
                                output = parsed;
                            }
                            return output;
                        })
                        .Where(x => x != null & x != -1)
                        .Select(x => x.Value)
                        .ToList();
            foreach (int i in listofints)
                if (i % 2 == 0)
                {
                    suda.Add(i);
                }
                else
                {
                    licha.Add(i);
                }
            Console.Write(String.Join(' ', suda) + ' ');
            Console.Write(String.Join(' ', licha));
        }
    }
}
