using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkUI
{
	public class LinkItem : ListBoxItem
	{
		#region DependencyProperties

		public static readonly DependencyProperty DestinationPointProperty = DependencyProperty.Register(
			"DestinationPoint",
			typeof(Point),
			typeof(LinkItem),
			new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty SourcePointProperty = DependencyProperty.Register(
			"SourcePoint",
			typeof(Point),
			typeof(LinkItem),
			new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
			"ZIndex",
			typeof(int),
			typeof(LinkItem),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		internal static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register(
			"ParentNetworkView",
			typeof(NetworkView),
			typeof(LinkItem),
			new FrameworkPropertyMetadata(ParentNetworkView_PropertyChanged));

		public Point DestinationPoint
		{
			get { return (Point)GetValue(DestinationPointProperty); }
			set { SetValue(DestinationPointProperty, value); }
		}

		public NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		public Point SourcePoint
		{
			get { return (Point)GetValue(SourcePointProperty); }
			set { SetValue(SourcePointProperty, value); }
		}

		public int ZIndex
		{
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		#endregion Properties

		#region Constructor

		static LinkItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkItem), new FrameworkPropertyMetadata(typeof(LinkItem)));
		}

		#endregion Constructor

		#region Methods

		public void BringToFront()
		{
			if (ParentNetworkView == null)
			{
				return;
			}
			ParentNetworkView.BringLinkToFront(this);
		}

		#endregion Methods

		#region Event Handlers

		private static void ParentNetworkView_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//Bring to front
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}