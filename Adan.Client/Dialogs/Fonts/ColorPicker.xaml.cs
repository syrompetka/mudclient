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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Adan.Client.Fonts;
using Adan.Client.ViewModel;

namespace Adan.Client.Dialogs.Fonts
{
    /// <summary>
    /// Логика взаимодействия для ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private ColorPickerViewModel viewModel;

        /// <summary>
        /// Color Changed Event
        /// </summary>
        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
            "ColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPicker));

        /// <summary>
        /// Selected COlor Property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor", typeof(FontColor), typeof(ColorPicker), new UIPropertyMetadata(null));

        /// <summary>
        /// Constructor ColorPicker
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
            this.viewModel = new ColorPickerViewModel();
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// ColorChanged event
        /// </summary>
        public event RoutedEventHandler ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        /// <summary>
        /// Get Selected Color
        /// </summary>
        public FontColor SelectedColor
        {
            get
            {
                FontColor fc = (FontColor)this.GetValue(SelectedColorProperty);
                if (fc == null)
                {
                    fc = AvailableColors.GetFontColor("Black");
                }
                return fc;
            }

            set
            {
                this.viewModel.SelectedFontColor = value;
                SetValue(SelectedColorProperty, value);
            }
        }

        private void RaiseColorChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ColorPicker.ColorChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void superCombo_DropDownClosed(object sender, EventArgs e)
        {
            this.SetValue(SelectedColorProperty, this.viewModel.SelectedFontColor);
            this.RaiseColorChangedEvent();
        }

        private void superCombo_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetValue(SelectedColorProperty, this.viewModel.SelectedFontColor);
        }

    }
}
