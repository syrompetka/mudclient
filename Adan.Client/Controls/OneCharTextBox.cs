using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Adan.Client.Controls
{    
    /// <summary>
    /// TextBox for one char only
    /// </summary>
    public class OneCharTextBox : TextBox
    {
        /// <summary>
        /// Checking one char
        /// </summary>
        /// <param name="e">?</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            TextBox textBox = (TextBox)e.Source;

            if (textBox == null)
                return;

            if(e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.LeftCtrl && 
                e.Key != Key.LeftCtrl && e.Key != Key.LeftShift && e.Key != Key.LeftShift &&
                e.Key != Key.System)
                textBox.Text = String.Empty;

            base.OnKeyDown(e);
        }
    }
}
