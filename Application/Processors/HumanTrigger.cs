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
	public class HumanTrigger : Processor
	{
		#region Properties

		private double m_OutputValue = 0;

		public double OutputValue
		{
			get
			{
				return m_OutputValue;
			}
			set
			{
				OnPropertyChanging("OutputValue");
				m_OutputValue = value;
				OnPropertyChanged("OutputValue");
			}
		}

		#endregion Properties

		#region Constructor

		public HumanTrigger(Pipeline pipeline)
			: base(pipeline)
		{
			new OutputChannel(this) { Name = "Out", OutputTypes = { typeof(double) } };
		}

		#endregion Constructor

		#region Methods

		public override void Process()
		{
			WriteToOutput("Out", OutputValue);
		}

		#endregion Methods
	}
}