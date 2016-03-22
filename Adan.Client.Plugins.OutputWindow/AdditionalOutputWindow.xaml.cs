// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalOutputWindow.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for AdditionalOutputWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Adan.Client.Common.Messages;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Adan.Client.Plugins.OutputWindow
{

    /// <summary>
    /// Interaction logic for AdditionalOutputWindow.xaml
    /// </summary>
    public partial class AdditionalOutputWindow : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalOutputWindow"/> class.
        /// </summary>
        public AdditionalOutputWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void ChangeOutputWindow([NotNull] List<TextMessage> messages)
        {
            Assert.ArgumentNotNull(messages, "messages");

            _textBoxNative.ChangeMessageList(messages);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            _textBoxNative.Refresh();
        }
    }
}
