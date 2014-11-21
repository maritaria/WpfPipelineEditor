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

		private EndpointItem m_DraggedEndpoint;
		private object m_DraggedEndpointClosestMatch;
		private object m_DraggedEndpointSide;
		private object m_DraggedLinkItemDataContext;

		#endregion Properties

		#region Event Handlers

		private void EndpointItem_DragCompleted(object sender, EndpointDragCompletedEventArgs e)
		{
			e.Handled = true;
			Trace.Assert((EndpointItem)e.OriginalSource == m_DraggedEndpoint);

			Point mousePos = Mouse.GetPosition(this);

			//Raise an event to inform application code that connection dragging is complete.
			//The application code can determine if the connection between the two connectors
			//is valid and if so it is free to make the appropriate connection in the view-model.
			OnEndpointLinkDragCompleted(m_DraggedLinkItemDataContext, m_DraggedEndpointSide, m_DraggedEndpointClosestMatch);

			m_DraggedLinkItemDataContext = null;
			m_DraggedEndpoint = null;
			m_DraggedEndpointSide = null;
			m_DraggedEndpointClosestMatch = null;
		}

		private void EndpointItem_Dragging(object sener, EndpointDraggingEventArgs e)
		{
			e.Handled = true;
			Trace.Assert((EndpointItem)e.OriginalSource == m_DraggedEndpoint);

			Point mousePos = Mouse.GetPosition(this);

			//Raise an event so that application code can compute intermediate connection points.
			OnEndpointLinkDragging(m_DraggedLinkItemDataContext, m_DraggedEndpointSide, e.HorizontalChange, e.VerticalChange);

			//Raise an event so that application code can specify if the connector that was dragged over is valid or not.
			var results = OnEndpointLinkFeedbackQuery(m_DraggedLinkItemDataContext, m_DraggedEndpointSide);

			m_DraggedEndpointClosestMatch = results.ClosestConnector;

			OnEndpointLinkFeedbackResult(m_DraggedLinkItemDataContext, m_DraggedEndpointSide, m_DraggedEndpointClosestMatch, results.AcceptConnection);
		}

		private void EndpointItem_DragStarted(object sender, EndpointDragStartedEventArgs e)
		{
			Focus();
			e.Handled = true;

			//Get datacontexts
			m_DraggedEndpoint = (EndpointItem)e.OriginalSource;
			var linkItem = m_DraggedEndpoint.ParentLinkItem;
			m_DraggedLinkItemDataContext = linkItem.DataContext ?? linkItem;
			m_DraggedEndpointSide = m_DraggedEndpoint.Side ?? m_DraggedEndpoint;

			//Viewmodel will cancel drag operation if neccessary
			e.Cancel = OnEndpointLinkDragStarted(m_DraggedLinkItemDataContext, m_DraggedEndpointSide);
		}

		#endregion Event Handlers

		#region Events

		public static readonly RoutedEvent EndpointLinkDragCompletedEvent = EventManager.RegisterRoutedEvent(
			"EndpointLinkDragCompleted", RoutingStrategy.Bubble,
			typeof(EndpointLinkDragCompletedEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent EndpointLinkFeedbackQueryEvent = EventManager.RegisterRoutedEvent(
			"EndpointLinkFeedbackQuery", RoutingStrategy.Bubble,
			typeof(EndpointLinkFeedbackQueryEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent EndpointLinkFeedbackResultEvent = EventManager.RegisterRoutedEvent(
			"EndpointLinkFeedbackResult", RoutingStrategy.Bubble,
			typeof(EndpointLinkFeedbackResultEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent EndpointLinkDraggingEvent = EventManager.RegisterRoutedEvent(
			"EndpointLinkDragging", RoutingStrategy.Bubble,
			typeof(EndpointLinkDraggingEventHandler), typeof(NetworkView));

		public static readonly RoutedEvent EndpointLinkDragStartedEvent = EventManager.RegisterRoutedEvent(
			"EndpointLinkDragStarted", RoutingStrategy.Bubble,
			typeof(EndpointLinkDragStartedEventHandler), typeof(NetworkView));

		public event EndpointLinkDragCompletedEventHandler EndpointLinkDragCompleted
		{
			add { AddHandler(EndpointLinkDragCompletedEvent, value); }
			remove { RemoveHandler(EndpointLinkDragCompletedEvent, value); }
		}

		public event EndpointLinkFeedbackQueryEventHandler EndpointLinkFeedbackQuery
		{
			add { AddHandler(EndpointLinkFeedbackQueryEvent, value); }
			remove { RemoveHandler(EndpointLinkFeedbackQueryEvent, value); }
		}

		public event EndpointLinkFeedbackResultEventHandler EndpointLinkFeedbackResult
		{
			add { AddHandler(EndpointLinkFeedbackResultEvent, value); }
			remove { RemoveHandler(EndpointLinkFeedbackResultEvent, value); }
		}

		public event EndpointLinkDraggingEventHandler EndpointLinkDragging
		{
			add { AddHandler(EndpointLinkDraggingEvent, value); }
			remove { RemoveHandler(EndpointLinkDraggingEvent, value); }
		}

		public event EndpointLinkDragStartedEventHandler EndpointLinkDragStarted
		{
			add { AddHandler(EndpointLinkDragStartedEvent, value); }
			remove { RemoveHandler(EndpointLinkDragStartedEvent, value); }
		}

		protected virtual void OnEndpointLinkDragCompleted(object link, object draggedSide, object endConnector)
		{
			RaiseEvent(new EndpointLinkDragCompletedEventArgs(EndpointLinkDragCompletedEvent, this, link, draggedSide, endConnector));
		}

		protected virtual EndpointLinkFeedbackQueryEventArgs OnEndpointLinkFeedbackQuery(object link, object draggedSide)
		{
			var e = new EndpointLinkFeedbackQueryEventArgs(EndpointLinkFeedbackQueryEvent, this, link, draggedSide);
			RaiseEvent(e);
			return e;
		}

		protected virtual void OnEndpointLinkFeedbackResult(object link, object draggedSide, object closestConnector, bool accepted)
		{
			RaiseEvent(new EndpointLinkFeedbackResultEventArgs(EndpointLinkFeedbackResultEvent, this, link, draggedSide, closestConnector, accepted));
		}

		protected virtual void OnEndpointLinkDragging(object link, object draggedSide, double x, double y)
		{
			RaiseEvent(new EndpointLinkDraggingEventArgs(EndpointLinkDraggingEvent, this, link, draggedSide, x, y));
		}

		protected virtual bool OnEndpointLinkDragStarted(object link, object draggedSide)
		{
			var e = new EndpointLinkDragStartedEventArgs(EndpointLinkDragStartedEvent, this, link, draggedSide);
			RaiseEvent(e);
			return e.Cancel;
		}

		#endregion Events
	}
}