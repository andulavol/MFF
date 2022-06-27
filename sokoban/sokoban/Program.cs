
namespace sokoban
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    class Program
    {
        // sokoban musi najit box
        // hlidat, jestli neni okolo boxu zed
        // can sokoban push it there? zjistit směr a jestli na pozici sokobana není zed:)
        class cell
        {
            public int x, y, x2, y2;
            public int dist;

            public cell(int x = 0, int y = 0, int skbx = 0, int skby = 0, int vzdalenost = 0)
            {
                this.x = x;
                this.y = y;
                this.x2 = skbx;
                this.y2 = skby;
                this.dist = vzdalenost;
            }

        }

        static bool CanBoxMovetoxy(List<List<string>> warehouse, int x, int y, int skbx, int skby)
        {
            if (x >= 0 && x <= 9 && y >= 0 && y <= 9)
            {
                if (skbx >= 0 && skbx <= 9 && skby >= 0 && skby <= 9)
                    if ((warehouse[x][y] != "X") && (warehouse[skbx][skby] != "X"))
                        return true;
            }
            return false;

        }
        static int SokobanMoves(List<List<string>> warehouse, int x, int y, int boxx, int boxy, int skbx, int skby)
        {
            if (x == skbx && y == skby)
                return 0;
            int[] dx2 = { 0, 0, 1, -1 };
            int[] dy2 = { 1, -1, 0, 0 };
            Queue<cell> skbque = new Queue<cell>();
            skbque.Enqueue(new cell(skbx, skby, 0, 0, 0));
            cell v;

            bool[,] visit = new bool[10, 10];

            for (int k = 0; k <= 9; k++)
                for (int l = 0; l <= 9; l++)
                    if (warehouse[k][l] == "X")
                        visit[k, l] = true;

            visit[skbx, skby] = true;
            visit[boxx, boxy] = true;

            while (skbque.Count != 0)
            {
                v = skbque.Peek();
                skbque.Dequeue();


                if (v.x == x && v.y == y)
                {
                    return v.dist;
                }

                int o, p;
                for (int i = 0; i <= 3; i++)
                {
                    o = v.x + dx2[i];
                    p = v.y + dy2[i];
                    if (o >= 0 && o <= 9 && p >= 0 && p <= 9)
                    {
                        if (visit[o, p] == false)
                        {
                            visit[o, p] = true;
                            skbque.Enqueue(new cell(o, p, 0, 0, v.dist + 1));

                        }

                    }

                }
            }
            return -1;
        }


        static void Main(string[] args)
        {
            int[] end = new int[2];
            end[0] = -1;
            end[1] = -1;
            int[] sokoban = new int[2];
            sokoban[0] = -1;
            sokoban[1] = -1;
            int[] box = new int[2];
            box[0] = -1;
            box[1] = -1;
            List<int> walls = new List<int>();
            List<List<string>> warehouse = new List<List<string>>()
            {
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", "B", "X", "C", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", "S", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." },
               new List<string> { ".", ".", ".", ".", ".", ".", ".", ".", ".", "." }
            };

            for (int i = 0; i <= 9; i++)
                for (int n = 0; n <= 9; n++)
                {
                    if (warehouse[i][n] == "C")
                    {
                        end[0] = i;
                        end[1] = n;
                    }
                    if (warehouse[i][n] == "B")
                    {
                        box[0] = i;
                        box[1] = n;
                    }
                    if (warehouse[i][n] == "S")
                    {
                        sokoban[0] = i;
                        sokoban[1] = n;
                    }
                }


            if (end[0] == -1 && end[1] == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            if (sokoban[0] == -1 && sokoban[1] == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            if (box[0] == -1 && box[1] == -1)
            {
                Console.WriteLine(-1);
                System.Environment.Exit(0);
            }

            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            Queue<cell> que = new Queue<cell>();
            cell t;
            que.Enqueue(new cell(box[0], box[1], sokoban[0], sokoban[1], 0));
            bool[,] visited = new bool[10, 10];

            for (int k = 0; k <= 9; k++)
                for (int l = 0; l <= 9; l++)
                    if (warehouse[k][l] == "X")
                        visited[k, l] = true;

            visited[box[0], box[1]] = true;
            int vzdalenost = -1;
            int a = -1;
            int b = -1;

            while (que.Count != 0)
            {
                t = que.Peek();
                que.Dequeue();


                if (t.x == end[0] && t.y == end[1])
                {
                    vzdalenost = t.dist;
                    break;
                }

                int o, p;

                List<cell> moves = new List<cell>();
                for (int i = 0; i <= 3; i++)
                {

                    o = t.x + dx[i];
                    p = t.y + dy[i];
                    if (o > t.x) // sever
                    {
                        a = t.x - 1;
                        b = t.y;
                    }
                    if (o < t.x) //  jih
                    {
                        a = t.x + 1;
                        b = t.y;
                    }

                    if (p > t.y) // východ
                    {
                        a = t.x;
                        b = t.y - 1;
                    }
                    if (p < t.y) // západ
                    {
                        a = t.x;
                        b = t.y + 1;
                    }

                    if (CanBoxMovetoxy(warehouse, o, p, a, b) && !visited[o, p])
                    {
                        visited[o, p] = true;
                        int numberofmoves = SokobanMoves(warehouse, a, b, t.x, t.y, t.x2, t.y2);
                        if (numberofmoves != -1)
                            que.Enqueue(new cell(o, p, t.x, t.y, t.dist + numberofmoves + 1));
                    }

                }

            }

            if (vzdalenost != -1)
            {
                Console.WriteLine(vzdalenost);
                System.Environment.Exit(0);
            }
            Console.WriteLine(-1);
        }
    }
}
