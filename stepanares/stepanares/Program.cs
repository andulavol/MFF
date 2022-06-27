using System;
using System.Collections.Generic;
using System.IO;

namespace ZarovnaniDoBloku
{
    class Writer
    {
        public static StreamWriter writer;

        public static void Write(string str)
        {
            try
            {
                writer.Write(str);
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }
        public static void OpenFile(string outFile)
        {
            try
            {
                writer = new StreamWriter(outFile);
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }
        public static void WriteNewLine()
        {
            try
            {
                if (Reader.highlightSpaces)
                    writer.Write("<-");
                writer.Write('\n');
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }
        public static void CloseFile()
        {
            try
            {
                writer.Close();
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }

        public static void WriteLine(int numOfWords, string outFile, int numOfChars, bool newParagraph)
        {
            int numOfSpaces = Program.maxWidth - numOfChars;
            int[] spaces;
            if (numOfWords > 1)
            {
                spaces = new int[numOfWords - 1];
                if (!newParagraph)
                {
                    int j = 0;
                    for (int i = 0; i < numOfSpaces; i++)
                    {
                        spaces[j]++;
                        j++;
                        j %= numOfWords - 1;
                    }
                }
                else
                {
                    for (int i = 0; i < numOfWords - 1; i++)
                    {
                        spaces[i] = 1;
                    }
                }
            }
            else
            {
                spaces = new int[1];
                spaces[0] = 0;
            }
            for (int i = 0; i < numOfWords; i++)
            {
                string word = Reader.words.Dequeue();
                Write(word);
                if (i != numOfWords - 1)
                    for (int j = 0; j < spaces[i]; j++)
                        if (Reader.highlightSpaces)
                            Write(".");
                        else
                            Write(" ");
                else
                {
                    if (Reader.highlightSpaces)
                        Write("<-");

                    Write("\n");
                }
            }
        }
    }
    class Reader
    {
        public static bool expectingNewLine = false;
        public static StreamReader reader;
        public static Queue<string> words = new Queue<string>();
        public static int currentArg;
        public static int inFilesLeft;
        public static string inFile;
        public static bool highlightSpaces = false;

        public static Tuple<string, int, int> ReadWord(int lastChar)
        {
            int wordLen = 0;
            string word = "";
            int character = '\0';

            while (!reader.EndOfStream)
            {
                character = reader.Read();

                if (character == '\n' || character == ' ' || character == '\t')
                {
                    if (character == '\n' && !expectingNewLine)
                    {
                        expectingNewLine = true;
                    }
                    else if (character == '\n' && expectingNewLine)
                    {
                        Tuple<string, int, int> tuple = Tuple.Create("", 0, character);
                        return tuple;
                    }
                    if (lastChar != ' ' && lastChar != '\t' && lastChar != '\n')
                    {
                        Tuple<string, int, int> tuple = Tuple.Create(word, wordLen, character);
                        return tuple;
                    }
                }
                else
                {
                    expectingNewLine = false;
                    word += Convert.ToChar(character);
                    wordLen += 1;
                    lastChar = Convert.ToChar(character);
                }
            }
            return Tuple.Create(word, wordLen, character);
        }
        static void FindNewInputFile()
        {
            bool keepLooking = true;

            while (keepLooking && inFilesLeft > 0)
            {
                currentArg++;
                inFilesLeft--;
                inFile = Program.arguments[currentArg];
                keepLooking = false;
                try
                {
                    reader = new StreamReader(inFile);
                }
                catch
                {
                    keepLooking = true;
                }
            }
        }
        public static void FormatText(string inFile, string outFile, int maxWidth)
        {
            // Catch file exceptions
            try
            {
                reader = new StreamReader(inFile);
            }
            catch
            {
                FindNewInputFile();
            }

            // Initialize variables
            int wordLen = 0;
            string word = "";
            int numOfChars = 0;
            int lastChar = '\0';
            bool queueIsEmpty = false;
            bool newParagraph = false;

            while (inFilesLeft > 0)
            {
                var tuple = ReadWord(lastChar);
                word = tuple.Item1;
                wordLen = tuple.Item2;
                lastChar = tuple.Item3;
                if (reader.EndOfStream)
                    FindNewInputFile();

                if (wordLen > 0)
                {
                    // Add word to the queue
                    words.Enqueue(word);
                    numOfChars += wordLen;

                }
                if (newParagraph && words.Count > 0)
                {
                    // If we are expecting new paragraph and there are some words left in the queue, we add '\n' to the output file.
                    Writer.WriteNewLine();
                    newParagraph = false;
                }
                // Print a line of words in the queue
                if (wordLen == 0 && (words.Count > 0 || queueIsEmpty))
                {
                    Writer.WriteLine(words.Count, outFile, numOfChars, true);
                    numOfChars = 0;
                    queueIsEmpty = false;
                    newParagraph = true;
                }
                else if (numOfChars + words.Count - 1 == maxWidth)
                {
                    Writer.WriteLine(words.Count, outFile, numOfChars, false);
                    numOfChars = 0;
                    queueIsEmpty = true;
                }
                else if (wordLen >= maxWidth && words.Count == 1)
                {
                    Writer.WriteLine(1, outFile, wordLen, false);
                    queueIsEmpty = true;
                }
                else if (numOfChars + words.Count - 1 >= maxWidth)
                {
                    Writer.WriteLine(words.Count - 1, outFile, numOfChars - wordLen, false);
                    numOfChars = wordLen;
                    queueIsEmpty = false;
                }
            }
            while (words.Count > 0)
            {
                if (numOfChars + words.Count - 1 >= maxWidth)
                {
                    if (words.Count == 1)
                    {
                        Writer.WriteLine(1, outFile, numOfChars - wordLen, false);
                    }
                    else
                    {
                        Writer.WriteLine(words.Count - 1, outFile, numOfChars - wordLen, false);
                        numOfChars = wordLen;
                    }
                }
                else if (wordLen >= maxWidth)
                {
                    Writer.WriteLine(1, outFile, wordLen, false);
                }
                else
                {
                    Writer.WriteLine(words.Count, outFile, numOfChars, true);
                    numOfChars = 0;
                }
            }
        }
        public static Tuple<string, string, int, int, int> ReadArgs(string[] args)
        {
            int argCount = args.Length;
            if (args[0] == "--highlight-spaces")
            {
                argCount--;
                highlightSpaces = true;
            }
            if (argCount >= 3 && int.TryParse(args[args.Length - 1], out _))
            {
                if (Convert.ToInt32(args[args.Length - 1]) <= 0)
                {
                    Console.WriteLine("Argument Error");
                    return null;
                }
                currentArg = args.Length - argCount;
                inFilesLeft = argCount - 2;
                inFile = args[currentArg];
                string outFile = args[args.Length - 2];
                int maxWidth = Convert.ToInt32(args[args.Length - 1]);
                return Tuple.Create(inFile, outFile, maxWidth, currentArg, inFilesLeft);
            }
            else
            {
                Console.WriteLine("Argument Error");
                return null;
            }
        }
    }
    class Program
    {
        public static int maxWidth;
        public static string[] arguments;
        static void Main(string[] args)
        {
            arguments = args;
            var input = Reader.ReadArgs(args);
            if (input == null)
            {
                return;
            }
            string inFile = input.Item1;
            string outFile = input.Item2;
            maxWidth = input.Item3;
            Writer.OpenFile(outFile);
            Reader.FormatText(inFile, outFile, maxWidth);
            Writer.CloseFile();
        }
    }
}