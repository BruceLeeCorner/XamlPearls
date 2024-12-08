using System.ComponentModel;
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
using XamlPearls.GlobalKeyShorts;

namespace WpfApp1
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.RegisterGlobalHotKey(new HotKeyModel()
            {
                Name = "Minium",
                IsSelectCtrl = true,
                SelectKey = Keys.N
            }, (hotkeyModel) =>
            {
                this.WindowState = WindowState.Minimized;
            });
        }
        protected override void OnClosed(EventArgs e)
        {
            this.UnregisterGlobalHotKey("Minium");
            base.OnClosed(e);
        }
    }
}