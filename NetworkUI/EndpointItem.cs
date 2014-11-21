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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkUI
{
	public class EndpointItem : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty HotspotProperty = DependencyProperty.Register(
			"Hotspot",
			typeof(Point),
			typeof(EndpointItem));

		public static readonly DependencyProperty ParentLinkItemProperty = DependencyProperty.Register(
			"ParentLinkItem",
			typeof(LinkItem),
			typeof(EndpointItem));

		public static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register(
			"ParentNetworkView",
			typeof(NetworkView),
			typeof(EndpointItem));

		public static readonly DependencyProperty SideProperty = DependencyProperty.Register(
			"Side",
			typeof(object),
			typeof(EndpointItem),
			new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public object Side
		{
			get { return GetValue(SideProperty);}
			set { SetValue(SideProperty, value); }
		}

		public Point Hotspot
		{
			get { return (Point)GetValue(HotspotProperty); }
			set { SetValue(HotspotProperty, value); }
		}

		internal LinkItem ParentLinkItem
		{
			get { return (LinkItem)GetValue(ParentLinkItemProperty); }
			set { SetValue(ParentLinkItemProperty, value); }
		}

		internal NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		private static readonly double m_DragThreshold = 2;
		private bool m_IsDragging = false;
		private bool m_IsLeftMouseDown = false;
		private Point m_PreviousMousePos;
		private Point m_DragStartingPos;
		#endregion Properties

		#region Constructor

		static EndpointItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EndpointItem), new FrameworkPropertyMetadata(typeof(EndpointItem)));
		}

		public EndpointItem()
		{
			Focusable = false;
		}

		#endregion Constructor

		#region Methods

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (ParentLinkItem != null)
			{
				ParentLinkItem.BringToFront();
			}

			if (ParentNetworkView != null)
			{
				ParentNetworkView.Focus();
			}

			if (e.ChangedButton == MouseButton.Left)
			{
				if (ParentLinkItem != null)
				{
					//Execute selection logic on parent NodeItem
					ParentLinkItem.LeftMouseDownSelectionLogic();
				}
				m_PreviousMousePos = e.GetPosition(ParentNetworkView);
				m_DragStartingPos = m_PreviousMousePos;
				m_IsLeftMouseDown = true;
				e.Handled = true;
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				if (ParentLinkItem != null)
				{
					//Execute selection logic on parent NodeItem
					ParentLinkItem.RightMouseDownSelectionLogic();
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
					OnEndpointDragging(offset.X, offset.Y);
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
						if (OnEndpointDragStarted())
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
						OnEndpointDragCompleted(
							m_DragStartingPos.X, m_DragStartingPos.Y,
							m_PreviousMousePos.X, m_PreviousMousePos.Y);
						m_IsDragging = false;
					}
					else
					{
						// Execute mouse up selection logic only if there was no drag operation.
						if (ParentLinkItem != null)
						{
							// Delegate to parent node to execute selection logic.
							ParentLinkItem.LeftMouseUpSelectionLogic();
						}
					}
					m_IsLeftMouseDown = false;
					ReleaseMouseCapture();
					e.Handled = true;
				}
			}
		}

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		internal static readonly RoutedEvent EndpointDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"EndpointDragCompleted", RoutingStrategy.Bubble,
			typeof(EndpointDragCompletedEventHandler), typeof(EndpointItem));

		internal static readonly RoutedEvent EndpointDraggingEvent = EventManager.RegisterRoutedEvent(
			"EndpointDragging", RoutingStrategy.Bubble,
			typeof(EndpointDraggingEventHandler), typeof(EndpointItem));

		internal static readonly RoutedEvent EndpointDragStartedEvent = EventManager.RegisterRoutedEvent(
			"EndpointDragStarted", RoutingStrategy.Bubble,
			typeof(EndpointDragStartedEventHandler), typeof(EndpointItem));

		internal event EndpointDragCompletedEventHandler EndpointDragCompleted
		{
			add { AddHandler(EndpointDragCompletedEvent, value); }
			remove { RemoveHandler(EndpointDragCompletedEvent, value); }
		}

		internal event EndpointDraggingEventHandler EndpointDragging
		{
			add { AddHandler(EndpointDraggingEvent, value); }
			remove { RemoveHandler(EndpointDraggingEvent, value); }
		}

		internal event EndpointDragStartedEventHandler EndpointDragStarted
		{
			add { AddHandler(EndpointDragStartedEvent, value); }
			remove { RemoveHandler(EndpointDragStartedEvent, value); }
		}

		private void OnEndpointDragCompleted(double startX, double startY,double endX,double endY)
		{
			RaiseEvent(new EndpointDragCompletedEventArgs(EndpointDragCompletedEvent, this, startX, startY, endX, endY));
		}

		private void OnEndpointDragging(double x, double y)
		{
			RaiseEvent(new EndpointDraggingEventArgs(EndpointDraggingEvent, this, x, y));
		}

		private bool OnEndpointDragStarted()
		{
			var e = new EndpointDragStartedEventArgs(EndpointDragStartedEvent, this);
			RaiseEvent(e);
			return e.Cancel;
		}

		#endregion Events
	}
}