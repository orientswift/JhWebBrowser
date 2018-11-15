using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace JhWebBrowser
{
   public class WinBaseforAlert:Window
    {
        public WinBaseforAlert()
        {
            this.Background = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.White;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.FontSize = 14;
            this.MouseLeftButtonDown += WinBaseforAlert_MouseLeftButtonDown;
            this.Style = FindResource("for_noresize_window") as Style;
        }
        public virtual void WinBaseforAlert_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}
