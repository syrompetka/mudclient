using Adan.Client.Controls;
using Adan.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Controls;

namespace Adan.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class LayoutContainerStyleSelector : StyleSelector
    {
        /// <summary>
        /// 
        /// </summary>
        public Style WidgetStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Style MainOutputStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (container is LayoutItem)
            {
                if (((LayoutItem)container).LayoutElement.ContentId.StartsWith("Plugin"))
                    return WidgetStyle;
                else
                    return MainOutputStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
