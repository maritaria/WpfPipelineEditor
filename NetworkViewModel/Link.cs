using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace NetworkVM
{
	public class Link : ViewModelBase
	{
		#region Properties

		private Connector m_DestinationConnector;
		private Point m_DestinationHotspot;
		private Connector m_SourceConnector;
		private Point m_SourceHotspot;
		private Network m_Network;
		private Connector m_GhostSourceConnector;
		private Point m_GhostSourceHotspot;
		private Connector m_GhostDestinationConnector;
		private Point m_GhostDestinationHotspot;
		private bool m_IsGhostVisible = false;
		private bool m_IsGhostAccepted = false;
		private bool m_IsSelected = false;

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
		public bool IsGhostVisible
		{
			get
			{
				return m_IsGhostVisible;
			}
			set
			{
				OnPropertyChanging("IsGhostVisible");
				m_IsGhostVisible = value;
				OnPropertyChanged("IsGhostVisible");
			}
		}
		public bool IsGhostAccepted
		{
			get
			{
				return m_IsGhostAccepted;
			}
			set
			{
				OnPropertyChanging("IsGhostAccepted");
				m_IsGhostAccepted = value;
				OnPropertyChanged("IsGhostAccepted");
			}
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
				if (m_Network!=null)
				{
					m_Network.Links.Remove(this);
				}
				m_Network = value;
				if (m_Network !=null)
				{
					m_Network.Links.Add(this);
				}
				OnPropertyChanged("Network");
			}
		}

		public Connector DestinationConnector
		{
			get
			{
				return m_DestinationConnector;
			}
			set
			{
				OnPropertyChanging("DestinationConnector");
				if (m_DestinationConnector != null)
				{
					m_DestinationConnector.HotspotChanged -= DestinationConnector_HotspotChanged;
					if (m_DestinationConnector.ParentNode != null)
					{
						m_DestinationConnector.ParentNode.Links.Remove(this);
					}
					m_DestinationConnector.Links.Remove(this);
				}
				m_DestinationConnector = value;
				if (m_DestinationConnector != null)
				{
					m_DestinationConnector.HotspotChanged += DestinationConnector_HotspotChanged;
					if (m_DestinationConnector.ParentNode != null)
					{
						m_DestinationConnector.ParentNode.Links.Add(this);
					}
					m_DestinationConnector.Links.Add(this);
					DestinationConnector_HotspotChanged(this, EventArgs.Empty);
				}
				OnPropertyChanged("DestinationConnector");
			}
		}

		public Point DestinationHotspot
		{
			get
			{
				return m_DestinationHotspot;
			}
			set
			{
				OnPropertyChanging("DestinationHotspot");
				m_DestinationHotspot = value;
				OnPropertyChanged("DestinationHotspot");
			}
		}
		public Connector GhostSourceConnector
		{
			get
			{
				return m_GhostSourceConnector;
			}
			set
			{
				OnPropertyChanging("GhostSourceConnector");
				if (m_GhostSourceConnector != null)
				{
					m_GhostSourceConnector.HotspotChanged -= GhostSourceConnector_HotspotChanged;
				}
				m_GhostSourceConnector = value;
				if (m_GhostSourceConnector != null)
				{
					m_GhostSourceConnector.HotspotChanged += GhostSourceConnector_HotspotChanged;
					GhostSourceConnector_HotspotChanged(this, EventArgs.Empty);
				}
				OnPropertyChanged("GhostSourceConnector");
			}
		}


		public Point GhostSourceHotspot
		{
			get
			{
				return m_GhostSourceHotspot;
			}
			set
			{
				OnPropertyChanging("GhostSourceHotspot");
				m_GhostSourceHotspot = value;
				OnPropertyChanged("GhostSourceHotspot");
			}
		}
		public Connector GhostDestinationConnector
		{
			get
			{
				return m_GhostDestinationConnector;
			}
			set
			{
				OnPropertyChanging("GhostDestinationConnector");
				if (m_GhostDestinationConnector != null)
				{
					m_GhostDestinationConnector.HotspotChanged -= GhostDestinationConnector_HotspotChanged;
				}
				m_GhostDestinationConnector = value;
				if (m_GhostDestinationConnector != null)
				{
					m_GhostDestinationConnector.HotspotChanged += GhostDestinationConnector_HotspotChanged;
					GhostDestinationConnector_HotspotChanged(this, EventArgs.Empty);
				}
				OnPropertyChanged("GhostDestinationConnector");
			}
		}


		public Point GhostDestinationHotspot
		{
			get
			{
				return m_GhostDestinationHotspot;
			}
			set
			{
				OnPropertyChanging("GhostDestinationHotspot");
				m_GhostDestinationHotspot = value;
				OnPropertyChanged("GhostDestinationHotspot");
			}
		}

		public Connector SourceConnector
		{
			get
			{
				return m_SourceConnector;
			}
			set
			{
				OnPropertyChanging("SourceConnector");
				if (m_SourceConnector != null)
				{
					m_SourceConnector.HotspotChanged -= SourceConnector_HotspotChanged;
					if (m_SourceConnector.ParentNode != null)
					{
						m_SourceConnector.ParentNode.Links.Remove(this);
					}
					m_SourceConnector.Links.Remove(this);
				}
				m_SourceConnector = value;
				if (m_SourceConnector != null)
				{
					m_SourceConnector.HotspotChanged += SourceConnector_HotspotChanged;
					if (m_SourceConnector.ParentNode != null)
					{
						m_SourceConnector.ParentNode.Links.Add(this);
					}
					m_SourceConnector.Links.Add(this);
					SourceConnector_HotspotChanged(this, EventArgs.Empty);
				}
				OnPropertyChanged("SourceConnector");
			}
		}

		public Point SourceHotspot
		{
			get
			{
				return m_SourceHotspot;
			}
			set
			{
				OnPropertyChanging("SourceHotspot");
				m_SourceHotspot = value;
				OnPropertyChanged("SourceHotspot");
			}
		}

		#endregion Properties

		#region Constructor

		public Link(Network network)
		{
			Network = network;
		}

		#endregion Constructor

		#region Methods

		public void DisableGhost()
		{
			IsGhostVisible = false;
		}

		public void UpdateGhost(ConnectorType startType, Connector ghostEndpoint, bool accepted)
		{
			if (startType == ConnectorType.Source)
			{
				GhostSourceConnector = SourceConnector;
				//In case sourceconnector is null, the hotspot is overriden
				GhostSourceHotspot = SourceHotspot;
			}
			else
			{
				GhostSourceConnector = DestinationConnector;
				//In case destinationconnector is null, the hotspot is overriden
				GhostSourceHotspot = DestinationHotspot;
			}
			GhostDestinationConnector = ghostEndpoint;
			IsGhostVisible = true;
			IsGhostAccepted = accepted;
		}

		#endregion Methods

		#region Event Handlers

		private void DestinationConnector_HotspotChanged(object sender, EventArgs e)
		{
			DestinationHotspot = DestinationConnector.Hotspot;
		}

		private void SourceConnector_HotspotChanged(object sender, EventArgs e)
		{
			SourceHotspot = SourceConnector.Hotspot;
		}

		private void GhostSourceConnector_HotspotChanged(object sender, EventArgs e)
		{
			GhostSourceHotspot = GhostSourceConnector.Hotspot;
		}

		private void GhostDestinationConnector_HotspotChanged(object sender, EventArgs e)
		{
			GhostDestinationHotspot = GhostDestinationConnector.Hotspot;
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}