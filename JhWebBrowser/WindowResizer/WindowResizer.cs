using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace JhWebBrowser
{
    public class WindowResizer
    {
        private readonly Dictionary<BorderPosition, Cursor> cursors = new Dictionary<BorderPosition, Cursor>
        {
            { BorderPosition.Left, Cursors.SizeWE },
            { BorderPosition.Right, Cursors.SizeWE },
            { BorderPosition.Top, Cursors.SizeNS },
            { BorderPosition.Bottom, Cursors.SizeNS },
            { BorderPosition.BottomLeft, Cursors.SizeNESW },
            { BorderPosition.TopRight, Cursors.SizeNESW },
            { BorderPosition.BottomRight, Cursors.SizeNWSE },
            { BorderPosition.TopLeft, Cursors.SizeNWSE }
        };

        /// <summary>
        /// 窗口的边框。
        /// </summary>
        private readonly WindowBorder[] borders;

        /// <summary>
        /// 窗口的句柄。
        /// </summary>
        private HwndSource hwndSource;

        /// <summary>
        /// WPF窗口。
        /// </summary>
        private readonly Window window;

        /// <summary>
        /// 创建一个新的windowresizer为指定的窗口使用/指定的边界元素。
        /// </summary>
        /// <param name="window">The Window which should be resized.</param>
        /// <param name="borders">The elements which can be used to resize the window.</param>
        public WindowResizer(Window window, params WindowBorder[] borders)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            if (borders == null)
            {
                throw new ArgumentNullException("borders");
            }

            this.window = window;
            this.borders = borders;

            foreach (var border in borders)
            {
                border.Element.PreviewMouseLeftButtonDown += Resize;
                border.Element.MouseMove += DisplayResizeCursor;
                border.Element.MouseLeave += ResetCursor;
            }

            window.SourceInitialized += (o, e) => hwndSource = (HwndSource)PresentationSource.FromVisual((Visual)o);
        }

        /// <summary>
        /// 在消息队列上坚持一个消息。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 在指定的边界位置的消息队列上放一个调整大小的消息。
        /// </summary>
        /// <param name="direction"></param>
        private void ResizeWindow(BorderPosition direction)
        {
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)direction, IntPtr.Zero);
        }

        /// <summary>
        /// 将光标当鼠标左键按不。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                window.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Resizes窗口。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resize(object sender, MouseButtonEventArgs e)
        {
            var border = borders.Single(b => b.Element.Equals(sender));
            window.Cursor = cursors[border.Position];
            ResizeWindow(border.Position);
        }

        /// <summary>
        /// 确保显示正确的光标。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            var border = borders.Single(b => b.Element.Equals(sender));
            window.Cursor = cursors[border.Position];
        }
    }
}
