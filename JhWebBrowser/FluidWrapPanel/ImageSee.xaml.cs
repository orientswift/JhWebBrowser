using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ImageSee.xaml 的交互逻辑
    /// </summary>
    public partial class ImageSee : UserControl
    {
        #region Images

        /// <summary>
        /// Images Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImagesProperty =
            DependencyProperty.Register("Images", typeof(List<string>), typeof(ImageSee),
                new FrameworkPropertyMetadata((List<string>)null));

        public List<string> Images
        {
            get { return (List<string>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        #endregion
        ObservableCollection<UIElement> source1 = new ObservableCollection<UIElement>();
        public ImageSee()
        {
            InitializeComponent();
            this.Loaded += ImageSee_Loaded;
        }
        ImageIcon imgIcon;
        void ImageSee_Loaded(object sender, RoutedEventArgs e)
        {
            openImg();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "图像文件|*.png;*.jpg";
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String[] names = openFile.FileNames;
                foreach (var item in names)
                {
                    imgIcon = new ImageIcon()
                    {
                        Width = 200,
                        Height = 200,
                        ImagePath = item
                    };
                    source1.Add(imgIcon);
                }
                fluidWrapPanel.ItemsSource = null;
                fluidWrapPanel.ItemsSource = source1;
            }
        }
        public void openImg()
        {
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/1.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/2.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/3.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/4.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/5.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/6.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/7.jpg" };
            source1.Add(imgIcon);
            imgIcon = new ImageIcon() { Width = 200, Height = 200, ImagePath = "/Images/Icons/8.jpg" };
            source1.Add(imgIcon);
            fluidWrapPanel.ItemsSource = source1;
        }
    }
}
