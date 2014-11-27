using NetworkVM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace PipelineVM
{
	public class InputChannel : InputConnector, IChannel, IPipelineComponent
	{
		#region Properties

		private UniqueObservableList<Type> m_AcceptedTypes = new UniqueObservableList<Type>();
		private Queue<object> m_Data = new Queue<object>();

		public UniqueObservableList<Type> AcceptedTypes
		{
			get
			{
				return m_AcceptedTypes;
			}
			private set
			{
				OnPropertyChanging("AcceptedTypes");
				m_AcceptedTypes = value;
				OnPropertyChanged("AcceptedTypes");
			}
		}

		public Processor ParentProcessor
		{
			get { return (Processor)ParentNode; }
			set { ParentNode = value; }
		}

		#endregion Properties

		#region Constructor

		public InputChannel(Processor parent)
			: base(parent)
		{
			Indentifier = Guid.NewGuid();
		}

		#endregion Constructor

		#region Methods

		public void Clear()
		{
			m_Data.Clear();
		}

		public bool HasData()
		{
			if (HasDirectData())
			{
				return true;
			}
			return OnInputRequest();
		}
		public bool HasData<T>()
		{
			if (HasDirectData<T>())
			{
				return true;
			}
			return OnInputRequest();
		}

		public bool HasDirectData()
		{
			return (m_Data.Count > 0);
		}

		public bool HasDirectData<T>()
		{
			return m_Data.Count(delegate(object o) { return o is T; }) > 0;
		}

		public object Read()
		{
			return m_Data.Dequeue();
		}

		public void Write(object data)
		{
			if (AcceptsType(data.GetType()))
			{
				m_Data.Enqueue(data);
				OnDataReceived();
			}
			else
			{
				throw new Exception("Type '" + data.GetType() + "' not accepted");
			}
		}

		public bool AcceptsType(Type t)
		{
			if (AcceptedTypes.Count == 0)
			{
				return true;
			}
			foreach (Type at in AcceptedTypes)
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
			if (AcceptedTypes.Count == 0)
			{
				return base.AllowConnection(otherSide);
			}
			if (otherSide is OutputChannel)
			{
				//The validation system is based on the idea that outputs are connected to inputs
				OutputChannel channel = otherSide as OutputChannel;
				if (channel.OutputTypes.Count == 0)
				{
					return true;
				}
				foreach(Type t in AcceptedTypes)
				{
					if (t==typeof(string))
					{

					}
					foreach(Type at in channel.OutputTypes)
					{
						if (t.IsAssignableFrom(at))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		public event EventHandler DataReceived;

		public event InputRequestEventHandler InputRequest;

		protected virtual void OnDataReceived()
		{
			if (DataReceived != null)
			{
				DataReceived(this, EventArgs.Empty);
			}
		}

		protected virtual bool OnInputRequest()
		{
			if (InputRequest != null)
			{
				var e = new InputRequestEventArgs(this);
				InputRequest(this, e);
				if (e.ResponseHasData)
				{
					m_Data.Enqueue(e.ResponseData);
					return true;
				}
			}
			return false;
		}

		#endregion Events

		#region IPipelineComponent Members

		public Guid Indentifier { get; set; }


		#endregion

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
				if (reader.Name == "AcceptedTypes")
				{
					ReadAcceptedTypes(reader);
					reader.Read();//Consume <AcceptedTypes /> or </AcceptedTypes>
				}
			}
		}

		protected virtual void ReadAcceptedTypes(XmlReader reader)
		{
			AcceptedTypes.Clear();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement("AcceptedTypes");//Consume <AcceptedTypes>
				while (reader.IsStartElement())
				{
					if (!reader.IsEmptyElement)
					{
						throw new XmlException("AcceptedType tag must be an empty element (<... />, not <...></...>)");
					}
					string fullname = reader.GetAttribute("FullName");
					Type acceptedType = System.Type.GetType(fullname);
					AcceptedTypes.Add(acceptedType);
					reader.Read();//Consume Type tag
				}
			}
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("FullName", GetType().AssemblyQualifiedName);
			writer.WriteAttributeString("ID", Indentifier.ToString());
			writer.WriteAttributeString("Name", Name);
			writer.WriteStartElement("AcceptedTypes");
			foreach(Type t in AcceptedTypes)
			{
				writer.WriteStartElement("Type");
				writer.WriteAttributeString("FullName", t.AssemblyQualifiedName);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}


		#endregion
	}
}