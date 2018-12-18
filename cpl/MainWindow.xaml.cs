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
using System.Windows.Media.Effects;

namespace cpl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Compiler cpl;
        private BlurEffect effect;
        public MainWindow()
        {
            cpl= new Compiler();
            effect = new BlurEffect
            {
                Radius = 10
            };
            InitializeComponent();
            this.Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.scroView1.ScrollChanged -= new ScrollChangedEventHandler(ScroView1_ScrollChanged);
            this.scroView2.ScrollChanged -= new ScrollChangedEventHandler(ScroView2_ScrollChanged);
            this.scroView3.ScrollChanged -= new ScrollChangedEventHandler(ScroView3_ScrollChanged);
            this.scroView4.ScrollChanged -= new ScrollChangedEventHandler(ScroView4_ScrollChanged);
            this.cplBtn.Click -= new RoutedEventHandler(Button_Click);
            this.soureCodeTb.TextChanged -= new TextChangedEventHandler(SoureCodeTb_TextChanged);
            this.menuItem1.Click -= new RoutedEventHandler(MenuItem_Click);
            this.menuItem2.Click -= new RoutedEventHandler(MenuItem_Click_1);
            this.zh_CN.Click-= new RoutedEventHandler(Lang_Click);
            this.en_US.Click -= new RoutedEventHandler(Lang_Click);
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.soureCodeTb.IsReadOnly)
            {
                this.cplBtn.Content = International.GetString("LIC");
            }
            else
            {
                this.cplBtn.Content = International.GetString("UIC");
            }
            this.soureCodeTb.IsReadOnly = !this.soureCodeTb.IsReadOnly;
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
                this.rowBox.Text = cpl.RowString.ToString();

                this.cplBtn.IsEnabled = true;
                this.soureCodeTb.IsEnabled = true;
                this.resultTb.IsEnabled = true;
            }
            e.Handled = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Effect = effect;
            Window win = new Help();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
            this.Effect = null;
            e.Handled = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Effect = effect;
            Window win = new About();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
            win = null;
            this.Effect = null;
            e.Handled = true;
        }

        private void ScroView1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.scroView2.ScrollToVerticalOffset(this.scroView1.VerticalOffset);
            this.scroView3.ScrollToVerticalOffset(this.scroView1.VerticalOffset);
            this.scroView4.ScrollToVerticalOffset(this.scroView1.VerticalOffset);
            e.Handled = true;
        }

        private void ScroView3_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.scroView2.ScrollToVerticalOffset(this.scroView3.VerticalOffset);
            this.scroView1.ScrollToVerticalOffset(this.scroView3.VerticalOffset);
            this.scroView4.ScrollToVerticalOffset(this.scroView3.VerticalOffset);
            e.Handled = true;
        }

        private void ScroView2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.scroView1.ScrollToVerticalOffset(this.scroView2.VerticalOffset);
            this.scroView3.ScrollToVerticalOffset(this.scroView2.VerticalOffset);
            this.scroView4.ScrollToVerticalOffset(this.scroView2.VerticalOffset);
            e.Handled = true;
        }

        private void ScroView4_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.scroView2.ScrollToVerticalOffset(this.scroView4.VerticalOffset);
            this.scroView3.ScrollToVerticalOffset(this.scroView4.VerticalOffset);
            this.scroView1.ScrollToVerticalOffset(this.scroView4.VerticalOffset);
            e.Handled = true;
        }

        private void Lang_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item != null)
                International.SetCurrentLanguage(item.Name.Replace('_','-'));
            e.Handled = true;
        }
    }
}
