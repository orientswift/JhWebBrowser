using System;
using System.Collections.Generic;
using System.Linq;
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

namespace JhWebBrowser
{
    /// <summary>
    /// ImageIcon.xaml 的交互逻辑
    /// </summary>
    public partial class ImageIcon : UserControl
    {
        #region ImagePath

        /// <summary>
        /// ImagePath Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(ImageIcon),
                new FrameworkPropertyMetadata((new PropertyChangedCallback(OnImagePathChanged))));

        /// <summary>
        /// Gets or sets the ImagePath property. This dependency property 
        /// indicates the path of the image.
        /// </summary>
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ImagePath property.
        /// </summary>
        /// <param name="d">ImageIcon</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageIcon icon = (ImageIcon)d;
            string oldImagePath = (string)e.OldValue;
            string newImagePath = icon.ImagePath;
            icon.OnImagePathChanged(oldImagePath, newImagePath);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ImagePath property.
        /// </summary>
        /// <param name="oldImagePath">Old Value</param>
        /// <param name="newImagePath">New Value</param>
        protected virtual void OnImagePathChanged(string oldImagePath, string newImagePath)
        {
            BitmapImage bmp = new BitmapImage(new Uri(newImagePath, UriKind.RelativeOrAbsolute));

            imageFrame.Source = bmp;
        }

        #endregion
        public ImageIcon()
        {
            InitializeComponent();
        }
    }
}
