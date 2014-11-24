using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NetworkUI
{
	public delegate void NodeResizeDeltaEventHandler(object sender, NodeResizeDeltaEventArgs e);
	public delegate void NodeResizeStartedEventHandler(object sender, NodeResizeStartedEventArgs e);
	public delegate void NodeResizeCompletedEventHandler(object sender, NodeResizeCompletedEventArgs e);
	public class NodeResizeCompletedEventArgs : NodeResizeEventArgs
	{
		#region Properties

		public double EndHeight { get; protected set; }

		public double EndWidth { get; protected set; }

		public double StartingHeight { get; protected set; }

		public double StartingWidth { get; protected set; }

		#endregion Properties

		#region Constructor

		public NodeResizeCompletedEventArgs(RoutedEvent routedEvent, object source, object node,Sides draggedSides, double startingWidth, double startingHeight, double endWidth, double endHeight)
			: base(routedEvent, source, node, draggedSides)
		{
			StartingWidth = startingWidth;
			StartingHeight = startingHeight;
			EndWidth = endWidth;
			EndHeight = endHeight;
		}

		#endregion Constructor
	}

	public class NodeResizeDeltaEventArgs : NodeResizeEventArgs
	{
		#region Properties


		public double HorizontalChange { get; private set; }

		public double VerticalChange { get; private set; }

		#endregion Properties

		#region Constructor

		public NodeResizeDeltaEventArgs(RoutedEvent routedEvent, object source, object node, Sides draggedSides, double x, double y)
			: base(routedEvent, source, node, draggedSides)
		{
			HorizontalChange = x;
			VerticalChange = y;
		}

		#endregion Constructor
	}

	public abstract class NodeResizeEventArgs : RoutedEventArgs
	{
		#region Properties

		public object Node { get; protected set; }
		public Sides DraggedSides { get; private set; }

		#endregion Properties

		#region Constructor

		public NodeResizeEventArgs(RoutedEvent routedEvent, object source, object node, Sides draggedSides)
			: base(routedEvent, source)
		{
			Node = node;
			DraggedSides = draggedSides;
		}

		#endregion Constructor
	}

	public class NodeResizeStartedEventArgs : NodeResizeEventArgs
	{
		#region Properties

		public bool Cancel { get; set; }

		#endregion Properties

		#region Constructor

		public NodeResizeStartedEventArgs(RoutedEvent routedEvent, object source, object node, Sides draggedSides)
			: base(routedEvent, source, node, draggedSides)
		{
			Cancel = false;
		}

		#endregion Constructor
	}

	#region Helper enum

	[Flags]
	public enum Sides
	{
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
	}

	#endregion Helper enum
}