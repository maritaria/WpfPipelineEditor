using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utils
{
	public static class VisualTreeFinder
	{
		public static ParentT FindVisualParentWithType<ParentT>(FrameworkElement childElement) where ParentT : class
		{
			FrameworkElement parentElement = (FrameworkElement)VisualTreeHelper.GetParent(childElement);
			if (parentElement != null)
			{
				ParentT parent = parentElement as ParentT;
				if (parent != null)
				{
					return parent;
				}

				return FindVisualParentWithType<ParentT>(parentElement);
			}

			return null;
		}
	}
}