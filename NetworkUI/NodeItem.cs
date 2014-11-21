using System;
using System.Collections;
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
	public class NodeItem : ListBoxItem
	{
		#region DependencyProperties

		internal static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register(
			"ParentNetworkView",
			typeof(NetworkView), 
			typeof(NodeItem),
			new FrameworkPropertyMetadata(ParentNetworkView_PropertyChanged));

		public static readonly DependencyProperty XProperty = DependencyProperty.Register(
			"X",
			typeof(double),
			typeof(NodeItem),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty YProperty = DependencyProperty.Register(
			"Y",
			typeof(double),
			typeof(NodeItem),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
			"ZIndex", 
			typeof(int), 
			typeof(NodeItem),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		public double X
		{
			get { return (double)GetValue(XProperty); }
			set { SetValue(XProperty, value); }
		}

		public double Y
		{
			get { return (double)GetValue(YProperty); }
			set { SetValue(YProperty, value); }
		}

		public int ZIndex
		{
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		private static readonly double m_DragThreshold = 5;
		private bool m_IsControlDown = false;
		private bool m_IsDragging = false;
		private bool m_IsLeftMouseDown = false;
		private Point m_DragStartingPos;
		private Point m_MousePreviousPos;

		#endregion Properties

		#region Constructor

		static NodeItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeItem), new FrameworkPropertyMetadata(typeof(NodeItem)));
		}

		#endregion Constructor

		#region Methods

		public void BringToFront()
		{
			if (ParentNetworkView==null)
			{
				return;
			}
			ParentNetworkView.BringNodeToFront(this);
		}

		public void LeftMouseDownSelectionLogic()
		{
			m_IsControlDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			if (!m_IsControlDown)
			{
				if (this.ParentNetworkView.SelectedNodes.Count == 0)
				{
					//There are no items selected, this item becomes the selection
					this.IsSelected = true;
				}
				else if (this.ParentNetworkView.SelectedNodes.Contains(this) ||
						 this.ParentNetworkView.SelectedNodes.Contains(this.DataContext))
				{
					//Current item is already selected, further handling depends on dragging
				}
				else
				{
					//Item is not selected, clear selection and select this item
					this.ParentNetworkView.SelectedNodes.Clear();
					this.IsSelected = true;
				}
			}
		}

		public void LeftMouseUpSelectionLogic()
		{
			if (m_IsControlDown)
			{
				//Control was held, toggle selection
				this.IsSelected = !this.IsSelected;
			}
			else
			{
				if (this.ParentNetworkView.SelectedNodes.Count == 1 &&
					(this.ParentNetworkView.SelectedNode == this ||
					 this.ParentNetworkView.SelectedNode == this.DataContext))
				{
					//This item is the only selected item
				}
				else
				{
					//Clear selection and select the current item
					this.ParentNetworkView.SelectedNodes.Clear();
					this.IsSelected = true;
				}
			}
			m_IsControlDown = false;
		}

		public void RightMouseDownSelectionLogic()
		{
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (ParentNetworkView == null)
			{
				return;
			}

			//Give focus to NetworkView
			ParentNetworkView.Focus();

			//Bring the node to the front, so it is also on top of other selected nodes (if any)
			BringToFront();

			if (e.ChangedButton == MouseButton.Left)
			{
				//LMB down logic
				m_IsLeftMouseDown = true;
				m_IsDragging = false;
				LeftMouseDownSelectionLogic();
				m_MousePreviousPos = e.GetPosition(this.ParentNetworkView);
				e.Handled = true;
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				//RMB down logic
				RightMouseDownSelectionLogic();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (m_IsDragging)
			{
				//Ensure the node is selected
				this.IsSelected = true;
				//Raise new event to update the drag operation
				Point curMousePoint = e.GetPosition(this.ParentNetworkView);

				//Provide datacontext if available
				object item = DataContext ?? this;
				Vector offset = curMousePoint - m_MousePreviousPos;
				if (offset.X != 0.0 || offset.Y != 0.0)
				{
					m_MousePreviousPos = curMousePoint;
					RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, new object[] { item }, offset.X, offset.Y));
				}
			}
			else if (m_IsLeftMouseDown)// && this.ParentNetworkView.EnableNodeDragging)
			{
				//User is dragging the mouse while holding the button
				Point mousePos = e.GetPosition(this.ParentNetworkView);
				var dragDelta = mousePos - m_MousePreviousPos;
				double dragDistance = Math.Abs(dragDelta.Length);
				if (dragDistance > m_DragThreshold)
				{
					//Mouse has passed the threshold and the movement now counts as a drag operation
					NodeDragStartedEventArgs eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, new NodeItem[] { this });
					RaiseEvent(eventArgs);
					if (eventArgs.Cancelled)
					{
						//Drag has been cancelled
						m_IsLeftMouseDown = false;
						m_IsControlDown = false;
						return;
					}
					m_IsDragging = true;
					m_DragStartingPos = mousePos;
					e.Handled = true;
				}
				CaptureMouse();
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (m_IsLeftMouseDown && e.ChangedButton == MouseButton.Left)
			{
				if (m_IsDragging)
				{
					m_IsDragging = false;
					RaiseEvent(new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, new NodeItem[] { this },
						m_DragStartingPos.X,m_DragStartingPos.Y,
						m_MousePreviousPos.X,m_MousePreviousPos.Y));
				}
				else
				{
					LeftMouseUpSelectionLogic();
				}
				m_IsLeftMouseDown = false;
				e.Handled = true;
				ReleaseMouseCapture();
			}
		}

		#endregion Methods

		#region Event Handlers

		private static void ParentNetworkView_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			(sender as NodeItem).BringToFront();
		}

		#endregion Event Handlers

		#region Events

		public static readonly RoutedEvent NodeDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"NodeDragCompleted", RoutingStrategy.Bubble,
			typeof(NodeDragCompletedEventHandler), typeof(NodeItem));

		public static readonly RoutedEvent NodeDraggingEvent = EventManager.RegisterRoutedEvent(
			"NodeDragging", RoutingStrategy.Bubble,
			typeof(NodeDraggingEventHandler), typeof(NodeItem));

		public static readonly RoutedEvent NodeDragStartedEvent = EventManager.RegisterRoutedEvent(
			"NodeDragStarted", RoutingStrategy.Bubble,
			typeof(NodeDragStartedEventHandler), typeof(NodeItem));

		public event NodeDragCompletedEventHandler NodeDragCompleted
		{
			add { AddHandler(NodeDragCompletedEvent, value); }
			remove { RemoveHandler(NodeDragCompletedEvent, value); }
		}

		public event NodeDraggingEventHandler NodeDragging
		{
			add { AddHandler(NodeDraggingEvent, value); }
			remove { RemoveHandler(NodeDraggingEvent, value); }
		}

		public event NodeDragStartedEventHandler NodeDragStarted
		{
			add { AddHandler(NodeDragStartedEvent, value); }
			remove { RemoveHandler(NodeDragStartedEvent, value); }
		}

		protected virtual void OnNodeDragCompleted(ICollection nodes, double startX, double startY, double endX, double endY)
		{
			RaiseEvent(new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, nodes, startX, startY, endX, endY));
		}

		protected virtual void OnNodeDragged(ICollection nodes, double horizontal, double vertical)
		{
			RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, nodes, horizontal, vertical));
		}

		protected virtual bool OnNodeDragStarted(ICollection nodes)
		{
			NodeDragStartedEventArgs e = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, nodes);
			RaiseEvent(e);
			return e.Cancelled;
		}

		#endregion Events
	}
}