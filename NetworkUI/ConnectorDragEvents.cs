using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkUI
{
	internal delegate void ConnectorDragCompletedEventHandler(object sender, ConnectorDragCompletedEventArgs e);

	internal delegate void ConnectorDraggingEventHandler(object sender, ConnectorDraggingEventArgs e);

	internal delegate void ConnectorDragStartedEventHandler(object sender, ConnectorDragStartedEventArgs e);

	internal class ConnectorDragCompletedEventArgs : ConnectorDragEventArgs
	{
		public double EndX { get; private set; }

		public double EndY { get; private set; }

		public double StartX { get; private set; }

		public double StartY { get; private set; }

		public ConnectorDragCompletedEventArgs(RoutedEvent routedEvent, ConnectorItem source,
			double startX, double startY,
			double endX, double endY)
			: base(routedEvent, source)
		{
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
		}
	}

	internal abstract class ConnectorDragEventArgs : RoutedEventArgs
	{
		public ConnectorDragEventArgs(RoutedEvent routedEvent, ConnectorItem source)
			: base(routedEvent, source)
		{
		}
	}

	internal class ConnectorDraggingEventArgs : ConnectorDragEventArgs
	{
		public double HorizontalChange { get; private set; }

		public double VerticalChange { get; private set; }

		public ConnectorDraggingEventArgs(RoutedEvent routedEvent, ConnectorItem source, double horizontal, double vertical)
			: base(routedEvent, source)
		{
			HorizontalChange = horizontal;
			VerticalChange = vertical;
		}
	}

	internal class ConnectorDragStartedEventArgs : ConnectorDragEventArgs
	{
		public bool Cancelled { get; set; }

		public ConnectorDragStartedEventArgs(RoutedEvent routedEvent, ConnectorItem source)
			: base(routedEvent, source)
		{
			Cancelled = false;
		}
	}
}