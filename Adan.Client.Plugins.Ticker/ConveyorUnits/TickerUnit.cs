namespace Adan.Client.Plugins.Ticker.ConveyorUnits
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
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Themes;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Common.Settings;
    using System.Threading;

    public class TickerUnit : ConveyorUnit
    {
        private static Regex _whiteSpaceRx = new Regex(@" {2,}", RegexOptions.Compiled);
        private static string _tickMessage = "#TICK";
        Timer Timer;
        bool isRunning;
        int Period = 60000;
        DateTime shouldRunNext;

        public TickerUnit(MessageConveyor conveyor) : base(conveyor)
        {
            Timer = new Timer(PrintTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void PrintTick(Object state)
        {
            TimeSpan skew = DateTime.Now - shouldRunNext;
            shouldRunNext = DateTime.Now.AddMilliseconds(Period) - skew;

            // Do stuff here.
            var message = new OutputToMainWindowMessage(_tickMessage, TextColor.BrightWhite);
            PushMessageToConveyor(message);
            //

            // Adjust the timer, because each time it runs a few ms later than it should have.
            TimeSpan diff = shouldRunNext - DateTime.Now;
            Timer.Change(diff, new TimeSpan(-1));
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Empty<int>();
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

        // When the player types "#tick", we start/stop ticking.
        public override void HandleCommand(Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");
            
            if (isImport)
                return;

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = _whiteSpaceRx.Replace(textCommand.CommandText.Trim(), " ");
            
            if(commandText.StartsWith("#tickset"))
            {
                if (isRunning)
                {
                    Timer.Change(Period, Timeout.Infinite);
                }
            }
            else if (commandText.StartsWith("#tick"))
            {
                if (isRunning)
                {
                    Timer.Change(Timeout.Infinite, Timeout.Infinite);
                    PushMessageToConveyor(new InfoMessage("#STOPPED TICKING", TextColor.BrightWhite));
                }
                else
                {
                    Timer.Change(Period, Timeout.Infinite);
                    shouldRunNext = DateTime.Now.AddMilliseconds(Period);

                    PushMessageToConveyor(new InfoMessage("#STARTED TICKING", TextColor.BrightWhite));
                }

                isRunning = !isRunning;

                return;
            }
        }
    }
}
