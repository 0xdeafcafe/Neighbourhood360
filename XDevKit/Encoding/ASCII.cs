using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDevKit.Encoding
{
	public class ASCII
	{
		public static byte[] ToBytes(string s)
		{
			byte[] retval = new byte[s.Length];
			for (int ix = 0; ix < s.Length; ++ix)
			{
				char ch = s[ix];
				if (ch <= 0x7f) retval[ix] = (byte)ch;
				else retval[ix] = (byte)'?';
			}
			return retval;
		}

		public static string GetString(byte[] bytes)
		{
			return string.Concat(bytes.Select(b => b <= 0x7f ? (char)b : '?'));
		}
	}
}
