using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public class DescriptionAttribute :Attribute
	{
		public string Description { get; protected set; }
		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}
