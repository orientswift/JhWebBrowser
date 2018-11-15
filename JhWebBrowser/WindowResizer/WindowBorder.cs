using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace JhWebBrowser
{
    public class WindowBorder
    {
        /// <summary>
        /// 作为边界的元素.
        /// </summary>
        public FrameworkElement Element { get; private set; }

        /// <summary>
        /// 边界位置的位置.
        /// </summary>
        public BorderPosition Position { get; private set; }

        /// <summary>
        /// 使用指定的元素和位置创建一个新的窗口边框.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="element"></param>
        public WindowBorder(BorderPosition position, FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            Position = position;
            Element = element;
        }
    }
}
