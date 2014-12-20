// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Adan.Client.Common.Settings;
    using Adan.Client.Common.Utils;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ErrorLogger.Instance.Write(string.Format("Dispatcher unhandled exception: {0}\r\n{1}", e.Exception.Message, e.Exception.StackTrace));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ErrorLogger.Instance.Write(string.Format("Task unhandled exception: {0}\r\n{1}", e.Exception.InnerException.Message, e.Exception.InnerException.StackTrace));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            ErrorLogger.Instance.Write(string.Format("Domain unhandled exception: {0}\r\n{1}", ex.Message, ex.StackTrace));

            if (e.IsTerminating)
            {
                try
                {
                    ((MainWindow)this.MainWindow).SaveAllSettings();
                }
                catch (Exception)
                { }
            }
        }
    }
}
