using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XamlPearls.XamlFont;

namespace FontDialog_Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog(true, true, true);
            fontDialog.Owner = this;
            fontDialog.Font = FontInfo.GetControlFont(TextBlockSample);
            fontDialog.FontSizes = [10, 12, 14, 16, 18];
            if (fontDialog.ShowDialog()== true)
            {
                FontInfo font = fontDialog.Font;
                if (font != null)
                {
                    FontInfo.ApplyFont(this.TextBlockSample, font);
                }
            }
            
        }
    }
}