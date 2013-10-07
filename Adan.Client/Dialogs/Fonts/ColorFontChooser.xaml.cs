using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Adan.Client.Fonts;

namespace Adan.Client.Dialogs.Fonts
{
    /// <summary>
    /// Логика взаимодействия для ColorFontChooser.xaml
    /// </summary>
    public partial class ColorFontChooser : UserControl
    {
        /// <summary>
        /// Color Font Chooser
        /// </summary>
        public ColorFontChooser()
        {
            InitializeComponent();
            this.txtSampleText.IsReadOnly = true;
        }

        /// <summary>
        /// Get Selected Font
        /// </summary>
        public FontInfo SelectedFont
        {
            get
            {
                return new FontInfo(this.txtSampleText.FontFamily,
                                    this.txtSampleText.FontSize,
                                    this.txtSampleText.FontStyle,
                                    this.txtSampleText.FontStretch,
                                    this.txtSampleText.FontWeight,
                                    this.colorPicker.SelectedColor.Brush);
            }

        }

        private void colorPicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            this.txtSampleText.Foreground = this.colorPicker.SelectedColor.Brush;
        }
    }
}
