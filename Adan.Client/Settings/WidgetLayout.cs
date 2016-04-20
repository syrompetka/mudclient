namespace Adan.Client.Settings
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class WidgetLayout
    {
        public List<WidgetLayoutItem> Widgets
        {
            get;
            set;
        }
    }

    [Serializable]
    public class WidgetLayoutItem
    {
        public string WidgetName { get; set; }

        public bool Visible { get; set; }

        public double Left { get; set; }

        public double Top { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
