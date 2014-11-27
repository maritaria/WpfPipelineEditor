using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class StringExtensions
	{
		public static string LeadingLettersOnly(this string source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (source.Length == 0)
				return source;

			char[] buffer = new char[source.Length];
			int bufferIndex = 0;

			for (int sourceIndex = 0; sourceIndex < source.Length; sourceIndex++)
			{
				char c = source[sourceIndex];

				if (!char.IsLetter(c))
					break;

				buffer[bufferIndex++] = c;
			}
			return new string(buffer, 0, bufferIndex);
		}
	}
}
