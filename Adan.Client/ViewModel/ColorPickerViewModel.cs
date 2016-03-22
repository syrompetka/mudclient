using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Adan.Client.Fonts;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerViewModel : INotifyPropertyChanged
    {
        private ReadOnlyCollection<FontColor> roFontColors;
        private FontColor selectedFontColor;

        /// <summary>
        /// 
        /// </summary>
        public ColorPickerViewModel()
        {
            this.selectedFontColor = AvailableColors.GetFontColor(Colors.Black);
            this.roFontColors = new ReadOnlyCollection<FontColor>(new AvailableColors());
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<FontColor> FontColors
        {
            get { return this.roFontColors; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontColor SelectedFontColor
        {
            get
            {
                return this.selectedFontColor;
            }

            set
            {
                if (this.selectedFontColor == value) return;

                this.selectedFontColor = value;
                OnPropertyChanged("SelectedFontColor");
            }
        }

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
