using EditorApplication.Processors;
using NetworkUI;
using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utils;
using System.Windows.Controls.Ribbon;

namespace EditorApplication
{
	/// <summary>
	///  Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Properties

		public MainViewModel ViewModel
		{
			get
			{
				return DataContext as MainViewModel;
			}
		}

		private Point m_MouseDragStart;
		private Point m_ViewOffsetStart;
		private double m_DraggedDistance = 0;
		private const double m_DragThreshold = 5;
		private bool m_IsLeftMouseDown = false;
		private bool m_IsDragging = false;
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
			foreach (Node node in ViewModel.Pipeline.Nodes)
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
			ViewModel.Test();
			//System.Diagnostics.Debugger.Break();
		}

		private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Point pos;
			TypeInfo pType = null;
			if (e.Parameter is MenuItem)
			{
				pos = ContextMenu.TranslatePoint(new Point(0, 0), PART_NetworkView);
				pType = (e.Parameter as MenuItem).DataContext as TypeInfo;
			}
			else
			{
				pType = e.Parameter as TypeInfo;
				pos = Mouse.GetPosition(PART_NetworkView);
			}
			ViewModel.CreateProcessor(pType, pos);
		}

		private void SaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.SaveFile();
		}
		private void SaveFileAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.SaveFileAs();
		}
		private void LoadFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//string s = ViewModel.AskFilePath(true);
			//if (s != null)
			{
			//	ViewModel.LoadFile(s);
			}
		}
		private void DeleteLink_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.DeleteSelectedLinks();
		}

		private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.DeleteSelectedNodes();
		}

		private void DeleteSelection_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.DeleteSelectedLinks();
			ViewModel.Pipeline.DeleteSelectedNodes();
		}

		private void Processor_ForceProcess_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			FrameworkElement source = e.OriginalSource as FrameworkElement;
			if (source == null)
			{
				return;
			}
			Processor p = source.DataContext as HumanTrigger;
			if (p == null)
			{
				return;
			}
			p.Process();
		}

		private void NetworkView_ConnectionDragCompleted(object sender, ConnectorLinkDragCompletedEventArgs e)
		{
			Link link = e.Connection as Link;
			link.DisableGhost();
			Connector connector = e.DraggedOutConnector as Connector;
			Connector endconnector = e.ConnectorDraggedOver as Connector;
			ViewModel.Pipeline.ConnectionCompleted(link, connector.Type.Opposite(), endconnector);
		}

		private void NetworkView_ConnectionDragging(object sender, ConnectorLinkDraggingEventArgs e)
		{
			Link link = e.Connection as Link;
			Connector connector = e.DraggedOutConnector as Connector;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Pipeline.ConnectionUpdated(link, connector.Type.Opposite(), mousePos);
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
			e.Connection = ViewModel.Pipeline.ConnectionStarted(start, mousePos);
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
			ViewModel.Pipeline.ConnectionCompleted(link, draggedSide, endConnector);
		}

		private void NetworkView_EndpointLinkDragging(object sender, EndpointLinkDraggingEventArgs e)
		{
			Link link = e.Link as Link;
			ConnectorType type = (ConnectorType)e.DraggedSide;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Pipeline.ConnectionUpdated(link, type, mousePos);
		}

		private void NetworkView_EndpointLinkDragStarted(object sender, EndpointLinkDragStartedEventArgs e)
		{
			Link link = e.Link as Link;
			ConnectorType type = (ConnectorType)e.DraggedSide;
			Point mousePos = Mouse.GetPosition(PART_NetworkView);
			ViewModel.Pipeline.ConnectionUpdated(link, type, mousePos);
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


		private void Simulation_Run_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.WantedStatus = SimulationStatus.Running;
			BackgroundWorkerWindow window = new BackgroundWorkerWindow();
			window.BackgroundWorker = ViewModel.Pipeline.SimulationWorker;
			ViewModel.Pipeline.SimulationWorker.RunWorkerAsync();
			window.ShowDialog();
		}
		private void Simulation_Pause_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.WantedStatus = SimulationStatus.Paused;
			BackgroundWorkerWindow window = new BackgroundWorkerWindow();
			window.BackgroundWorker = ViewModel.Pipeline.SimulationWorker;
			ViewModel.Pipeline.SimulationWorker.RunWorkerAsync();
			window.ShowDialog();
		}
		private void Simulation_Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Pipeline.WantedStatus = SimulationStatus.Stopped;
			BackgroundWorkerWindow window = new BackgroundWorkerWindow();
			window.BackgroundWorker = ViewModel.Pipeline.SimulationWorker;
			ViewModel.Pipeline.SimulationWorker.RunWorkerAsync();
			window.ShowDialog();
		}

		private void NetworkViewContainer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Point mousePos = Mouse.GetPosition(NetworkViewContainer);
			if (e.ChangedButton == MouseButton.Left)
			{
				m_IsLeftMouseDown = true;
				m_IsDragging = false;
				m_MouseDragStart = mousePos;
				m_ViewOffsetStart = ViewModel.Pipeline.ViewOffset;
				NetworkViewContainer.CaptureMouse();
			}
		}

		private void NetworkViewContainer_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePos = Mouse.GetPosition(NetworkViewContainer);
			if (m_IsDragging)
			{
				ViewModel.Pipeline.ViewOffset = new Point(m_ViewOffsetStart.X + (mousePos.X - m_MouseDragStart.X), m_ViewOffsetStart.Y + (mousePos.Y - m_MouseDragStart.Y));
			}
			else if (m_IsLeftMouseDown)
			{
				m_DraggedDistance += Math.Abs(mousePos.Delta(m_MouseDragStart).Length());
				if (m_DraggedDistance > m_DragThreshold)
				{
					m_IsDragging = true;
				}
			}
		}

		private void NetworkViewContainer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Point mousePos = Mouse.GetPosition(NetworkViewContainer);
			if (e.ChangedButton == MouseButton.Left)
			{
				m_IsLeftMouseDown = false;
				m_IsDragging = false;
				NetworkViewContainer.ReleaseMouseCapture();
			}
		}

		private void NetworkViewContainer_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Source != NetworkViewContainer)
			{
				return;
			}
			Point mousePos = Mouse.GetPosition(NetworkViewContainer);
			double zoomStart = ViewModel.Pipeline.ViewZoom;
			if (e.Delta > 0)
			{
				//Up, zoom in
				ViewModel.Pipeline.ViewZoom *= 1.1;
			}
			if (e.Delta < 0)
			{
				//Down, zoom out
				ViewModel.Pipeline.ViewZoom /= 1.1;
			}
			double zoomDelta = ViewModel.Pipeline.ViewZoom / zoomStart;
			Point mouseCoordinate = new Point(mousePos.X - ViewModel.Pipeline.ViewOffset.X, mousePos.Y - ViewModel.Pipeline.ViewOffset.Y);
			double horizontalShift = mouseCoordinate.X - (mouseCoordinate.X * zoomDelta);
			double verticalShift = mouseCoordinate.Y - (mouseCoordinate.Y * zoomDelta);
			ViewModel.Pipeline.ViewOffset = new Point(ViewModel.Pipeline.ViewOffset.X + horizontalShift, ViewModel.Pipeline.ViewOffset.Y + verticalShift);
		}

		private void ResetZoom_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Point mousePos = new Point(NetworkViewContainer.ActualWidth / 2, NetworkViewContainer.ActualHeight / 2);
			if (e.Parameter != null)
			{

			}
			double zoomDelta = 1 / ViewModel.Pipeline.ViewZoom;
			ViewModel.Pipeline.ViewZoom = 1;
			Point mouseCoordinate = new Point(mousePos.X - ViewModel.Pipeline.ViewOffset.X, mousePos.Y - ViewModel.Pipeline.ViewOffset.Y);
			double horizontalShift = mouseCoordinate.X - (mouseCoordinate.X * zoomDelta);
			double verticalShift = mouseCoordinate.Y - (mouseCoordinate.Y * zoomDelta);
			ViewModel.Pipeline.ViewOffset = new Point(ViewModel.Pipeline.ViewOffset.X + horizontalShift, ViewModel.Pipeline.ViewOffset.Y + verticalShift);
		}

		private void AlwaysExecutableCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			e.Handled = true;
		}

		#endregion Event Handlers

	}
}