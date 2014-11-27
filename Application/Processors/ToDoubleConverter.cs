using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using PipelineVM;
using NetworkVM;
using Utils;
namespace EditorApplication.Processors
{
	public class ToDoubleConverter : Processor
	{
		#region Properties
		private InputChannel m_Input;
		private OutputChannel m_Output;



		#endregion Properties

		#region Constructor
		public ToDoubleConverter(Pipeline pipeline)
			: base(pipeline)
		{
			m_Input = new InputChannel(this) { Name = "Characters", AcceptedTypes = { typeof(IEnumerable<char>) } };
			m_Output = new OutputChannel(this) { Name = "Value", OutputTypes = { typeof(int), typeof(double) } };
		}
		#endregion Constructor

		#region Methods

		public override void Process()
		{
			IEnumerable<char> chars = (IEnumerable<char>)m_Input.Read();
			string s = new string(chars.ToArray());
			double d = 0;
			if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
			{
				m_Output.Write(d);
			}
		}

		#endregion Methods
	}
}
