using NetworkUI;
using NetworkVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Utils;

namespace EditorApplication
{
	/// <summary>
	///  Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Properties

		public ApplicationViewModel ViewModel
		{
			get
			{
				return DataContext as ApplicationViewModel;
			}
		}

		#endregion Properties

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
		}

		#endregion Constructor

		#region Methods

		private Connector FindConnector(Point mousePos, Connector linkEndpoint, Nullable<ConnectorType> wantedType, double maxDistance = 30)
		{
			Connector closest = null;
			bool isAccepted = false;
			foreach (Node node in ViewModel.Network.Nodes)
			{
				IEnumerable<Connector> connectors;
				if (!wantedType.HasValue && linkEndpoint == null)
				{
					List<Connector> both = new List<Connector>(node.InputConnectors);
					both.AddRange(node.OutputConnectors);
					connectors = both;
				}
				else
				{
					if (!wantedType.HasValue)
					{
						wantedType = linkEndpoint.Type.Opposite();
					}
					if (wantedType == ConnectorType.Input)
					{
						connectors = node.InputConnectors;
					}
					else
					{
						connectors = node.OutputConnectors;
					}
				}
				foreach (Connector c in connectors)
				{
					double dist = c.Hotspot.Delta(mousePos).Length();
					bool accepted = true;
					if (linkEndpoint != null)
					{
						accepted = c.AllowConnection(linkEndpoint);
						if (isAccepted && !accepted)
						{
							//Don't accept connectors that won't be accepted anyway (if any have already been accepted)
							continue;
						}
					}
					if (dist < maxDistance)
					{
						//If this is is the first unaccepted that is still within range, then set it but don't lower
						//the range so another connector that is further away can still override the connector (only
						//if it would accept the link)
						isAccepted = accepted;
						closest = c;
						if (isAccepted)
						{
							maxDistance = dist;
						}
					}
				}
			}
			return closest;
		}

		#endregion Methods

		#region Event Handlers

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();
		}

		private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Point mousepos = Mouse.GetPosition(PART_NetworkView);
			Node n = new Node(this.ViewModel.Network)
			{
				Name = "Node " +(ViewModel.Network.Nodes.Count + 1),
				X = mousepos.X,
				Y = mousepos.Y,
			};
		}

		private void DeleteLink_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Network.DeleteSelectedLinks();
		}

		private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Network.DeleteSelectedNodes();
		}

		private void NetworkView_ConnectionDragCompleted(object sender, ConnectorLinkDragCompletedEventArgs e)
		{
			Link link = e.Connection as Link;
			link.DisableGhost();
			Connector connector = e.DraggedOutConnector as Connector;
			Connector endconnector = e.ConnectorDraggedOver as Connector;
			ViewModel.Network.ConnectionCompleted(link, connector.Type.Opposite(), endconnector);
		}

		private void NetworkView_ConnectionDragging(object sender, ConnectorLinkDraggingEventArgs e)
		{
			Link link = e.Connection as Link;
			Connector connector = e.DraggedOutConnector as Connector;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Network.ConnectionUpdated(link, connector.Type.Opposite(), mousePos);
		}

		private void NetworkView_ConnectionDragStarted(object sender, ConnectorLinkDragStartedEventArgs e)
		{
			Node node = e.Node as Node;
			if (node == null)
			{
				return;
			}
			Connector start = e.DraggedOutConnector as Connector;
			if (start == null)
			{
				return;
			}
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			e.Connection = ViewModel.Network.ConnectionStarted(start, mousePos);
		}

		private void NetworkView_ConnectorLinkFeedbackQuery(object sender, ConnectorLinkFeedbackQueryEventArgs e)
		{
			var link = e.Connection as Link;
			var start = e.DraggedOutConnector as Connector;
			var draggedSide = start.Type.Opposite();
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			Connector closest = FindConnector(mousePos, start, draggedSide);
			e.ClosestConnector = closest;
			if (closest != null)
			{
				e.AcceptConnection = closest.AllowConnection(e.DraggedOutConnector as Connector);
			}
		}

		private void NetworkView_ConnectorLinkFeedbackResult(object sender, ConnectorLinkFeedbackResultEventArgs e)
		{
			var link = e.Connection as Link;
			Connector closest = e.ClosestConnector as Connector;
			if (closest == null)
			{
				link.DisableGhost();
			}
			else
			{
				Connector dragged = e.DraggedOutConnector as Connector;
				link.UpdateGhost(dragged.Type.Opposite(), closest, e.ConnectionAccepted);
			}
		}

		private void NetworkView_EndpointLinkDragCompleted(object sender, EndpointLinkDragCompletedEventArgs e)
		{
			Link link = e.Link as Link;
			link.DisableGhost();
			ConnectorType draggedSide = (ConnectorType)e.DraggedSide;
			Connector endConnector = e.EndConnector as Connector;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Network.ConnectionCompleted(link, draggedSide, endConnector);
		}

		private void NetworkView_EndpointLinkDragging(object sender, EndpointLinkDraggingEventArgs e)
		{
			Link link = e.Link as Link;
			ConnectorType type = (ConnectorType)e.DraggedSide;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Network.ConnectionUpdated(link, type, mousePos);
		}

		private void NetworkView_EndpointLinkDragStarted(object sender, EndpointLinkDragStartedEventArgs e)
		{
			Link link = e.Link as Link;
			ConnectorType type = (ConnectorType)e.DraggedSide;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Network.ConnectionUpdated(link, type, mousePos);
		}

		private void NetworkView_EndpointLinkFeedbackQuery(object sender, EndpointLinkFeedbackQueryEventArgs e)
		{
			var link = e.Link as Link;
			var draggedSide = (ConnectorType)e.DraggedSide;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			Connector opposite = null;
			if (draggedSide == ConnectorType.Source)
			{
				opposite = link.DestinationConnector;
			}
			else
			{
				opposite = link.SourceConnector;
			}
			Connector closest = FindConnector(mousePos, null, draggedSide);
			e.ClosestConnector = closest;
			if (closest != null && opposite != null)
			{
				e.AcceptConnection = closest.AllowConnection(opposite);
			}
		}

		private void NetworkView_EndpointLinkFeedbackResult(object sender, EndpointLinkFeedbackResultEventArgs e)
		{
			var link = e.Link as Link;
			Connector closest = e.ClosestConnector as Connector;
			if (closest == null)
			{
				link.DisableGhost();
			}
			else
			{
				ConnectorType draggedSide = (ConnectorType)e.DraggedSide;
				link.UpdateGhost(draggedSide, closest, e.Accepted);
			}
		}

		#endregion Event Handlers
	}
}