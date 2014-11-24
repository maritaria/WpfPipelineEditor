using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
namespace Utils
{
	public class ConverterBase : System.Windows.Markup.MarkupExtension
	{
		protected ConverterBase()
		{
			//This forces implementations to also have a constructor
		}
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}

	public class DoubleShiftConverter : ConverterBase ,IValueConverter
	{
		public DoubleShiftConverter()
		{

		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value == DependencyProperty.UnsetValue)
			{
				return null;
			}
			if (parameter == null)
			{
				return value;
			}
			double v = (double)value;
			string s = (string)parameter;
			double offset = 0;
			if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out offset))
			{
				return v + offset;
			}
			else
			{
				return v;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value == DependencyProperty.UnsetValue)
			{
				return null;
			}
			if (parameter == null)
			{
				return value;
			}
			double v = (double)value;
			string s = (string)parameter;
			double offset = 0;
			if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out offset))
			{
				return v - offset;
			}
			else
			{
				return v;
			}
		}

		#endregion
	}

	public class PointGroupCenterConverter : ConverterBase, IMultiValueConverter
	{
		public PointGroupCenterConverter()
		{

		}

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			foreach(object o in values)
			{
				if (o == DependencyProperty.UnsetValue)
				{
					return null;
				}
			}
			Point total = new Point(0, 0);
			foreach(Point p in values)
			{
				total.Offset(p.X, p.Y);
			}
			return new Point(total.X / values.Length, total.Y / values.Length);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
	public class PointShiftConverter : ConverterBase, IValueConverter
	{
		public PointShiftConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			Point p = (Point)value;
			if (p==null)
			{
				return null;
			}
			string param = (string)parameter;
			if (param == null)
			{
				return value;
			}
			string[] parts = param.Split(';');

			double x = double.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat);
			double y = double.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat);
			return new Point(p.X + x, p.Y + y);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Point p = (Point)value;
			if (p == null)
			{
				return null;
			}
			string param = (string)parameter;
			if (param == null)
			{
				return value;
			}
			string[] parts = param.Split(';');

			double x = double.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat);
			double y = double.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat);
			return new Point(p.X - x, p.Y - y);
		}
	}

	public class BoolToVisibilityConverter : ConverterBase, IValueConverter
	{
		public BoolToVisibilityConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : parameter ?? Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (parameter ?? Visibility.Visible) == value;
		}
	}
	public class BoolToColorConverter : ConverterBase, IValueConverter
	{
		public BoolToColorConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || parameter.GetType() != typeof(string) || value.GetType() != typeof(bool))
			{
				return null;
			}
			bool val = (bool)value;
			string param = (string)parameter;
			string[] parts = param.Split(';');
			return ColorConverter.ConvertFromString(val ? parts[0] : parts[1]);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
