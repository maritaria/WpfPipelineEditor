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
using SWF = System.Windows.Forms;
using Utils;

namespace EditorApplication
{
	public class MainViewModel : ViewModelBase
	{
		#region Properties

		private Pipeline m_Pipeline = new Pipeline();
		private UniqueObservableList<TypeInfo> m_ProcessorTypes = new UniqueObservableList<TypeInfo>();

		private bool m_IsSaved = false;
		public bool IsSaved
		{
			get
			{
				return m_IsSaved;
			}
			set
			{
				OnPropertyChanging("IsSaved");
				m_IsSaved = value;
				OnPropertyChanged("IsSaved");
			}
		}

		private string m_FilePath = null;
		public string FilePath
		{
			get
			{
				return m_FilePath;
			}
			set
			{
				OnPropertyChanging("FilePath");
				m_FilePath = value;
				OnPropertyChanged("FilePath");
			}
		}

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

		public void LoadFile(string path)
		{
			if (IsSaved || ConfirmNoSave())
			{
				//Load file
				FileStream fs = null;
				try
				{
					XmlSerializer s = new XmlSerializer(Pipeline.GetType());
					fs = new FileStream(path, FileMode.Open);
					Pipeline = s.Deserialize(fs) as Pipeline;
					FilePath = path;
					IsSaved = true;
				}
				catch(Exception e)
				{
					MessageBox.Show(e.ToString());
				}
				finally
				{
					if (fs !=null)
					{
						fs.Close();
					}
				}
			}
		}
		public bool ConfirmNoSave()
		{
			var result = MessageBox.Show("Do you want to save?", "Unsaved changes", MessageBoxButton.YesNoCancel);
			if (result == MessageBoxResult.Yes)
			{
				return SaveFile();
			}
			return result == MessageBoxResult.No;
		}
		public bool SaveFile()
		{
			if (FilePath != null && FilePath != "")
			{
				SaveFile(FilePath);
				return true;
			}
			FilePath = AskFilePath();
			return (FilePath != null) ? SaveFile() : false;
		}
		public void SaveFile(string path)
		{
			//Write to file
			FileStream fs = null;
			try
			{
				XmlSerializer s = new XmlSerializer(Pipeline.GetType());
				fs = new FileStream(path, FileMode.Create);
				s.Serialize(fs, Pipeline);
				FilePath = path;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			finally
			{
				if (fs!=null)
				{
					fs.Close();
				}
			}
		}
		public string AskFilePath()
		{
			var dialog = new SWF.SaveFileDialog();
			dialog.DefaultExt = ".pipe.xml";
			if (dialog.ShowDialog() == SWF.DialogResult.OK)
			{
				return dialog.FileName;
			}
			return null;
		}
		public void SaveFileAs()
		{
			string s = AskFilePath();
			if (s != null)
			{
				SaveFile(s);
			}
		}

		#endregion Methods
	}
}