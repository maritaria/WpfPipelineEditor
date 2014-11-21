using NetworkUI;
using NetworkViewModel;
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
		public ApplicationViewModel ViewModel
		{
			get
			{
				return DataContext as ApplicationViewModel;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();
		}

		private void NetworkView_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
		{
			Link link = e.Connection as Link;
			link.DisableGhost();
			Connector connector = e.DraggedOutConnector as Connector;
			Connector endconnector = e.ConnectorDraggedOver as Connector;
			ViewModel.Network.ConnectionCompleted(link, connector.Type.Opposite(), endconnector);
		}

		private void NetworkView_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
		{
			Link link = e.Connection as Link;
			Connector connector = e.DraggedOutConnector as Connector;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Network.ConnectionUpdated(link, connector.Type.Opposite(), mousePos);
		}

		private void NetworkView_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
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

		private void NetworkView_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
		{
			var link = e.Connection as Link;
			var start = e.DraggedOutConnector as Connector;
			var draggedSide = start.Type.Opposite();
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			Connector closest = null;
			double distance = 30;//Set to maximum distance (exclusive)
			bool isAccepted = false;
			foreach (Node node in ViewModel.Network.Nodes)
			{
				IEnumerable<Connector> connectors;
				if (draggedSide == ConnectorType.Input)
				{
					connectors = node.InputConnectors;
				}
				else
				{
					connectors = node.OutputConnectors;
				}
				foreach (Connector c in connectors)
				{
					double dist = c.Hotspot.Delta(mousePos).Length();
					bool accepted = c.AllowConnection(start);
					if (isAccepted && !accepted)
					{
						//Don't accept connectors that won't be accepted anyway (if any have already been accepted)
						continue;
					}
					if (dist < distance)
					{
						//If this is is the first unaccepted that is still within range, then set it but don't lower
						//the range so another connector that is further away can still override the connector (only
						//if it would accept the link)
						isAccepted = accepted;
						closest = c;
						if (isAccepted)
						{
							distance = dist;
						}
					}
				}
			}
			e.ClosestConnector = closest;
			if (closest != null)
			{
				e.AcceptConnection = closest.AllowConnection(e.DraggedOutConnector as Connector);
			}
		}

		private void NetworkView_QueryConnectionResult(object sender, QueryConnectionResultEventArgs e)
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
				link.UpdateGhost(dragged.Type.Opposite(), closest,e.ConnectionAccepted);
			}
		}
	}
}