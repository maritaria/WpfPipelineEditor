using NetworkVM;
using OxyPlot;
using OxyPlot.Series;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utils;
using System.Threading;

namespace EditorApplication.Processors
{
	public class PlotProcessor : Processor
	{
		#region Properties

		private PlotModel m_OxyPlotModel = new PlotModel();

		public PlotModel OxyPlotModel
		{
			get
			{
				return m_OxyPlotModel;
			}
			set
			{
				OnPropertyChanging("OxyPlotModel");
				m_OxyPlotModel = value;
				OnPropertyChanged("OxyPlotModel");
			}
		}

		public override bool MultiThreaded
		{
			get
			{
				return true;
			}
			set
			{
				//Force multithreading
			}
		}
		
		private Dictionary<InputChannel, LineSeries> GraphDictionary = new Dictionary<InputChannel, LineSeries>();

		#endregion Properties

		#region Constructor

		public PlotProcessor(Pipeline pipeline)
			: base(pipeline)
		{
			GraphDictionary.Add(new InputChannel(this) { Name = "Red", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Red });
			GraphDictionary.Add(new InputChannel(this) { Name = "Orange", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Orange });
			GraphDictionary.Add(new InputChannel(this) { Name = "Yellow", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Yellow });
			GraphDictionary.Add(new InputChannel(this) { Name = "Green", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Green });
			GraphDictionary.Add(new InputChannel(this) { Name = "Blue", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Blue });
			GraphDictionary.Add(new InputChannel(this) { Name = "Purple", AcceptedTypes = { typeof(IConvertible) } }, new LineSeries() { Color = OxyColors.Purple });
			foreach(KeyValuePair<InputChannel,LineSeries> pair in GraphDictionary)
			{
				OxyPlotModel.Series.Add(pair.Value);
			}
		}
		private DateTime m_LastDraw = DateTime.Now;
		protected override void SingleProcessLoop()
		{
			lock (OxyPlotModel.SyncRoot)
			{
				//While results have been found, give each channel a chance to add data
				//Automaticly stop if the update takes longer then 10 milliseconds
				bool update = true;
				while (update && DateTime.Now.Subtract(m_LastDraw).TotalMilliseconds < 10)
				{
					update = false;
					foreach (KeyValuePair<InputChannel, LineSeries> pair in GraphDictionary)
					{
						if (pair.Key.HasData())
						{
							double value = (pair.Key.Read() as IConvertible).ToDouble(CultureInfo.InvariantCulture);
							pair.Value.Points.Add(new DataPoint(pair.Value.Points.Count + 1, value));
							update = true;
						}
					}
				}
				OxyPlotModel.InvalidatePlot(true);
			}
			int millis = 20 - (int)DateTime.Now.Subtract(m_LastDraw).TotalMilliseconds;
			m_LastDraw = DateTime.Now;
			if (millis > 0)
			{
				Thread.Sleep(millis);
			}
		}
		public override void Prepare()
		{
			base.Prepare();
			foreach (KeyValuePair<InputChannel, LineSeries> pair in GraphDictionary)
			{
				pair.Value.Points.Clear();
			}
			OxyPlotModel.ResetAllAxes();
			OxyPlotModel.InvalidatePlot(true);
			m_LastDraw = DateTime.Now;
		}

		#endregion Constructor
	}
}