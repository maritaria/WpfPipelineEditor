using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace Utils
{
	public static class RoutedCommandExtensions
	{
		public static void SimpleBind(this RoutedCommand command, Type owner, ExecutedRoutedEventHandler handler)
		{
			CommandBinding binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += handler;
			CommandManager.RegisterClassCommandBinding(owner, binding);
		}
	}
}
