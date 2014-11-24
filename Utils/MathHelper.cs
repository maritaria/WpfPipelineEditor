using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class MathHelper
	{
		public static byte Clamp(byte value, byte lower, byte upper)
		{
			return Math.Max(lower, Math.Min(value, upper));
		}
		public static int Clamp(int value, int lower, int upper)
		{
			return Math.Max(lower, Math.Min(value, upper));
		}
		public static double Clamp(double value, double lower, double upper)
		{
			return Math.Max(lower, Math.Min(value, upper));
		}
		public static float Clamp(float value, float lower, float upper)
		{
			return Math.Max(lower, Math.Min(value, upper));
		}
		public static IComparable Clamp(IComparable value, IComparable lower, IComparable upper)
		{
			IComparable result = value;
			if (result.CompareTo(lower) < 0)
			{
				result = lower;
			}
			if (result.CompareTo(upper) > 0)
			{
				result = upper;
			}
			return result;
		}
	}
}
