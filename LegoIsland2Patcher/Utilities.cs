using LegoIsland2Patcher.Base;
using System;
using System.IO;
using System.Linq;

namespace LegoIsland2Patcher
{
	public static class Utilities
	{
		public static int StringToInt(string s)
		{
			if (s.All(char.IsDigit) == true)
			{
				int result = Int32.Parse(s);

				if (result > 0)
				{
					return result;
				}
			}

			return 0;
		}

		public static long HexToLong(string hex)
		{
			hex = hex.Trim();

			long result = 0;
			char[] hexchars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

			int position = 0;
			for (int i = hex.Length - 1; i >= 0; i--)
			{
				int convert = 0;

				for (int place = 0; place < 16; place++)
				{
					if (Char.ToLower(hex[i]) == hexchars[place])
					{
						convert = place;
						place = 16;
					}
				}

				result += convert * (int)Math.Pow(16, position);

				position += 1;
			}

			return result;
		}

		public static byte[] ToBytes(string raw)
		{
			raw = raw.Trim();

			if (raw == "")
			{
				return null;
			}

			if (raw.Length % 2 != 0)
			{
				raw = '0' + raw;
			}

			int numofbytes = raw.Length / 2;
			byte[] result = new byte[numofbytes];

			for (int i = 0; i < raw.Length; i += 2)
			{
				result[i / 2] = (byte)HexToLong(Char.ToString(raw[i]) + Char.ToString(raw[i + 1]));
			}

			return result;
		}

		//Check if a series of bytes exist in a file
		public static bool BytesMatch(string filename, long offset, byte[] bytes)
		{
			bool match = true;
			byte[] virtualFile = File.ReadAllBytes(filename);

			for (int i = 0; i < bytes.Length; i++)
			{
				if (virtualFile[offset + i] != bytes[i])
				{
					match = false;
				}
			}

			return match;
		}

		public static bool BytesMatch(LegoIslandExe legoIsland, long offset, byte[] bytes)
		{
			using var fs = legoIsland.ReadFile();

			for (int i = 0; i < bytes.Length; i++)
			{
				fs.Seek(offset + i, SeekOrigin.Begin);

				if (fs.ReadByte() != bytes[i])
				{
					return false;
				}
			}

			return true;
		}

		public static long ReadLong(this FileStream fileStream)
		{
			// 32-bit long
			byte[] longBytes = new byte[4];
			fileStream.Read(longBytes, 0, 4);

			return BitConverter.ToInt32(longBytes, 0);
		}

		/// <summary>
		/// Compares the bytes of two similar files using the same offset
		/// </summary>
		/// <param name="fileStream"></param>
		/// <param name="cFileStream"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns>true - different byte values</returns>
		public static bool CompareBytes(this FileStream fileStream, FileStream cFileStream, long offset, int length)
		{
			fileStream.Seek(offset, SeekOrigin.Begin);
			cFileStream.Seek(offset, SeekOrigin.Begin);

			for (int i = 0; i < length; i++)
			{
				if (fileStream.ReadByte() != cFileStream.ReadByte())
				{
					return true;
				}
			}

			return false;
		}
	}
}
