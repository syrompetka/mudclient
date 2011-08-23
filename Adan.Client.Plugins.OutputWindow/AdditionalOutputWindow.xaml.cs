// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalOutputWindow.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for AdditionalOutputWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for AdditionalOutputWindow.xaml
    /// </summary>
    public partial class AdditionalOutputWindow
    {
        private readonly Queue<OutputToAdditionalWindowMessage> _messageQueue = new Queue<OutputToAdditionalWindowMessage>();
        private readonly object _messageQueueLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalOutputWindow"/> class.
        /// </summary>
        public AdditionalOutputWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the message to output window.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void AddMessage([NotNull] OutputToAdditionalWindowMessage message)
        {
            Assert.ArgumentNotNull(message, "message");

            lock (_messageQueueLockObject)
            {
                _messageQueue.Enqueue(message);
            }

            Dispatcher.BeginInvoke((Action)ProcessMessageQueue);
        }

        private void ProcessMessageQueue()
        {
            IList<OutputToAdditionalWindowMessage> messages;
            lock (_messageQueueLockObject)
            {
                messages = _messageQueue.ToList();
                _messageQueue.Clear();
            }

            if (messages.Count > 0)
            {
                additionalOutputWindow.AddMessages(messages);
            }
        }
    }
}
