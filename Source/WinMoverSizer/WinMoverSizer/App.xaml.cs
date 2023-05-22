using System.Windows;
using System.Windows.Threading;
using WinMoverSizer.CoreApplication;

namespace WinMoverSizer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
   private ApplicationLogicRoot _applicationLogicRoot;
   private void App_OnStartup(object sender, StartupEventArgs e)
   {
      SingleInstanceEnforcer.ShutdownIfAnotherApplicationInstanceIsRunning();
      CreateApplicationLogicRoot();
      _applicationLogicRoot.Start();
   }


   private void CreateApplicationLogicRoot()
   {

      _applicationLogicRoot = new ApplicationLogicRoot(this);
   }

   private void App_OnExit(object sender, ExitEventArgs e)
   {
      _applicationLogicRoot.Stop();
   }

   private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
   {
      e.Handled = true;
      UserInteractions.ShowExceptionDialog($"Unhandled exception during application execution", e.Exception);
      Current?.Shutdown();
   }
}