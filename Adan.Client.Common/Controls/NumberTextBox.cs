namespace Adan.Client.Common.Controls
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// TextBox for only number
    /// </summary>
    public class NumberTextBox : TextBox
    {
        /// <summary>
        /// Checking for only number in textbox
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);
        }

        private bool AreAllValidNumericChars(string str)
        {
            bool ret = true;

            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                char ch = str[i];
                ret &= Char.IsDigit(ch);
            }

            return ret;
        }
    }
}
