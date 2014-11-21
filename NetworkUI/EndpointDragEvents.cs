using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkUI
{
	internal delegate void EndpointDragStartedEventHandler(object sender, EndpointDragStartedEventArgs e);
	internal delegate void EndpointDraggingEventHandler(object sener, EndpointDraggingEventArgs e);
	internal delegate void EndpointDragCompletedEventHandler(object sender, EndpointDragCompletedEventArgs e);
	public delegate void EndpointLinkDragStartedEventHandler(object sender, EndpointLinkDragStartedEventArgs e);
	public delegate void EndpointLinkDraggingEventHandler(object sender, EndpointLinkDraggingEventArgs e);
	public delegate void EndpointLinkDragCompletedEventHandler(object sender, EndpointLinkDragCompletedEventArgs e);
	public delegate void EndpointLinkFeedbackResultEventHandler(object sender, EndpointLinkFeedbackResultEventArgs e);
	public delegate void EndpointLinkFeedbackQueryEventHandler(object sender, EndpointLinkFeedbackQueryEventArgs e);
	public class EndpointLinkDragCompletedEventArgs : LinkEndpointDragEventArgs
	{
		/// <summary>
		///  Gets the connector that the link should be connected to
		/// </summary>
		public object EndConnector { get; protected set; }

		internal EndpointLinkDragCompletedEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide, object endConnector)
			: base(routedEvent, source, link, draggedSide)
		{
			EndConnector = endConnector;
		}
	}

	public abstract class LinkEndpointDragEventArgs : RoutedEventArgs
	{
		public object DraggedSide { get; protected set; }

		public object Link { get; protected set; }

		internal LinkEndpointDragEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide)
			: base(routedEvent, source)
		{
			Link = link;
			DraggedSide = draggedSide;
		}
	}

	public class EndpointLinkDraggingEventArgs : LinkEndpointDragEventArgs
	{
		public double HorizontalChange { get; protected set; }
		public double VerticalChange { get; protected set; }
		internal EndpointLinkDraggingEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide, double horizontal, double vertical)
			: base(routedEvent, source, link, draggedSide)
		{
			HorizontalChange = horizontal;
			VerticalChange = vertical;
		}
	}

	public class EndpointLinkFeedbackQueryEventArgs : LinkEndpointDragEventArgs
	{
		public bool AcceptConnection { get; set; }

		public object ClosestConnector { get; set; }

		internal EndpointLinkFeedbackQueryEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide)
			: base(routedEvent, source, link, draggedSide)
		{
		}
	}

	public class EndpointLinkFeedbackResultEventArgs : LinkEndpointDragEventArgs
	{
		public bool Accepted { get; protected set; }

		public object ClosestConnector { get; protected set; }

		internal EndpointLinkFeedbackResultEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide, object closestConnector, bool accepted)
			: base(routedEvent, source, link, draggedSide)
		{
			ClosestConnector = closestConnector;
			Accepted = accepted;
		}
	}

	public class EndpointLinkDragStartedEventArgs : LinkEndpointDragEventArgs
	{
		/// <summary>
		///  Gets or sets whether the drag operation should be cancelled
		/// </summary>
		public bool Cancel { get; set; }

		internal EndpointLinkDragStartedEventArgs(RoutedEvent routedEvent, object source, object link, object draggedSide)
			: base(routedEvent, source, link, draggedSide)
		{
		}
	}

	internal abstract class EndpointDragEventArgs : RoutedEventArgs
	{
		public EndpointDragEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{

		}
	}
	internal class EndpointDragStartedEventArgs : EndpointDragEventArgs
	{
		public bool Cancel { get; set; }
		public EndpointDragStartedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{

		}
	}
	internal class EndpointDraggingEventArgs : EndpointDragEventArgs
	{
		public double HorizontalChange { get; protected set; }
		public double VerticalChange { get; protected set; }
		public EndpointDraggingEventArgs(RoutedEvent routedEvent, object source, double x, double y)
			: base(routedEvent, source)
		{
			HorizontalChange = x;
			VerticalChange = y;
		}
	}
	internal class EndpointDragCompletedEventArgs : EndpointDragEventArgs
	{
		public double StartX { get; protected set; }
		public double StartY { get; protected set; }
		public double EndX { get; protected set; }
		public double EndY { get; protected set; }
		public EndpointDragCompletedEventArgs(RoutedEvent routedEvent, object source,double startX, double startY, double endX, double endY)
			: base(routedEvent, source)
		{
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
		}
	}
}