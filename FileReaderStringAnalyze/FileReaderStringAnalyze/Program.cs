using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FileReaderStringAnalyze
{
    public static class Program
    {

        public static SortedDictionary<string, int> CountOccurences(string[] subs, SortedDictionary<string, int> dict)
        {
            int val;


            foreach (var word in subs)
            {
                if (dict.TryGetValue(word, out val))
                {
                    dict[word] = val + 1;
                }
                else
                {
                    dict.Add(word, 1);
                }
            }
            return dict;
        }

        public static string DictionaryToString(SortedDictionary<string, int> dictionary)
        {
            string dictionaryString = "";
            foreach (KeyValuePair<string, int> keyValues in dictionary)
            {
                int numberofOccurences = keyValues.Value;
                dictionaryString += keyValues.Key + ": " + numberofOccurences.ToString() + "\n";
            }
            return dictionaryString.TrimEnd('\n', ' ');
        }
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                System.Environment.Exit(0);
            }

            SortedDictionary<string, int> wordCount = new SortedDictionary<string, int>();
            try
            {
                using (var sr = new StreamReader(args[0]))
                {
                    string line;
                    char[] separators = new char[] { ' ', '\n', '\t' };

                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] subs = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        wordCount = CountOccurences(subs, wordCount);
                    }
                    foreach (KeyValuePair<string, int> keyValues in wordCount)
                    {
                        int numberofOccurences = keyValues.Value;
                        Console.WriteLine(keyValues.Key.ToString() + ": " + numberofOccurences.ToString());
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("File Error");
            }
        }
    }
}