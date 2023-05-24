using System.Windows;
using WinMoverSizer.CoreApplication;
using WinMoverSizer.Models;
using WinMoverSizer.Ports;

namespace WinMoverSizer.Adapters;

public class MainWindowProxy : IMainWindowProxy, IWinMoverStateObserver
{
   private readonly MainWindow _mainWindow;

   public MainWindowProxy(MainWindow mainWindow)
   {
      _mainWindow = mainWindow;
   }

   public void ShowWindow()
   {
      _mainWindow.WindowState = WindowState.Normal;
      _mainWindow.Activate();
      _mainWindow.Show();
   }

   public bool IsWindowVisible()
   {
      return _mainWindow.IsVisible;
   }

   public void HideWindow()
   {
      _mainWindow.Hide();
   }

   public void Notify(WinDraggerState state)
   {
      _mainWindow.UpdateControlsFromState(state);
   }
}
