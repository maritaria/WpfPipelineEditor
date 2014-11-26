using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorApplication
{
	/// <summary>
	///  Interaction logic for BackgroundWorkerWindow.xaml
	/// </summary>
	public partial class BackgroundWorkerWindow : Window
	{
		public static readonly DependencyProperty BackgroundWorkerProperty = DependencyProperty.Register(
			"BackgroundWorker",
			typeof(BackgroundWorker),
			typeof(BackgroundWorkerWindow),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(asdf)));

		private const int GWL_STYLE = -16;

		private const int WS_SYSMENU = 0x80000;

		public BackgroundWorker BackgroundWorker
		{
			get { return (BackgroundWorker)GetValue(BackgroundWorkerProperty); }
			set { SetValue(BackgroundWorkerProperty, value); }
		}

		public BackgroundWorkerWindow()
		{
			InitializeComponent();
		}

		private static void asdf(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BackgroundWorkerWindow bww = (d as BackgroundWorkerWindow);
			bww.BackgroundWorkerChanged(e);
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private void BackgroundWorkerChanged(DependencyPropertyChangedEventArgs e)
		{
			BackgroundWorker oldValue = e.OldValue as BackgroundWorker;
			if (oldValue != null)
			{
				oldValue.ProgressChanged -= BackgroundWorkerWindow_ProgressChanged;
				oldValue.RunWorkerCompleted -= BackgroundWorkerWindow_RunWorkerCompleted;
			}
			BackgroundWorker newValue = e.NewValue as BackgroundWorker;
			if (newValue != null)
			{
				newValue.ProgressChanged += BackgroundWorkerWindow_ProgressChanged;
				newValue.RunWorkerCompleted += BackgroundWorkerWindow_RunWorkerCompleted;
			}
		}

		private void BackgroundWorkerWindow_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			PART_ProgessBar.Value = e.ProgressPercentage;
		}

		private void BackgroundWorkerWindow_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Close();
		}

		private void PART_CancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (BackgroundWorker != null)
			{
				BackgroundWorker.CancelAsync();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//Disable close button
			var hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}
	}
}