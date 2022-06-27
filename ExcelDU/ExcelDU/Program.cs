using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelDU
{
    public class Cell
    {
        public int Value { get; set; }
        public int Type { get; set; } // 0 -> num, 2 -> inval, , 3 -> formula, 4 err, 5 div0, 6 cycle, 7 missop, 8 formula err
        public Tuple<char, int, int, int, int> Formula { get; set; }

    }
    class Reader
    {
        public static List<List<Cell>> table;
        public static List<Cell> rowList;

        public static List<List<Cell>> ReadFile(string inFile)
        {
            table = new List<List<Cell>>();
            try
            {
                StreamReader fs = new StreamReader(inFile);
                int c;
                char character;
                bool inCell = false;
                bool inRow = false;
                bool formula = false;
                string cell = "";
                rowList = new List<Cell>();

                while ((c = fs.Read()) > -1)
                {
                    character = (char)c;
                    if (character == ' ' || character == '\t')
                    {
                        if (inCell == true)
                        {
                            ProcessStringToCell(formula, cell);
                        }
                        formula = false;
                        inCell = false;
                        cell = "";
                    }
                    else if (character == '\n' || character == '\r')
                    {
                        if (inCell == true)
                        {
                            ProcessStringToCell(formula, cell);
                        }
                        formula = false;
                        inCell = false;
                        cell = "";

                        if (inRow == true)
                        {
                            table.Add(rowList.ToList());
                            rowList.Clear();
                        }
                        inRow = false;
                    }
                    else if (character == '=')
                    {
                        if (!formula)
                        {
                            formula = true;
                            inCell = true;
                        }
                        else
                        {
                            cell += character.ToString();
                        }
                    }
                    else
                    {
                        cell += character.ToString();
                        inCell = true;
                        inRow = true;
                    }
                }
                if (inCell == true)
                {
                    ProcessStringToCell(formula, cell);
                }
                formula = false;
                inCell = false;
                cell = "";

                if (inRow == true)
                {
                    table.Add(rowList.ToList());
                    rowList.Clear();
                }
                inRow = false;
                return table;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException ||
                    ex is IOException ||
                    ex is UnauthorizedAccessException ||
                    ex is System.Security.SecurityException
                )
                {
                    Console.WriteLine("File Error");
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static void ProcessStringToCell(bool formula, string cell)
        {
            int number;
            if (cell == "[]")
            {
                rowList.Add(new Cell { Value = 0, Type = 1 });
            }
            else if (formula)
            {
                if (cell == "")
                {
                    rowList.Add(new Cell { Type = 8 });
                }
                else
                {
                    var cells = Controler.ReadFormula(table, cell);
                    rowList.Add(cells);
                }

            }
            else if (Int32.TryParse(cell, out number))
            {
                if (number < 0)
                {
                    rowList.Add(new Cell { Type = 2 });
                }
                else
                {
                    rowList.Add(new Cell { Value = number, Type = 0 });
                }
            }
            else
            {
                rowList.Add(new Cell { Type = 2 });
            }
            return;
        }
    }
    class Controler
    {
        public static int NumberFromExcelColumn(string column)
        {
            int retVal = 0;
            string col = column.ToUpper();
            if (column == "")
            {
                return -1;
            }
            for (int x = col.Length - 1; x >= 0; x--)
            {
                char colPiece = col[x];
                if (!char.IsUpper(colPiece))
                {
                    return -1;
                }
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (x + 1));
            }
            return retVal;
        }
        public static Cell ReadFormula(List<List<Cell>> table, string input)
        {
            // 4 => error, 5 => div0, 6=> cycle, 7=> missop, 8=> formula
            char[] separators = new char[] { '+', '-', '*', '/' };
            string[] subs = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string colString = "";
            string rowString = "";
            int col1 = -1;
            int row1 = -1;
            int col2 = -1;
            int row2 = -1;
            int indexOp = subs[0].Length;
            string cells = String.Join("", subs);
            if (subs.Length == 1)
            {
                return new Cell { Type = 7 };
            }
            else if (subs.Length > 2)
            {
                return new Cell { Type = 8 };
            }

            for (int x = 0; x < cells.Length; x++)
            {
                if (char.IsLetter(Convert.ToChar(cells[x])))
                {
                    if(char.IsLower(Convert.ToChar(cells[x])))
                    {
                        break; 
                    }
                    colString += cells[x];
                    if (row1 == -1 & col1 != -1)
                    {
                        if (rowString == "")
                        {
                            break;
                        }
                        row1 = Convert.ToInt32(rowString);
                        rowString = "";
                    }
                }
                else if (char.IsDigit(Convert.ToChar(cells[x])))
                {
                    if (col1 == -1)
                    {
                        if (colString == "")
                        {
                            break;
                        }
                        col1 = NumberFromExcelColumn(colString);
                        colString = "";
                    }
                    else if(col2 == -1 & row1 != -1)
                    {
                        col2 = NumberFromExcelColumn(colString);
                        colString = "";
                    }
                    rowString += cells[x];
                }
                else
                {
                    return new Cell { Type = 8 };
                }
            }
            if (rowString != "")
            {
                row2 = Convert.ToInt32(rowString);
            }

            if (col1 != -1 & col2 != -1 & row1 != -1 & row2 != -1)
            {
                return new Cell { Type = 3, Formula = Tuple.Create(Convert.ToChar(input[indexOp]), row1 - 1, col1 - 1, row2 - 1, col2 - 1) };
            }
            return new Cell { Type = 8 };
        }
        public static Cell Calculation(Cell cell, List<List<Cell>> table)
        {
            if (cell.Type == 3) //formula
            {
                Cell oper1;
                Cell oper2;

                if (cell.Formula.Item2 > table.Count - 1)
                {
                    oper1 = new Cell { Value = 0, Type = 1 };
                }
                else if (cell.Formula.Item3 > table[cell.Formula.Item2].Count - 1)
                {
                    oper1 = new Cell { Value = 0, Type = 1 };
                }
                else
                {
                    oper1 = table[cell.Formula.Item2][cell.Formula.Item3];
                }

                if (cell.Formula.Item4 > table.Count - 1)
                {
                    oper2 = new Cell { Value = 0, Type = 1 };
                }
                else if (cell.Formula.Item5 > table[cell.Formula.Item4].Count - 1)
                {
                    oper2 = new Cell { Value = 0, Type = 1 };
                }
                else
                {
                    oper2 = table[cell.Formula.Item4][cell.Formula.Item5];
                }

                if (cell.Formula.Item1 == '/')
                {
                    if (oper2.Value == 0)
                    {
                        cell.Type = 5;
                        return cell;
                    }
                    if (oper1.Value == 0)
                    {
                        cell.Type = 4;
                        return cell;
                    }
                }
                if(oper1.Type == 1 & oper2.Type == 1)
                {
                    cell.Value = 0;
                    cell.Type = 0;
                    return cell;
                }
                if ((oper1.Type == 0 & oper2.Type == 0) || (oper1.Type == 1 & oper2.Type == 0) || (oper1.Type == 0 & oper2.Type == 1)) //obe dve cisla
                {
                    if (cell.Formula.Item1 == '+')
                    {
                        cell.Value = oper1.Value + oper2.Value;
                        cell.Type = 0;
                    }
                    else if (cell.Formula.Item1 == '*')
                    {
                        cell.Value = oper1.Value * oper2.Value;
                        cell.Type = 0;
                    }
                    else if (cell.Formula.Item1 == '-')
                    {
                        cell.Value = oper1.Value - oper2.Value;
                        cell.Type = 0;
                    }
                    else if (cell.Formula.Item1 == '/')
                    {
                        if(!(oper1.Type == 1 & oper2.Type == 1))
                        {
                            cell.Value = oper1.Value / oper2.Value;
                            cell.Type = 0;
                        }
                        
                    }
                    return cell;
                }
                if (oper1.Type == 3) //referuje na bunku, co neni jeste vypocitana
                {
                    if (oper2.Type == 0 || oper2.Type == 3 || oper2.Type == 1)
                    {
                        cell.Type = 3;
                    }
                    else
                    {
                        cell.Type = 4;
                    }
                    return cell;
                }
                else if (oper2.Type == 3) //referuje na bunku, co neni jeste vypocitana
                {
                    if (oper1.Type == 0 || oper1.Type == 3 || oper1.Type == 1)
                    {
                        cell.Type = 3;
                    }
                    else
                    {
                        cell.Type = 4;
                    }
                    return cell;
                }
                else // neni ani vzorec ani cislo -> chyba 
                {
                    cell.Type = 4;
                }
            }
            return cell;
        }

        public static List<List<Cell>> CalculLater(List<List<Cell>> table)
        {
            List<Tuple<int, int>> cellsWithFormula = new List<Tuple<int, int>>();
            Cell cell;
            for (int i = 0; i < table.Count; i++)
            {
                for (int j = 0; j < table[i].Count; j++)
                {
                    cell = table[i][j];
                  
                    if (cell.Type == 3)
                    {
                        if (cell.Formula.Item2 == i & cell.Formula.Item3 == j)
                        {
                            cell.Type = 6;
                        }
                        else if (cell.Formula.Item4 == i & cell.Formula.Item5 == j)
                        {
                            cell.Type = 6;
                        }
                        else
                        {
                            cell = Calculation(table[i][j], table);
                        }
                        if (cell.Type == 3)
                        {
                            cellsWithFormula.Add(Tuple.Create(i, j));
                        }
                    }
                }
            }
            int beforeNumOfCells = cellsWithFormula.Count;
            while (cellsWithFormula.Count != 0)
            {
                for (int x = 0; x < cellsWithFormula.Count; x++)
                {
                    int row = cellsWithFormula[x].Item1;
                    int col = cellsWithFormula[x].Item2;
                    if (row == table[row][col].Formula.Item2 & col == table[row][col].Formula.Item3)
                    {
                        table[row][col].Type = 6;
                        cellsWithFormula.RemoveAt(x);
                    }
                    else if (row == table[row][col].Formula.Item4 & col == table[row][col].Formula.Item5)
                    {
                        table[row][col].Type = 6;
                        cellsWithFormula.RemoveAt(x);
                    }
                    else
                    {
                        cell = Calculation(table[row][col], table);
                        if (cell.Type != 3)
                        {
                            table[row][col] = cell;
                            cellsWithFormula.RemoveAt(x);
                        }
                    }
                }

                if (cellsWithFormula.Count == beforeNumOfCells)
                {
                    int[] visited = new int[cellsWithFormula.Count];
                    int[] imYourFather = new int[cellsWithFormula.Count];
                    int[] cycleNrs = new int[cellsWithFormula.Count];
                    for (int i = 0; i < cellsWithFormula.Count; i++)
                    {
                        if(visited[i] == 0)
                        {
                            DetectCycle(cellsWithFormula, table, i, -1, visited, imYourFather, cycleNrs);
                        }
                        if (cycleNrs[i] == 0)
                        {
                            table[cellsWithFormula[i].Item1][cellsWithFormula[i].Item2].Type = 4;
                        }
                        else
                        {
                            table[cellsWithFormula[i].Item1][cellsWithFormula[i].Item2].Type = 6;
                        }
                    }


                    break;

                }

                beforeNumOfCells = cellsWithFormula.Count;
            }
            return table;
        }
        public static int cycleNr;
        public static void DetectCycle(List<Tuple<int, int>> cellsWithFormula, List<List<Cell>> table, int index, int pIndex, int[] visited, int[] imYourFather, int[] cycleNrs)
        {
            if (visited[index] == 2)
            {
                return;
            }

            if (visited[index] == 1)
            {
                cycleNr++;
                if (pIndex != -1)
                {
                    int curIndex = pIndex;
                    cycleNrs[curIndex] = cycleNr;
                    while (curIndex != index)
                    {
                        curIndex = imYourFather[curIndex];
                        cycleNrs[curIndex] = cycleNr;
                    }
                }

                return;
            }
            imYourFather[index] = pIndex;
            visited[index] = 1;

            Cell cell = table[cellsWithFormula[index].Item1][cellsWithFormula[index].Item2];
            for (int i = 0; i < cellsWithFormula.Count; i++)
            {
                if ((cellsWithFormula[i].Item1 == cell.Formula.Item2 & cellsWithFormula[i].Item2 == cell.Formula.Item3) || (cellsWithFormula[i].Item1 == cell.Formula.Item4 & cellsWithFormula[i].Item2 == cell.Formula.Item5))
                {
                    if (visited[i] != 2)
                    {
                        DetectCycle(cellsWithFormula, table, i, index, visited, imYourFather, cycleNrs);
                    }
                }
            }
            visited[index] = 2;
        }
    }
    class View
    {
        public static void WriteTable(string outFile, List<List<Cell>> table)
        {
            try
            {
                StreamWriter writer = new StreamWriter(outFile);
                bool first = true;
                bool firstrow = true;

                foreach (List<Cell> row in table)
                {
                    if (!firstrow)
                    {
                        writer.Write('\n');
                    }

                    foreach (Cell cell in row)
                    {
                        if (!first)
                        {
                            writer.Write(' ');
                        }
                        switch (cell.Type)
                        {
                            case 0:
                                writer.Write(cell.Value);
                                break;
                            case 1:
                                writer.Write("[]");
                                break;
                            case 2:
                                writer.Write("#INVVAL");
                                break;
                            case 4:
                                writer.Write("#ERROR");
                                break;
                            case 5:
                                writer.Write("#DIV0");
                                break;
                            case 6:
                                writer.Write("#CYCLE");
                                break;
                            case 7:
                                writer.Write("#MISSOP");
                                break;
                            case 8:
                                writer.Write("#FORMULA");
                                break;
                            default:
                                break;
                        }
                        first = false;
                    }
                    first = true;
                    firstrow = false;
                }
                writer.Close();
            }
            catch (System.Exception ex)
            {
                if (ex is IOException)
                {
                    Console.WriteLine("File Error");
                }
                throw;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Argument Error");
                return;
            }
            //string inFile = "in.txt";
            //string outFile = "out.txt";
            string inFile = args[0];
            string outFile = args[1];
            try
            {
                List<List<Cell>> table = Reader.ReadFile(inFile);
                if (table == null)
                {
                    return;
                }
                table = Controler.CalculLater(table);
                View.WriteTable(outFile, table);
            }
            catch (System.Exception ex)
            {
                if (ex is IOException)
                {
                    Console.WriteLine("File Error");
                }
                throw;
            }
        }
    }
}
