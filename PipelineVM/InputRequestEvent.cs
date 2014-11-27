using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipelineVM;
namespace PipelineVM
{
	public class InputRequestEventArgs : EventArgs
	{
		public object ResponseData { get; set; }
		public bool ResponseHasData { get; set; }
		public InputChannel RequestSource { get; private set; }
		public InputRequestEventArgs(InputChannel source)
		{
			RequestSource = source;
			ResponseHasData = false;
			ResponseData = 0;
		}
	}
	public delegate void InputRequestEventHandler(object sender, InputRequestEventArgs e);
}
