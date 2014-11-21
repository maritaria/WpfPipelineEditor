using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace NetworkUI
{
	public partial class NetworkView : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty ConnectionItemContainerStyleProperty = DependencyProperty.Register(
			"ConnectionItemContainerStyle",
			typeof(Style),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty ConnectionItemTemplateProperty = DependencyProperty.Register(
			"ConnectionItemTemplate",
			typeof(DataTemplate),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty ConnectionItemTemplateSelectorProperty = DependencyProperty.Register(
			"ConnectionItemTemplateSelector",
			typeof(DataTemplateSelector),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty ConnectionsSourceProperty = DependencyProperty.Register(
			"ConnectionsSource",
			typeof(IEnumerable),
			typeof(NetworkView),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(ConnectionsSource_PropertyChanged)));

		public static readonly DependencyProperty NodeItemContainerStyleProperty = DependencyProperty.Register(
			"NodeItemContainerStyle",
			typeof(Style),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty NodeItemTemplateProperty = DependencyProperty.Register(
			"NodeItemTemplate",
			typeof(DataTemplate),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty NodeItemTemplateSelectorProperty = DependencyProperty.Register(
			"NodeItemTemplateSelector",
			typeof(DataTemplateSelector),
			typeof(NetworkView),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty NodesSourceProperty = DependencyProperty.Register(
			"NodesSource",
			typeof(IEnumerable),
			typeof(NetworkView),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(NodesSource_PropertyChanged)));

		private static readonly DependencyPropertyKey ConnectionsPropertyKey = DependencyProperty.RegisterReadOnly(
			"Connections",
			typeof(ObservableCollection<object>),
			typeof(NetworkView),
			new FrameworkPropertyMetadata(new ObservableCollection<object>()));

		private static readonly DependencyPropertyKey NodesPropertyKey = DependencyProperty.RegisterReadOnly(
			"Nodes",
			typeof(ObservableCollection<object>),
			typeof(NetworkView),
			new FrameworkPropertyMetadata(new ObservableCollection<object>()));

		#region DependencyProperties from Keys

		public static readonly DependencyProperty ConnectionsProperty = ConnectionsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty NodesProperty = NodesPropertyKey.DependencyProperty;

		#endregion DependencyProperties from Keys

		public Style ConnectionItemContainerStyle
		{
			get { return (Style)GetValue(ConnectionItemContainerStyleProperty); }
			set { SetValue(ConnectionItemContainerStyleProperty, value); }
		}

		public DataTemplate ConnectionItemTemplate
		{
			get { return (DataTemplate)GetValue(ConnectionItemTemplateProperty); }
			set { SetValue(ConnectionItemTemplateProperty, value); }
		}

		public DataTemplateSelector ConnectionItemTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(ConnectionItemTemplateSelectorProperty); }
			set { SetValue(ConnectionItemTemplateSelectorProperty, value); }
		}

		public ObservableCollection<object> Connections
		{
			get { return (ObservableCollection<object>)GetValue(ConnectionsProperty); }
			private set { SetValue(ConnectionsProperty, value); }
		}

		public ObservableCollection<object> ConnectionsSource
		{
			get { return (ObservableCollection<object>)GetValue(ConnectionsSourceProperty); }
			set { SetValue(ConnectionsSourceProperty, value); }
		}

		public Style NodeItemContainerStyle
		{
			get { return (Style)GetValue(NodeItemContainerStyleProperty); }
			set { SetValue(NodeItemContainerStyleProperty, value); }
		}

		public DataTemplate NodeItemTemplate
		{
			get { return (DataTemplate)GetValue(NodeItemTemplateProperty); }
			set { SetValue(NodeItemTemplateProperty, value); }
		}

		public DataTemplateSelector NodeItemTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(NodeItemTemplateSelectorProperty); }
			set { SetValue(NodeItemTemplateSelectorProperty, value); }
		}

		public ObservableCollection<object> Nodes
		{
			get { return (ObservableCollection<object>)GetValue(NodesProperty); }
			private set { SetValue(NodesPropertyKey, value); }
		}

		public IEnumerable NodesSource
		{
			get { return (IEnumerable)GetValue(NodesSourceProperty); }
			set { SetValue(NodesSourceProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		private List<object> m_InitialSelection = new List<object>();
		private NodeItemsControl m_NodeItemsControl = null;

		public object SelectedNode
		{
			get
			{
				if (m_NodeItemsControl != null)
				{
					return m_NodeItemsControl.SelectedItem;
				}
				if (m_InitialSelection.Count != 1)
				{
					return null;
				}
				return m_InitialSelection[0];
			}
			set
			{
				if (m_NodeItemsControl != null)
				{
					m_NodeItemsControl.SelectedItem = value;
				}
				else
				{
					m_InitialSelection.Clear();
					m_InitialSelection.Add(value);
				}
			}
		}

		public IList SelectedNodes
		{
			get
			{
				if (m_NodeItemsControl != null)
				{
					return m_NodeItemsControl.SelectedItems;
				}
				else
				{
					return m_InitialSelection;
				}
			}
		}

		#endregion Properties

		#region Constructor

		static NetworkView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NetworkView), new FrameworkPropertyMetadata(typeof(NetworkView)));
		}

		public NetworkView()
		{
			//Add handlers, these get called due to the bubble event stategy
			AddHandler(NodeItem.NodeDragStartedEvent, new NodeDragStartedEventHandler(NodeItem_DragStarted));
			AddHandler(NodeItem.NodeDraggingEvent, new NodeDraggingEventHandler(NodeItem_Dragging));
			AddHandler(NodeItem.NodeDragCompletedEvent, new NodeDragCompletedEventHandler(NodeItem_DragCompleted));
			AddHandler(ConnectorItem.ConnectorDragStartedEvent, new ConnectorDragStartedEventHandler(ConnectorItem_DragStarted));
			AddHandler(ConnectorItem.ConnectorDraggingEvent, new ConnectorDraggingEventHandler(ConnectorItem_Dragging));
			AddHandler(ConnectorItem.ConnectorDragCompletedEvent, new ConnectorDragCompletedEventHandler(ConnectorItem_DragCompleted));
		}

		#endregion Constructor

		#region Methods

		public void BringNodeToFront(NodeItem item)
		{
			if (this.m_NodeItemsControl == null)
			{
				return;
			}
			int nodeIndex = 0;
			NodeItem nodeItem = null;
			while ((nodeItem = (NodeItem)m_NodeItemsControl.ItemContainerGenerator.ContainerFromIndex(nodeIndex++)) != null)
			{
				if (nodeItem.ZIndex > item.ZIndex)
				{
					nodeItem.ZIndex--;
				}
			}
			item.ZIndex = nodeIndex;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			m_NodeItemsControl = GetTemplateChild("PART_Nodes") as NodeItemsControl;
			if (m_NodeItemsControl == null)
			{
				throw new ApplicationException("Failed to find 'PART_Nodes' in the visual tree for 'NetworkView'.");
			}
			m_NodeItemsControl.SelectionChanged += PART_Nodes_SelectionChanged;
		}

		internal NodeItem FindAssociatedNodeItem(object node)
		{
			//Gets the NodeItem control that is or represents the node
			NodeItem nodeItem = node as NodeItem;
			if (nodeItem == null)
			{
				nodeItem = m_NodeItemsControl.FindAssociatedNodeItem(node);
			}
			return nodeItem;
		}

		#endregion Methods

		#region Event Handlers

		private static void ConnectionsSource_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			NetworkView networkView = sender as NetworkView;
			if (networkView == null)
			{
				return;
			}
			networkView.Connections.Clear();
			if (e.OldValue != null)
			{
				var inotify = e.OldValue as INotifyCollectionChanged;
				if (inotify != null)
				{
					inotify.CollectionChanged -= networkView.ConnectionsSource_CollectionChanged;
				}
			}
			if (e.NewValue != null)
			{
				var ienumerable = e.NewValue as IEnumerable;
				if (ienumerable != null)
				{
					foreach (object o in ienumerable)
					{
						networkView.Connections.Add(o);
					}
				}
				var inotify = e.NewValue as INotifyCollectionChanged;
				if (inotify != null)
				{
					inotify.CollectionChanged += networkView.ConnectionsSource_CollectionChanged;
				}
			}
		}

		private static void NodesSource_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			NetworkView networkView = sender as NetworkView;
			if (networkView == null)
			{
				return;
			}
			networkView.Nodes.Clear();
			if (e.OldValue != null)
			{
				var inotify = e.OldValue as INotifyCollectionChanged;
				if (inotify != null)
				{
					inotify.CollectionChanged -= networkView.NodesSource_CollectionChanged;
				}
			}
			if (e.NewValue != null)
			{
				var ienumerable = e.NewValue as IEnumerable;
				if (ienumerable != null)
				{
					foreach (object o in ienumerable)
					{
						networkView.Nodes.Add(o);
					}
				}
				var inotify = e.NewValue as INotifyCollectionChanged;
				if (inotify != null)
				{
					inotify.CollectionChanged += networkView.NodesSource_CollectionChanged;
				}
			}
		}

		private void ConnectionsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Connections.Clear();
			}
			else
			{
				if (e.OldItems != null)
				{
					foreach (object o in e.OldItems)
					{
						Connections.Remove(o);
					}
				}
				if (e.NewItems != null)
				{
					foreach (object o in e.NewItems)
					{
						Connections.Add(o);
					}
				}
			}
		}

		private void NodesSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Nodes.Clear();
			}
			else
			{
				if (e.OldItems != null)
				{
					foreach (object o in e.OldItems)
					{
						Nodes.Remove(o);
					}
				}
				if (e.NewItems != null)
				{
					foreach (object o in e.NewItems)
					{
						Nodes.Add(o);
					}
				}
			}
		}

		private void PART_Nodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnSelectionChanged(new SelectionChangedEventArgs(ListBox.SelectionChangedEvent, e.RemovedItems, e.AddedItems));
		}

		#endregion Event Handlers

		#region Events

		public static readonly RoutedEvent ConnectionDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"ConnectionDragCompleted", RoutingStrategy.Bubble,
			typeof(ConnectionDragCompletedEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent ConnectionDraggingEvent = EventManager.RegisterRoutedEvent(
			"ConnectionDragging", RoutingStrategy.Bubble,
			typeof(ConnectionDraggingEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent ConnectionDragStartedEvent = EventManager.RegisterRoutedEvent(
			"ConnectionDragStarted", RoutingStrategy.Bubble,
			typeof(ConnectionDragStartedEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent NodeDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"NodeDragCompleted", RoutingStrategy.Bubble,
			typeof(NodeDragCompletedEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent NodeDraggingEvent = EventManager.RegisterRoutedEvent(
			"NodeDragging", RoutingStrategy.Bubble,
			typeof(NodeDraggingEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent NodeDragStartedEvent = EventManager.RegisterRoutedEvent(
			"NodeDragStarted", RoutingStrategy.Bubble,
			typeof(NodeDragStartedEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent QueryConnectionFeedbackEvent = EventManager.RegisterRoutedEvent(
			"QueryConnectionFeedback", RoutingStrategy.Bubble,
			typeof(QueryConnectionFeedbackEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent QueryConnectionResultEvent = EventManager.RegisterRoutedEvent(
			"QueryConnectionResult", RoutingStrategy.Bubble,
			typeof(QueryConnectionResultEventHandler), typeof(NetworkView));

		public event ConnectionDragCompletedEventHandler ConnectionDragCompleted
		{
			add { AddHandler(ConnectionDragCompletedEvent, value); }
			remove { RemoveHandler(ConnectionDragCompletedEvent, value); }
		}

		public event ConnectionDraggingEventHandler ConnectionDragging
		{
			add { AddHandler(ConnectionDraggingEvent, value); }
			remove { RemoveHandler(ConnectionDraggingEvent, value); }
		}

		public event ConnectionDragStartedEventHandler ConnectionDragStarted
		{
			add { AddHandler(ConnectionDragStartedEvent, value); }
			remove { RemoveHandler(ConnectionDragStartedEvent, value); }
		}

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

		public event QueryConnectionFeedbackEventHandler QueryConnectionFeedback
		{
			add { AddHandler(QueryConnectionFeedbackEvent, value); }
			remove { RemoveHandler(QueryConnectionFeedbackEvent, value); }
		}

		public event QueryConnectionResultEventHandler QueryConnectionResult
		{
			add { AddHandler(QueryConnectionResultEvent, value); }
			remove { RemoveHandler(QueryConnectionResultEvent, value); }
		}

		public event SelectionChangedEventHandler SelectionChanged;

		protected virtual void OnConnectionDragCompleted(object node, object connection, object connector, object endConnector)
		{
			RaiseEvent(new ConnectionDragCompletedEventArgs(ConnectionDragCompletedEvent, this, node, connection, connector, endConnector));
		}

		protected virtual void OnConnectionDragging(object node, object connection, object connector)
		{
			RaiseEvent(new ConnectionDraggingEventArgs(ConnectionDraggingEvent, this, node, connection, connector));
		}

		protected virtual object OnConnectionDragStarted(object node, object connector)
		{
			var e = new ConnectionDragStartedEventArgs(ConnectionDragStartedEvent, this, node, connector);
			RaiseEvent(e);
			return e.Connection;
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

		protected virtual QueryConnectionFeedbackEventArgs OnQueryConnectionFeedback(object node, object connection, object connector)
		{
			var e = new QueryConnectionFeedbackEventArgs(QueryConnectionFeedbackEvent, this, node, connection, connector);
			RaiseEvent(e);
			return e;
		}

		protected virtual void OnQueryConnectionResult(object node, object connection, object connector, object closestConnector, bool accepted)
		{
			RaiseEvent(new QueryConnectionResultEventArgs(QueryConnectionResultEvent, this, node, connection, connector, closestConnector, accepted));
		}

		protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, e);
			}
		}

		#endregion Events
	}
}