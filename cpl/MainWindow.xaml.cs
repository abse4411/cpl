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

namespace cpl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Compiler cpl = new Compiler();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.soureCodeTb.Text = "";
            this.resultTb.Text = "Put you soure code in SoureCode Box \r\n <=";
        }

        private void SoureCodeTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.resultTb != null && this.soureCodeTb != null)
            {
                this.cplBtn.IsEnabled = false;
                this.soureCodeTb.IsEnabled = false;
                this.resultTb.Text = "Compiling ...";
                this.resultTb.IsEnabled = false;

                this.resultTb.Text = cpl.Work(this.soureCodeTb.Text);

                this.cplBtn.IsEnabled = true;
                this.soureCodeTb.IsEnabled = true;
                this.resultTb.IsEnabled = true;
            }
        }
    }
}
