using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Example.Controls;
using WPF.MDI;

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

            Container.Children.Add(new MdiChild()
            {
                Title = "Empty Window Using Code",
                Icon = new BitmapImage(new Uri("OriginalLogo.png", UriKind.Relative))
            });

            Container.Children.Add(new MdiChild()
            {
                Title = "Window Using Code",
                Content = new ExampleControl(),
                Width = 714,
                Height = 734,
                Margin = new Thickness(200, 30, 0, 0)
            });
        }

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

            Container.Theme = MdiContainer.ThemeType.Generic;
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

            Container.Theme = MdiContainer.ThemeType.Luna;
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

            Container.Theme = MdiContainer.ThemeType.Aero;
        }

        #endregion

        #region Menu Events

        /// <summary>
        /// Handles the Click event of the AddWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            Container.Children.Add(new MdiChild());
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