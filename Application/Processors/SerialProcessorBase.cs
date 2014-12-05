using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EditorApplication.Processors
{
	public class SerialProcessorBase : Processor
	{
		#region Properties
		protected OutputChannel Out;
		protected InputChannel In;
		private SerialPort m_SerialPort;
		public SerialPort SerialPort
		{
			get
			{
				return m_SerialPort;
			}
			protected set
			{
				OnPropertyChanging("SerialPort");
				m_SerialPort = value;
				OnPropertyChanged("SerialPort");
			}
		}
		public override bool MultiThreaded
		{
			get
			{
				return true;
			}
			set
			{

			}
		}

		#endregion Properties

		#region Constructor

		public SerialProcessorBase(Pipeline pipeline)
			: base(pipeline)
		{
			SerialPort = new SerialPort();
		}

		#endregion Constructor

		#region Methods
		public override void Rebuild()
		{
			base.Rebuild();
			Out = GetOutputChannel("Bytes") ?? new OutputChannel(this) { Name = "Bytes" };
			In = GetInputChannel("Device") ?? new InputChannel(this) { Name = "Device" };
		}
		public override void Prepare()
		{
			base.Prepare();
			if (!SerialPort.IsOpen)
			{
				SerialPort.Open();
			}
		}

		public override void Stop()
		{
			base.Stop();
			SerialPort.Close();
		}
		
		protected override void SingleProcessLoop()
		{
			if (SerialPort.IsOpen)
			{
				while (SerialPort.BytesToRead > 0)
				{
					Out.Write(SerialPort.ReadByte());
				}
			}
		}

		#endregion Methods
	}
}