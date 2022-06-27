namespace Namespace
{

    using System;

    using System.Collections.Generic;

    using System.Linq;

    class Waterfall
    {

        class cell
        {
            public int MVx, AVx, MVy, AVy, MVz, AVz;
            public int rnd;

            public cell(int MVx = 0, int AVx = 0, int MVy = 0, int AVy = 0, int MVz = 0, int AVz = 0, int round = 0)
            {
                this.MVx = MVx;
                this.AVx = AVx;
                this.MVy = MVy;
                this.AVy = AVy;
                this.MVz = MVz;
                this.AVz = AVz;
                this.rnd = round;
            }

        }

        static List<int> Input()
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
                    .Where(x => x != null)
                    .Select(x => x.Value)
                    .ToList();
            return listofints;

        }
        static bool ContainsList(List<List<int>> list1, List<int> list2)
        {
            foreach (List<int> IntList in list1)
                    if ((IntList[0] == list2[0]) & (IntList[1] == list2[1]) & (IntList[1] == list2[1]))
                        return true;
            return false;
        }

        static Dictionary<int, int> Pourover(int nofpourovers, List<int> listofV, Dictionary<int, int> states)
        {
            Queue<cell> que = new Queue<cell>();
            que.Enqueue(new cell(listofV[0], listofV[3], listofV[1], listofV[4], listofV[2], listofV[5], 0));
            List<List<int>> watchover = new List<List<int>>();
            watchover.Add(new List<int> { listofV[3], listofV[4], listofV[5] });
            cell t;

            while (que.Count != 0)
            {
                t = que.Peek();
                que.Dequeue();

                int numberofPO = t.rnd;
                int MaxV_x = t.MVx;
                int ActV_x = t.AVx;
                int MaxV_y = t.MVy;
                int ActV_y = t.AVy;
                int MaxV_z = t.MVz;
                int ActV_z = t.AVz;

                int[] original = { MaxV_x, ActV_x, MaxV_y, ActV_y, MaxV_z, ActV_z };
                
                List<List<int>> options = new List<List<int>>();
                options.Add(new List<int> { 0, 1, 2, 3, 4, 5 });
                options.Add(new List<int> { 0, 1, 4, 5, 2, 3 });
                options.Add(new List<int> { 2, 3, 0, 1, 4, 5 });
                options.Add(new List<int> { 2, 3, 4, 5, 0, 1 });
                options.Add(new List<int> { 4, 5, 0, 1, 2, 3 });
                options.Add(new List<int> { 4, 5, 2, 3, 0, 1 });

                for (int i = 0; i < 6; i++)
                {
                    List<Int32> anotherlist = original.ToList();
                    if (anotherlist[options[i][2]] > anotherlist[options[i][3]])
                    {
                        if (anotherlist[options[i][0]] >= anotherlist[options[i][1]])
                            while (anotherlist[options[i][2]] > anotherlist[options[i][3]] & anotherlist[options[i][1]] != 0)
                            {
                                anotherlist[options[i][3]] += 1;
                                anotherlist[options[i][1]] = anotherlist[options[i][1]] - 1;    
                            }

                        cell newstate = new cell(anotherlist[0], anotherlist[1], anotherlist[2], anotherlist[3], anotherlist[4], anotherlist[5], numberofPO + 1);
                        List<int> stateforwatching = new List<int>();
                        stateforwatching.Add(anotherlist[1]);
                        stateforwatching.Add(anotherlist[3]);
                        stateforwatching.Add(anotherlist[5]);

                        for (int a = 0; a < 3; a++)
                            if (!(states.ContainsKey(stateforwatching[a])) || (states[stateforwatching[a]] > numberofPO + 1))
                                    states[stateforwatching[a]] = numberofPO + 1;
                        if (ContainsList(watchover,stateforwatching) == false)
                        {
                            que.Enqueue(newstate);
                            watchover.Add(stateforwatching);
                        }
                    }
                }
            }
            return states;
        }
        static void Main()
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
                        .Where(x => x != null)
                        .Select(x => x.Value)
                        .ToList();
            int max = listofints.Sum();
            var states = new Dictionary<int, int>();
            states[listofints[3]] = 0;
            states[listofints[4]] = 0;
            states[listofints[5]] = 0;

            var result = new Dictionary<int, int>();
            result = Pourover(0, listofints, states);
            string stringforprint = "";

            for (int j=0; j < (max + 1); j ++)
                if (result.ContainsKey(j))
                    if (j == max)
                    {
                        stringforprint = stringforprint + j + ':' + result[j];
                    }
                    else
                    {
                        stringforprint = stringforprint + j + ':' + result[j] + ' ';
                    }
        Console.WriteLine(stringforprint);
        }

    }
}
