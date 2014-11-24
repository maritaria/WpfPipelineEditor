using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetworkVM
{
	public class Node : ViewModelBase
	{
		#region Properties

		private UniqueObservableList<Connector> m_InputConnectors = new UniqueObservableList<Connector>();
		private bool m_IsSelected = false;
		private UniqueObservableList<Link> m_Links = new UniqueObservableList<Link>();
		private Network m_Network;
		private UniqueObservableList<Connector> m_OutputConnectors = new UniqueObservableList<Connector>();
		private double m_X = 0;
		private double m_Y = 0;
		private int m_ZIndex = 0;
		private string m_Name = "Unnamed";

		public UniqueObservableList<Connector> InputConnectors
		{
			get { return m_InputConnectors; }
		}
		public string Name
		{
			get { return m_Name;
			}
			set
			{
				OnPropertyChanging("Name");
				m_Name = value;
				OnPropertyChanged("Name");
			}
		}
		public bool IsSelected
		{
			get
			{
				return m_IsSelected;
			}
			set
			{
				OnPropertyChanging("IsSelected");
				m_IsSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}

		public UniqueObservableList<Link> Links
		{
			get { return m_Links; }
		}

		public Network Network
		{
			get
			{
				return m_Network;
			}
			private set
			{
				OnPropertyChanging("Network");
				m_Network = value;
				OnPropertyChanged("Network");
			}
		}

		public UniqueObservableList<Connector> OutputConnectors
		{
			get { return m_OutputConnectors; }
		}

		public double X
		{
			get
			{
				return m_X;
			}
			set
			{
				OnPropertyChanging("X");
				m_X = value;
				OnPropertyChanged("X");
			}
		}

		public double Y
		{
			get
			{
				return m_Y;
			}
			set
			{
				OnPropertyChanging("Y");
				m_Y = value;
				OnPropertyChanged("Y");
			}
		}

		public int ZIndex
		{
			get
			{
				return m_ZIndex;
			}
			set
			{
				OnPropertyChanging("ZIndex");
				m_ZIndex = value;
				OnPropertyChanged("ZIndex");
			}
		}

		#endregion Properties

		#region Constructor

		public Node(Network network)
		{
			Network = network;
			Network.Nodes.Add(this);
			InputConnectors.CollectionChanged += InputConnectors_CollectionChanged;
			OutputConnectors.CollectionChanged += OutputConnectors_CollectionChanged;
			Links.CollectionChanged += Links_CollectionChanged;
		}

		#endregion Constructor

		#region Methods

		public virtual bool AllowConnection(Connector start, Connector end)
		{
			if (start == null || end==null)
			{
				return true;
			}
			return start.AllowConnection(end) && end.AllowConnection(start);
		}

		public virtual bool AllowLinkCreation(Connector start)
		{
			if (start.Type == ConnectorType.Output)
			{
				return true;
			}
			return start.Links.Count == 0;
		}

		public virtual bool AllowUnlinkDrag(Link model)
		{
			return true;
		}

		#endregion Methods

		#region Event Handlers

		private void InputConnectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			//Ensure that the new connectors have the right owner
			if (e.NewItems != null)
			{
				foreach (Connector input in e.NewItems)
				{
					input.ParentNode = this;
				}
			}
		}

		private void Links_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			//Add the connections to the network's list, because this list will automaticly discard duplicates
			if (e.NewItems != null)
			{
				foreach (Link link in e.NewItems)
				{
					Network.Links.Add(link);
				}
			}
		}

		private void OutputConnectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			//Ensure that the new connectors have the right owner
			if (e.NewItems != null)
			{
				foreach (Connector output in e.NewItems)
				{
					output.ParentNode = this;
				}
			}
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}