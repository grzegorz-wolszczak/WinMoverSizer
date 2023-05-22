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
      //if (_mainWindow.WindowState == WindowState.Minimized)
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
      //_mainWindow.labelWindowUnderMouse.Content =  $"Window under mouse handle : {state.WindowUnderMouse?.Handle}";
      //_mainWindow.labelMousePosition.Content = $"Mouse: {state.MousePositionRelativeToDesktopPosition}";
      // todo:
      // display state
   }
}
