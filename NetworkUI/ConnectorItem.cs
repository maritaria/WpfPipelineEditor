using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkUI
{
	public partial class ConnectorItem : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty HotspotProperty = DependencyProperty.Register(
			"Hotspot",
			typeof(Point),
			typeof(ConnectorItem));

		public static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register(
			"ParentNetworkView",
			typeof(NetworkView),
			typeof(ConnectorItem),
			new FrameworkPropertyMetadata(ParentNetworkView_PropertyChanged));

		public static readonly DependencyProperty ParentNodeItemProperty = DependencyProperty.Register(
			"ParentNodeItem",
			typeof(NodeItem),
			typeof(ConnectorItem));

		public Point Hotspot
		{
			get { return (Point)GetValue(HotspotProperty); }
			set { SetValue(HotspotProperty, value); }
		}

		internal NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		internal NodeItem ParentNodeItem
		{
			get { return (NodeItem)GetValue(ParentNodeItemProperty); }
			set { SetValue(ParentNodeItemProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		private static readonly double m_DragThreshold = 2;
		private Point m_DragStartingPos;
		private bool m_IsDragging = false;
		private bool m_IsLeftMouseDown = false;
		private Point m_PreviousMousePos;

		#endregion Properties

		#region Constructor

		static ConnectorItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorItem), new FrameworkPropertyMetadata(typeof(ConnectorItem)));
		}

		public ConnectorItem()
		{
			Focusable = false;
			this.LayoutUpdated += new EventHandler(ConnectorItem_LayoutUpdated);
		}

		#endregion Constructor

		#region Methods

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (ParentNodeItem != null)
			{
				ParentNodeItem.BringToFront();
			}

			if (ParentNetworkView != null)
			{
				ParentNetworkView.Focus();
			}

			if (e.ChangedButton == MouseButton.Left)
			{
				if (this.ParentNodeItem != null)
				{
					//Execute selection logic on parent NodeItem
					this.ParentNodeItem.LeftMouseDownSelectionLogic();
				}
				m_PreviousMousePos = e.GetPosition(this.ParentNetworkView);
				m_DragStartingPos = m_PreviousMousePos;
				m_IsLeftMouseDown = true;
				e.Handled = true;
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				if (this.ParentNodeItem != null)
				{
					//Execute selection logic on parent NodeItem
					this.ParentNodeItem.RightMouseDownSelectionLogic();
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (m_IsDragging)
			{
				// Raise the event to notify that dragging is in progress.
				Point mousePos = e.GetPosition(this.ParentNetworkView);
				Vector offset = mousePos - m_PreviousMousePos;
				if (offset.X != 0.0 || offset.Y != 0.0)
				{
					m_PreviousMousePos = mousePos;
					OnConnectorDragging(offset.X, offset.Y);
				}

				e.Handled = true;
			}
			else if (m_IsLeftMouseDown)
			{
				if (ParentNetworkView != null)//&&this.ParentNetworkView.EnableConnectionDragging)
				{
					// The user is left-dragging the connector and connection dragging is enabled,
					// but don't initiate the drag operation until the mouse cursor has moved more
					// than the threshold distance.
					Point curMousePoint = e.GetPosition(this.ParentNetworkView);
					var dragDelta = curMousePoint - m_PreviousMousePos;
					double dragDistance = Math.Abs(dragDelta.Length);
					if (dragDistance > m_DragThreshold)
					{
						//Event returns true if the drag operation should be cancelled
						if (OnConnectorDragStarted())
						{
							m_IsLeftMouseDown = false;
							ReleaseMouseCapture();
							return;
						}
						m_IsDragging = true;
						e.Handled = true;
					}
					CaptureMouse();
				}
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Left)
			{
				if (m_IsLeftMouseDown)
				{
					if (m_IsDragging)
					{
						OnConnectorDragCompleted(
							m_DragStartingPos.X, m_DragStartingPos.Y,
							m_PreviousMousePos.X, m_PreviousMousePos.Y);
						m_IsDragging = false;
					}
					else
					{
						// Execute mouse up selection logic only if there was no drag operation.
						if (ParentNodeItem != null)
						{
							// Delegate to parent node to execute selection logic.
							ParentNodeItem.LeftMouseUpSelectionLogic();
						}
					}
					m_IsLeftMouseDown = false;
					ReleaseMouseCapture();
					e.Handled = true;
				}
			}
		}

		private void UpdateHotspot()
		{
			if (ParentNetworkView == null)
			{
				return;
			}

			if (!ParentNetworkView.IsAncestorOf(this))
			{
				// The parent NetworkView is no longer an ancestor of the connector. This happens
				// when the connector (and its parent node) has been removed from the network. Reset
				// the property null so we don't attempt to check again.
				ParentNetworkView = null;
				return;
			}

			// Compute the center point of the connector.
			var centerPoint = new Point(ActualWidth / 2, ActualHeight / 2);

			// Transform the center point so that it is relative to the parent NetworkView. Then
			// assign it to Hotspot. Usually Hotspot will be data-bound to the application
			// view-model using OneWayToSource so that the value of the hotspot is then pushed
			// through to the view-model.
			Hotspot = TransformToAncestor(ParentNetworkView).Transform(centerPoint);
		}

		#endregion Methods

		#region Event Handlers

		private static void ParentNetworkView_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as ConnectorItem).UpdateHotspot();
		}

		private void ConnectorItem_LayoutUpdated(object sender, EventArgs e)
		{
			UpdateHotspot();
		}

		#endregion Event Handlers

		#region Events

		internal static readonly RoutedEvent ConnectorDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"ConnectorDragCompleted", RoutingStrategy.Bubble,
			typeof(ConnectorDragCompletedEventHandler), typeof(ConnectorItem));

		internal static readonly RoutedEvent ConnectorDraggingEvent = EventManager.RegisterRoutedEvent(
			"ConnectorDragging", RoutingStrategy.Bubble,
			typeof(ConnectorDraggingEventHandler), typeof(ConnectorItem));

		internal static readonly RoutedEvent ConnectorDragStartedEvent = EventManager.RegisterRoutedEvent(
			"ConnectorDragStarted", RoutingStrategy.Bubble,
			typeof(ConnectorDragStartedEventHandler), typeof(ConnectorItem));

		internal event ConnectorDragCompletedEventHandler ConnectorDragCompleted
		{
			add { AddHandler(ConnectorDragCompletedEvent, value); }
			remove { RemoveHandler(ConnectorDragCompletedEvent, value); }
		}

		internal event ConnectorDraggingEventHandler ConnectorDragging
		{
			add { AddHandler(ConnectorDraggingEvent, value); }
			remove { RemoveHandler(ConnectorDraggingEvent, value); }
		}

		internal event ConnectorDragStartedEventHandler ConnectorDragStarted
		{
			add { AddHandler(ConnectorDragStartedEvent, value); }
			remove { RemoveHandler(ConnectorDragStartedEvent, value); }
		}

		internal virtual void OnConnectorDragCompleted(double startX, double startY, double endX, double endY)
		{
			RaiseEvent(new ConnectorDragCompletedEventArgs(ConnectorDragCompletedEvent, this, startX, startY, endX, endY));
		}

		internal virtual void OnConnectorDragging(double horizontal, double vertical)
		{
			RaiseEvent(new ConnectorDraggingEventArgs(ConnectorDraggingEvent, this, horizontal, vertical));
		}

		internal virtual bool OnConnectorDragStarted()
		{
			var e = new ConnectorDragStartedEventArgs(ConnectorDragStartedEvent, this);
			RaiseEvent(e);
			return e.Cancelled;
		}

		#endregion Events
	}
}