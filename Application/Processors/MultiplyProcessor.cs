using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Globalization;
namespace EditorApplication.Processors
{
	public class MultiplyProcessor : Processor
	{
		#region Properties

		private InputChannel m_InputA;
		private InputChannel m_InputB;
		private OutputChannel m_Output;

		#endregion Properties

		#region Constructor

		public MultiplyProcessor(Pipeline pipeline) : base(pipeline)
		{
			m_InputA = new InputChannel(this) { Name = "A", AcceptedTypes = { typeof(IConvertible) } };
			m_InputB = new InputChannel(this) { Name = "B", AcceptedTypes = { typeof(IConvertible) } };
			m_Output = new OutputChannel(this) { Name = "Out", OutputTypes = { typeof(double) } };
		}

		#endregion Constructor

		#region Methods
		public override void Process()
		{
			//Since they are convertible, convert them to doubles, which is sufficient for most cases.
			double a = (m_InputA.Read() as IConvertible).ToDouble(CultureInfo.InvariantCulture);
			double b = (m_InputB.Read() as IConvertible).ToDouble(CultureInfo.InvariantCulture);
			//Double is automaticly convertible
			m_Output.Write(a * b);
		}
		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}