using NetworkVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utils;

namespace PipelineVM
{
	public class OutputChannel : OutputConnector, IChannel, IPipelineComponent
	{
		#region Properties

		private UniqueObservableList<Type> m_OutputTypes = new UniqueObservableList<Type>();

		public UniqueObservableList<Type> OutputTypes
		{
			get
			{
				return m_OutputTypes;
			}
			private set
			{
				OnPropertyChanging("AcceptedTypes");
				m_OutputTypes = value;
				OnPropertyChanged("AcceptedTypes");
			}
		}

		#endregion Properties

		#region Constructor

		public OutputChannel(Processor parent)
			: base(parent)
		{
			Indentifier = Guid.NewGuid();
		}

		#endregion Constructor

		#region Methods

		public bool AcceptsType(Type t)
		{
			if (OutputTypes.Count == 0)
			{
				return true;
			}
			foreach (Type at in OutputTypes)
			{
				if (at.IsAssignableFrom(t))
				{
					return true;
				}
			}
			return false;
		}

		public override bool AllowConnection(Connector otherSide)
		{
			if (OutputTypes.Count == 0)
			{
				return base.AllowConnection(otherSide);
			}
			if (otherSide is InputChannel)
			{
				InputChannel channel = otherSide as InputChannel;
				foreach (Type t in OutputTypes)
				{
					return true;
				}
			}
			return false;
		}

		public void Write(object data)
		{
			if (AcceptsType(data.GetType()))
			{
				foreach (Link link in this.Links)
				{
					if (link.DestinationConnector != null)
					{
						(link.DestinationConnector as InputChannel).Write(data);
					}
				}
			}
			else
			{
				throw new Exception("Type '" + data.GetType() + "' not accepted");
			}
		}

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events

		#region IPipelineComponent Members

		public Guid Indentifier { get; set; }


		#endregion IPipelineComponent Members

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public virtual void ReadXml(XmlReader reader)
		{
			Indentifier = Guid.Parse(reader.GetAttribute("ID"));
			Name = reader.GetAttribute("Name") ?? "Missing name";
			if (!reader.IsEmptyElement)
			{
				reader.Read();//Consume starting tag
				if (reader.Name == "OutputTypes")
				{
					ReadOutputTypes(reader);
					reader.Read();//Consume <OutputTypes /> or </OutputTypes>
				}
			}
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("FullName", GetType().AssemblyQualifiedName);
			writer.WriteAttributeString("ID", Indentifier.ToString());
			writer.WriteAttributeString("Name", Name);
			writer.WriteStartElement("OutputTypes");
			foreach (Type t in OutputTypes)
			{
				writer.WriteStartElement("Type");
				writer.WriteAttributeString("FullName", t.AssemblyQualifiedName);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		protected virtual void ReadOutputTypes(XmlReader reader)
		{
			OutputTypes.Clear();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement("OutputTypes");//Consume <OutputTypes ...>
				while (reader.IsStartElement())
				{
					if (!reader.IsEmptyElement)
					{
						throw new XmlException("OutputTypes tag must be an empty element");
					}
					if (reader.Name != "Type")
					{
						throw new XmlException("Unexpected child tag " + reader.Name);
					}
					string fullname = reader.GetAttribute("FullName");
					Type acceptedType = System.Type.GetType(fullname);
					OutputTypes.Add(acceptedType);
					reader.Read();//Consume <Type />
				}
			}
		}

		#endregion IXmlSerializable Members
	}
}