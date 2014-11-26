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
	public class MultiplyProcessor : Processor
	{
		#region Properties

		#endregion Properties

		#region Constructor

		public MultiplyProcessor(Pipeline pipeline) : base(pipeline)
		{
			new InputChannel(this) { Name = "A" };
			new InputChannel(this) { Name = "B" };
			new OutputChannel(this) { Name = "Out" };
		}

		#endregion Constructor

		#region Methods
		public override void Process()
		{
			double a = ReadFromInput("A");
			double b = ReadFromInput("B");
			WriteToOutput("Out",a * b);
		}
		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}