using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EditorApplication.Processors
{
	public class BurstGenerator : Processor
	{
		#region Properties

		private double m_StartValue = 0;
		private double m_EndValue = 10;
		private double m_IncValue = 0.1;

		private InputChannel m_Input;
		private OutputChannel m_Output;
		public double StartValue
		{
			get
			{
				return m_StartValue;
			}
			set
			{
				OnPropertyChanging("StartValue");
				m_StartValue = value;
				OnPropertyChanged("StartValue");
			}
		}
		public double EndValue
		{
			get
			{
				return m_EndValue;
			}
			set
			{
				OnPropertyChanging("EndValue");
				m_EndValue = value;
				OnPropertyChanged("EndValue");
			}
		}
		public double IncValue
		{
			get
			{
				return m_IncValue;
			}
			set
			{
				OnPropertyChanging("IncValue");
				m_IncValue = value;
				OnPropertyChanged("IncValue");
			}
		}

		#endregion Properties

		#region Constructor

		public BurstGenerator(Pipeline pipeline)
			: base(pipeline)
		{
		}

		#endregion Constructor

		#region Methods
		public override void Rebuild()
		{
			base.Rebuild();
			m_Input = GetInputChannel("Trigger") ?? new InputChannel(this) { Name = "Trigger" };
			m_Output = GetOutputChannel("Out") ?? new OutputChannel(this) { Name = "Out", OutputTypes = { typeof(double) } };
		}
		public override void Process()
		{
			//Read, so data won't infinitly trigger a process call whenever the first object comes in
			m_Input.Read();
			for (double value = StartValue; value < EndValue; value += IncValue)
			{
				m_Output.Write(value);
			}
		}
		#endregion Methods
	}
}