using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace NetworkViewModel
{
	public class Network : ViewModelBase
	{
		#region Properties

		private UniqueObservableList<Link> m_Links = new UniqueObservableList<Link>();
		private UniqueObservableList<Node> m_Nodes = new UniqueObservableList<Node>();

		public UniqueObservableList<Link> Links
		{
			get { return m_Links; }
		}

		public UniqueObservableList<Node> Nodes
		{
			get { return m_Nodes; }
		}

		#endregion Properties

		#region Constructor

		public Network()
		{
			Links.CollectionChanged += Links_CollectionChanged;
		}

		#endregion Constructor

		#region Methods

		public Link ConnectionStarted(Connector start, Point endpoint)
		{
			Node node = start.ParentNode;
			if (node == null)
			{
				return null;
			}
			if (node.AllowLinkCreation(start))
			{
				Link l = new Link(node.Network);
				if (start.Type == ConnectorType.Source)
				{
					l.SourceConnector = start;
					l.DestinationHotspot = endpoint;
				}
				else
				{
					l.DestinationConnector = start;
					l.SourceHotspot = endpoint;
				}
				return l;
			}
			return null;
		}
		public void ConnectionUpdated(Link link, ConnectorType draggedSide, Point endpoint)
		{
			if (draggedSide == ConnectorType.Source)
			{
				link.SourceConnector = null;
				link.SourceHotspot = endpoint;
			}
			else
			{
				link.DestinationConnector = null;
				link.DestinationHotspot = endpoint;
			}
		}
		public void ConnectionCompleted(Link link, ConnectorType side,  Connector endpoint)
		{
			if (endpoint==null)
			{
				return;
			}
			if (endpoint.Type != side)
			{
				return;
			}
			if (side == ConnectorType.Source)
			{
				if (endpoint.AllowConnection(link.DestinationConnector))
				{
					link.SourceConnector = endpoint;
				}
			}
			else
			{
				if (endpoint.AllowConnection(link.SourceConnector))
				{
					link.DestinationConnector = endpoint;
				}
			}
		}

		#endregion Methods

		#region Event Handlers

		private void Links_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Link link in e.NewItems)
				{
					if (link.SourceConnector != null)
					{
						link.SourceConnector.Links.Add(link);
						link.SourceConnector.ParentNode.Links.Add(link);
					}
					if (link.DestinationConnector != null)
					{
						link.DestinationConnector.Links.Add(link);
						link.DestinationConnector.ParentNode.Links.Add(link);
					}
				}
			}
			IEnumerable removeList = null;
			if (e.OldItems != null)
			{
				removeList = e.OldItems;
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				removeList = Nodes;
			}
			if (removeList != null)
			{
				foreach (Link link in removeList)
				{
					if (link.DestinationConnector != null)
					{
						link.DestinationConnector.Links.Remove(link);
					}
					if (link.SourceConnector != null)
					{
						link.SourceConnector.Links.Remove(link);
					}
					foreach (Node node in Nodes)
					{
						node.Links.Remove(link);
					}
				}
			}
		}

		private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			IEnumerable removeList = null;
			if (e.OldItems != null)
			{
				removeList = e.OldItems;
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				removeList = Nodes;
			}
			if (removeList != null)
			{
				foreach (Node node in removeList)
				{
					IEnumerable copy = node.Links.ToArray();
					foreach (Link link in copy)
					{
						if (link.SourceConnector != null && link.SourceConnector.ParentNode == node)
						{
							link.SourceConnector = null;
						}
						if (link.DestinationConnector != null && link.DestinationConnector.ParentNode == node)
						{
							link.DestinationConnector = null;
						}
					}
				}
			}
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}