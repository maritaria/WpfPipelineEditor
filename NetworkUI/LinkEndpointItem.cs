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
	public class LinkEndpointItem : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty HotspotProperty = DependencyProperty.Register(
			"Hotspot",
			typeof(Point),
			typeof(LinkEndpointItem));

		public static readonly DependencyProperty ParentNetworkViewProperty = DependencyProperty.Register(
			"ParentNetworkView",
			typeof(NetworkView),
			typeof(LinkEndpointItem));

		public static readonly DependencyProperty ParentLinkItemProperty = DependencyProperty.Register(
			"ParentLinkItem",
			typeof(LinkItem),
			typeof(LinkEndpointItem));

		public Point Hotspot
		{
			get { return (Point)GetValue(HotspotProperty); }
			set { SetValue(HotspotProperty, value); }
		}

		internal NetworkView ParentNetworkView
		{
			get { return (NetworkView)GetValue(ParentNetworkViewProperty); }
			set { SetValue(ParentNetworkViewProperty, value); }
		}

		internal LinkItem ParentLinkItem
		{
			get { return (LinkItem)GetValue(ParentLinkItemProperty); }
			set { SetValue(ParentLinkItemProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		#endregion Properties

		#region Constructor

		static LinkEndpointItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkEndpointItem), new FrameworkPropertyMetadata(typeof(LinkEndpointItem)));
		}

		public LinkEndpointItem()
		{
			Focusable = false;
		}

		#endregion Constructor

		#region Methods

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
		}

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}