namespace Namespace
{

    using System;

    using System.Collections.Generic;

    using System.Linq;

    class Hledanicesty
    {

        class cell
        {
            public int x, y;
            public int dist;

            public cell(int x = 0, int y = 0, int vzdalenost = 0)
            {
                this.x = x;
                this.y = y;
                this.dist = vzdalenost;
            }

        }

        static bool Jevsachovnici(int x, int y)
        {
            if (x >= 1 && x <= 8 && y >= 1 && y <= 8)
                return true;
            return false;

        }

        static void Main()

        {
            int pocetradku = Convert.ToInt32(Console.ReadLine());
            List<int> souradniceprekazek = new List<int>();
            int pocetprekazek = 0;

            for (int i = 1; i < pocetradku + 1; i++)
            {
                var souradnice = Console.ReadLine().Split(' ')
                    .Select(input =>
                    {
                        int? output = null;
                        if (int.TryParse(input, out var parsed))
                        {
                            output = parsed;
                        }
                        return output;
                    })
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .ToList();

                if (souradnice.Count == 2)
                {
                    souradniceprekazek.AddRange(souradnice);
                    pocetprekazek++;
                }
                souradnice.Clear();
            }

            var startsouradnice = Console.ReadLine().Split(' ')
                    .Select(input =>
                    {
                        int? output = null;
                        if (int.TryParse(input, out var parsed))
                        {
                            output = parsed;
                        }
                        return output;
                    })
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .ToList();

            var cilsouradnice = Console.ReadLine().Split(' ')
                    .Select(input =>
                    {
                        int? output = null;
                        if (int.TryParse(input, out var parsed))
                        {
                            output = parsed;
                        }
                        return output;
                    })
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .ToList();


            int[] dx = { 1, 1, -1, -1, 1, -1, 0, 0 };
            int[] dy = { 1, -1, -1, 1, 0, 0, -1, 1 };

            if (startsouradnice.Count < 2 | cilsouradnice.Count < 2)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            Queue<cell> fronta = new Queue<cell>();
            fronta.Enqueue(new cell(startsouradnice[0], startsouradnice[1], 0));
            List<List<int>> paths = new List<List<int>>();
            paths.Add(new List<int> {0,0, startsouradnice[0], startsouradnice[1], 0 });

            cell t;
            bool[,] visit = new bool[9, 9];

            for (int k = 1; k <= 8; k++)
                for (int l = 1; l <= 8; l++)
                    visit[k, l] = false;

            for (int a = 0, b = 1; a < 2 * pocetprekazek; a += 2, b += 2)
                visit[souradniceprekazek[a], souradniceprekazek[b]] = true;

            visit[startsouradnice[0], startsouradnice[1]] = true;
            int vzdalenost = -1; 

            while (fronta.Count != 0)
            {
                t = fronta.Peek();
                fronta.Dequeue();

                
                    if (t.x == cilsouradnice[0] && t.y == cilsouradnice[1])
                {
                    vzdalenost = t.dist;
                    break;
                }

                int o, p;

                for (int i = 0; i < 8; i++)
                {
                    o = t.x + dx[i];
                    p = t.y + dy[i];

                    if (Jevsachovnici(o, p) && !visit[o, p])
                    {
                        visit[o, p] = true;
                        fronta.Enqueue(new cell(o, p, t.dist + 1));
                        paths.Add(new List<int> {t.x,t.y, o, p, t.dist + 1});
                    }
                }
            }
            //Console.WriteLine(vzdalenost);
            if (vzdalenost == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            int u = cilsouradnice[0];
            int v = cilsouradnice[1];
            List<List<int>> path = new List<List<int>>();
            path.Add(new List<int> {u,v});


            for (int i = vzdalenost; i>= 0; i--)
            {
                foreach (List<int> ListofPoints in paths)
                    if (ListofPoints[4] == i && (ListofPoints[2] == u & ListofPoints[3] == v))
                    {
                        u = ListofPoints[0];
                        v = ListofPoints[1];
                        path.Add(new List<int> { u, v });
                    }
            }


            for (int n = path.Count()-2; n>=0; n-- )
                Console.WriteLine(String.Join(' ', path[n]));
        }
    }
}
            for (int i = 1; i <= 10; i++)
            {
                string line = Console.ReadLine();
                List<string> characters = line.Select(x => x.ToString()).ToList();
                for (int n = 0; n <= 9; n++)
                {
                    if (characters[n] == "C")
                    {
                        end[0] = i;
                        end[1] = n;
                    }
                    if (characters[n] == "B")
                    {
                        box[0] = i;
                        box[1] = n;
                    }
                    if (characters[n] == "S")
                    {
                        sokoban[0] = i;
                        sokoban[1] = n;
                    }
                }
                warehouse.Add(characters);
            }

                        List<List<string>> warehouse = new List<List<string>>()
            {
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", "X", ".", ".", ".", "." },
               new List<string> { ".", ".", "S", ".", ".", "B", "X", "C", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." }
            };

                        for (int i = 1; i <= 10; i++)
            {
                string line = Console.ReadLine();
                List<string> characters = line.Select(x => x.ToString()).ToList();
                for (int n = 0; n <= 9; n++)
                {
                    if (characters[n] == "C")
                    {
                        end[0] = i;
                        end[1] = n;
                    }
                    if (characters[n] == "B")
                    {
                        box[0] = i;
                        box[1] = n;
                    }
                    if (characters[n] == "S")
                    {
                        sokoban[0] = i;
                        sokoban[1] = n;
                    }
                }
                warehouse.Add(characters)

            for (int i = 1; i <= 10; i++)
            {
                string line = Console.ReadLine();
                List<string> characters = line.Select(x => x.ToString()).ToList();
                for (int n = 0; n <= 9; n++)
                {
                    if (characters[n] == "C")
                    {
                        end[0] = i;
                        end[1] = n;
                    }
                    if (characters[n] == "B")
                    {
                        box[0] = i;
                        box[1] = n;
                    }
                    if (characters[n] == "S")
                    {
                        sokoban[0] = i;
                        sokoban[1] = n;
                    }
                }
                warehouse.Add(characters);
            }

