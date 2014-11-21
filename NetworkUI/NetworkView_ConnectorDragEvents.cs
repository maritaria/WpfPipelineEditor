using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NetworkUI
{
	public partial class NetworkView
	{
		#region Properties

		private object m_DraggedConnectionDataContext;
		private object m_DraggedConnectorEndpointDataContext;
		private ConnectorItem m_DraggedConnectorItem;
		private object m_DraggedConnectorItemDataContext;
		private object m_DraggedNodeDataContext;

		#endregion Properties

		#region Event Handlers

		private void ConnectorItem_DragCompleted(object sender, ConnectorDragCompletedEventArgs e)
		{
			e.Handled = true;

			Trace.Assert((ConnectorItem)e.OriginalSource == m_DraggedConnectorItem);

			Point mousePos = Mouse.GetPosition(this);

			//Figure out if the end of the connection was dropped on a connector.
			//ConnectorItem connectorDraggedOver = null;
			//object connectorDataContextDraggedOver = null;
			//DetermineConnectorItemDraggedOver(mousePos, out connectorDraggedOver, out connectorDataContextDraggedOver);

			//Now that connection dragging has completed, don't any feedback adorner.
			//ClearFeedbackAdorner();

			//Raise an event to inform application code that connection dragging is complete.
			//The application code can determine if the connection between the two connectors
			//is valid and if so it is free to make the appropriate connection in the view-model.
			OnConnectionDragCompleted(m_DraggedNodeDataContext, m_DraggedConnectionDataContext, m_DraggedConnectorItemDataContext, m_DraggedConnectorEndpointDataContext);

			//this.IsDragging = false;
			//this.IsNotDragging = true;
			//this.IsDraggingConnection = false;
			//this.IsNotDraggingConnection = true;
			m_DraggedNodeDataContext = null;
			m_DraggedConnectorItem = null;
			m_DraggedConnectorItemDataContext = null;
			m_DraggedConnectionDataContext = null;
		}

		private void ConnectorItem_Dragging(object sender, ConnectorDraggingEventArgs e)
		{
			e.Handled = true;
			System.Diagnostics.Trace.Assert((ConnectorItem)e.OriginalSource == m_DraggedConnectorItem);

			Point mousePos = Mouse.GetPosition(this);

			//Raise an event so that application code can compute intermediate connection points.
			OnConnectionDragging(m_DraggedNodeDataContext, m_DraggedConnectionDataContext, m_DraggedConnectorItemDataContext);

			//Raise an event so that application code can specify if the connector that was dragged over is valid or not.
			var results = OnQueryConnectionFeedback(m_DraggedNodeDataContext, m_DraggedConnectionDataContext, m_DraggedConnectorItemDataContext);

			m_DraggedConnectorEndpointDataContext = results.ClosestConnector;

			OnQueryConnectionResult(m_DraggedNodeDataContext, m_DraggedConnectionDataContext, m_DraggedConnectorItemDataContext, results.ClosestConnector, results.AcceptConnection);
		}

		private void ConnectorItem_DragStarted(object sender, ConnectorDragStartedEventArgs e)
		{
			Focus();
			e.Handled = true;

			//Update bindable properties
			//this.IsDragging = true;
			//this.IsNotDragging = false;
			//this.IsDraggingConnection = true;
			//this.IsNotDraggingConnection = false;

			//Get datacontexts
			m_DraggedConnectorItem = (ConnectorItem)e.OriginalSource;
			var nodeItem = m_DraggedConnectorItem.ParentNodeItem;
			m_DraggedNodeDataContext = nodeItem.DataContext ?? nodeItem;
			m_DraggedConnectorItemDataContext = m_DraggedConnectorItem.DataContext ?? m_DraggedConnectorItem;

			//Raise an event so that application code can create a connection and add it to the view-model.
			//Retrieve the the view-model object for the connection was created by application code.
			m_DraggedConnectionDataContext = OnConnectionDragStarted(m_DraggedNodeDataContext, m_DraggedConnectorItemDataContext);

			if (m_DraggedConnectionDataContext == null)
			{
				//Handler didn't create connection
				e.Cancelled = true;
				return;
			}
		}

		#endregion Event Handlers
	}
}