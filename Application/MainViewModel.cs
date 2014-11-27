using EditorApplication.Processors;
using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Threading;
using Utils;

namespace EditorApplication
{
	public class MainViewModel : ViewModelBase
	{
		#region Properties

		private Pipeline m_Pipeline = new Pipeline();
		private UniqueObservableList<TypeInfo> m_ProcessorTypes = new UniqueObservableList<TypeInfo>();

		public Pipeline Pipeline
		{
			get
			{
				return m_Pipeline;
			}
			set
			{
				OnPropertyChanging("Pipeline");
				m_Pipeline = value;
				OnPropertyChanged("Pipeline");
			}
		}

		public UniqueObservableList<TypeInfo> ProcessorTypes
		{
			get
			{
				return m_ProcessorTypes;
			}
			set
			{
				OnPropertyChanging("ProcessorTypes");
				m_ProcessorTypes = value;
				OnPropertyChanged("ProcessorTypes");
			}
		}

		#endregion Properties

		#region Constructor

		public MainViewModel()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			int i = 0;
			foreach (TypeInfo tinfo in asm.DefinedTypes)
			{
				if (tinfo.IsSubclassOf(typeof(Processor)))
				{
					ProcessorTypes.Add(tinfo);

					//CreateProcessor(tinfo, new Point((i / 3) * 160, 200 * (i % 3)));
					i++;
				}
			}
		}

		#endregion Constructor

		#region Methods

		public void CreateProcessor(TypeInfo processorTypeInfo, Point pos)
		{
			Processor p = (Processor)processorTypeInfo.GetConstructor(new Type[] { typeof(Pipeline) }).Invoke(new object[] { Pipeline });
			p.X = pos.X;
			p.Y = pos.Y;
			p.Name = "#" + (Pipeline.Nodes.Count + 1) + " " + processorTypeInfo.Name;
		}

		public void Test()
		{
			ThreadPool.QueueUserWorkItem(delegate {
				FileStream fs = new FileStream("test.xml", FileMode.Create);
				XmlSerializer s = new XmlSerializer(typeof(Pipeline));
				s.Serialize(Console.Out, Pipeline);
				s.Serialize(fs, Pipeline);
				Console.WriteLine();
				Console.WriteLine();

				Thread.Sleep(500);
				Pipeline = null;
				Thread.Sleep(2000);
				fs.Position = 0;
				Pipeline = (Pipeline)s.Deserialize(fs);

				Console.WriteLine("Done");
			}, null);
		}

		#endregion Methods
	}
}