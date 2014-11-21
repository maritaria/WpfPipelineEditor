using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkUI
{
	public delegate void NodeDragCompletedEventHandler(object sender, NodeDragCompletedEventArgs e);

	public delegate void NodeDraggingEventHandler(object sender, NodeDraggingEventArgs e);

	public delegate void NodeDragStartedEventHandler(object sender, NodeDragStartedEventArgs e);

	public class NodeDragCompletedEventArgs : NodeDragEventArgs
	{
		public double EndX { get; private set; }

		public double EndY { get; private set; }

		public double StartX { get; private set; }

		public double StartY { get; private set; }

		public NodeDragCompletedEventArgs(RoutedEvent routedEvent, object source, ICollection nodes, double startX, double startY, double endX, double endY)
			: base(routedEvent, source, nodes)
		{
			StartX = startX;
			StartY = startY;
			EndX = EndX;
			EndY = EndY;
		}
	}

	public abstract class NodeDragEventArgs : RoutedEventArgs
	{
		/// <summary>
		///  The NodeItem's or their DataContext (when non-NULL).
		/// </summary>
		public ICollection Nodes { get; private set; }

		protected NodeDragEventArgs(RoutedEvent routedEvent, object source, ICollection nodes)
			: base(routedEvent, source)
		{
			Nodes = nodes;
		}
	}

	public class NodeDraggingEventArgs : NodeDragEventArgs
	{
		public double HorizontalChange { get; private set; }

		public double VerticalChange { get; private set; }

		public NodeDraggingEventArgs(RoutedEvent routedEvent, object source, ICollection nodes, double horizontal, double vertical)
			: base(routedEvent, source, nodes)
		{
			HorizontalChange = horizontal;
			VerticalChange = vertical;
		}
	}

	public class NodeDragStartedEventArgs : NodeDragEventArgs
	{
		public bool Cancelled { get; set; }

		public NodeDragStartedEventArgs(RoutedEvent routedEvent, object source, ICollection nodes)
			: base(routedEvent, source, nodes)
		{
			Cancelled = false;
		}
	}
}