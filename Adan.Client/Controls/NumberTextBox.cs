using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Adan.Client.Controls
{
    /// <summary>
    /// TextBox for only number
    /// </summary>
    public class NumberTextBox : TextBox
    {
        /// <summary>
        /// Checking for only number
        /// </summary>
        /// <param name="e">???</param>
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);
        }

        private bool AreAllValidNumericChars(string str)
        {
            bool ret = true;
            //if (str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeInfinitySymbol |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberGroupSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentDecimalSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentGroupSeparator |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentSymbol |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PerMilleSymbol |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol |
            //    str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveSign)
            //    return ret;

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
