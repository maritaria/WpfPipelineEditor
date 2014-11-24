using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Windows;

namespace NetworkVM
{
	public class Connector : ViewModelBase
	{
		#region Properties

		private Node m_ParentNode = null;
		private UniqueObservableList<Link> m_Links = new UniqueObservableList<Link>();
		private ConnectorType m_Type;
		private string m_Name = "Unnamed";
		private Point m_Hotspot;

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				OnPropertyChanging("Name");
				m_Name = value;
				OnPropertyChanged("Name");
			}
		}

		public ConnectorType Type
		{
			get
			{
				return m_Type;
			}
			private set
			{
				OnPropertyChanging("Type");
				m_Type = value;
				OnPropertyChanged("Type");
			}
		}

		public Point Hotspot
		{
			get
			{
				return m_Hotspot;
			}
			set
			{
				OnPropertyChanging("Hotspot");
				m_Hotspot = value;
				OnHotspotChanged();
			}
		}

		public bool IsConnected
		{
			get
			{
				foreach(Link link in Links)
				{
					if (link.SourceConnector != null && link.DestinationConnector !=null)
					{
						return true;
					}
				}
				return false;
			}
		}

		public UniqueObservableList<Link> Links
		{
			get { return m_Links; }
		}

		public Node ParentNode
		{
			get
			{
				return m_ParentNode;
			}
			set
			{
				if (m_ParentNode == value)
				{
					return;
				}
				OnPropertyChanging("ParentNode");
				if (m_ParentNode != null)
				{
					if (Type == ConnectorType.Input)
					{
						m_ParentNode.InputConnectors.Remove(this);
					}
					else
					{
						m_ParentNode.OutputConnectors.Remove(this);
					}
				}
				m_ParentNode = value;
				if (m_ParentNode != null)
				{
					if (Type == ConnectorType.Input)
					{
						m_ParentNode.InputConnectors.Add(this);
					}
					if (Type == ConnectorType.Output)
					{
						m_ParentNode.OutputConnectors.Add(this);
					}
				}
				OnPropertyChanged("ParentNode");
			}
		}

		#endregion Properties

		#region Constructor

		public Connector(ConnectorType type, Node parent)
		{
			Type = type;
			ParentNode = parent;
		}

		#endregion Constructor

		#region Methods

		public virtual bool AllowConnection(Connector otherSide)
		{
			if (otherSide == null)
			{
				return true;
			}
			if (Type == otherSide.Type)
			{
				return false;
			}
			if (Type == ConnectorType.Input)
			{
				return Links.Count == 0;
			}
			return true;
		}

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		public event EventHandler HotspotChanged;
		protected virtual void OnHotspotChanged()
		{
			if (HotspotChanged!=null)
			{
				HotspotChanged(this, EventArgs.Empty);
			}
			OnPropertyChanged("Hotspot");
		}

		#endregion Events
	}
}