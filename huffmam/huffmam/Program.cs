using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace HuffmanTest
{
    public class Node
    {
        public byte Symbol { get; set; }
        public long Weight { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }
        public Node Parent { get; set;  }

        public Node(Node left, Node right, byte c, long val)
        {
            this.Left = left;
            this.Right = right;
            this.Weight = val;
            this.Symbol = c;

        }
        struct SymbolCode
        {
            public ulong code;
            public int codeBits;
        }

        public class HuffmanTree
        {
            public static Node Root;
            public static Node Build(long[] symbolCounts)
            {
                List<Node> nodes = new List<Node>();
                for (int i = 0; i < symbolCounts.Length; i++)
                {
                    if (symbolCounts[i] > 0)
                    {
                        nodes.Add(new Node(null, null, (byte)i, symbolCounts[i]));
                    }
                }
                while (nodes.Count > 1)
                {
                    Node node1 = TakeMinNode(nodes);
                    nodes.Remove(node1);

                    Node node2 = TakeMinNode(nodes);
                    nodes.Remove(node2);

                    Node parent = new Node(node1, node2, 0, node1.Weight + node2.Weight);
                    nodes.Add(parent);

                }
                return nodes[0];
            }

            public static Node TakeMinNode(List<Node> nodes)
            {
                Node nodeMinFreq = nodes[0];
                for (int i = 1; i < nodes.Count; i++)
                {
                    Node x = nodes[i];
                    if (x != null)
                    {
                        if (x.Weight < nodeMinFreq.Weight)
                        {
                            nodeMinFreq = x;
                        }
                        else if (x.Weight == nodeMinFreq.Weight && x.Symbol != 0)
                        {
                            if (x.Symbol < nodeMinFreq.Symbol || nodeMinFreq.Symbol == 0)
                            {
                                nodeMinFreq = x;
                            }
                        }
                    }
                }
                return nodeMinFreq;
            }


        }

        class Program
        {
            const int CodeSubwordLength = sizeof(uint) * 8;
            const int CodeWordLength = 256 / CodeSubwordLength;

            const int BufferSubwordLength = 8;
            const int BufferThreshold = 256 / BufferSubwordLength;

            static byte[] buffer = new byte[2 * BufferThreshold];
            static int bufferBitIndex;
            public static void generateCode(Node node, int depth, ulong currentCode, SymbolCode[] symbolCodes)
            {
                if (node.Left == null && node.Right == null)
                {
                    symbolCodes[node.Symbol].code = currentCode;
                    symbolCodes[node.Symbol].codeBits = depth;
                }

                if (node.Left != null)
                {
                    currentCode &= ~(1u << depth);
                    generateCode(node.Left, depth + 1, currentCode, symbolCodes);
                }

                if (node.Right != null)
                {
                    currentCode |= (1u << depth);
                    generateCode(node.Right, depth + 1, currentCode, symbolCodes);
                }
            }

            static void WriteCode(Stream s, ulong code, int codeBits)
            {
                int sourceBit = 0;

                while (codeBits > 0)
                {
                    buffer[bufferBitIndex / BufferSubwordLength] |= (byte)(((code >> sourceBit) & 1u) << (bufferBitIndex % BufferSubwordLength));

                    sourceBit++;
                    bufferBitIndex++;
                    codeBits--;
                }

                if (bufferBitIndex / BufferSubwordLength >= BufferThreshold)
                {
                    s.Write(buffer, 0, BufferThreshold);
                    Array.Copy(buffer, BufferThreshold, buffer, 0, BufferThreshold);
                    Array.Clear(buffer, BufferThreshold, BufferThreshold);
                    bufferBitIndex -= BufferSubwordLength * BufferThreshold;
                }
            }

            static void FlushCodeBuffer(Stream s)
            {
                if (bufferBitIndex > 0)
                {
                    if (bufferBitIndex % BufferSubwordLength == 0)
                    {
                        s.Write(buffer, 0, (bufferBitIndex / BufferSubwordLength));
                    }
                    else
                    {
                        s.Write(buffer, 0, (bufferBitIndex / BufferSubwordLength) + 1);
                    }
                }
            }

            static void SaveTree(Stream s, Node root)
            {
                Stack<Node> stack = new Stack<Node>();
                stack.Push(root);

                while (stack.Count > 0)
                {
                    Node currentNode = stack.Pop();

                    ulong code = ((ulong)currentNode.Weight) << 1;  
                    if (currentNode.Left == null)
                    {
                        code |= ((ulong)currentNode.Symbol) << 56;
                        code |= 1;
                    }
                    else
                    {
                        stack.Push(currentNode.Right);
                        stack.Push(currentNode.Left);
                    }

                    byte[] bytes = BitConverter.GetBytes(code);
                    s.Write(bytes, 0, bytes.Length);
                }
            }

            static void ReportFileError()
            {
                Console.WriteLine("File Error");
            }

            static long[] ReadBytes(long[] counts, string fileName, FileStream fs)
            {
                int x;
                bool empty = true;
                try
                {
                    while ((x = fs.ReadByte()) > -1)
                    {
                        empty = false;
                        counts[x]++;
                    }
                    if (empty)
                    {
                        return null;
                    }
                    return counts;
                }
                catch (IOException)
                {
                    Console.WriteLine("File Error");
                    return null;
                }
            }

            static void Main(string[] args)
            {
                //!
                if (args.Length != 1 || args[0] == "")
                {
                    Console.WriteLine("Argument Error");
                }
                else
                {
                    long[] counts = new long[256];
                    string fileName = args[0];
                    string fileNameout = fileName + ".huff";
                    byte[] data = { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
                    try
                    {
                        FileStream fs = new FileStream(fileName, FileMode.Open);
                        FileStream fsout = new FileStream(fileNameout, FileMode.Create);
                        var arr = ReadBytes(counts, fileName, fs);

                        if (arr != null)
                        {
                            Node root = HuffmanTree.Build(counts);
                            SymbolCode[] symbolCodes = new SymbolCode[256];
                            generateCode(root, 0, 0, symbolCodes);
                            fs.Seek(0, SeekOrigin.Begin);
                            fsout.Write(data, 0, data.Length);
                            SaveTree(fsout, root);
                            byte[] zeros = new byte[8];
                            fsout.Write(zeros, 0, 8);
                            int inputByte;
                            while ((inputByte = fs.ReadByte()) != -1)
                            {
                                WriteCode(fsout, symbolCodes[inputByte].code, symbolCodes[inputByte].codeBits);
                            }

                            FlushCodeBuffer(fsout);

                        }
                    }
                    catch (FileNotFoundException)
                    {
                        ReportFileError();
                    }
                    catch (IOException)
                    {
                        ReportFileError();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        ReportFileError();
                    }
                    catch (System.Security.SecurityException)
                    {
                        ReportFileError();
                    }
                }
            }
        }
    }
}

