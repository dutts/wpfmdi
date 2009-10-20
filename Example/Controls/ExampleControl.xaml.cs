using System.Windows.Controls;

namespace Example.Controls
{
    /// <summary>
    /// Interaction logic for ExampleControl.xaml
    /// </summary>
    public partial class ExampleControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleControl"/> class.
        /// </summary>
        public ExampleControl()
        {
            InitializeComponent();

            Width = double.NaN;
            Height = double.NaN;
        }
    }
}
