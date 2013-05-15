using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Example.Controls;
using WPF.MDI;
using System.Windows.Controls;

namespace Example
{
	/// <summary>
	/// Interaction logic for Main.xaml
	/// </summary>
	public partial class Main : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Main"/> class.
		/// </summary>
		public Main()
		{
			InitializeComponent();
			_original_title = Title;
			Container.Children.CollectionChanged += (o, e) => Menu_RefreshWindows();
			Container.MdiChildTitleChanged += Container_MdiChildTitleChanged;

			Container.Children.Add(new MdiChild
			{
				Title = "Empty Window Using Code",
				Icon = new BitmapImage(new Uri("OriginalLogo.png", UriKind.Relative))
			});

			Container.Children.Add(new MdiChild
			{
				Title = "Window Using Code",
				Content = new ExampleControl(),
				Width = 714,
				Height = 734,
				Position = new Point(300, 80)
			});
		}

		#region Mdi-like title

		string _original_title;

		void Container_MdiChildTitleChanged(object sender, RoutedEventArgs e)
		{
			if (Container.ActiveMdiChild != null && Container.ActiveMdiChild.WindowState == WindowState.Maximized)
				Title = _original_title + " - [" + Container.ActiveMdiChild.Title + "]";
			else
				Title = _original_title;
		}

		#endregion

		#region Theme Menu Events

		/// <summary>
		/// Handles the Click event of the Generic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Generic_Click(object sender, RoutedEventArgs e)
		{
			Generic.IsChecked = true;
			Luna.IsChecked = false;
			Aero.IsChecked = false;

			Container.Theme = ThemeType.Generic;
		}

		/// <summary>
		/// Handles the Click event of the Luna control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Luna_Click(object sender, RoutedEventArgs e)
		{
			Generic.IsChecked = false;
			Luna.IsChecked = true;
			Aero.IsChecked = false;

			Container.Theme = ThemeType.Luna;
		}

		/// <summary>
		/// Handles the Click event of the Aero control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Aero_Click(object sender, RoutedEventArgs e)
		{
			Generic.IsChecked = false;
			Luna.IsChecked = false;
			Aero.IsChecked = true;

			Container.Theme = ThemeType.Aero;
		}

		#endregion

		#region Menu Events

		int ooo = 1;

		/// <summary>
		/// Handles the Click event of the 'Normal window' menu item.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void AddWindow_Click(object sender, RoutedEventArgs e)
		{
			Container.Children.Add(new MdiChild { Content = new Label { Content = "Normal window" }, Title = "Window " + ooo++ });
		}

		/// <summary>
		/// Handles the Click event of the 'Fixed window' menu item.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void AddFixedWindow_Click(object sender, RoutedEventArgs e)
		{
			Container.Children.Add(new MdiChild { Content = new Label { Content = "Fixed width window" }, Title = "Window " + ooo++, Resizable = false });
		}

		/// <summary>
		/// Handles the Click event of the 'Scroll window' menu item.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void AddScrollWindow_Click(object sender, RoutedEventArgs e)
		{
			StackPanel sp = new StackPanel { Orientation = Orientation.Vertical };
			sp.Children.Add(new TextBlock { Text = "Window with scroll", Margin = new Thickness(5) });
			sp.Children.Add(new ComboBox { Margin = new Thickness(20), Width = 300 });
			ScrollViewer sv = new ScrollViewer { Content = sp, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };

			Container.Children.Add(new MdiChild { Content = sv, Title = "Window " + ooo++ });
		}

		/// <summary>
		/// Refresh windows list
		/// </summary>
		void Menu_RefreshWindows()
		{
			WindowsMenu.Items.Clear();
			MenuItem mi;
			for (int i = 0; i < Container.Children.Count; i++)
			{
				MdiChild child = Container.Children[i];
				mi = new MenuItem { Header = child.Title };
				mi.Click += (o, e) => child.Focus();
				WindowsMenu.Items.Add(mi);
			}
			WindowsMenu.Items.Add(new Separator());
			WindowsMenu.Items.Add(mi = new MenuItem { Header = "Cascade" });
			mi.Click += (o, e) => Container.MdiLayout = MdiLayout.Cascade;
			WindowsMenu.Items.Add(mi = new MenuItem { Header = "Horizontally" });
			mi.Click += (o, e) => Container.MdiLayout = MdiLayout.TileHorizontal;
			WindowsMenu.Items.Add(mi = new MenuItem { Header = "Vertically" });
			mi.Click += (o, e) => Container.MdiLayout = MdiLayout.TileVertical;

			WindowsMenu.Items.Add(new Separator());
			WindowsMenu.Items.Add(mi = new MenuItem { Header = "Close all" });
			mi.Click += (o, e) => Container.Children.Clear();
		}

		#endregion

		#region Content Button Events

		/// <summary>
		/// Handles the Click event of the DisableMinimize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void DisableMinimize_Click(object sender, RoutedEventArgs e)
		{
			Window1.MinimizeBox = false;
		}

		/// <summary>
		/// Handles the Click event of the EnableMinimize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void EnableMinimize_Click(object sender, RoutedEventArgs e)
		{
			Window1.MinimizeBox = true;
		}

		/// <summary>
		/// Handles the Click event of the DisableMaximize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void DisableMaximize_Click(object sender, RoutedEventArgs e)
		{
			Window1.MaximizeBox = false;
		}

		/// <summary>
		/// Handles the Click event of the EnableMaximize control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void EnableMaximize_Click(object sender, RoutedEventArgs e)
		{
			Window1.MaximizeBox = true;
		}

        /// <summary>
        /// Handles the Click event of the DisableClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableClose_Click(object sender, RoutedEventArgs e)
        {
            Window1.CloseBox = false;
        }

        /// <summary>
        /// Handles the Click event of the EnableClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EnableClose_Click(object sender, RoutedEventArgs e)
        {
            Window1.CloseBox = true;
        }

		/// <summary>
		/// Handles the Click event of the ShowIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void ShowIcon_Click(object sender, RoutedEventArgs e)
		{
			Window1.ShowIcon = true;
		}

		/// <summary>
		/// Handles the Click event of the HideIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void HideIcon_Click(object sender, RoutedEventArgs e)
		{
			Window1.ShowIcon = false;
		}
		#endregion
	}
}