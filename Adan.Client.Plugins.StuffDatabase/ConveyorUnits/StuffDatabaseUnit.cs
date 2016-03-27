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
    using Adan.Client.Common.Settings;
    using Adan.Client.Common.Model;

    /// <summary>
    /// <see cref="ConveyorUnit"/> implementation to handle lore messages and commands.
    /// </summary>
    public class StuffDatabaseUnit : ConveyorUnit
    {
        private string _lastShownObjectName = string.Empty;

        private static Regex _whiteSpaceRx = new Regex(@" {2,}", RegexOptions.Compiled); 

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
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand(Command command, RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }
            
            var commandText = _whiteSpaceRx.Replace(textCommand.CommandText.Trim(), " ");
            if (commandText.StartsWith(Resources.LoreHelpCommand + " ", StringComparison.CurrentCultureIgnoreCase)
                || commandText.Equals(Resources.LoreHelpCommand, StringComparison.CurrentCultureIgnoreCase))
            {
                PushMessageToConveyor(new InfoMessage(Resources.LoreHelp, TextColor.BrightYellow), rootModel);
                PushMessageToConveyor(new InfoMessage(Resources.LoreCommentsHelp, TextColor.BrightYellow), rootModel);
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
                            var serializer = new XmlSerializer(typeof(LoreMessage));
                            lore = (LoreMessage)serializer.Deserialize(inStream);
                            lore.Comments = commandText.Equals(Resources.LoreCommentCommand, StringComparison.CurrentCultureIgnoreCase)
                                                ? string.Empty
                                                : commandText.Remove(0, Resources.LoreCommentCommand.Length + 1);
                        }
                    }

                    if (lore != null)
                    {
                        SaveOrUpdateObjectLore(lore, rootModel);
                    }
                }
                else
                {
                    PushMessageToConveyor(new ErrorMessage(Resources.LoreCommentError), rootModel);
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
                            PushMessageToConveyor(new InfoMessage(Resources.LoreTooMuchFound, TextColor.BrightYellow), rootModel);
                            break;
                        }

                        using (var stream = File.OpenRead(file))
                        {
                            var serializer = new XmlSerializer(typeof(LoreMessage));
                            var message = (LoreMessage)serializer.Deserialize(stream);
                            PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.CurrentCulture, Resources.LoreFoundObject, message.ObjectName), TextColor.BrightYellow), rootModel);
                            foreach (var displayMessage in message.ConvertToMessages())
                            {
                                PushMessageToConveyor(displayMessage, rootModel);
                            }

                            _lastShownObjectName = message.ObjectName;
                        }
                    }

                    if (foundItems == 0)
                    {
                        PushMessageToConveyor(new InfoMessage(Resources.LoreNothingFound, TextColor.BrightYellow), rootModel);
                    }
                }
                else
                {
                    PushMessageToConveyor(new InfoMessage(Resources.LoreNothingFound, TextColor.BrightYellow), rootModel);
                }

                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");
            var loreMessage = message as LoreMessage;
            if (loreMessage == null)
            {
                return;
            }

            foreach (var infoMessage in loreMessage.ConvertToMessages())
            {
                PushMessageToConveyor(infoMessage, rootModel);
            }

            if (loreMessage.IsFull)
            {
                SaveOrUpdateObjectLore(loreMessage, rootModel);
            }
        }

        [NotNull]
        private static string GetStuffDbFolder()
        {
            return Path.Combine(SettingsHolder.Instance.Folder, "Stuff");
        }

        private void SaveOrUpdateObjectLore([NotNull]LoreMessage loreMessage, RootModel rootModel)
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
                    var serializer = new XmlSerializer(typeof(LoreMessage));
                    oldLore = (LoreMessage)serializer.Deserialize(inStream);
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
                    var serializer = new XmlSerializer(typeof(LoreMessage));
                    serializer.Serialize(streamWriter, loreMessage);
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
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.CurrentCulture, Resources.LoreUpdated, loreMessage.ObjectName), TextColor.BrightYellow), rootModel);
            }
            else
            {
                PushMessageToConveyor(new InfoMessage(string.Format(CultureInfo.CurrentCulture, Resources.LoreCreated, loreMessage.ObjectName), TextColor.BrightYellow), rootModel);
            }

            PushMessageToConveyor(new InfoMessage(Resources.LoreGetHelp, TextColor.BrightYellow), rootModel);
        }
    }
}
