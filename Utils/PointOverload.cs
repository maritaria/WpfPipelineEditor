using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Utils
{
	public static class PointOverload
	{
		public static double Length(this Point point)
		{
			return Math.Sqrt((point.X * point.X) + (point.Y * point.Y));
		}
		public static Point Delta(this Point end, Point start)
		{
			return new Point(end.X - start.X, end.Y - start.Y);
		}
		public static Point Normalize(this Point vector)
		{
			double length = vector.Length();
			return new Point(vector.X / length, vector.Y / length);
		}
	}
}
