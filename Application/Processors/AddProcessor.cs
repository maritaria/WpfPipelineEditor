using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using PipelineVM;
namespace EditorApplication.Processors
{
	public class AddProcessor : Processor
	{
		#region Properties
		#endregion
		#region Constructor

		public AddProcessor(Pipeline pipeline) : base(pipeline)
		{
			new InputChannel(this) { Name = "A" };
			new InputChannel(this) { Name = "B" };
			new OutputChannel(this) { Name = "Out" };
		}
		#endregion
		#region Methods
		public override void Process()
		{
			double a = ReadFromInput("A");
			double b = ReadFromInput("B");
			WriteToOutput("Out", a + b);
		}
		#endregion
	}
}
