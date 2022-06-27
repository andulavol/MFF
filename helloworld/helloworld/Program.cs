using System;
using System.Linq;
using System.Collections.Generic;

namespace helloworld
{
    using System;
    using System.Collections.Generic;

    class GFG
    {

        // Class for storing a cell's data 
        public class cell
        {
            public int x, y;
            public int dis;

            public cell(int x, int y, int dis)
            {
                this.x = x;
                this.y = y;
                this.dis = dis;
            }
        }

        // Utility method returns true 
        // if (x, y) lies inside Board 
        static bool isInside(int x, int y, int N)
        {
            if (x >= 1 && x <= N && y >= 1 && y <= N)
                return true;
            return false;
        }

        // Method returns minimum step 
        // to reach target position 
        static int minStepToReachTarget(int[] knightPos,
                                        int[] targetPos, int N)
        {
            // x and y direction, where a knight can move 
            int[] dx = { -2, -1, 1, 2, -2, -1, 1, 2 };
            int[] dy = { -1, -2, -2, -1, 1, 2, 2, 1 };

            // queue for storing states of knight in board 
            Queue<cell> q = new Queue<cell>();

            // push starting position of knight with 0 distance 
            q.Enqueue(new cell(knightPos[0],
                               knightPos[1], 0));

            cell t;
            int x, y;
            bool[,] visit = new bool[N + 1, N + 1];

            // make all cell unvisited 
            for (int i = 1; i <= N; i++)
                for (int j = 1; j <= N; j++)
                    visit[i, j] = false;

            // visit starting state 
            visit[knightPos[0], knightPos[1]] = true;

            // loop untill we have one element in queue 
            while (q.Count != 0)
            {
                t = q.Peek();
                q.Dequeue();

                // if current cell is equal to target cell, 
                // return its distance 
                if (t.x == targetPos[0] && t.y == targetPos[1])
                    return t.dis;

                // loop for all reachable states 
                for (int i = 0; i < 8; i++)
                {
                    x = t.x + dx[i];
                    y = t.y + dy[i];

                    // If reachable state is not yet visited and 
                    // inside board, push that state into queue 
                    if (isInside(x, y, N) && !visit[x, y])
                    {
                        visit[x, y] = true;
                        q.Enqueue(new cell(x, y, t.dis + 1));
                    }
                }
            }
            return int.MaxValue;
        }

        // Driver code 
        public static void Main(String[] args)
        {
            int N = 8;
            int[] knightPos = { 1, 1 };
            int[] targetPos = { 1, 2 };
            Console.WriteLine(
                minStepToReachTarget(
                    knightPos,
                    targetPos, N));
        }
    }
}
