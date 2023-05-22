using System;
using System.Drawing;
using System.Windows.Forms;
using WinMoverSizer.CoreApplication;
using WinMoverSizer.Ports;

namespace WinMoverSizer.Tray;

public class TrayIconProxy
{
   private IMainWindowProxy _mainWindowProxy;
   private readonly IApplicationProxy _applicationProxy;
   private ContextMenuStrip _menu;
   private NotifyIcon _notifyIcon;

   public TrayIconProxy(
      IMainWindowProxy mainWindowProxy,
      IApplicationProxy applicationProxy,
      Icon notifyIcon,
      string applicationName)
   {
      _mainWindowProxy = mainWindowProxy;
      _applicationProxy = applicationProxy;
      /* */
      _notifyIcon = new NotifyIcon();
      _notifyIcon.Icon = notifyIcon;
      _notifyIcon.Text = applicationName;
      _notifyIcon.Visible = true;


      _notifyIcon.DoubleClick += TrayDoubleClickHandler;

      /* menu */
      // todo: BUG: prevent tooltip to jump to other screen
      // some solutions:  https://stackoverflow.com/questions/26587843/prevent-toolstripmenuitems-from-jumping-to-second-screen
      _menu = new ContextMenuStrip();
      _menu.Items.Add(new ToolStripMenuItem("Show", null, ShowMainWindow!, "Show"));
      _menu.Items.Add(new ToolStripMenuItem("Exit", null, ApplicationCloseClickHandler!, "Exit"));
      _notifyIcon.ContextMenuStrip = _menu;

   }

   private void ShowMainWindow(object sender, EventArgs eventArgs)
   {
      _mainWindowProxy.ShowWindow();
   }

   private void ApplicationCloseClickHandler(object sender, EventArgs e)
   {
      _applicationProxy.Exit();
   }

   private void TrayDoubleClickHandler(object sender, EventArgs e)
   {
      if (_mainWindowProxy.IsWindowVisible())
      {
         _mainWindowProxy.HideWindow();
      }
      else
      {
         _mainWindowProxy.ShowWindow();
      }
   }


   public void Cleanup()
   {
      _notifyIcon.Visible = false;
      _menu.Dispose();
      _notifyIcon.Dispose();
   }
}
