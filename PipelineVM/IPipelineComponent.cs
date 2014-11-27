using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace PipelineVM
{
	public interface IPipelineComponent : IXmlSerializable
	{
		Guid Indentifier { get; set; }
	}
}
