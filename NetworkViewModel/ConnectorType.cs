using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkVM
{
	public enum ConnectorType : int
	{
		/// <summary>
		///  Indicates an input of a node (destination of data)
		/// </summary>
		Input = 0x01,

		/// <summary>
		///  Indicates an input of a node (destination of data)
		/// </summary>
		Destination = 0x01,

		/// <summary>
		///  Indicates an output of a node (source of data)
		/// </summary>
		Output = 0x02,

		/// <summary>
		///  Indicates an output of a node (source of data)
		/// </summary>
		Source = 0x02,
	}
	public static class ConnectorTypeHelper
	{
		public static ConnectorType Opposite(this ConnectorType value)
		{
			return value == ConnectorType.Source ? ConnectorType.Destination : ConnectorType.Source;
		}
	}
}
