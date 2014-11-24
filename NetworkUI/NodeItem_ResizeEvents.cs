using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace NetworkUI
{
	public partial class NodeItem
	{
		#region Properties

		private Size m_ResizeStartingSize;

		#endregion Properties

		#region Methods

		private void OnApplyTemplate_Thumbs()
		{
			Thumb topleft = (Thumb)GetTemplateChild("PART_Thumb_TopLeft");
			Thumb topright = (Thumb)GetTemplateChild("PART_Thumb_TopRight");
			Thumb bottomleft = (Thumb)GetTemplateChild("PART_Thumb_BottomLeft");
			Thumb bottomright = (Thumb)GetTemplateChild("PART_Thumb_BottomRight");
			Thumb top = (Thumb)GetTemplateChild("PART_Thumb_Top");
			Thumb bottom = (Thumb)GetTemplateChild("PART_Thumb_Bottom");
			Thumb left = (Thumb)GetTemplateChild("PART_Thumb_Left");
			Thumb right = (Thumb)GetTemplateChild("PART_Thumb_Right");

			//Drag Started
			topleft.DragStarted += Thumb_TopLeft_DragStarted;
			topright.DragStarted += Thumb_TopRight_DragStarted;
			bottomleft.DragStarted += Thumb_BottomLeft_DragStarted;
			bottomright.DragStarted += Thumb_BottomRight_DragStarted;
			top.DragStarted += Thumb_Top_DragStarted;
			bottom.DragStarted += Thumb_Bottom_DragStarted;
			left.DragStarted += Thumb_Left_DragStarted;
			right.DragStarted += Thumb_Right_DragStarted;

			//Drag Delta
			topleft.DragDelta += Thumb_TopLeft_DragDelta;
			topright.DragDelta += Thumb_TopRight_DragDelta;
			bottomleft.DragDelta += Thumb_BottomLeft_DragDelta;
			bottomright.DragDelta += Thumb_BottomRight_DragDelta;
			top.DragDelta += Thumb_Top_DragDelta;
			bottom.DragDelta += Thumb_Bottom_DragDelta;
			left.DragDelta += Thumb_Left_DragDelta;
			right.DragDelta += Thumb_Right_DragDelta;

			//Drag Completed
			topleft.DragCompleted += Thumb_TopLeft_DragCompleted;
			topright.DragCompleted += Thumb_TopRight_DragCompleted;
			bottomleft.DragCompleted += Thumb_BottomLeft_DragCompleted;
			bottomright.DragCompleted += Thumb_BottomRight_DragCompleted;
			top.DragCompleted += Thumb_Top_DragCompleted;
			bottom.DragCompleted += Thumb_Bottom_DragCompleted;
			left.DragCompleted += Thumb_Left_DragCompleted;
			right.DragCompleted += Thumb_Right_DragCompleted;
		}

		#endregion Methods

		#region DragDelta Event

		private void Thumb_Bottom_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Bottom, 0, e.VerticalChange);
		}

		private void Thumb_BottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Bottom | Sides.Left, e.HorizontalChange, e.VerticalChange);
		}

		private void Thumb_BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Bottom | Sides.Right, e.HorizontalChange, e.VerticalChange);
		}

		private void Thumb_Left_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Left, e.HorizontalChange, 0);
		}

		private void Thumb_Right_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Right, e.HorizontalChange, 0);
		}

		private void Thumb_Top_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Top, 0, e.VerticalChange);
		}

		private void Thumb_TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Top | Sides.Left, e.HorizontalChange, e.VerticalChange);
		}

		private void Thumb_TopRight_DragDelta(object sender, DragDeltaEventArgs e)
		{
			OnNodeResizeDelta(DataContext, Sides.Top | Sides.Right, e.HorizontalChange, e.VerticalChange);
		}

		#endregion DragDelta Event

		#region DragStarted Event

		private void Thumb_Bottom_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Bottom, sender as Thumb);
		}

		private void Thumb_BottomLeft_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Bottom | Sides.Left, sender as Thumb);
		}

		private void Thumb_BottomRight_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Bottom | Sides.Right, sender as Thumb);
		}

		private void Thumb_Left_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Left, sender as Thumb);
		}

		private void Thumb_Right_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Right, sender as Thumb);
		}

		private void Thumb_Top_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Top, sender as Thumb);
		}

		private void Thumb_TopLeft_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Top | Sides.Left, sender as Thumb);
		}

		private void Thumb_TopRight_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumbs_DragStarted(Sides.Top | Sides.Right, sender as Thumb);
		}

		private void Thumbs_DragStarted(Sides sides, Thumb thumb)
		{
			m_ResizeStartingSize = this.RenderSize;
			if (OnNodeResizeStarted(DataContext, sides))
			{
				thumb.CancelDrag();
			}
		}

		#endregion DragStarted Event

		#region DragCompleted Event

		private void Thumb_Bottom_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Bottom);
		}

		private void Thumb_BottomLeft_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Bottom | Sides.Left);
		}

		private void Thumb_BottomRight_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Bottom | Sides.Right);
		}

		private void Thumb_Left_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Left);
		}

		private void Thumb_Right_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Right);
		}

		private void Thumb_Top_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Top);
		}

		private void Thumb_TopLeft_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Top | Sides.Left);
		}

		private void Thumb_TopRight_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumbs_DragCompleted(Sides.Top | Sides.Right);
		}

		private void Thumbs_DragCompleted(Sides sides)
		{
			OnNodeResizeCompleted(DataContext, sides, m_ResizeStartingSize.Width, m_ResizeStartingSize.Height, Width, Height);
		}

		#endregion DragCompleted Event
	}
}