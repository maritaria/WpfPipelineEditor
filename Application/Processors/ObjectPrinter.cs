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
	public class ObjectPrinter : Processor
	{
		#region Properties

		private object m_Data = null;
		public object Data
		{
			get
			{
				return m_Data;
			}
			set
			{
				OnPropertyChanging("Data");
				m_Data = value;
				OnPropertyChanged("Data");
			}
		}

		#endregion Properties

		#region Constructor

		public ObjectPrinter(Pipeline pipeline)
			: base(pipeline)
		{
			new InputChannel(this) { Name = "Display" };
		}

		#endregion Constructor

		#region Methods

		public override void Process()
		{
			Data = this.ReadFromInput("Display");
		}

		#endregion Methods
	}
}