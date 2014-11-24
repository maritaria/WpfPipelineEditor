using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetworkUI
{
	public partial class NetworkView
	{
		#region Event Handlers
		private void NodeItem_NodeResized(object sender, NodeResizeDeltaEventArgs e)
		{
			NodeItem nodeItem = e.OriginalSource as NodeItem;
			if (nodeItem == null)
			{
				//Unexpected state, get the nodeitem from the Node property of the eventargs
				throw new NotImplementedException();
			}
			if (!nodeItem.AllowResize)
			{
				return;
			}
			if (e.DraggedSides.HasFlag(Sides.Left) || e.DraggedSides.HasFlag(Sides.Right))
			{
				if (double.IsNaN(nodeItem.Width))
				{
					nodeItem.Width = nodeItem.ActualWidth;
				}
				//Horizontal
				if (e.DraggedSides.HasFlag(Sides.Left))
				{
					double newWidth = MathHelper.Clamp(nodeItem.Width - e.HorizontalChange, nodeItem.MinWidth, nodeItem.MaxWidth);
					double delta = nodeItem.Width - newWidth;
					nodeItem.X += delta;
					nodeItem.Width = newWidth;
				}
				else
				{
					nodeItem.Width = MathHelper.Clamp(nodeItem.Width + e.HorizontalChange, nodeItem.MinWidth, nodeItem.MaxWidth);
				}
			}
			if (e.DraggedSides.HasFlag(Sides.Top) || e.DraggedSides.HasFlag(Sides.Bottom))
			{
				if (double.IsNaN(nodeItem.Height))
				{
					nodeItem.Height = nodeItem.ActualHeight;
				}
				//Vertical
				if (e.DraggedSides.HasFlag(Sides.Top))
				{
					double newHeight = MathHelper.Clamp(nodeItem.Height - e.VerticalChange, nodeItem.MinHeight, nodeItem.MaxHeight);
					double delta = nodeItem.Height - newHeight;
					nodeItem.Y += delta;
					nodeItem.Height = newHeight;
				}
				else
				{
					nodeItem.Height = MathHelper.Clamp(nodeItem.Height + e.VerticalChange, nodeItem.MinHeight, nodeItem.MaxHeight);
				}
			}
		}
		#endregion
	}
}
