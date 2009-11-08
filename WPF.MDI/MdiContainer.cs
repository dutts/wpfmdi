using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WPF.MDI
{
    [ContentProperty("Children")]
    public class MdiContainer : UserControl
    {
        /// <summary>
        /// Type of theme to use.
        /// </summary>
        public enum ThemeType
        {
            /// <summary>
            /// Generic Visual Studio designer theme.
            /// </summary>
            Generic,
            /// <summary>
            /// Windows XP blue theme.
            /// </summary>
            Luna,
            /// <summary>
            /// Windows Vista and 7 theme.
            /// </summary>
            Aero
        }

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

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the child elements.
        /// </summary>
        /// <value>The child elements.</value>
        public ObservableCollection<MdiChild> Children { get; set; }

        private Canvas windowCanvas;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MdiContainer"/> class.
        /// </summary>
        public MdiContainer()
        {
            Background = Brushes.DarkGray;

            Children = new ObservableCollection<MdiChild>();
            Children.CollectionChanged += new NotifyCollectionChangedEventHandler(Children_CollectionChanged);

            Content = new ScrollViewer()
            {
                Content = windowCanvas = new Canvas(),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            if (Environment.OSVersion.Version.Major == 5)
                ThemeValueChanged(this, new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Luna));
            else
                ThemeValueChanged(this, new DependencyPropertyChangedEventArgs(ThemeProperty, Theme, ThemeType.Aero));

            Loaded += new RoutedEventHandler(MdiContainer_Loaded);
            SizeChanged += new SizeChangedEventHandler(MdiContainer_SizeChanged);
        }

        #region Container Events

        /// <summary>
        /// Handles the Loaded event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Activated += new EventHandler(MdiContainer_Activated);
            Window.GetWindow(this).Deactivated += new EventHandler(MdiContainer_Deactivated);

            windowCanvas.Width = windowCanvas.ActualWidth;
            windowCanvas.Height = windowCanvas.ActualHeight;

            windowCanvas.VerticalAlignment = VerticalAlignment.Top;
            windowCanvas.HorizontalAlignment = HorizontalAlignment.Left;

            InvalidateSize();
        }

        /// <summary>
        /// Handles the Activated event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Activated(object sender, EventArgs e)
        {
            if (windowCanvas.Children.Count == 0)
                return;

            ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).Focused = true;
        }

        /// <summary>
        /// Handles the Deactivated event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MdiContainer_Deactivated(object sender, EventArgs e)
        {
            if (windowCanvas.Children.Count == 0)
                return;

            ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).Focused = false;
        }

        /// <summary>
        /// Handles the SizeChanged event of the MdiContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (windowCanvas.Children.Count == 0 || ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).WindowState != WindowState.Maximized)
                return;

            ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).Width = ActualWidth;
            ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).Height = ActualHeight;
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
                        MdiChild mdiChild = (MdiChild)Children[e.NewStartingIndex];

                        Canvas.SetLeft(mdiChild, (windowCanvas.Children.Count * 25) + 2);
                        Canvas.SetTop(mdiChild, (windowCanvas.Children.Count * 25) + 2);

                        if (windowCanvas.Children.Count == 0 || ((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]).WindowState != WindowState.Maximized)
                        {
                            windowCanvas.Children.Add(mdiChild);

                            mdiChild.Loaded += (s, a) =>
                            {
                                Focus(mdiChild);
                            };
                        }
                        else
                            windowCanvas.Children.Insert(windowCanvas.Children.Count - 1, mdiChild);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        windowCanvas.Children.Remove((MdiChild)e.OldItems[0]);

                        if (windowCanvas.Children.Count > 0)
                            Focus((MdiChild)windowCanvas.Children[windowCanvas.Children.Count - 1]);
                    }
                    break;
            }
        }

        #endregion

        #region IAddChild Members

        /// <summary>
        /// Adds a child object.
        /// </summary>
        /// <param name="value">The child object to add.</param>
        public new void AddChild(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetType() != typeof(MdiChild))
                throw new ArgumentException("Value must be an MdiChild control");

            Children.Add((MdiChild)value);
        }

        /// <summary>
        /// Adds the text content of a node to the object.
        /// </summary>
        /// <param name="text">The text to add to the object.</param>
        public new void AddText(string text)
        {

        }

        #endregion

        /// <summary>
        /// Focuses a child and brings it into view.
        /// TODO: Look into how ZIndex works.
        /// </summary>
        /// <param name="mdiChild">The MDI child.</param>
        internal void Focus(MdiChild mdiChild)
        {
            for (int i = 0; i < windowCanvas.Children.Count; i++)
            {
                if (windowCanvas.Children[i] != mdiChild && windowCanvas.Children[i].GetType() == typeof(MdiChild))
                    ((MdiChild)windowCanvas.Children[i]).Focused = false;
                else
                    ((MdiChild)windowCanvas.Children[i]).Focused = true;
            }

            windowCanvas.Children.Remove(mdiChild);
            windowCanvas.Children.Add(mdiChild);
        }

        /// <summary>
        /// Invalidates the size checking to see if the furthest
        /// child point exceeds the current height and width.
        /// </summary>
        internal void InvalidateSize()
        {
            Point largestPoint = new Point(0, 0);

            for (int i = 0; i < windowCanvas.Children.Count; i++)
            {
                MdiChild mdiChild = (MdiChild)windowCanvas.Children[i];

                Point farPosition = new Point(Canvas.GetLeft(mdiChild) + mdiChild.ActualWidth, Canvas.GetTop(mdiChild) + mdiChild.ActualHeight);

                if (farPosition.X > largestPoint.X)
                    largestPoint.X = farPosition.X;

                if (farPosition.Y > largestPoint.Y)
                    largestPoint.Y = farPosition.Y;
            }

            if (windowCanvas.Width != largestPoint.X)
                windowCanvas.Width = largestPoint.X;

            if (windowCanvas.Height != largestPoint.Y)
                windowCanvas.Height = largestPoint.Y;
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

            if (currentResourceDictionary != null)
                Application.Current.Resources.MergedDictionaries.Remove(currentResourceDictionary);

            switch (themeType)
            {
                case ThemeType.Luna:
                    Application.Current.Resources.MergedDictionaries.Add(currentResourceDictionary = (ResourceDictionary)XamlReader.Load(Application.GetResourceStream(new Uri(@"WPF.MDI;component/Themes/Luna.xaml", UriKind.Relative)).Stream));
                    break;
                case ThemeType.Aero:
                    Application.Current.Resources.MergedDictionaries.Add(currentResourceDictionary = (ResourceDictionary)XamlReader.Load(Application.GetResourceStream(new Uri(@"WPF.MDI;component/Themes/Aero.xaml", UriKind.Relative)).Stream));
                    break;
            }
        }

        #endregion
    }
}