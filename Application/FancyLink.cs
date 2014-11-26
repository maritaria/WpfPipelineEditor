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
using Utils;
using NetworkVM;
using NetworkUI;

namespace EditorApplication
{
	public class FancyLink : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(
			"Link",
			typeof(Link),
			typeof(FancyLink),
			new FrameworkPropertyMetadata());
		
		public static readonly DependencyProperty SourceHotspotProperty = DependencyProperty.Register(
			"SourceHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(SourceHotspot_PropertyChanged)));


		public static readonly DependencyProperty DestinationHotspotProperty = DependencyProperty.Register(
			"DestinationHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(DestinationHotspot_PropertyChanged)));
		
		public Link Link
		{
			get { return (Link)GetValue(LinkProperty); }
			set { SetValue(LinkProperty, value); }
		}
		public Point SourceHotspot
		{
			get { return (Point)GetValue(SourceHotspotProperty); }
			set { SetValue(SourceHotspotProperty, value); }
		}
		public Point DestinationHotspot
		{
			get { return (Point)GetValue(DestinationHotspotProperty); }
			set { SetValue(DestinationHotspotProperty, value); }
		}

		private Path PART_Path;

		#endregion DependencyProperties

		#region Properties

		#endregion Properties

		#region Constructor

		static FancyLink()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FancyLink), new FrameworkPropertyMetadata(typeof(FancyLink)));
		}

		#endregion Constructor

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			PART_Path = (Path)GetTemplateChild("PART_Path");
		}

		public void UpdatePath()
		{
			if (PART_Path==null)
			{
				return;
			}
			PathGeometry pathGeometry = (PART_Path.Data as PathGeometry) ?? (PathGeometry)(PART_Path.Data = new PathGeometry());
			PathFigureCollection figures = pathGeometry.Figures ?? (pathGeometry.Figures = new PathFigureCollection());
			figures.Clear();
			figures.Add(new PathFigure());
			PathFigure figure = figures[0];
			figure.Segments = figure.Segments ?? new PathSegmentCollection();
			figure.Segments.Clear();

			Point sourceStart = new Point(SourceHotspot.X + 10, SourceHotspot.Y);
			Point sourceControl = new Point(SourceHotspot.X + 40, SourceHotspot.Y);

			Point destinationStart = new Point(DestinationHotspot.X - 10, DestinationHotspot.Y);
			Point destinationControl = new Point(DestinationHotspot.X - 40, DestinationHotspot.Y);

			Point halfway = new Point((SourceHotspot.X + DestinationHotspot.X) / 2, (SourceHotspot.Y + DestinationHotspot.Y) / 2);
			figure.StartPoint = sourceStart;
			figure.Segments.Add(new QuadraticBezierSegment(sourceControl, halfway, true));
			figure.Segments.Add(new QuadraticBezierSegment(destinationControl, destinationStart, true));

			Point delta = PointOverload.Delta(DestinationHotspot,SourceHotspot);

			PART_Path.Stroke = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0),
				EndPoint = delta.Normalize(),
				GradientStops =
				{
					new GradientStop(Colors.Red, 0),
					new GradientStop(Colors.Red, 0.45),
					new GradientStop(Colors.Lime, 0.55),
					new GradientStop(Colors.Lime, 1),
				},
			};
		}

		#endregion Methods

		#region Event Handlers
		private static void SourceHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdatePath();
		}
		private static void DestinationHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdatePath();
		}
		#endregion Event Handlers

		#region Event

		#endregion Event
	}
}