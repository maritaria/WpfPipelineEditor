using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Utils;
using NetworkVM;
using PipelineVM;
namespace EditorApplication.Processors
{
	public class CsvDecoder : Processor
	{
		#region Properties
		private InputChannel m_Input;
		private string m_ColumnSeperator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		private string m_ParserStorage = "";
		public string ColumnSeperator
		{
			get
			{
				return m_ColumnSeperator;
			}
			set
			{
				OnPropertyChanging("ColumnSeperator");
				m_ColumnSeperator = value;
				OnPropertyChanged("ColumnSeperator");
			}
		}
		public int ColumnCount
		{
			get
			{
				return OutputConnectors.Count;
			}
			set
			{
				OnPropertyChanging("ColumnCount");

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("ColumnCount cannot be lower than zero");
				}

				while (OutputConnectors.Count > value)
				{
					OutputConnectors.RemoveAt(OutputConnectors.Count - 1);
				}
				while (OutputConnectors.Count < value)
				{
					//Small note: when the name property is set, OutputConnectors has already been updated to contain the new connector/channel
					new OutputChannel(this) { Name = "Column " + OutputConnectors.Count, OutputTypes = { typeof(IEnumerable<char> )} };
				}

				OnPropertyChanged("ColumnCount");
			}
		}
		#endregion
		#region Constructor
		public CsvDecoder(Pipeline pipeline) : base(pipeline)
		{
		}
		#endregion
		#region Methods
		public override void Rebuild()
		{
			base.Rebuild();
			m_Input = GetInputChannel("Characters")?? new InputChannel(this) { Name = "Characters", AcceptedTypes = { typeof(char), typeof(IEnumerable<char>) } };
		}
		public override void Process()
		{
			object o = m_Input.Read();
			if (o is IEnumerable<char>)
			{
				m_ParserStorage += o as IEnumerable<char>;
			}
			else
			{
				m_ParserStorage += (char)o;
			}
			m_ParserStorage = Parse(m_ParserStorage);
		}

		protected virtual string Parse(string source)
		{
			int i = 0;
			while (true)
			{
				if (source[i] == '$')
				{
					//If the current character is the start of a row, parse the row
					string sub = "";
					int j = i;
					bool ended = false;
					while (j < source.Length)
					{
						if (source[j] == '\n')
						{
							ended = true;
							break;
						}
						j++;
					}
					if (ended)
					{
						sub = source.Substring(i + 1, j - i - 1);
						string[] parts = sub.Split(',');
						for (int cnum = 0; cnum < parts.Length && cnum < ColumnCount; cnum++)
						{
							WriteToOutput("Column " + (cnum + 1), parts[cnum]);
						}
						i = j + 1;
					}
					else
					{
						break;
					}
				}
				else
				{
					//Jump to the next character, because we're currently not at the beginning of a row
					i++;
				}
				//In case we ran out of stuff to parse, stop the loop
				if (i >= source.Length)
				{
					break;
				}
			}
			if (i < source.Length)
			{
				return source.Substring(i);
			}
			else
			{
				return "";
			}
		}
	
		#endregion
	}
}
