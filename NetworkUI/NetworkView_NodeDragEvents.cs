using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUI
{
	public partial class NetworkView
	{
		#region Properties

		private List<NodeItem> m_DraggedNodesCache = null;

		#endregion Properties

		#region Event Handlers

		private void NodeItem_DragCompleted(object sender, NodeDragCompletedEventArgs e)
		{
			e.Handled = true;

			//Expose event
			var eventArgs = new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, this.SelectedNodes, e.StartX, e.StartY, e.EndX, e.EndY);
			RaiseEvent(eventArgs);

			m_DraggedNodesCache = null;

			//Update bindable properties
			//this.IsDragging = false;
			//this.IsNotDragging = true;
			//this.IsDraggingNode = false;
			//this.IsNotDraggingNode = true;
		}

		private void NodeItem_Dragging(object sender, NodeDraggingEventArgs e)
		{
			e.Handled = true;

			//Cache all selected nodes
			if (m_DraggedNodesCache == null)
			{
				m_DraggedNodesCache = new List<NodeItem>();
				foreach (var selectedNode in SelectedNodes)
				{
					NodeItem nodeItem = FindAssociatedNodeItem(selectedNode);
					if (nodeItem == null)
					{
						throw new ApplicationException("Selected node is not in visual tree");
					}
					m_DraggedNodesCache.Add(nodeItem);
				}
			}

			//Update positions
			foreach (var nodeItem in m_DraggedNodesCache)
			{
				nodeItem.X += e.HorizontalChange;
				nodeItem.Y += e.VerticalChange;
			}

			//Expose event
			var eventArgs = new NodeDraggingEventArgs(NodeDraggingEvent, this, this.SelectedNodes, e.HorizontalChange, e.VerticalChange);
			RaiseEvent(eventArgs);
		}

		private void NodeItem_DragStarted(object sender, NodeDragStartedEventArgs e)
		{
			e.Handled = true;

			//Update bindable properties
			//this.IsDragging = true;
			//this.IsNotDragging = false;
			//this.IsDraggingNode = true;
			//this.IsNotDraggingNode = false;

			//Expose event
			var eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, this.SelectedNodes);
			RaiseEvent(eventArgs);

			e.Cancelled = eventArgs.Cancelled;
		}

		#endregion Event Handlers
	}
}