using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
	class Reader
	{
		public static int ReadInt()
		{
			string x = Console.ReadLine();
			return int.Parse(x);
		}

		public static void ReadPair(out int a, out int b)
		{
			string line = Console.ReadLine();
			string[] pair = line.Split(' ');
			a = int.Parse(pair[0]) - 1;
			b = int.Parse(pair[1]) - 1;
		}
	}


	class Program
	{
		static void Main()
		{
			int o = Reader.ReadInt();
			int[,] chessboard = new int[8, 8];

			int x, y;
			for (int i = 0; i < o; ++i)
			{
				Reader.ReadPair(out x, out y);
				chessboard[x, y] = -1;
			}

			int[] start = new int[2];
			Reader.ReadPair(out start[0], out start[1]);
			List<List<int>> paths = new List<List<int>>();
			int[] end = new int[2];
			Reader.ReadPair(out end[0], out end[1]);

			chessboard[start[0], start[1]] = 1;
			BFSearch(chessboard, start, end, paths);
			
			if (chessboard[end[0], end[1]] == 0)
			{
				Console.WriteLine(-1);
				System.Environment.Exit(0);
			}

			int u = end[0];
			int v = end[1];
			List<List<int>> path = new List<List<int>>();
			path.Add(new List<int> { u+1, v+1 });

			for (int i = chessboard[end[0], end[1]]; i >= 1; i--)
			{
				foreach (List<int> ListofPoints in paths)
					if (ListofPoints[4] == i && (ListofPoints[2] == u & ListofPoints[3] == v))
					{
						u = ListofPoints[0];
						v = ListofPoints[1];
						path.Add(new List<int> { u+1, v+1 });
					}
			}


			for (int n = path.Count() - 1; n >= 0; n--)
				Console.WriteLine(String.Join(' ', path[n]));
		}

		static void BFSearch(int[,] board, int[] start, int[] end, List<List<int>> paths)
		{
			Queue<int[]> q = new Queue<int[]>();
			q.Enqueue(start);

			while (q.Count > 0)
			{
				int[] pos = q.Dequeue();
				int level = board[pos[0], pos[1]] + 1;
				for (int dx = -1; dx <= 1; ++dx)
				{
					int x = pos[0] + dx;
					if (x < 0 || x > 7) continue;
					for (int dy = -1; dy <= 1; ++dy)
					{
						int y = pos[1] + dy;
						if (y < 0 || y > 7) continue;
						if (board[x, y] == 0)
						{
							int[] new_pos = new int[] { x, y };
							board[x, y] = level;
							paths.Add(new List<int> { pos[0], pos[1], x, y, level });
							if (x == end[0] && y == end[1])
								return;
							q.Enqueue(new_pos);
						}
					}
				}
			}
		}
	}
}
