using NetworkVM;
using PipelineVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Collections;

namespace EditorApplication.Processors
{
	public class ConstantSource : Processor
	{
		#region Properties
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

		private OutputChannel m_OutputChannel;

		#endregion Properties

		#region Constructor
		public ConstantSource(Pipeline pipeline):base(pipeline)
		{
			m_OutputChannel = new OutputChannel(this) { Name = "Out" };
			m_OutputChannel.Links.CollectionChanged += Links_CollectionChanged;

		}

		void Links_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems!=null)
			{
				foreach(Link l in e.NewItems)
				{
					l.PropertyChanging += l_PropertyChanging;
					l.PropertyChanged += l_PropertyChanged;
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
			else if (e.OldItems !=null)
			{
				oldItems = e.OldItems;
			}
			if (oldItems != null)
			{
				foreach (Link l in oldItems)
				{
					l.PropertyChanging -= l_PropertyChanging;
					l.PropertyChanged -= l_PropertyChanged;
					if (l.DestinationConnector != null)
					{
						(l.DestinationConnector as InputChannel).InputRequest -= ConstantSource_InputRequest;
					}
				}
			}
		}

		void l_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Link l = sender as Link;
			if (e.PropertyName == "DestinationConnector" && l.DestinationConnector != null)
			{
				//Destination connector of the link has been changed, now link the new connector back
				(l.DestinationConnector as InputChannel).InputRequest += ConstantSource_InputRequest;
			}
		}

		void l_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
		{
			Link l = sender as Link;
			if (e.PropertyName == "DestinationConnector" && l.DestinationConnector!=null)
			{
				//Destination connector of the link is about to be disconnected
				(l.DestinationConnector as InputChannel).InputRequest -= ConstantSource_InputRequest;
			}
		}

		void ConstantSource_InputRequest(object sender, InputRequestEventArgs e)
		{
			e.ResponseHasData = true;
			e.ResponseData = OutputValue;
		}
		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}