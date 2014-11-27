using NetworkVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Utils;

namespace PipelineVM
{
	public class Pipeline : Network, ISimulatable, IPipelineComponent
	{
		#region Properties

		private BackgroundWorker m_SimulationWorker;

		private Point m_ViewOffset = new Point(0, 0);
		private double m_ViewZoom = 1;

		public Point ViewOffset
		{
			get
			{
				return m_ViewOffset;
			}
			set
			{
				OnPropertyChanging("ViewOffset");
				m_ViewOffset = value;
				OnPropertyChanged("ViewOffset");
			}
		}

		public double ViewZoom
		{
			get
			{
				return m_ViewZoom;
			}
			set
			{
				OnPropertyChanging("ViewZoom");
				m_ViewZoom = value;
				OnPropertyChanged("ViewZoom");
			}
		}
		public BackgroundWorker SimulationWorker
		{
			get
			{
				return m_SimulationWorker;
			}
			protected set
			{
				OnPropertyChanging("SimulationWorker");
				if (m_SimulationWorker != null)
				{
					m_SimulationWorker.DoWork -= SimulationWorker_DoWork;
				}
				m_SimulationWorker = value;
				if (m_SimulationWorker != null)
				{
					m_SimulationWorker.DoWork += SimulationWorker_DoWork;
				}
				OnPropertyChanged("SimulationWorker");
			}
		}

		#endregion Properties

		#region Constructor

		public Pipeline()
		{
			SimulationWorker = new BackgroundWorker();
			SimulationWorker.WorkerSupportsCancellation = true;
			SimulationWorker.WorkerReportsProgress = true;
			Indentifier = Guid.NewGuid();
		}

		private void SimulationWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			if (worker == null)
			{
				throw new InvalidOperationException("sender is not a BackgroundWorker instance");
			}
			if (Status != WantedStatus)
			{
				worker.ReportProgress(0);
				switch (WantedStatus)
				{
					case SimulationStatus.Running:
						{
							double inc = 50 / Nodes.Count;
							if (!IsPrepared)
							{
								Status = SimulationStatus.Preparing;
								for (int i = 0; i < Nodes.Count; i++)
								{
									Processor p = Nodes[i] as Processor;
									p.Prepare();
									double progress = i * inc;
									worker.ReportProgress((int)progress);
								}
								IsPrepared = true;
							}
							worker.ReportProgress(50);
							for (int i = 0; i < Nodes.Count; i++)
							{
								Processor p = Nodes[i] as Processor;
								p.Start(false);
								double progress = 50d + (i * inc);
								worker.ReportProgress((int)progress);
							}
							worker.ReportProgress(100);
							Status = SimulationStatus.Running;
							break;
						}
					case SimulationStatus.Paused:
					case SimulationStatus.Pausing:
						{
							Status = SimulationStatus.Pausing;
							double inc = 100 / Nodes.Count;
							for (int i = 0; i < Nodes.Count; i++)
							{
								Processor p = Nodes[i] as Processor;
								p.Pause();
								double progress = i * inc;
								worker.ReportProgress((int)progress);
							}
							worker.ReportProgress(100);
							Status = SimulationStatus.Paused;
							break;
						}
					case SimulationStatus.Stopped:
					case SimulationStatus.Stopping:
						{
							double inc = 100 / Nodes.Count;
							for (int i = 0; i < Nodes.Count; i++)
							{
								Processor p = Nodes[i] as Processor;
								p.Stop();
								double progress = i * inc;
								worker.ReportProgress((int)progress);
							}
							worker.ReportProgress(100);
							Status = SimulationStatus.Stopped;
							IsPrepared = false;
							break;
						}
					case SimulationStatus.Preparing:
						{
							double inc = 50 / Nodes.Count;
							if (Status != SimulationStatus.Stopped)
							{
								Status = SimulationStatus.Stopping;
								for (int i = 0; i < Nodes.Count; i++)
								{
									Processor p = Nodes[i] as Processor;
									p.Stop();
									double progress = (i * inc);
									worker.ReportProgress((int)progress);
								}
							}
							Status = SimulationStatus.Preparing;
							worker.ReportProgress(50);
							for (int i = 0; i < Nodes.Count; i++)
							{
								Processor p = Nodes[i] as Processor;
								p.Prepare();
								double progress = 50d + (i * inc);
								worker.ReportProgress((int)progress);
							}
							IsPrepared = true;
							worker.ReportProgress(100);
							Status = SimulationStatus.Stopped;
							break;
						}
					default:
						worker.CancelAsync();
						break;
				}
			}
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Simulation

		private bool m_IsPrepared = false;
		private SimulationStatus m_Status = SimulationStatus.Stopped;
		private SimulationStatus m_WantedStatus = SimulationStatus.Stopped;

		public bool IsPrepared
		{
			get
			{
				return m_IsPrepared;
			}
			protected set
			{
				OnPropertyChanging("IsPrepared");
				m_IsPrepared = value;
				OnPropertyChanged("IsPrepared");
			}
		}

		public SimulationStatus Status
		{
			get
			{
				return m_Status;
			}
			protected set
			{
				OnPropertyChanging("Status");
				m_Status = value;
				OnPropertyChanged("Status");
			}
		}

