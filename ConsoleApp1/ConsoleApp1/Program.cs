using System;
using System.Collections.Generic;
using System.Linq;

namespace cesta_kralem_na_sachovnici
{
    public class Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y; //v Pythone self
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int pocet_prekazok = Convert.ToInt32(Console.ReadLine()); //prvý riadok
            List<int[]> suradnice = new List<int[]>(); //list, v ktorom sa mi nachádzajú polia
            //každé pole dva prvky, x a y-nové súradnice prekážok

            while (suradnice.Count != pocet_prekazok) //v Pythone len(suradnice)
            {
                int[] pole = new int[2]; //2 prvky, x a y-nové súradnice 
                string[] prekazky = Console.ReadLine().Split(); //druhý riadok
                int x_prekazky = Convert.ToInt32(prekazky[0]);
                int y_prekazky = Convert.ToInt32(prekazky[1]);
                pole[0] = x_prekazky;  //x súradnica prekážok
                pole[1] = y_prekazky;  //y súradnica prekážok
                suradnice.Add(pole);
            }
            string[] start = Console.ReadLine().Split(); //tretí riadok
            string[] ciel = Console.ReadLine().Split(); //štvrtý riadok

            int x_start = Convert.ToInt32(start[0]); //zo string na integer, aby som s tým vedela pracovať
            int y_start = Convert.ToInt32(start[1]);
            int x_ciel = Convert.ToInt32(ciel[0]);
            int y_ciel = Convert.ToInt32(ciel[1]);

            if (x_ciel == x_start && y_ciel == y_start) //ak som na želanom mieste :) 
            {
                Console.WriteLine(0);
                return;
            }

            int[,] sachovnica = new int[9, 9]; //2D pole 
            int[] x_tah = { -1, -1, -1, 0, 0, 1, 1, 1 }; //pohyb kráľa
            int[] y_tah = { 0, -1, 1, 1, -1, 1, 0, -1 };
            bool existuje_cesta = true; //pomocná premenná, či existuje cesta

            foreach (int[] i in suradnice) //pre prekážku sa mi dá -1
            {
                sachovnica[i[0], i[1]] = -1;
            } //prejde všetky políčka v šachovnici
            sachovnica[x_start, y_start] = -1; //tam sa už nechcem vraciať

            Queue<Position> fronta = new Queue<Position>();
            fronta.Enqueue(new Position(x_start, y_start)); //appendujem do fronty štart
            List<List<int>> cesty = new List<List<int>>();

            int krok;
            while (sachovnica[x_ciel, y_ciel] == 0) //dokým nenavštívim svoj cieľ
            {
                if (fronta.Count == 0)  //neexistuje žiadna cesta 
                {
                    existuje_cesta = false;
                    break;
                }
                else
                {
                    Position top = fronta.Dequeue(); //fronta nie je prázdna, dávam von z fronty, rozbalím
                    int i1 = top.x; //priradím
                    int i2 = top.y;
                    krok = sachovnica[i1, i2] + 1;
                    krok = (krok == 0) ? 1 : krok; //   krok = 1 if krok == 0 else krok
                    //ak je krok 0, vyhodnoť 1
                    for (int i = 0; i < x_tah.Length; ++i) //moja chôdza, priechod
                    {
                        int j1 = i1 + x_tah[i];
                        int j2 = i2 + y_tah[i];

                        if (j1 >= 1 && j1 <= 8 && j2 >= 1 && j2 <= 8) //aby som nevyšla zo šachovnice,
                                                                      //kontroluje hranicu
                        {
                            if (sachovnica[j1, j2] == 0) //nenaštívené, môžem tam vkročiť
                            {
                                sachovnica[j1, j2] = krok;
                                fronta.Enqueue(new Position(j1, j2));
                                cesty.Add(new List<int> { i1, i2, j1, j2, krok }); //pridávam nový prvok fronty aj do zoznamu suradnic, j1,j2 = nove policko, i1, i2 = odkud
                            }
                        }
                    }
                }
            }



            if (existuje_cesta is false)
            {
                Console.WriteLine(-1);
            }
            else
            {
                int hladanex = x_ciel;
                int hladaney = y_ciel;
                List<int> vyslednacesta = new List<int>();

                

                for (int i = sachovnica[x_ciel, y_ciel]; i >= 1; i--)
                {
                    foreach (List<int> zoznamsuradnic in cesty)
                        if (zoznamsuradnic[4] == i)
                            if (zoznamsuradnic[2] == hladanex & zoznamsuradnic[3] == hladaney)
                            {
                                hladanex = zoznamsuradnic[0];
                                hladaney = zoznamsuradnic[1];
                                vyslednacesta.Add(hladanex);
                                vyslednacesta.Add(hladaney);
                            }
                }
                for (int n = vyslednacesta.Count() - 1; n >= 1; n= n -2)
                    Console.WriteLine(String.Join(' ', vyslednacesta.GetRange(n-1,2)));
                Console.WriteLine(Convert.ToString(x_ciel) + " " + Convert.ToString(y_ciel));


            }

        }
    }
}
