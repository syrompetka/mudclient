// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StuffDatabaseUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StuffDatabaseUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.ConveyorUnits
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;
    using Adan.Client.Common;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Properties;
    using Adan.Client.Plugins.StuffDatabase.Messages;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle lore messages and commands.
    /// </summary>
    public class StuffDatabaseUnit : ConveyorUnit
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(LoreMessage));
        private string _lastShownObjectName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StuffDatabaseUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public StuffDatabaseUnit([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Repeat(Constants.LoreMessageType, 1);
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Repeat(BuiltInCommandTypes.TextCommand, 1);
            }
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = Regex.Replace(textCommand.CommandText.Trim(), @"\s+", " ");
            if (commandText.StartsWith(Resources.LoreHelpCommand + " ", StringComparison.CurrentCultureIgnoreCase)
                || commandText.Equals(Resources.LoreHelpCommand, StringComparison.CurrentCultureIgnoreCase))
            {
                PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(Resources.LoreHelp, TextColor.BrightYellow) }));
                PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(Resources.LoreCommentsHelp, TextColor.BrightYellow) }));
                command.Handled = true;
                return;
            }

            if (commandText.StartsWith(Resources.LoreCommentCommand + " ", StringComparison.CurrentCultureIgnoreCase)
                || commandText.Equals(Resources.LoreCommentCommand, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_lastShownObjectName))
                {
                    var fileName = Path.Combine(GetStuffDbFolder(), _lastShownObjectName.Replace(" ", "_"));
                    LoreMessage lore = null;
                    if (File.Exists(fileName))
                    {
                        using (var inStream = File.OpenRead(fileName))
                        {
                            lore = (LoreMessage)_serializer.Deserialize(inStream);
                            lore.Comments = commandText.Equals(Resources.LoreCommentCommand, StringComparison.CurrentCultureIgnoreCase)
                                                ? string.Empty
                                                : commandText.Remove(0, Resources.LoreCommentCommand.Length + 1);
                        }
                    }

                    if (lore != null)
                    {
                        SaveOrUpdateObjectLore(lore);
                    }
                }
                else
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.LoreCommentError));
                }

                command.Handled = true;
                return;
            }

            if (commandText.StartsWith(Resources.LoreCommand + " ", StringComparison.CurrentCultureIgnoreCase)
                || commandText.Equals(Resources.LoreCommand, StringComparison.CurrentCultureIgnoreCase))
            {
                command.Handled = true;
                var searchQuery = commandText.Equals(Resources.LoreCommand, StringComparison.CurrentCultureIgnoreCase)
                                      ? string.Empty
                                      : commandText.Remove(0, Resources.LoreCommand.Length + 1).Trim().Replace(" ", "_").Replace("\"", string.Empty);
                const int maxDisplayItems = 4;
                int foundItems = 0;
                if (Directory.Exists(GetStuffDbFolder()))
                {
                    foreach (var file in Directory.GetFiles(GetStuffDbFolder()))
                    {
                        if (file.IndexOf(searchQuery, StringComparison.CurrentCultureIgnoreCase) < 0)
                        {
                            continue;
                        }

                        foundItems++;
                        if (foundItems > maxDisplayItems)
                        {
                            PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(Resources.LoreTooMuchFound, TextColor.BrightYellow) }));
                            break;
                        }

                        using (var stream = File.OpenRead(file))
                        {
                            var message = (LoreMessage)_serializer.Deserialize(stream);
                            PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(string.Format(CultureInfo.CurrentCulture, Resources.LoreFoundObject, message.ObjectName), TextColor.BrightYellow) }));
                            foreach (var displayMessage in message.ConvertToMessages())
                            {
                                PushMessageToConveyor(displayMessage);
                            }

                            _lastShownObjectName = message.ObjectName;
                        }
                    }

                    if (foundItems == 0)
                    {
                        PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(Resources.LoreNothingFound, TextColor.BrightYellow) }));
                    }
                }

                return;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            var loreMessage = message as LoreMessage;
            if (loreMessage == null)
            {
                return;
            }

            foreach (var infoMessage in loreMessage.ConvertToMessages())
            {
                PushMessageToConveyor(infoMessage);
            }

            if (loreMessage.IsFull)
            {
                SaveOrUpdateObjectLore(loreMessage);
            }
        }

        [NotNull]
        private static string GetStuffDbFolder()
        {
            return Path.Combine(ProfileHolder.Instance.Folder, "Stuff");
        }

        private void SaveOrUpdateObjectLore([NotNull]LoreMessage loreMessage)
        {
            Assert.ArgumentNotNull(loreMessage, "loreMessage");

            if (!Directory.Exists(GetStuffDbFolder()))
            {
                Directory.CreateDirectory(GetStuffDbFolder());
            }

            var fileName = Path.Combine(GetStuffDbFolder(), loreMessage.ObjectName.Replace(" ", "_").Replace("\"", string.Empty));
            bool isUpdated = false;
            if (File.Exists(fileName))
            {
                LoreMessage oldLore;
                using (var inStream = File.OpenRead(fileName))
                {
                    oldLore = (LoreMessage)_serializer.Deserialize(inStream);
                    if (!string.IsNullOrEmpty(oldLore.Comments) && string.IsNullOrEmpty(loreMessage.Comments))
                    {
                        loreMessage.Comments = oldLore.Comments;
                    }

                    isUpdated = true;
                }
            }

            FileStream stream = null;
            try
            {
                stream = File.Open(fileName, FileMode.Create, FileAccess.Write);
                using (var streamWriter = new XmlTextWriter(stream, Encoding.Unicode))
                {
                    stream = null;
                    streamWriter.Formatting = Formatting.Indented;
                    _serializer.Serialize(streamWriter, loreMessage);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            _lastShownObjectName = loreMessage.ObjectName;
            if (isUpdated)
            {
                PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(string.Format(CultureInfo.CurrentCulture, Resources.LoreUpdated, loreMessage.ObjectName), TextColor.BrightYellow) }));
            }
            else
            {
                PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(string.Format(CultureInfo.CurrentCulture, Resources.LoreCreated, loreMessage.ObjectName), TextColor.BrightYellow) }));
            }

            PushMessageToConveyor(new InfoMessage(new[] { new TextMessageBlock(Resources.LoreGetHelp, TextColor.BrightYellow) }));
        }
    }
}
