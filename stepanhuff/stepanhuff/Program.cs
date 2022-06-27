using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Huffman
{
    public class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public ulong Value { get; private set; }
        public byte Char { get; private set; }
        public long Code { get; private set; }

        public Node(Node left, Node right, byte c, ulong val)
        {
            this.Left = left;
            this.Right = right;
            this.Value = val;
            this.Char = c;
            SetCode();
        }
        void SetCode()
        {
            long firstPart;
            if (Left == null && Right == null)
            {
                firstPart = 1;
            }
            else
            {
                firstPart = 0;
            }
            long secondPart = 0b0000000011111111111111111111111111111111111111111111111111111110 & ((long)Value << 1);
            long thirdPart;
            if (firstPart == 1)
            {
                thirdPart = (long)Char << 56;
            }
            else
            {
                thirdPart = 0;
            }
            Code = firstPart | secondPart | thirdPart;

        }
    }
    class HuffmanTree
    {
        public Node Root { get; private set; }
        public Dictionary<byte, List<byte>> bitStreams;

        public HuffmanTree(Dictionary<byte, ulong> bytes)
        {
            this.Root = Create(bytes);
            this.bitStreams = new Dictionary<byte, List<byte>>();
        }
        public void TraverseTree()
        {
            Prefix(Root, new List<byte>(), true);
            Writer.WriteCode(0);
        }
        private void Prefix(Node node, List<byte> bitStream, bool first = false)
        {
            Writer.WriteCode(node.Code);
            if (node.Left == null)
            {
                bitStreams[node.Char] = bitStream;
                return;
            }
            List<byte> left = new List<byte>(bitStream);
            List<byte> right = new List<byte>(bitStream);
            left.Add(0);
            right.Add(1);
            Prefix(node.Left, left);
            Prefix(node.Right, right);
        }

        private static Node Create(Dictionary<byte, ulong> bytes)
        {
            LinkedList<Node> nodes = new LinkedList<Node>();

            foreach (KeyValuePair<byte, ulong> entry in bytes)
            {
                nodes.AddLast(new Node(null, null, entry.Key, entry.Value));
            }

            Node node1;
            Node node2;
            while (nodes.Count > 1)
            {
                node1 = ExtractMin(nodes);
                node2 = ExtractMin(nodes);
                nodes.AddLast(new Node(node1, node2, 0, node1.Value + node2.Value));
            }
            return ExtractMin(nodes);
        }
        private static Node ExtractMin(LinkedList<Node> nodes)
        {

            LinkedListNode<Node> minNode = nodes.First;
            Node min = nodes.First();
            for (LinkedListNode<Node> i = nodes.First.Next; i != null; i = i.Next)
            {
                Node item = i.Value;
                if (item.Value < min.Value)
                {
                    min = item;
                    minNode = i;
                }
                else if (item.Value == min.Value && item.Char != 0)
                {
                    if (item.Char < min.Char || min.Char == 0)
                    {
                        min = item;
                        minNode = i;
                    }
                }
            }
            nodes.Remove(minNode);
            return min;
        }
    }
    class Writer
    {
        public static FileStream fs;

        public static void Header()
        {
            byte[] header = { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
            for (int i = 0; i < 8; i++)
            {
                fs.WriteByte(header[i]);
            }
        }
        public static void WriteByte(List<byte> bitStream1, List<byte> bitStream2)
        {
            byte b = 0;
            for (int i = 7; i >= 0; i--)
            {
                b |= (byte)(bitStream1[i] << i);
            }
            fs.WriteByte(b);
            if (bitStream2 != null)
            {
                b = 0;
                for (int i = 7; i >= 0; i--)
                {
                    b |= (byte)(bitStream2[i] << i);
                }
                fs.WriteByte(b);
            }
        }
        public static void WriteCode(long code)
        {
            byte[] codeBytes = LongToBytearray(code);
            for (int i = 0; i < 8; i++)
            {
                fs.WriteByte(codeBytes[i]);
            }
        }

        public static byte[] LongToBytearray(long num)
        {
            byte[] byteArray = { (byte)num, (byte)(num >> 8), (byte)(num >> 16), (byte)(num >> 24), (byte)(num >> 32), (byte)(num >> 40), (byte)(num >> 48), (byte)(num >> 56) };
            return byteArray;
        }
    }
    class Reader
    {

        public static string ReadArgs(string[] args)
        {
            if (args.Length != 1)
                return null;
            else
            {
                return args[0];
            }
        }
        public static void EncodeFile(string fileName)
        {
            FileStream fs;
            byte b;
            int c;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                List<byte> encodedByte1 = new List<byte>();
                List<byte> encodedByte2 = new List<byte>();
                bool byte1 = true;
                List<byte> bitStream;
                while ((c = fs.ReadByte()) > -1)
                {
                    b = (byte)c;
                    bitStream = Program.tree.bitStreams[b];
                    foreach (var item in bitStream)
                    {
                        if (byte1)
                        {
                            encodedByte1.Add(item);
                        }
                        else
                        {
                            encodedByte2.Add(item);
                        }
                        if (encodedByte1.Count() == 8)
                        {
                            if (encodedByte2.Count() != 8)
                            {
                                byte1 = false;
                            }
                            else
                            {
                                Writer.WriteByte(encodedByte1, encodedByte2);
                                encodedByte1.Clear();
                                encodedByte2.Clear();
                                byte1 = true;
                            }
                        }
                    }
                }
                if (encodedByte1.Count() != 8 && encodedByte1.Count() > 0)
                {
                    for (int i = encodedByte1.Count(); i < 8; i++)
                    {
                        encodedByte1.Add(0);
                    }
                    Writer.WriteByte(encodedByte1, null);
                }
                else if (encodedByte2.Count() > 0)
                {
                    for (int i = encodedByte2.Count(); i < 8; i++)
                    {
                        encodedByte2.Add(0);
                    }
                    Writer.WriteByte(encodedByte1, encodedByte2);
                }
                else if (encodedByte1.Count() == 8)
                {
                    Writer.WriteByte(encodedByte1, null);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                return;
            }
        }

        public static Dictionary<byte, ulong> ProcessFile(string fileName)
        {
            Dictionary<byte, ulong> dict = new Dictionary<byte, ulong>();

            FileStream fs;
            byte b;
            int c;
            bool emptyFile = true;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                while ((c = fs.ReadByte()) > -1)
                {
                    emptyFile = false;
                    b = (byte)c;
                    if (dict.ContainsKey(b))
                        dict[b] += 1;
                    else
                        dict.Add(b, 1);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                return null;
            }
            if (emptyFile)
                return null;

            return dict;
        }

    }
    class Program
    {
        public static HuffmanTree tree;
        static void Main(string[] args)
        {
            string inpFile = "simple.in";
            if (inpFile == null)
            {
                Console.WriteLine("Argument Error");
                return;
            }
            string outFile = "simple.out";
            try
            {
                var dict = Reader.ProcessFile(inpFile);
                Writer.fs = new FileStream(outFile, FileMode.Open);
                if (dict == null)
                {
                    Writer.Header();
                    Writer.WriteCode(0);
                    Writer.fs.Close();
                    return;
                }
                tree = new HuffmanTree(dict);
                Writer.Header();
                tree.TraverseTree();
                Reader.EncodeFile(inpFile);
                Writer.fs.Close();

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
