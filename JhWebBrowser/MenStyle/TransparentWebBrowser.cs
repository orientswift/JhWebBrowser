using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace JhWebBrowser
{
   public class TransparentWebBrowser : Control
    {
        private WebBrowserOverlayWindow _WebBrowserOverlayWindow;
        public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(FrameworkElement), typeof(TransparentWebBrowser), new PropertyMetadata(TargetElementPropertyChanged));
        public FrameworkElement TargetElement
        {
            get
            {
                return GetValue(TargetElementProperty) as FrameworkElement;
            }
            set
            {
                SetValue(TargetElementProperty, value);
            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(TransparentWebBrowser), new PropertyMetadata(SourcePropertyChanged));
        public string Source
        {
            get
            {
                return GetValue(SourceProperty) as string;
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        private static void SourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var transparentWebBrowser = sender as TransparentWebBrowser;
            if (transparentWebBrowser != null)
            {
                transparentWebBrowser._WebBrowserOverlayWindow.Source = args.NewValue as string;
            }
        }

        private static void TargetElementPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var transparentWebBrowser = sender as TransparentWebBrowser;
            if (transparentWebBrowser != null)
            {
                transparentWebBrowser._WebBrowserOverlayWindow.TargetElement = args.NewValue as FrameworkElement;
            }
        }

        public TransparentWebBrowser()
        {
            _WebBrowserOverlayWindow = new WebBrowserOverlayWindow();
            //TODO: Figure out how to automatically set the TargetElement binding...

            //var targetElementBinding = new Binding();
            //var rs = new RelativeSource();
            //rs.AncestorType = typeof(Border);
            //targetElementBinding.RelativeSource = rs;

            //_WebBrowserOverlayWindow.SetBinding(TransparentWebBrowser.TargetElementProperty, targetElementBinding);


        }

        static TransparentWebBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransparentWebBrowser), new FrameworkPropertyMetadata(typeof(TransparentWebBrowser)));
        }
    }
}
