using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace JhWebBrowser
{
    public delegate bool? DelegateAlertMask(Window win);
    public class MaskDelegate
    {
        public static DelegateAlertMask delegateAlertMethod;
    }
}
