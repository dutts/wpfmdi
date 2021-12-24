﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;

namespace WPF.MDI
{
	[ContentProperty("Children")]
	public class MdiContainer : UserControl
	{
		#region Constants

		/// <summary>
		/// Offset for iniial placement of window, and for cascade mode.
		/// </summary>
		const int WindowOffset = 25;

		#endregion

		#region Static Members

		private static ResourceDictionary currentResourceDictionary;

		#endregion
		
		#region Dependency Properties

		/// <summary>
		/// Identifies the WPF.MDI.MdiContainer.Theme dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiContainer.Theme property.</returns>
		public static readonly DependencyProperty ThemeProperty =
			DependencyProperty.Register("Theme", typeof(ThemeType), typeof(MdiContainer),
			new UIPropertyMetadata(ThemeType.Aero, new PropertyChangedCallback(ThemeValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiContainer.Menu dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiContainer.Menu property.</returns>
		public static readonly DependencyProperty MenuProperty =
			DependencyProperty.Register("Menu", typeof(UIElement), typeof(MdiContainer),
			new UIPropertyMetadata(null, new PropertyChangedCallback(MenuValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiContainer.MdiLayout dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.MdiContainer.MdiLayout property.</returns>
		public static readonly DependencyProperty MdiLayoutProperty =
			DependencyProperty.Register("MdiLayout", typeof(MdiLayout), typeof(MdiContainer),
			new UIPropertyMetadata(MdiLayout.ArrangeIcons, new PropertyChangedCallback(MdiLayoutValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiContainer.ActiveMdiChild dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.ActiveMdiChild.MdiLayout property.</returns>
		public static readonly DependencyProperty ActiveMdiChildProperty =
			DependencyProperty.Register("ActiveMdiChild", typeof(MdiChild), typeof(MdiContainer),
			new UIPropertyMetadata(null, new PropertyChangedCallback(ActiveMdiChildValueChanged)));

		/// <summary>
		/// Identifies the WPF.MDI.MdiContainer.Buttons dependency property.
		/// </summary>
		/// <returns>The identifier for the WPF.MDI.ActiveMdiChild.Buttons property.</returns>
		internal static readonly DependencyProperty ButtonsProperty =
			DependencyProperty.Register("Buttons", typeof(Panel), typeof(MdiContainer),
			new UIPropertyMetadata(null, new PropertyChangedCallback(ButtonsValueChanged)));

		#endregion

		#region Property Accessors

		/// <summary>
		/// Gets or sets the container theme.
		/// The default is determined by the operating system.
		/// This is a dependency property.
		/// </summary>
		/// <value>The container theme.</value>
		public ThemeType Theme
		{
			get { return (ThemeType)GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}

		/// <summary>
		/// Gets or sets the element to display as menu.
		/// Window buttons in maximized mode will be on the same level.
		/// This is a dependency property.
		/// </summary>
		/// <value>Element to be placed in menu row.</value>
		public UIElement Menu
		{
			get { return (UIElement)GetValue(MenuProperty); }
			set { SetValue(MenuProperty, value); }
		}

		/// <summary>
		/// Gets or sets the element to display as menu.
		/// Window buttons in maximized mode will be on the same level.
		/// This is a dependency property.
		/// </summary>
		/// <value>Mdi layout.</value>
		public MdiLayout MdiLayout
		{
			get { return (MdiLayout)GetValue(MdiLayoutProperty); }
			set { SetValue(MdiLayoutProperty, value); }
		}
		
		/// <summary>
		/// Gets active MDI child. Null if no one.
		/// This is a dependency property.
		/// </summary>
		/// <value>Active Mdi Child.</value>
		public MdiChild ActiveMdiChild
		{
			get { return (MdiChild)GetValue(ActiveMdiChildProperty); }
			internal set { SetValue(ActiveMdiChildProperty, value); }
		}

		/// <summary>
		/// Buttons on top; null for non-maximized mode;
		/// </summary>
		/// <value>Buttons panel.</value>
		internal Panel Buttons
		{
			get { return (Panel)GetValue(ButtonsProperty); }
			set { SetValue(ButtonsProperty, value); }
		}

		/// <summary>
		/// Gets correct canvas height for internal usage.
		/// </summary>
		internal double InnerHeight
		{
			get { return ActualHeight - _topPanel.ActualHeight; }
		}

		#endregion

		#region Member Declarations

		/// <summary>
		/// Gets or sets the child elements.
		/// </summary>
		/// <value>The child elements.</value>
		public ObservableCollection<MdiChild> Children { get; set; }

		private Canvas _windowCanvas;

		/// <summary>
		/// Contains user-specified element.
		/// </summary>
		private Border _menu;

		/// <summary>
		/// Contains window buttons in maximized mode.
		/// </summary>
		private Border _buttons;

		/// <summary>
		/// Container for _buttons and _menu.
		/// </summary>
		private Panel _topPanel;

		/// <summary>
		/// Offset for new window.
		/// </summary>
		private double _windowOffset;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="MdiContainer"/> class.
		/// </summary>
		public MdiContainer()
		{
			Background = Brushes.DarkGray;
			Focusable = IsTabStop = false;

			Children = new ObservableCollection<MdiChild>();
			Children.CollectionChanged += Children_CollectionChanged;

			Grid gr = new Grid();
			gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gr.RowDefinitions.Add(new RowDefinition());

			_topPanel = new DockPanel { Background = SystemColors.MenuBrush };
			_topPanel.Children.Add(_menu = new Border());
			DockPanel.SetDock(_menu, Dock.Left);
			_topPanel.Children.Add(_buttons = new Border());
			DockPanel.SetDock(_buttons, Dock.Right);
			_topPanel.SizeChanged += MdiContainer_SizeChanged;
			_topPanel.Children.Add(new UIElement());
			gr.Children.Add(_topPanel);

			ScrollViewer sv = new ScrollViewer {
				Content = _windowCanvas = new Canvas(),
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
			gr.Children.Add(sv);
			Grid.SetRow(sv, 1);
			Content = gr;

			if (Environment.OSVersion.Version.Major > 5)
				ThemeValueChanged(this, new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Aero));
			else
				ThemeValueChanged(this, new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Luna));

			Loaded += MdiContainer_Loaded;
			SizeChanged += MdiContainer_SizeChanged;
			KeyDown += new System.Windows.Input.KeyEventHandler(MdiContainer_KeyDown);
		}

		static void MdiContainer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			if (mdiContainer.Children.Count < 2)
				return;
			switch (e.Key)
			{
				case Key.Tab:
					if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
					{
						int minZindex = Panel.GetZIndex(mdiContainer.Children[0]);
						foreach (MdiChild mdiChild in mdiContainer.Children)
							if (Panel.GetZIndex(mdiChild) < minZindex)
								minZindex = Panel.GetZIndex(mdiChild);
						Panel.SetZIndex(mdiContainer.GetTopChild(), minZindex - 1);
						mdiContainer.GetTopChild().Focus();
						e.Handled = true;
					}
					break;
			}
		}

		#endregion

		#region Container Events

		/// <summary>
		/// Handles the Loaded event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
		{
			Window wnd = Window.GetWindow(this);
			if (wnd != null)
			{
				wnd.Activated += MdiContainer_Activated;
				wnd.Deactivated += MdiContainer_Deactivated;
			}

			_windowCanvas.Width = _windowCanvas.ActualWidth;
			_windowCanvas.Height = _windowCanvas.ActualHeight;

			_windowCanvas.VerticalAlignment = VerticalAlignment.Top;
			_windowCanvas.HorizontalAlignment = HorizontalAlignment.Left;

			InvalidateSize();
		}

		/// <summary>
		/// Handles the Activated event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Activated(object sender, EventArgs e)
		{
			if (ActiveMdiChild == null)
				return;
			ActiveMdiChild.Focused = true;
		}

		/// <summary>
		/// Handles the Deactivated event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MdiContainer_Deactivated(object sender, EventArgs e)
		{
			if (Children.Count == 0)
				return;

			for (int i = 0; i < Children.Count; i++)
				if (Children[i].WindowState != WindowState.Maximized)
					Children[i].Focused = false;
		}

		/// <summary>
		/// Handles the SizeChanged event of the MdiContainer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
		private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Children.Count == 0)
				return;

			for (int i = 0; i < Children.Count; i++)
			{
				MdiChild mdiChild = Children[i];
				if (mdiChild.WindowState == WindowState.Maximized)
				{
					mdiChild.Width = ActualWidth;
					mdiChild.Height = ActualHeight;
				}
				if (mdiChild.WindowState == WindowState.Minimized)
				{
					mdiChild.Position = new Point(mdiChild.Position.X, mdiChild.Position.Y + e.NewSize.Height - e.PreviousSize.Height);
				}
			}
		}

		#endregion

		#region ObservableCollection Events

		/// <summary>
		/// Handles the CollectionChanged event of the Children control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						MdiChild mdiChild = Children[e.NewStartingIndex],
							topChild = ActiveMdiChild;

						mdiChild.Loaded += (s, a) => ActiveMdiChild = mdiChild;
						if (topChild != null && topChild.WindowState == WindowState.Maximized)
							mdiChild.Loaded += (s, a) => mdiChild.WindowState = WindowState.Maximized;

						if (mdiChild.Position.X < 0 || mdiChild.Position.Y < 0)
							mdiChild.Position = new Point(_windowOffset, _windowOffset);
						_windowCanvas.Children.Add(mdiChild);

						_windowOffset += WindowOffset;
						if (_windowOffset + mdiChild.Width > ActualWidth)
							_windowOffset = 0;
						if (_windowOffset + mdiChild.Height > ActualHeight)
							_windowOffset = 0;
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						MdiChild oldChild = (MdiChild)e.OldItems[0];
						_windowCanvas.Children.Remove(oldChild);
						MdiChild newChild = GetTopChild();

						ActiveMdiChild = newChild;
						if (newChild != null && oldChild.WindowState == WindowState.Maximized)
							newChild.WindowState = WindowState.Maximized;
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					_windowCanvas.Children.Clear();
					break;
			}
			InvalidateSize();
		}

		#endregion

		/// <summary>
		/// Invalidates the size checking to see if the furthest
		/// child point exceeds the current height and width.
		/// </summary>
		internal void InvalidateSize()
		{
			Point largestPoint = new Point(0, 0);

			for (int i = 0; i < Children.Count; i++)
			{
				MdiChild mdiChild = Children[i];

				Point farPosition = new Point(mdiChild.Position.X + mdiChild.Width, mdiChild.Position.Y + mdiChild.Height);

				if (farPosition.X > largestPoint.X)
					largestPoint.X = farPosition.X;

				if (farPosition.Y > largestPoint.Y)
					largestPoint.Y = farPosition.Y;
			}

			if (_windowCanvas.Width != largestPoint.X)
				_windowCanvas.Width = largestPoint.X;

			if (_windowCanvas.Height != largestPoint.Y)
				_windowCanvas.Height = largestPoint.Y;
		}

		/// <summary>
		/// Gets MdiChild with maximum ZIndex.
		/// </summary>
		internal MdiChild GetTopChild()
		{
			if (Children.Count < 1)
				return null;

			int index = 0, maxZindex = Panel.GetZIndex(Children[0]);
			for (int i = 1, zindex; i < Children.Count; i++)
			{
				zindex = Panel.GetZIndex(Children[i]);
				if (zindex > maxZindex)
				{
					maxZindex = zindex;
					index = i;
				}
			}
			return Children[index];
		}

		#region Dependency Property Events

		/// <summary>
		/// Dependency property event once the theme value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ThemeValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			ThemeType themeType = (ThemeType)e.NewValue;

			bool max_mode = mdiContainer.ActiveMdiChild != null && mdiContainer.ActiveMdiChild.WindowState == WindowState.Maximized;
			if (max_mode)
				mdiContainer.ActiveMdiChild.WindowState = WindowState.Normal;

			if (currentResourceDictionary != null)
				Application.Current.Resources.MergedDictionaries.Remove(currentResourceDictionary);

			switch (themeType)
			{
				case ThemeType.Luna:
					Application.Current.Resources.MergedDictionaries.Add(currentResourceDictionary = new ResourceDictionary { Source = new Uri(@"/WPF.MDI;component/Themes/Luna.xaml", UriKind.Relative) });
					break;
				case ThemeType.Aero:
					Application.Current.Resources.MergedDictionaries.Add(currentResourceDictionary = new ResourceDictionary { Source = new Uri(@"/WPF.MDI;component/Themes/Aero.xaml", UriKind.Relative) });
					break;
			}

			//if (max_mode)
			//    mdiContainer.ActiveMdiChild.WindowState = WindowState.Maximized;
		}

		/// <summary>
		/// Dependency property event once the menu element has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void MenuValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			UIElement menu = (UIElement)e.NewValue;

			mdiContainer._menu.Child = menu;
		}

		/// <summary>
		/// Dependency property event once the MDI layout value has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void MdiLayoutValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			MdiLayout value = (MdiLayout)e.NewValue;

			if (value == MdiLayout.ArrangeIcons ||
				mdiContainer.Children.Count < 1)
				return;

			// 1. WindowState.Maximized -> WindowState.Normal
			List<MdiChild> minimizedWindows = new List<MdiChild>(),
				normalWindows = new List<MdiChild>();
			foreach (MdiChild mdiChild in mdiContainer.Children)
				switch (mdiChild.WindowState)
				{
					case WindowState.Minimized:
						minimizedWindows.Add(mdiChild);
						break;
					case WindowState.Maximized:
						mdiChild.WindowState = WindowState.Normal;
						normalWindows.Add(mdiChild);
						break;
					default:
						normalWindows.Add(mdiChild);
						break;
				}

			minimizedWindows.Sort(new MdiChildComparer());
			normalWindows.Sort(new MdiChildComparer());

			// 2. Arrange minimized windows
			double containerHeight = mdiContainer.InnerHeight;
			for (int i = 0; i < minimizedWindows.Count; i++)
			{
				MdiChild mdiChild = minimizedWindows[i];
				int capacity = Convert.ToInt32(mdiContainer.ActualWidth) / MdiChild.MinimizedWidth,
					row = i / capacity + 1,
					col = i % capacity;
				containerHeight = mdiContainer.InnerHeight - MdiChild.MinimizedHeight * row;
				double newLeft = MdiChild.MinimizedWidth * col;
				mdiChild.Position = new Point(newLeft, containerHeight);
			}

			// 3. Resize & arrange normal windows
			switch (value)
			{
				case MdiLayout.Cascade:
					{
						double newWidth = mdiContainer.ActualWidth * 0.58, // should be non-linear formula here
							newHeight = containerHeight * 0.67,
							windowOffset = 0;
						foreach (MdiChild mdiChild in normalWindows)
						{
							if (mdiChild.Resizable)
							{
								mdiChild.Width = newWidth;
								mdiChild.Height = newHeight;
							}
							mdiChild.Position = new Point(windowOffset, windowOffset);

							windowOffset += WindowOffset;
							if (windowOffset + mdiChild.Width > mdiContainer.ActualWidth)
								windowOffset = 0;
							if (windowOffset + mdiChild.Height > containerHeight)
								windowOffset = 0;
						}
					}
					break;
				case MdiLayout.TileHorizontal:
					{
						int cols = (int)Math.Sqrt(normalWindows.Count),
							rows = normalWindows.Count / cols;

						List<int> col_count = new List<int>(); // windows per column
						for (int i = 0; i < cols; i++)
						{
							if (normalWindows.Count % cols > cols - i - 1)
								col_count.Add(rows + 1);
							else
								col_count.Add(rows);
						}

						double newWidth = mdiContainer.ActualWidth / cols,
							newHeight = containerHeight / col_count[0],
							offsetTop = 0,
							offsetLeft = 0;

						for (int i = 0, col_index = 0, prev_count = 0; i < normalWindows.Count; i++)
						{
							if (i >= prev_count + col_count[col_index])
							{
								prev_count += col_count[col_index++];
								offsetLeft += newWidth;
								offsetTop = 0;
								newHeight = containerHeight / col_count[col_index];
							}

							MdiChild mdiChild = normalWindows[i];
							if (mdiChild.Resizable)
							{
								mdiChild.Width = newWidth;
								mdiChild.Height = newHeight;
							}
							mdiChild.Position = new Point(offsetLeft, offsetTop);
							offsetTop += newHeight;
						}
					}
					break;
				case MdiLayout.TileVertical:
					{
						int rows = (int)Math.Sqrt(normalWindows.Count),
							cols = normalWindows.Count / rows;

						List<int> col_count = new List<int>(); // windows per column
						for (int i = 0; i < cols; i++)
						{
							if (normalWindows.Count % cols > cols - i - 1)
								col_count.Add(rows + 1);
							else
								col_count.Add(rows);
						}

						double newWidth = mdiContainer.ActualWidth / cols,
							newHeight = containerHeight / col_count[0],
							offsetTop = 0,
							offsetLeft = 0;

						for (int i = 0, col_index = 0, prev_count = 0; i < normalWindows.Count; i++)
						{
							if (i >= prev_count + col_count[col_index])
							{
								prev_count += col_count[col_index++];
								offsetLeft += newWidth;
								offsetTop = 0;
								newHeight = containerHeight / col_count[col_index];
							}

							MdiChild mdiChild = normalWindows[i];
							if (mdiChild.Resizable)
							{
								mdiChild.Width = newWidth;
								mdiChild.Height = newHeight;
							}
							mdiChild.Position = new Point(offsetLeft, offsetTop);
							offsetTop += newHeight;
						}
					}
					break;
			}
			mdiContainer.InvalidateSize();
			mdiContainer.MdiLayout = MdiLayout.ArrangeIcons;
		}

		/// <summary>
		/// Dependency property event once the focused mdi child has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ActiveMdiChildValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			MdiChild newChild = (MdiChild)e.NewValue,
				oldChild = (MdiChild)e.OldValue;

			if (newChild == null || newChild == oldChild)
				return;

			if (oldChild != null &&  oldChild.WindowState == WindowState.Maximized)
				newChild.WindowState = WindowState.Maximized;

			int maxZindex = 0;
			for (int i = 0; i < mdiContainer.Children.Count; i++)
			{
				int zindex = Panel.GetZIndex(mdiContainer.Children[i]);
				if (zindex > maxZindex)
					maxZindex = zindex;
				if (mdiContainer.Children[i] != newChild)
					mdiContainer.Children[i].Focused = false;
				else
					newChild.Focused = true;
			}

			Panel.SetZIndex(newChild, maxZindex + 1);

			if (mdiContainer.MdiChildTitleChanged != null)
				mdiContainer.MdiChildTitleChanged(mdiContainer, new RoutedEventArgs());
		}

		/// <summary>
		/// Dependency property event once the MdiChild's Buttons property has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ButtonsValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MdiContainer mdiContainer = (MdiContainer)sender;
			Panel panel = (Panel)e.NewValue;

			mdiContainer._buttons.Child = panel;

            if (mdiContainer.MdiChildTitleChanged != null)
                mdiContainer.MdiChildTitleChanged(mdiContainer, new RoutedEventArgs());
		}

		#endregion

		#region Nested MdiChild comparerer

		internal class MdiChildComparer : IComparer<MdiChild>
		{
			#region IComparer<MdiChild> Members

			public int Compare(MdiChild x, MdiChild y)
			{
				return -1 * Canvas.GetZIndex(x).CompareTo(Canvas.GetZIndex(y));
			}

			#endregion
		}

		#endregion

		/// <summary>
		/// Occurs when a multiple-document interface (MDI) child form is activated or closed within an MDI application.
		/// </summary>
		public event RoutedEventHandler MdiChildTitleChanged;
	}
}