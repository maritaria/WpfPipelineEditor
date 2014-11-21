using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkUI
{
	/// <summary>
	///  Defines the event handler for the ConnectionDragCompleted event.
	/// </summary>
	public delegate void ConnectionDragCompletedEventHandler(object sender, ConnectionDragCompletedEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectionDragging event.
	/// </summary>
	public delegate void ConnectionDraggingEventHandler(object sender, ConnectionDraggingEventArgs e);

	/// <summary>
	///  Defines the event handler for the ConnectionDragStarted event.
	/// </summary>
	public delegate void ConnectionDragStartedEventHandler(object sender, ConnectionDragStartedEventArgs e);

	/// <summary>
	///  Defines the event handler for the QueryConnectionFeedback event.
	/// </summary>
	public delegate void QueryConnectionFeedbackEventHandler(object sender, QueryConnectionFeedbackEventArgs e);
	public delegate void QueryConnectionResultEventHandler(object sender, QueryConnectionResultEventArgs e);

	/// <summary>
	///  Arguments for event raised when the user has completed dragging a connector.
	/// </summary>
	public class ConnectionDragCompletedEventArgs : ConnectionDragEventArgs
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

		internal ConnectionDragCompletedEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object connectorDraggedOver)
			: base(routedEvent, source, node, connection, connector)
		{
			ConnectorDraggedOver = connectorDraggedOver;
		}

		#endregion Private Methods
	}

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
	///  Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class ConnectionDraggingEventArgs : ConnectionDragEventArgs
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

		internal ConnectionDraggingEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector)
			: base(routedEvent, source, node, connection, connector)
		{
		}

		#endregion Constructor
	}

	/// <summary>
	///  Arguments for event raised when the user starts to drag a connection out from a node.
	/// </summary>
	public class ConnectionDragStartedEventArgs : ConnectionDragEventArgs
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

		internal ConnectionDragStartedEventArgs(RoutedEvent routedEvent, object source, object node, object connector)
			: base(routedEvent, source, node, null, connector)
		{
		}

		#endregion Constructor
	}

	/// <summary>
	///  Arguments for event raised while user is dragging a node in the network.
	/// </summary>
	public class QueryConnectionFeedbackEventArgs : ConnectionDragEventArgs
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

		internal QueryConnectionFeedbackEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector)
			: base(routedEvent, source, node, connection, connector)
		{
			AcceptConnection = false;
		}

		#endregion Constructor
	}

	public class QueryConnectionResultEventArgs : ConnectionDragEventArgs
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

		internal QueryConnectionResultEventArgs(RoutedEvent routedEvent, object source, object node, object connection, object connector, object closestConnector, bool accepted)
			: base(routedEvent, source, node, connection, connector)
		{
			ClosestConnector = closestConnector;
			ConnectionAccepted = accepted;
		}

		#endregion Constructor
	}
}