using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Themes;

namespace Adan.Client.Resources.AvalonDock
{
    public class AvalonDockDarkTheme:Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/Adan.Client;component/Resources/AvalonDock/DarkTheme.xaml",
                UriKind.Relative);
        }
    }
}
