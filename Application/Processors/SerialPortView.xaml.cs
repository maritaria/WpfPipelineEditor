using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EditorApplication.Processors
{
	/// <summary>
	///  Interaction logic for SerialPortView.xaml
	/// </summary>
	public partial class SerialPortView : UserControl
	{
		#region Dependency Properties

		public static readonly DependencyProperty SerialProcessorProperty = DependencyProperty.Register(
			"SerialProcessor",
			typeof(SerialProcessorBase),
			typeof(SerialPortView),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(SerialProcessor_PropertyChanged)));


		public SerialProcessorBase SerialProcessor
		{
			get { return (SerialProcessorBase)GetValue(SerialProcessorProperty); }
			set { SetValue(SerialProcessorProperty, value); }
		}

		#endregion Dependency Properties

		#region Properties

		#endregion Properties

		#region Constructor

		public SerialPortView()
		{
			InitializeComponent();
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Event Handlers
		

		private void PortNameComboBox_DropDownOpened(object sender, EventArgs e)
		{
			PortNameComboBox.ItemsSource = SerialPort.GetPortNames();
		}
		private void PortNameComboBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//Apply portname
		}

		private static void SerialProcessor_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as SerialPortView).DataContext = e.NewValue;
		}

		private void BaudRateTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.T)
			{
				e.Handled = true;
			}
		}
		#endregion Event Handlers

	}
}