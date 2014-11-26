using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using NetworkVM;
using PipelineVM;
namespace EditorApplication.Processors
{
	public class DefaultSource : ConstantSource
	{
		public DefaultSource(Pipeline pipeline) : base(pipeline)
		{
			new InputChannel(this) { Name = "A" };
		}
		public override void Process()
		{
			WriteToOutput("Out", ReadFromInput("A"));
		}
	}
}
