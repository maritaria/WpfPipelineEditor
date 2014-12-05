using NetworkVM;
using PipelineVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EditorApplication.Processors
{
	public class ConstantSource : Processor
	{
		#region Properties

		private OutputChannel m_OutputChannel;
		private double m_OutputValue = 0;

		public double OutputValue
		{
			get
			{
				return m_OutputValue;
			}
			set
			{
				OnPropertyChanging("OutputValue");
				m_OutputValue = value;
				OnPropertyChanged("OutputValue");
			}
		}

		#endregion Properties

		#region Constructor

		public ConstantSource(Pipeline pipeline)
			: base(pipeline)
		{
		}

		#endregion Constructor

		#region Methods
		public override void Rebuild()
		{
			base.Rebuild();
#warning TODO: Create system in base classes to ask for a process loop
			m_OutputChannel = GetOutputChannel("Out") ?? new OutputChannel(this) { Name = "Out" };
			m_OutputChannel.Links.CollectionChanged += Links_CollectionChanged;

		}
		#endregion Methods

		#region Event Handlers

		private void ConstantSource_InputRequest(object sender, InputRequestEventArgs e)
		{
			e.ResponseHasData = true;
			e.ResponseData = OutputValue;
		}

		private void Link_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Link l = sender as Link;
			if (e.PropertyName == "DestinationConnector" && l.DestinationConnector != null)
			{
				//Destination connector of the link has been changed, now link the new connector back
				(l.DestinationConnector as InputChannel).InputRequest += ConstantSource_InputRequest;
			}
		}

		private void Link_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
		{
			Link l = sender as Link;
			if (e.PropertyName == "DestinationConnector" && l.DestinationConnector != null)
			{
				//Destination connector of the link is about to be disconnected
				(l.DestinationConnector as InputChannel).InputRequest -= ConstantSource_InputRequest;
			}
		}

		private void Links_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Link l in e.NewItems)
				{
					l.PropertyChanging += Link_PropertyChanging;
					l.PropertyChanged += Link_PropertyChanged;
					if (l.DestinationConnector != null)
					{
						(l.DestinationConnector as InputChannel).InputRequest += ConstantSource_InputRequest;
					}
				}
			}
			IEnumerable oldItems = null;
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				oldItems = m_OutputChannel.Links;
			}
			else if (e.OldItems != null)
			{
				oldItems = e.OldItems;
			}
			if (oldItems != null)
			{
				foreach (Link l in oldItems)
				{
					l.PropertyChanging -= Link_PropertyChanging;
					l.PropertyChanged -= Link_PropertyChanged;
					if (l.DestinationConnector != null)
					{
						(l.DestinationConnector as InputChannel).InputRequest -= ConstantSource_InputRequest;
					}
				}
			}
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}