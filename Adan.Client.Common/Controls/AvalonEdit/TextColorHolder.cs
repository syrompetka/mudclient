using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Themes;

namespace Adan.Client.Common.Controls.AvalonEdit
{
    /// <summary>
    /// 
    /// </summary>
    public struct TextColorHolder
    {
        private TextColor _foreground;
        private TextColor _background;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public TextColorHolder(TextColor foreground, TextColor background)
        {
            _foreground = foreground;
            _background = background;
        }

        /// <summary>
        /// 
        /// </summary>
        public TextColor Foreground
        {
            get
            {
                return _foreground;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextColor Background
        {
            get
            {
                return _background;
            }
        }
    }
}
