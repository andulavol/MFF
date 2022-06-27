using System;
using System.Collections.Generic;

using System.Linq;


namespace ReverseArray
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    .ToArray();

            Array.Reverse(listofints);

            Console.WriteLine(String.Join(' ', listofints));
        }
    }
}
