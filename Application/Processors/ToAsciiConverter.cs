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
	public class ToAsciiConverter : Processor
	{
		#region Properties
		private InputChannel m_Input;
		private OutputChannel m_Output;
		#endregion Properties

		#region Constructor
		public ToAsciiConverter(Pipeline pipeline) : base(pipeline)
		{
			m_Input = new InputChannel(this) { Name = "Bytes", AcceptedTypes = { typeof(IConvertible) } };
			m_Output = new OutputChannel(this) { Name = "Characters", OutputTypes = { typeof(char) } };
		}
		#endregion Constructor

		#region Methods

		public override void Process()
		{
			byte b = (m_Input.Read() as IConvertible).ToByte(CultureInfo.InvariantCulture);
			char c = Encoding.ASCII.GetChars(new byte[] { b }, 0, 1)[0];
			m_Output.Write(c);
		}

		#endregion Methods
	}
}