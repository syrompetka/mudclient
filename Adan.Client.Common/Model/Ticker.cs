using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Adan.Client.Common.Model
{
    /// <summary>
    /// Ticker
    /// </summary>
    public class Ticker
    {
        private static Ticker _instance = new Ticker();
        private Timer timer = new Timer();

        /// <summary>
        /// Get inctanse of ticker
        /// </summary>
        public static Ticker Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Enable ticker
        /// </summary>
        public void Enable()
        {
            timer.Start();
        }

        /// <summary>
        /// Disable ticker
        /// </summary>
        public void Disable()
        {
            timer.Stop();
        }
    }
}
