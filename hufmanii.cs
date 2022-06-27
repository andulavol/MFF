using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HuffmanCoding
{
	class Node
	{
		public Node Left;
		public Node Right;
		public long Weight;
		public byte Symbol;		// Required only for leaf nodes and has no meaning in intermediate nodes.

		//
		// Parent correctly assigned during tree generation, however later it's not used anywhere
		// (neither during compression nor decompression). We hold this value only for debugging purposes.
		//
		public Node Parent;

		public Node(long weight) : this(weight, 0) { }

		public Node(long weight, byte symbol) {
			Weight = weight;
			Symbol = symbol;
		}
	}

	struct SymbolCode {
		public ulong code;
		public int codeBits;
	}

	class Program
	{
		readonly static byte[] HuffFileMagic = { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
		const string DefaultExtension = ".huff";

		const int CodeSubwordLength = sizeof(uint) * 8;
		const int CodeWordLength = 256 / CodeSubwordLength;

		const int BufferSubwordLength = 8;
		const int BufferThreshold = 256 / BufferSubwordLength;

		//
		// Buffer of bits written to output. The WriteCode method flushes the buffer when exceeds BufferThreshold bytes.
		// Note the flush is done once at the end of the WriteCode method, so the buffer has to have capacity for at least 
		// BufferThreshold + CodeWordLength * bytes_per_uint bytes.
		//
		static byte[] buffer = new byte[2 * BufferThreshold];
		static int bufferBitIndex;

		static Node CreateTree(byte[] symbols, long[] symbolCounts) {
			Queue<Node> symbolQueue = new Queue<Node>(symbols.Length);
			Queue<Node> innerNodeQueue = new Queue<Node>(symbols.Length);

			for (int i = 0; i < symbols.Length; i++) {
				Node symbolNode = new Node(symbolCounts[i], symbols[i]);
				symbolQueue.Enqueue(symbolNode);
			}

			while (symbolQueue.Count > 0 || innerNodeQueue.Count > 1) {
				Node min1, min2;

				//
				// Following two commented if statements represent the less optimal choice of minimal value nodes.
				// Uncommented lines tend to generate more balanced tree with while retaining all Huffman properties.
				//

				if (innerNodeQueue.Count == 0 || (symbolQueue.Count > 0 && symbolQueue.Peek().Weight <= innerNodeQueue.Peek().Weight)) {
				// if (innerNodeQueue.Count == 0 || (symbolQueue.Count > 0 && symbolQueue.Peek().Weight < innerNodeQueue.Peek().Weight)) {
					min1 = symbolQueue.Dequeue();
				} else {
					min1 = innerNodeQueue.Dequeue();
				}

				if (innerNodeQueue.Count == 0 || (symbolQueue.Count > 0 && symbolQueue.Peek().Weight <= innerNodeQueue.Peek().Weight)) {
				// if (innerNodeQueue.Count == 0 || (symbolQueue.Count > 0 && symbolQueue.Peek().Weight < innerNodeQueue.Peek().Weight)) {
					min2 = symbolQueue.Dequeue();
				} else {
					min2 = innerNodeQueue.Dequeue();
				}

				Node node = new Node(min1.Weight + min2.Weight);
				node.Left = min1;
				node.Right = min2;
				min1.Parent = node;
				min2.Parent = node;
				innerNodeQueue.Enqueue(node);
			}

			return innerNodeQueue.Dequeue();
		}

		static void RecursiveGenerateCode(Node node, int depth, ulong currentCode, SymbolCode[] symbolCodes) {
			if (node.Left == null && node.Right == null) {
				//
				// Leaf node
				//
			    symbolCodes[node.Symbol].code = currentCode;
				symbolCodes[node.Symbol].codeBits = depth;
			}

			if (node.Left != null) {
				//
				// Don't be afraid of / and % here and in WriteCode(), FlushCodeBuffer().
				// While compiler will leave div and mod in the IL, JIT will recognize
				// the CodeSubwordLength/BufferSubwordLength constants as powers of 2
				// and generate shr and and instructions!
				//

				//
				// Using 1u just to be sure to stay unsigned. It won't be a problem here,
				// but we can get into trouble during decompression with right shift (being arithmetic on signed types),
				// so let's stay clear and consistent.
				// Also note C# does not support uint << uint (only uint << int is supported).
				//
				currentCode &= ~(1u << depth);
				RecursiveGenerateCode(node.Left, depth + 1, currentCode, symbolCodes);
			}

			if (node.Right != null) {
				currentCode |= (1u << depth);
				RecursiveGenerateCode(node.Right, depth + 1, currentCode, symbolCodes);
			}
		}

		static void WriteCode(Stream s, ulong code, int codeBits) {
			int sourceBit = 0;

			while (codeBits > 0) {
				buffer[bufferBitIndex / BufferSubwordLength] |= (byte) (((code >> sourceBit) & 1u) << (bufferBitIndex % BufferSubwordLength));

				sourceBit++;
				bufferBitIndex++;
				codeBits--;
			}

		    if (bufferBitIndex / BufferSubwordLength >= BufferThreshold) {
				//
				// If we exceeded the buffer threshold, flush the first half of the buffer.
				// Following code expects the Length of the buffer to be at least 2 * BufferThreshold.
				//
				s.Write(buffer, 0, BufferThreshold);
				Array.Copy(buffer, BufferThreshold, buffer, 0, BufferThreshold);
				Array.Clear(buffer, BufferThreshold, BufferThreshold);
				bufferBitIndex -= BufferSubwordLength * BufferThreshold;
			}
		}

		static void FlushCodeBuffer(Stream s) {
			if (bufferBitIndex > 0) {
				if (bufferBitIndex % BufferSubwordLength == 0) {
					s.Write(buffer, 0, (bufferBitIndex / BufferSubwordLength));
				} else {
					s.Write(buffer, 0, (bufferBitIndex / BufferSubwordLength) + 1);
				}
			}
		}

		static void PrintTree(Node root) {
			Stack<Node> stack = new Stack<Node>();
			stack.Push(root);

			while (stack.Count > 0) {
				Node currentNode = stack.Pop();
				
				if (currentNode.Left == null /* || currentNode.Right == null, which is implied */) {
					Console.Write('*');
					Console.Write(currentNode.Symbol);
					Console.Write(':');
				} else {
					stack.Push(currentNode.Right);
					stack.Push(currentNode.Left);
				}

				Console.Write(currentNode.Weight);

				if (stack.Count > 0) {
					Console.Write(' ');
				}
			}
		}

		static void SaveTree(Stream s, Node root) {
			Stack<Node> stack = new Stack<Node>();
			stack.Push(root);

			while (stack.Count > 0) {
				Node currentNode = stack.Pop();

				ulong code = ((ulong) currentNode.Weight) << 1;	// FIXME Implicitly expects Weight to be less than 2^56.
				if (currentNode.Left == null /* || currentNode.Right == null, which is implied */) {
					code |= ((ulong) currentNode.Symbol) << 56;
					code |= 1;
				} else {
					stack.Push(currentNode.Right);
					stack.Push(currentNode.Left);
				}

				byte[] bytes = BitConverter.GetBytes(code);
				s.Write(bytes, 0, bytes.Length);
			}
		}

		static long[] CountSymbolsInStream(Stream s) {
			long[] counts = new long[256];

			int symbol;
			while ((symbol = s.ReadByte()) != -1) {
				counts[symbol]++;
			}

			return counts;
		}

		static void InsertSort<K, V>(K[] keys, V[] values) where K : IComparable<K> {
			int i, j;

			for (i = 1; i < keys.Length; i++) {
				K key = keys[i];
				V value = values[i];
				j = i - 1;
				while ((j >= 0) && (keys[j].CompareTo(key) > 0)) {
					keys[j + 1] = keys[j];
					values[j + 1] = values[j];
					j--;
				}
				keys[j + 1] = key;
				values[j + 1] = value;
			}
		}
				
		static void Main(string[] args)
		{
			if (args.Length != 1 || args[0] == "") {
				Console.WriteLine("Argument Error");
				return;
			}

			FileStream fin = null;
			FileStream fout = null;

			try {
				
				fin = new FileStream(args[0], FileMode.Open);

				//
				// Create coding tree
				//

				long[] counts = CountSymbolsInStream(fin);

				int nonZeroCounts = 0;
				for (int i = 0; i < 256; i++) {
					if (counts[i] > 0) nonZeroCounts++;
				}

				long[] usedSymbolCounts = new long[nonZeroCounts];
				byte[] usedSymbols = new byte[nonZeroCounts];
				int used = 0;
				for (int i = 0; i < 256; i++) {
					if (counts[i] > 0) {
						usedSymbolCounts[used] = counts[i];
						usedSymbols[used] = (byte) i;
						used++;
					}
				}

				// Warning: Array.Sort is unstable, so it cannot be used in this context.
				// Array.Sort(usedSymbolCounts, usedSymbols);
				InsertSort(usedSymbolCounts, usedSymbols);

				Node root = CreateTree(usedSymbols, usedSymbolCounts);

				// PrintTree(root);

				// BinaryTreeVisualizer.DisplayTree(root);

				//
				// Generate codes for each input symbol
				//

				SymbolCode[] symbolCodes = new SymbolCode[256];

			    ulong currentCode = 0;
				RecursiveGenerateCode(root, 0, currentCode, symbolCodes);

				//
				// Start generating output file
				//

				fout = new FileStream(args[0] + DefaultExtension, FileMode.Create);

				fin.Seek(0, SeekOrigin.Begin);

				//
				// Write header
				//

				fout.Write(HuffFileMagic, 0, HuffFileMagic.Length);

				//
				// Write coding tree
				//

				SaveTree(fout, root);

				// End of tree mark
				byte[] zeros = new byte[8];
				fout.Write(zeros, 0, 8);

				//
				// Encode input file
				//

				int inputByte;
				while ((inputByte = fin.ReadByte()) != -1) {
					WriteCode(fout, symbolCodes[inputByte].code, symbolCodes[inputByte].codeBits);
				}

				FlushCodeBuffer(fout);

			} catch (FileNotFoundException) {
				Console.WriteLine("File Error");
			} catch (IOException) {
				Console.WriteLine("File Error");
			} catch (UnauthorizedAccessException) {
				Console.WriteLine("File Error");
			} catch (System.Security.SecurityException) {
				Console.WriteLine("File Error");
			} catch (ArgumentException) {
				Console.WriteLine("File Error");
			} finally {
				if (fout != null) fout.Close();
				if (fin != null) fin.Close();
			}
		}
	}
}