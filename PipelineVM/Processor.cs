using NetworkVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
namespace PipelineVM
{
	public class Processor : Node, ISimulatable, IPipelineComponent
	{
		#region Properties

		public static readonly object ReadFallbackValue = new object();

		private bool m_MultiThreaded = false;

		/// <summary>
		///  Gets or sets whether the processor runs in multithreaded mode during simulation
		/// </summary>
		/// <remarks>Throws an exception when set during simulation</remarks>
		/// <exception cref="InvalidOperationException">
		///  Thrown when setting the property during simulation
		/// </exception>
		public virtual bool MultiThreaded
		{
			get
			{
				return m_MultiThreaded;
			}
			set
			{
				if (Status == SimulationStatus.Running)
				{
					throw new InvalidOperationException("Cannot set MultiThreaded property Status equals SimulationStatus.Running");
				}
				OnPropertyChanging("MultiThreaded");
				m_MultiThreaded = value;
				OnPropertyChanged("MultiThreaded");
			}
		}
		/// <summary>
		/// Stores the workerthread used during simulation, it is not recommended to set/change it during simulation.
		/// </summary>
		protected Thread WorkerThread { get; set; }

		#endregion Properties

		#region Constructor

		public Processor(Pipeline pipeline)
			: base(pipeline)
		{
			InputConnectors.CollectionChanged += InputConnectors_CollectionChanged;
			Indentifier = Guid.NewGuid();
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Processing

		/// <summary>
		///  Checks if the InputChannels have sufficient data in order to produce output using the
		///  Process() method.
		/// </summary>
		/// <returns>
		///  Returns true if the InputChannels have sufficient data to procuce output in the
		///  Process() method, false otherwise
		/// </returns>
		public virtual bool CanProcess()
		{
			foreach (InputChannel channel in InputConnectors)
			{
				if (!channel.HasData())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///  When implemented, reads required input, then calculates and writes output.
		/// </summary>
		public virtual void Process()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///  Reads data from an InputChannel, or returns the fallback if no inputchannel was able to
		///  retrieve data
		/// </summary>
		/// <param name="channelName">The name the inputchannel to read from</param>
		/// <param name="fallback">   
		///  The value that is returned if no InputChannel has the given name, or if all matched
		///  InputChannels didn't have data available.
		/// </param>
		/// <returns>
		///  Returns the data from the input channel, or fallback if no suitable InputChannel was
		///  found nor had data
		/// </returns>
		protected object ReadFromInput(string channelName)
		{
			foreach (InputChannel channel in InputConnectors)
			{
				if (channel.Name == channelName && channel.HasData())
				{
					return channel.Read();
				}
			}
			return ReadFallbackValue;
		}

		/// <summary>
		///  Checks if the processor has sufficient input to run the Process() method, and runs the
		///  method if the input suffices.
		/// </summary>
		protected virtual void SingleProcessLoop()
		{
			if (CanProcess())
			{
				Process();
			}
		}

		/// <summary>
		///  Method executed by the workerthread in multithreaded mode.
		/// </summary>
		protected virtual void WorkerThreadMethod()
		{
			while (Status != SimulationStatus.Stopped && Status != SimulationStatus.Stopping)
			{
				if (Status == SimulationStatus.Pausing)
				{
					//Confirm pause
					Status = SimulationStatus.Paused;
				}
				while (Status == SimulationStatus.Paused)
				{
					//Wait for unpause
				}
				SingleProcessLoop();
			}

			//Confirm stopped state
			Status = SimulationStatus.Stopped;
		}

		/// <summary>
		///  Writes the given data to all OutputChannels that match the name
		/// </summary>
		/// <param name="channelName">The name of the channel to match</param>
		/// <param name="data">       The data to write to the channel if the names match</param>
		/// <returns>Returns the number of channels the data has been written to</returns>
		protected int WriteToOutput(string channelName, object data)
		{
			int i = 0;
			foreach (OutputChannel channel in OutputConnectors)
			{
				if (channel.Name == channelName)
				{
					channel.Write(data);
					i++;
				}
			}
			return i;
		}

		#endregion Processing

		#region Event Handlers

		private void InputChannel_DataReceived(object sender, EventArgs e)
		{
			if (!MultiThreaded)
			{
				SingleProcessLoop();
			}
		}

		private void InputConnectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (InputChannel channel in e.NewItems)
				{
					channel.DataReceived += InputChannel_DataReceived;
				}
			}
			IEnumerable oldItems = null;
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				oldItems = InputConnectors;
			}
			else
			{
				oldItems = e.OldItems;
			}
			if (oldItems != null)
			{
				foreach (InputChannel channel in oldItems)
				{
					channel.DataReceived -= InputChannel_DataReceived;
				}
			}
		}

