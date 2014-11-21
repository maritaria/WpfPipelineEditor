using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
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
	public class BoolToVisibilityConverter : ConverterBase, IValueConverter
	{
		public BoolToVisibilityConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : parameter ?? Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (parameter ?? Visibility.Visible) == value;
		}
	}
	public class BoolToColorConverter : ConverterBase, IValueConverter
	{
		public BoolToColorConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
