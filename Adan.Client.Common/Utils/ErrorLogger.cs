using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Adan.Client.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorLogger : IDisposable
    {
        private static ErrorLogger _instance;

        private StreamWriter _writer;

        /// <summary>
        /// 
        /// </summary>
        public ErrorLogger()
        {
            try
            {
                _writer = new StreamWriter(Path.Combine(Settings.SettingsHolder.Instance.Folder, "error.zzz"), true);
                _writer.AutoFlush = true;
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ErrorLogger Instance
        {
            get
            {
                return _instance ?? (_instance = new ErrorLogger());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            if (_writer != null)
            {
                try
                {
                    _writer.WriteLine(message);
                }
                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Close();
                }
                catch (Exception)
                { }
            }
        }
    }
}