		#endregion Event Handlers

		#region Events

		#endregion Events

		#region ISimulatable Members

		private bool m_IsPrepared = false;
		private SimulationStatus m_Status = SimulationStatus.Stopped;

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
				OnStatusChanged();
			}
		}

		public event EventHandler StatusChanged;

		public void Pause()
		{
			Status = SimulationStatus.Pausing;
			if (MultiThreaded)
			{
				if (WorkerThread != null)
				{
					while (Status == SimulationStatus.Pausing && WorkerThread.IsAlive)
					{
						//Wait for thread to confirm paused state
					}
				}
			}

			//Override state to paused state
			Status = SimulationStatus.Paused;
		}

		public virtual void Prepare()
		{
			foreach (InputChannel channel in InputConnectors)
			{
				channel.Clear();
			}
			IsPrepared = true;
		}

		public void Start(bool forcePrepare)
		{
			if (!IsPrepared || forcePrepare)
			{
				Status = SimulationStatus.Preparing;
				Prepare();
			}
			if (WorkerThread != null)
			{
				WorkerThread.Abort();
				while (WorkerThread.IsAlive)
				{
					//This loop blocks until the thread is dead
				}
				WorkerThread = null;
			}
			if (MultiThreaded)
			{
				WorkerThread = new Thread(new ThreadStart(WorkerThreadMethod));
				WorkerThread.IsBackground = true;
				WorkerThread.Start();
			}
			Status = SimulationStatus.Running;
		}

		public virtual void Stop()
		{
			Status = SimulationStatus.Stopping;

			//If multithreading, wait for the thread to confirm the stopped status
			if (MultiThreaded)
			{

				//If the thread isn't dead, kill it.
				if (WorkerThread != null && WorkerThread.IsAlive)
				{
					while (Status == SimulationStatus.Stopping)
					{
						//Wait for confirmation
					}
					WorkerThread.Abort();
					while (WorkerThread.IsAlive)
					{
						//Wait for thread to die.
					}
				}
			}

			//Override status
			Status = SimulationStatus.Stopped;
			IsPrepared = false;
		}

		protected virtual void OnStatusChanged()
		{
			if (StatusChanged != null)
			{
				StatusChanged(this, EventArgs.Empty);
			}
			OnPropertyChanged("StatusChanged");
		}

		#endregion ISimulatable Members

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
			X = double.Parse(reader.GetAttribute("X") ?? "0");
			Y = double.Parse(reader.GetAttribute("Y") ?? "0");
			//Width = double.Parse(reader.GetAttribute("Width") ?? "NaN");//NaN = autosize
			//Height = double.Parse(reader.GetAttribute("Height") ?? "NaN");
			if (!reader.IsEmptyElement)
			{
				reader.Read();//Consume <Processor> tag
				if (reader.Name == "Channels")
				{
					ReadChannels(reader);
					reader.Read();//Consume <Channels /> or </Channels>
				}
			}
		}
		protected virtual void ReadChannels(XmlReader reader)
		{
			InputConnectors.Clear();
			OutputConnectors.Clear();
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement("Channels");//Consume <Channels>
				while (reader.IsStartElement())
				{
					if (reader.Name != "Input" && reader.Name != "Output")
					{
						throw new XmlException("Unexpected child tag " + reader.Name);
					}
					//Read the FullName attribute to get the qualified name of the type
					string fullname = reader.GetAttribute("FullName");
					Type processorType = Type.GetType(fullname);
					//Side note: the constructor will add itself to the correct input/output list
					IPipelineComponent channel = (IPipelineComponent)Activator.CreateInstance(processorType, this);
					channel.ReadXml(reader);
					reader.Read();//Consume <Input />, </Input>, <Output /> or </Output>
				}
			}
		}
		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("FullName", GetType().AssemblyQualifiedName);
			writer.WriteAttributeString("ID", Indentifier.ToString());
			writer.WriteAttributeString("X", Math.Round(X).ToString());
			writer.WriteAttributeString("Y", Math.Round(Y).ToString());
#warning TODO: Store width and height in node
			//writer.WriteAttributeString("Width", Math.Round(Width).ToString());
			//writer.WriteAttributeString("Height", Math.Round(Height).ToString());

			writer.WriteStartElement("Channels");
			foreach (InputChannel channel in InputConnectors)
			{
				writer.WriteStartElement("Input");
				channel.WriteXml(writer);
				writer.WriteEndElement();
			}
			foreach (OutputChannel channel in OutputConnectors)
			{
				writer.WriteStartElement("Output");
				channel.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}