		public SimulationStatus WantedStatus
		{
			get
			{
				return m_WantedStatus;
			}
			set
			{
				OnPropertyChanging("WantedStatus");
				m_WantedStatus = value;
				OnPropertyChanged("WantedStatus");
			}
		}

		public event EventHandler StatusChanged;

		public void Pause()
		{
			Status = SimulationStatus.Pausing;
			foreach (Processor p in Nodes)
			{
				p.Pause();
			}
			Status = SimulationStatus.Paused;
		}

		public void Prepare()
		{
			if (Status != SimulationStatus.Stopped)
			{
				Stop();
			}
			Status = SimulationStatus.Preparing;
			foreach (Processor p in Nodes)
			{
				p.Prepare();
			}
			Status = SimulationStatus.Stopped;
		}

		public void Start(bool forcePrepare)
		{
			if (!IsPrepared || forcePrepare)
			{
				Status = SimulationStatus.Preparing;
				Prepare();
			}
			foreach (Processor p in Nodes)
			{
				p.Start(false);
			}
			Status = SimulationStatus.Running;
		}

		public void Stop()
		{
			Status = SimulationStatus.Stopping;
			foreach (Processor p in Nodes)
			{
				p.Stop();
			}
			Status = SimulationStatus.Stopped;
		}

		protected virtual void OnStatusChanged()
		{
			if (StatusChanged != null)
			{
				StatusChanged(this, EventArgs.Empty);
			}
			OnPropertyChanged("Status");
		}

		#endregion Simulation

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events

		#region IPipelineComponent Members

		public Guid Indentifier { get; set; }

		#endregion IPipelineComponent Members

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public override Link CreateLink(Connector start, Point endpoint)
		{
			PipelineLink l = new PipelineLink(this);
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

		public void ReadXml(XmlReader reader)
		{
			Indentifier = Guid.Parse(reader.GetAttribute("ID"));
			if (!reader.IsEmptyElement)
			{
				reader.Read();//Consume starting tag
				if (reader.Name == "Processors")
				{
					ReadProcessors(reader);
					reader.Read();//Consume <Processors /> or </Processors>
				}
				if (reader.Name == "Links")
				{
					ReadLinks(reader);
					reader.Read();//Consume <Links /> or </Links>
				}
			}
		}

		protected virtual void ReadProcessors(XmlReader reader)
		{
			Nodes.Clear();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Processors");//Consume <Processors>
				while (reader.IsStartElement())
				{
					if (reader.Name != "Processor")
					{
						throw new XmlException("Unexpected child tag " + reader.Name);
					}
					//Read the FullName attribute to get the qualified name of the type
					string fullname = reader.GetAttribute("FullName");
					Type processorType = Type.GetType(fullname);
					Processor processor = (Processor)Activator.CreateInstance(processorType, this);
					processor.ReadXml(reader);
					reader.Read();//Consume <Processor /> or </Processor>
				}
			}
		}
		protected virtual void ReadLinks(XmlReader reader)
		{
			Links.Clear();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Links");//Consume <Links>
				while (reader.IsStartElement())
				{
					if (reader.Name != "Link")
					{
						throw new XmlException("Unexpected child tag " + reader.Name);
					}
					//Read the FullName attribute to get the qualified name of the type
					string fullname = reader.GetAttribute("FullName");
					Type processorType = Type.GetType(fullname);
					PipelineLink link = (PipelineLink)Activator.CreateInstance(processorType, this);
					link.ReadXml(reader);
					reader.Read();//Consume <Link /> or </Link>
				}
			}
		}

		protected virtual void ReadViewport(XmlReader reader)
		{
			if(!reader.IsEmptyElement)
			{
				throw new XmlException("Viewport tag must be an empty element");
			}
			double offsetX = double.Parse(reader.GetAttribute("OffsetX") ?? "0");
			double offsetY = double.Parse(reader.GetAttribute("OffsetX") ?? "0");
			double viewZoom = double.Parse(reader.GetAttribute("OffsetX") ?? "1");

			//IF either of the offset values are not valid, then reset the view to {0,0}
			if (double.IsInfinity(offsetX) || double.IsNaN(offsetX)||double.IsInfinity(offsetY) || double.IsNaN(offsetY))
			{
				offsetX = 0;
				offsetY = 0;
			}
			//If the zoom is invalid, reset it to 1:1
			if (double.IsInfinity(viewZoom) || double.IsNaN(viewZoom) || viewZoom <= 0)
			{
				viewZoom = 1;
			}
		}


		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("ID", Indentifier.ToString());
			writer.WriteStartElement("Processors");
			foreach (Processor p in Nodes)
			{
				writer.WriteStartElement("Processor");
				p.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Links");
			foreach (PipelineLink link in Links)
			{
				writer.WriteStartElement("Link");
				link.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Viewport");
			writer.WriteAttributeString("OffsetX",ViewOffset.X.ToString());
			writer.WriteAttributeString("OffsetY", ViewOffset.X.ToString());
			writer.WriteAttributeString("ZoomScale", ViewZoom.ToString());
			writer.WriteEndElement();
		}

		#endregion IXmlSerializable Members
	}
}