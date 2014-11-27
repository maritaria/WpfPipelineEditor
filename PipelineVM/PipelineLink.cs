using NetworkVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Collections;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utils;

namespace PipelineVM
{
	public class PipelineLink : Link, IPipelineComponent
	{
		#region Properties
		#endregion
		#region Constructor
		public PipelineLink(Network network)
			: base(network)
		{
			Indentifier = Guid.NewGuid();
		}
		#endregion

		#region IPipelineComponent Members

		public Guid Indentifier { get; set; }

		#endregion IPipelineComponent Members

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			Indentifier = Guid.Parse(reader.GetAttribute("ID"));
			if (!reader.IsEmptyElement)
			{
				reader.Read();//Consume starting tag
				if (reader.Name == "Source")
				{
					ReadSource(reader);
					reader.Read();//Consume <Source /> or </Source>
				}
				if (reader.Name == "Destination")
				{
					ReadDestination(reader);
					reader.Read();//Consume <Source /> or </Source>
				}
			}

		}
		private Connector FindChannel(ConnectorType type, Guid cid, Guid pid)
		{
			Pipeline pipeline = Network as Pipeline;
			if (pid != null)
			{
				foreach (Processor p in Network.Nodes)
				{
					if (p.Indentifier == pid)
					{
						IEnumerable list = type == ConnectorType.Input ? p.InputConnectors : p.OutputConnectors;
						foreach (IPipelineComponent channel in list)
						{
							if (channel.Indentifier == cid)
							{
								return (Connector)channel;
							}
						}
						break;
					}
				}
			}
			//Search all channels on all processors to find the channel
			foreach (Processor p in Network.Nodes)
			{
				IEnumerable list = type == ConnectorType.Input ? p.InputConnectors : p.OutputConnectors;
				foreach (IPipelineComponent channel in list)
				{
					if (channel.Indentifier == cid)
					{
						return (Connector)channel;
					}
				}
			}
			return null;
		}
		protected virtual void ReadSource(XmlReader reader)
		{
			SourceConnector = null;
			if (!reader.IsEmptyElement)
			{
				throw new XmlException("Source tag must be an empty element");
			}
			string pids = reader.GetAttribute("ProcessorID");
			string cids = reader.GetAttribute("ChannelID");
			double hx = double.Parse(reader.GetAttribute("HotspotX") ?? "NaN");
			double hy = double.Parse(reader.GetAttribute("HotspotY") ?? "NaN");
			if (cids != null)
			{
				Guid cid = Guid.Parse(cids);
				Guid pid;
				Guid.TryParse(pids,out pid);
				SourceConnector = FindChannel(ConnectorType.Source, cid, pid);
			}
			else
			{
				if (hx == double.NaN)
				{
					throw new XmlException("Missing attribute HotspotX");
				}
				if (hy == double.NaN)
				{
					throw new XmlException("Missing attribute HotspotY");
				}
				SourceHotspot = new Point(hx, hy);
			}
		}
		protected virtual void ReadDestination(XmlReader reader)
		{
			DestinationConnector = null;
			if (!reader.IsEmptyElement)
			{
				throw new XmlException("Destination tag must be an empty element");
			}
			string pids = reader.GetAttribute("ProcessorID");
			string cids = reader.GetAttribute("ChannelID");
			double hx = double.Parse(reader.GetAttribute("HotspotX") ?? "NaN");
			double hy = double.Parse(reader.GetAttribute("HotspotY") ?? "NaN");
			if (cids != null)
			{
				Guid cid = Guid.Parse(cids);
				Guid pid;
				Guid.TryParse(pids, out pid);
				DestinationConnector = FindChannel(ConnectorType.Destination, cid, pid);
			}
			else
			{
				if (hx == double.NaN)
				{
					throw new XmlException("Missing attribute HotspotX");
				}
				if (hy == double.NaN)
				{
					throw new XmlException("Missing attribute HotspotY");
				}
				DestinationHotspot = new Point(hx, hy);
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("FullName", GetType().AssemblyQualifiedName);
			writer.WriteAttributeString("ID", Indentifier.ToString());
			writer.WriteStartElement("Source");
			if (SourceConnector != null)
			{
				writer.WriteAttributeString("ProcessorID", (SourceConnector.ParentNode as IPipelineComponent).Indentifier.ToString());
				writer.WriteAttributeString("ChannelID", (SourceConnector as IPipelineComponent).Indentifier.ToString());
			}
			else
			{
				writer.WriteAttributeString("HotspotX", SourceHotspot.X.ToString());
				writer.WriteAttributeString("HotspotY", SourceHotspot.Y.ToString());
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Destination");
			if (DestinationConnector != null)
			{
				writer.WriteAttributeString("ProcessorID", (DestinationConnector.ParentNode as IPipelineComponent).Indentifier.ToString());
				writer.WriteAttributeString("ChannelID", (DestinationConnector as IPipelineComponent).Indentifier.ToString());
			}
			else
			{
				writer.WriteAttributeString("HotspotX", DestinationHotspot.X.ToString());
				writer.WriteAttributeString("HotspotY", DestinationHotspot.Y.ToString());
			}
			writer.WriteEndElement();
		}

		#endregion IXmlSerializable Members
	}
}