using System;
using WinMoverSizer.Adapters;
using WinMoverSizer.Models;
using WinMoverSizer.Tray;

namespace WinMoverSizer.CoreApplication;

public interface IWinMoverStateObserver
{
   public void Notify(WinDraggerState state);
}

public class ApplicationLogicRoot
{
   private readonly TrayIconProxy _trayIcon;
   private MainWindow _mainWindow;
   private MoverSizerLogic _moverSizer;

   public ApplicationLogicRoot(App app)
   {
      var applicationProxy = new ApplicationProxy(app);

      _mainWindow = new MainWindow();

      var mainWindowProxy = new MainWindowProxy(_mainWindow);

      _trayIcon = new TrayIconProxy(
         mainWindowProxy,
         applicationProxy,
         Constants.ApplicationIcon,
         Constants.ApplicationName);

      var windowResizer = new WindowResizer();
      var windowMover = new WindowMover();

      _moverSizer = new MoverSizerLogic();
      _moverSizer.RegisterStateObserver(mainWindowProxy);
      _moverSizer.RegisterStateObserver(windowResizer);
      _moverSizer.RegisterStateObserver(windowMover);
   }

   public void Start()
   {
      _mainWindow.InitializeComponent();
      ShowMainWindow();
      _moverSizer.StartMonitoring();
   }

   private void ShowMainWindow()
   {
      _mainWindow.Title = Constants.ApplicationName;
      _mainWindow.Show();
   }

   public void Stop()
   {
      _moverSizer.StopMonitoring();
      _trayIcon.Cleanup();
   }
}
