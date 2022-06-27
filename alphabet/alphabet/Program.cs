using System;

namespace alphabet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    class Program
    {
        class cell
        {
            public int x, y, dist, pis;

            public cell(int x = 0, int y = 0, int index = 0, int vzdalenost = 0)
            {
                this.x = x;
                this.y = y;
                this.pis = index;
                this.dist = vzdalenost;
            }

        }

        static int PathBetweenChars(string[,] map, int x, List<string> word, int width, int heigth)
        {
            int[] dx2 = { 0, 0, 1, -1 };
            int[] dy2 = { 1, -1, 0, 0 };
            Queue<cell> pathque = new Queue<cell>();
            pathque.Enqueue(new cell(0, 0, 0, 0));
            cell v;
            int wordlen;
            int keystrokes = 99;

            while (pathque.Count != 0)
            {
                v = pathque.Peek();
                wordlen = v.pis;
                pathque.Dequeue();
                if (pathque.Count() > 50)
                    return keystrokes;

                if (map[v.x, v.y] == word[v.pis])
                {
                    if (v.pis + 1 <= x)
                        pathque.Enqueue(new cell(v.x, v.y, v.pis+1, v.dist+1));
                    if (x == wordlen-1)
                        if (v.dist < keystrokes)
                            keystrokes = v.dist;

                }

                int o, p;
                for (int i = 0; i <= 3; i++)
                {
                    o = v.x + dx2[i];
                    p = v.y + dy2[i];
                    if (o >= 0 && o <= heigth - 1 && p >= 0 && p <= width - 1)
                    {
                        if (v.pis <= x)
                            pathque.Enqueue(new cell(o, p, v.pis, v.dist + 1));
                    }

                }
            }
            return keystrokes;
        }
        static void Main(string[] args)
        {
            string line1 = Console.ReadLine();
            List<string> widtharr = line1.Select(x => x.ToString()).ToList();
            int width = int.Parse(widtharr[0]);

            string line2 = Console.ReadLine();
            List<string> heightarr = line2.Select(x => x.ToString()).ToList();
            int heigth = int.Parse(heightarr[0]);

            string line3 = Console.ReadLine();
            List<string> grid = new List<string>();
            if (string.IsNullOrEmpty(line3))
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }
            else
            {
                line3.Trim();
                grid = line3.Select(x => x.ToString()).ToList();
            }

            int delka = 0;
            string[,] map = new string[heigth, width];
            for (int i = 0; i <= heigth - 1; i++)
                for (int j = 0; j <= width - 1; j++)
                {
                    map[i, j] = grid[delka];
                    delka++;
                }

            string line4 = Console.ReadLine();
            List<string> word = line4.Select(x => x.ToString()).ToList();
            if (string.IsNullOrEmpty(line4))
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            int lenght = word.Count();
            int vysledek = PathBetweenChars(map, lenght-1, word, width, heigth);
            if (vysledek ==  99)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }
            Console.WriteLine(vysledek);
        }
    }
}
