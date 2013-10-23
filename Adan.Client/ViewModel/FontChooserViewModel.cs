using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using Adan.Client.Common.ViewModel;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FontChooserViewModel : ViewModelBase
    {
        private ICollection<FontFamily> _familyCollection;          // see FamilyCollection property

        /// <summary>
        /// Collection of font families to display in the font family list. By default this is Fonts.SystemFontFamilies,
        /// but a client could set this to another collection returned by Fonts.GetFontFamilies, e.g., a collection of
        /// application-defined fonts.
        /// </summary>
        public ICollection<FontFamily> FontFamilyCollection
        {
            get
            {
                return (_familyCollection == null) ? System.Windows.Media.Fonts.SystemFontFamilies : _familyCollection;
            }

            set
            {
                if (value != _familyCollection)
                {
                    _familyCollection = value;
                    OnPropertyChanged("FontFamilyCollection");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Typeface> TypeFaceCollection
        {
            get;
            set;
        }
    }
}
