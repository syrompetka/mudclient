using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Media;

namespace Adan.Client.Fonts
{
    /// <summary>
    /// Font Color
    /// </summary>
    public class FontColor
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SolidColorBrush Brush { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Font name</param>
        /// <param name="brush">Font brush</param>
        public FontColor(string name, SolidColorBrush brush)
        {
            Name = name;
            Brush = brush;
        }

        /// <summary>
        /// Checking Equals
        /// </summary>
        /// <param name="obj">param</param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            FontColor p = obj as FontColor;
            if ((System.Object)p == null)
            {
                return false;
            }

            return (this.Name == p.Name) && (this.Brush.Equals(p.Brush));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Equals(FontColor p)
        {
            if ((object)p == null)
            {
                return false;
            }

            return (this.Name == p.Name) && (this.Brush.Equals(p.Brush));
        }

        /// <summary>
        /// GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "FontColor [Color=" + this.Name + ", " + this.Brush.ToString() + "]";
        }
    }
}
