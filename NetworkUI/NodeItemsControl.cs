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
	public class NodeItemsControl : ListBox
	{
		#region DependencyProperties

		#endregion DependencyProperties

		#region Properties

		#endregion Properties

		#region Constructor

		static NodeItemsControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeItemsControl), new FrameworkPropertyMetadata(typeof(NodeItemsControl)));
		}

		public NodeItemsControl()
		{
			Focusable = false;
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		///  Creates or identifies the element that is used to display the given item.
		/// </summary>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new NodeItem();
		}

		/// <summary>
		/// Find the NodeItem UI element that has the specified data context.
		/// Return null if no such NodeItem exists.
		/// </summary>
		internal NodeItem FindAssociatedNodeItem(object nodeDataContext)
		{
			return (NodeItem)this.ItemContainerGenerator.ContainerFromItem(nodeDataContext);
		}

		/// <summary>
		///  Determines if the specified item is (or is eligible to be) its own container.
		/// </summary>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is NodeItem;
		}

		#endregion Methods

		#region Event Handlers

		#endregion Event Handlers

		#region Events

		#endregion Events
	}
}