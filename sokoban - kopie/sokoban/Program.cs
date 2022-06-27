
namespace sokoban
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    class Program
    {
        // musim hlidat 3 policka
        // musim hlidat i smer
        class cell
        {
            public int x, y, smer, dist;

            public cell(int x = 0, int y = 0, int smer = 0, int vzdalenost = 0)
            {
                this.x = x;
                this.y = y;
                this.smer = smer;
                this.dist = vzdalenost;
            }

        }

        static bool CanBoxMovetoxy(List<List<string>> warehouse, int x, int y)
        {
            if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
            {
                if (warehouse[x][y] != "x")
                        return true;
            }
            return false;

        }


        static void Main(string[] args)
        {
            int[] end = new int[2];
            end[0] = -1;
            end[1] = -1;
            int[] start = new int[2];
            start[0] = -1;
            start[1] = -1;

            List<List<string>> chessboard = new List<List<string>>();

            for (int i = 0; i <= 7; i++)
            {
                string line = Console.ReadLine();
                List<string> characters = line.Select(x => x.ToString()).ToList();
                for (int n = 0; n <= 7; n++)
                {
                    if (characters[n] == "c")
                    {
                        end[0] = i;
                        end[1] = n;
                    }
                    if (characters[n] == "v")
                    {
                        start[0] = i;
                        start[1] = n;
                    }
                }
                chessboard.Add(characters);
            }

            if (end[0] == -1 && end[1] == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            if (start[0] == -1 && start[1] == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            Queue<cell> que = new Queue<cell>();
            cell t;
            que.Enqueue(new cell(start[0], start[1], 0, 0));
            bool[,] visited = new bool[8, 8];

            visited[start[0], start[1]] = true;
            int smerveze = 0;
            int tahy = 100;

            while (que.Count != 0)
            {
                t = que.Peek();
                que.Dequeue();


                if (t.x == end[0] && t.y == end[1])
                {
                    if (t.dist < tahy)
                        tahy = t.dist;
                }

                int o, p;

                List<cell> moves = new List<cell>();
                for (int i = 0; i <= 3; i++)
                {

                    o = t.x + dx[i];
                    p = t.y + dy[i];
                    if (o > t.x) // sever
                    {
                        smerveze = 1;
                    }
                    if (o < t.x) //  jih
                    {
                        smerveze = 2;
                    }

                    if (p > t.y) // východ
                    {
                        smerveze = 3;
                    }
                    if (p < t.y) // západ
                    {
                        smerveze = 4;
                    }

                    if (CanBoxMovetoxy(chessboard, o, p) && !visited[o, p])
                    {
                        visited[o, p] = true;
                        if (t.smer != smerveze)
                            que.Enqueue(new cell(o, p, smerveze, t.dist + 1));
                        if (t.smer == smerveze || t.smer == 0)
                            que.Enqueue(new cell(o, p, smerveze, t.dist));
                    }

                }

            }

            if (tahy != 100)
            {
                if (tahy == 0)
                {
                    Console.WriteLine(1);
                    System.Environment.Exit(0);
                }
                Console.WriteLine(tahy);
                System.Environment.Exit(0);
            }
            Console.WriteLine(-1);
        }
    }
}
