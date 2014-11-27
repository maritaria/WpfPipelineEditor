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
	public class ToStringProcessor : Processor
	{
		#region Properties
		private InputChannel m_Input;
		private OutputChannel m_Output;
		#endregion
		#region Constructor
		public ToStringProcessor(Pipeline pipeline) : base(pipeline)
		{
			m_Input = new InputChannel(this) { Name = "In" };
			m_Output = new OutputChannel(this) { Name = "Out", OutputTypes = { typeof(string) } };
		}
		#endregion
		#region Methods
		public override void Process()
		{
			m_Output.Write(m_Input.Read().ToString());
		}
		#endregion
	}
}
