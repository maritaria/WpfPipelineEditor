using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkUI
{
	/// <summary>
	///  Defines the event handler for the ConnectorLinkDragCompleted event.
	/// </summary>
	public delegate void ConnectorLinkDragCompletedEventHandler(object sender, ConnectorLinkDragCompletedEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectorLinkDragging event.
	/// </summary>
	public delegate void ConnectorLinkDraggingEventHandler(object sender, ConnectorLinkDraggingEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectorLinkDragStarted event.
	/// </summary>
	public delegate void ConnectorLinkDragStartedEventHandler(object sender, ConnectorLinkDragStartedEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectorLinkFeedbackQuery event.
	/// </summary>
	public delegate void ConnectorLinkFeedbackQueryEventHandler(object sender, ConnectorLinkFeedbackQueryEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectorLinkFeedbackResult event.
	/// </summary>
	public delegate void ConnectorLinkFeedbackResultEventHandler(object sender, ConnectorLinkFeedbackResultEventArgs e);

	internal delegate void ConnectorDragCompletedEventHandler(object sender, ConnectorDragCompletedEventArgs e);

	internal delegate void ConnectorDraggingEventHandler(object sender, ConnectorDraggingEventArgs e);

	internal delegate void ConnectorDragStartedEventHandler(object sender, ConnectorDragStartedEventArgs e);

	public abstract class ConnectionDragEventArgs : RoutedEventArgs
	{
		#region Properties

		/// <summary>
		///  The connector that will be dragged out.
		/// </summary>
		protected object m_Connection;

		/// <summary>
		///  The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		public object DraggedOutConnector { get; protected set; }

		/// <summary>
		///  The NodeItem or it's DataContext (when non-NULL).
		/// </summary>
		public object Node { get; protected set; }

		#endregion Properties

		#region Constructor

		public ConnectionDragEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector)
			: base(routedEvent, source)
		{
			Node = node;
			DraggedOutConnector = connector;
			m_Connection = connection;
		}

		#endregion Constructor
	}

	/// <summary>
	///  Arguments for event raised when the user has completed dragging a connector.
	/// </summary>
	public class ConnectorLinkDragCompletedEventArgs : ConnectionDragEventArgs
	{
		#region Private Data Members

		#endregion Private Data Members

		/// <summary>
		///  The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return m_Connection;
			}
		}

		/// <summary>
		///  The ConnectorItem or it's DataContext (when non-NULL).
		/// </summary>
		public object ConnectorDraggedOver { get; private set; }

		#region Private Methods

		internal ConnectorLinkDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object connectorDraggedOver)
			: base(routedEvent, source, node, connection, connector)
		{
			ConnectorDraggedOver = connectorDraggedOver;
		}

		#endregion Private Methods
	}

	/// <summary>
	///  Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class ConnectorLinkDraggingEventArgs : ConnectionDragEventArgs
	{
		#region Properties

		/// <summary>
		///  The connection being dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return m_Connection;
			}
		}

		#endregion Properties

		#region Constructor

		internal ConnectorLinkDraggingEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector)
			: base(routedEvent, source, node, connection, connector)
		{
		}

		#endregion Constructor
	}

	/// <summary>
	///  Arguments for event raised when the user starts to drag a connection out from a node.
	/// </summary>
	public class ConnectorLinkDragStartedEventArgs : ConnectionDragEventArgs
	{
		#region Properties

		/// <summary>
		///  The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return m_Connection;
			}
			set
			{
				m_Connection = value;
			}
		}

		#endregion Properties

		#region Constructor

		internal ConnectorLinkDragStartedEventArgs(RoutedEvent routedEvent, object source, object node, object connector)
			: base(routedEvent, source, node, null, connector)
		{
		}

		#endregion Constructor
	}

	/// <summary>
	///  Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class ConnectorLinkFeedbackQueryEventArgs : ConnectionDragEventArgs
	{
		#region Properties

		/// <summary>
		///  Sets whether the ClosestConnector will accept the connection
		/// </summary>
		public bool AcceptConnection { get; set; }

		/// <summary>
		///  Sets the closest connector available for linking
		/// </summary>
		public object ClosestConnector { get; set; }

		/// <summary>
		///  The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return m_Connection;
			}
		}

		#endregion Properties

		#region Constructor

		internal ConnectorLinkFeedbackQueryEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector)
			: base(routedEvent, source, node, connection, connector)
		{
			AcceptConnection = false;
		}

		#endregion Constructor
	}

	public class ConnectorLinkFeedbackResultEventArgs : ConnectionDragEventArgs
	{
		#region Properties

		/// <summary>
		///  Gets the closest connector suitable for linking, null if none are suited
		/// </summary>
		public object ClosestConnector { get; private set; }

		/// <summary>
		///  The connection that will be dragged out.
		/// </summary>
		public object Connection
		{
			get
			{
				return m_Connection;
			}
		}

		/// <summary>
		///  Gets whether the ClosestConnector would accept the connection
		/// </summary>
		public bool ConnectionAccepted { get; private set; }

		#endregion Properties

		#region Constructor

		internal ConnectorLinkFeedbackResultEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object closestConnector, bool accepted)
			: base(routedEvent, source, node, connection, connector)
		{
			ClosestConnector = closestConnector;
			ConnectionAccepted = accepted;
		}

		#endregion Constructor
	}

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