using System;
using System.Windows;
using System.Windows.Threading;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            this.HandleException(sender, e, e.StackTrace);
        }

        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            Exception e = args.Exception;
            this.HandleException(sender, e, e.StackTrace);
        }
        
        private void HandleException(object sender, Exception e, string AdditionalText)
        {
            string s = "Source: " + sender.ToString() + Environment.NewLine + Environment.NewLine + "Exception: " + e.Message + Environment.NewLine;
            if (!string.IsNullOrEmpty(AdditionalText)) { s = s + Environment.NewLine + AdditionalText; }
            MessageBox.Show(s, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (Window win in this.Windows)
            {
                win.Close();
            }
        }
    }
}
