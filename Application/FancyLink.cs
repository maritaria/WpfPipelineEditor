using NetworkUI;
using NetworkVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace EditorApplication
{
	public class FancyLink : Control
	{
		#region DependencyProperties

		public static readonly DependencyProperty DestinationHotspotProperty = DependencyProperty.Register(
			"DestinationHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(DestinationHotspot_PropertyChanged)));

		public static readonly DependencyProperty GhostDestinationHotspotProperty = DependencyProperty.Register(
			"GhostDestinationHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(GhostDestinationHotspot_PropertyChanged)));

		public static readonly DependencyProperty IsGhostAcceptedProperty = DependencyProperty.Register(
			"IsGhostAccepted",
			typeof(bool),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(IsGhostAccepted_PropertyChanged)));

		public static readonly DependencyProperty IsGhostVisibleProperty = DependencyProperty.Register(
			"IsGhostVisible",
			typeof(bool),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(IsGhostVisible_PropertyChanged)));

		public static readonly DependencyProperty GhostSourceHotspotProperty = DependencyProperty.Register(
			"GhostSourceHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(GhostSourceHotspot_PropertyChanged)));

		public static readonly DependencyProperty LinkProperty = DependencyProperty.Register(
			"Link",
			typeof(Link),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(Link_PropertyChanged)));


		public static readonly DependencyProperty SourceHotspotProperty = DependencyProperty.Register(
			"SourceHotspot",
			typeof(Point),
			typeof(FancyLink),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(SourceHotspot_PropertyChanged)));

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
		public Point GhostSourceHotspot
		{
			get { return (Point)GetValue(GhostSourceHotspotProperty); }
			set { SetValue(GhostSourceHotspotProperty, value); }
		}

		public Point DestinationHotspot
		{
			get { return (Point)GetValue(DestinationHotspotProperty); }
			set { SetValue(DestinationHotspotProperty, value); }
		}
		public Point GhostDestinationHotspot
		{
			get { return (Point)GetValue(GhostDestinationHotspotProperty); }
			set { SetValue(GhostDestinationHotspotProperty, value); }
		}

		public bool IsGhostAccepted
		{
			get { return (bool)GetValue(IsGhostAcceptedProperty); }
			set { SetValue(IsGhostAcceptedProperty, value); }
		}

		public bool IsGhostVisible
		{
			get { return (bool)GetValue(IsGhostVisibleProperty); }
			set { SetValue(IsGhostVisibleProperty, value); }
		}

		#endregion DependencyProperties

		#region Properties

		private Path PART_GhostPath;
		private Path PART_NormalPath;

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
			PART_NormalPath = (Path)GetTemplateChild("PART_NormalPath");
			PART_GhostPath = (Path)GetTemplateChild("PART_GhostPath");
		}

		private void UpdateGhostPath()
		{
			if (PART_GhostPath == null)
			{
				return;
			}
			PathGeometry pathGeometry = (PART_GhostPath.Data as PathGeometry) ?? (PathGeometry)(PART_GhostPath.Data = new PathGeometry());
			PathFigureCollection figures = pathGeometry.Figures ?? (pathGeometry.Figures = new PathFigureCollection());
			figures.Clear();
			figures.Add(new PathFigure());
			PathFigure figure = figures[0];
			figure.Segments = figure.Segments ?? new PathSegmentCollection();
			figure.Segments.Clear();
			if (IsGhostVisible)
			{
				figure.StartPoint = GhostDestinationHotspot;
				figure.Segments.Add(new LineSegment(GhostSourceHotspot, IsGhostVisible));

				PART_GhostPath.Stroke = new SolidColorBrush()
				{
					Color = (IsGhostAccepted) ? Colors.Lime : Colors.Red,
				};
			}
		}

		private void UpdateNormalPath()
		{
			if (PART_NormalPath == null)
			{
				return;
			}
			PathGeometry pathGeometry = (PART_NormalPath.Data as PathGeometry) ?? (PathGeometry)(PART_NormalPath.Data = new PathGeometry());
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

			Point delta = PointExtensions.Delta(DestinationHotspot, SourceHotspot);

			//The end and starting points need to be within the domain {0,0} to {1,1}
			Point StrokeStart = new Point(0, 0);
			Point StrokeEnd = delta.Normalize();
			if (StrokeEnd.X < 0)
			{
				StrokeStart.X = -StrokeEnd.X;
				StrokeEnd.X = 0;
			}
			if (StrokeEnd.Y < 0)
			{
				StrokeStart.Y = -StrokeEnd.Y;
				StrokeEnd.Y = 0;
			}
			PART_NormalPath.Stroke = new LinearGradientBrush()
			{
				StartPoint = StrokeStart,
				EndPoint = StrokeEnd,
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

		private static void DestinationHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateNormalPath();
		}

		private static void GhostDestinationHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateGhostPath();
		}

		private static void GhostSourceHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateGhostPath();
		}

		private static void Link_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateGhostPath();
		}
		private static void IsGhostAccepted_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateGhostPath();
		}

		private static void IsGhostVisible_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateGhostPath();
		}

		private static void SourceHotspot_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as FancyLink).UpdateNormalPath();
		}

		#endregion Event Handlers
	}
}