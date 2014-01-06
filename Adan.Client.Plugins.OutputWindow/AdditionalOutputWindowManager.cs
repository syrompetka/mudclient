using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Adan.Client.Common.Model;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils;
using Adan.Client.Common.Controls.AvalonEdit;
using Adan.Client.Plugins.OutputWindow.Messages;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using ICSharpCode.AvalonEdit.Document;
using System.Windows;

namespace Adan.Client.Plugins.OutputWindow
{
    /// <summary>
    /// 
    /// </summary>
    public class AdditionalOutputWindowManager
    {
        private AdditionalOutputWindow _window;
        private Dictionary<string, TextDocument> _additionalOutputWindows;
        private string CurrentUid = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public AdditionalOutputWindowManager(AdditionalOutputWindow window)
        {
            _window = window;
            _additionalOutputWindows = new Dictionary<string, TextDocument>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void OutputWindowCreated([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _additionalOutputWindows.Add(rootModel.Uid, new TextDocument());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void OutputWindowClosed([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            if (_additionalOutputWindows.ContainsKey(rootModel.Uid))
                _additionalOutputWindows.Remove(rootModel.Uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public void OutputWindowChanged([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            TextDocument document;

            if (_additionalOutputWindows.TryGetValue(rootModel.Uid, out document))
            {
                _window.ChangeOutputWindow(document);
                CurrentUid = rootModel.Uid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="message"></param>
        public void AddText(RootModel rootModel, OutputToAdditionalWindowMessage message)
        {
            TextDocument document;

            if (_additionalOutputWindows.TryGetValue(rootModel.Uid, out document))
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        document.Insert(document.TextLength, "\r\n");
                        document.Insert(document.TextLength, message.ColoredText);
                        _window.additionalOutputWindow.ScrollToEnd();
                    }), DispatcherPriority.Background);
            }
        }
    }
}