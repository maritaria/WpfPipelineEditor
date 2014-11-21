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

		public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(
			"Link",
			typeof(object),
			typeof(LinkItem),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

		public object Link
		{
			get { return GetValue(LinkProperty); }
			set { SetValue(LinkProperty, value); }
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

		private bool m_IsControlDown = false;

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

		public void LeftMouseDownSelectionLogic()
		{
			m_IsControlDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			if (!m_IsControlDown)
			{
				if (ParentNetworkView.SelectedLinks.Count == 0)
				{
					//There are no items selected, this item becomes the selection
					IsSelected = true;
				}
				else if (ParentNetworkView.SelectedLinks.Contains(this) ||
						 ParentNetworkView.SelectedLinks.Contains(DataContext))
				{
					//Current link is already selected, further handling depends on dragging
				}
				else
				{
					//Link is not selected, clear selection and select this item
					ParentNetworkView.SelectedLinks.Clear();
					IsSelected = true;
				}
			}
		}

		public void LeftMouseUpSelectionLogic()
		{
			if (m_IsControlDown)
			{
				//Control was held, toggle selection
				IsSelected = !IsSelected;
			}
			else
			{
				if (ParentNetworkView.SelectedLinks.Count == 1 &&
					(ParentNetworkView.SelectedLink == this ||
					 ParentNetworkView.SelectedLink == DataContext))
				{
					//This item is the only selected item
				}
				else
				{
					//Clear selection and select the current item
					ParentNetworkView.SelectedLinks.Clear();
					IsSelected = true;
				}
			}
			m_IsControlDown = false;
		}

		public void RightMouseDownSelectionLogic()
		{
			m_IsControlDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			if (!m_IsControlDown)
			{
				if (this.ParentNetworkView.SelectedNodes.Count == 0)
				{
					//There are no items selected, this item becomes the selection
					this.IsSelected = true;
				}
				else if (this.ParentNetworkView.SelectedNodes.Contains(this) ||
						 this.ParentNetworkView.SelectedNodes.Contains(this.DataContext))
				{
					//Current item is already selected, further handling depends on dragging
				}
				else
				{
					//Item is not selected, clear selection and select this item
					this.ParentNetworkView.SelectedNodes.Clear();
					this.IsSelected = true;
				}
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
#warning TODO: drag functionality will move closest endpoint to the mouse
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
#warning TODO: drag functionality will move closest endpoint to the mouse
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
#warning TODO: drag functionality will move closest endpoint to the mouse
		}

		private static void ParentNetworkView_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//Bring to front
		}

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}