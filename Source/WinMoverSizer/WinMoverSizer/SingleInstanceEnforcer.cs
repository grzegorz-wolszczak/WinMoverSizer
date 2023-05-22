using System.Threading;
using WinMoverSizer.Utils;

namespace WinMoverSizer;

internal static class SingleInstanceEnforcer
{
   private static Mutex? _mutex = null;

   public static void ShutdownIfAnotherApplicationInstanceIsRunning()
   {
      bool isNewInstance = false;
      _mutex = new Mutex(true, Constants.ApplicationName, out isNewInstance);
      if (!isNewInstance)
      {
         var msg = $"Another instance of this application '{Constants.ApplicationName}' is already running.";
         MessageWindow.Error(msg);
         System.Windows.Application.Current.Shutdown();
      }
   }
}
