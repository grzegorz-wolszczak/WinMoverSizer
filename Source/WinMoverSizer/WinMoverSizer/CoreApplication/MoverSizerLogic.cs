using System;
using System.Collections.Generic;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class MoverSizerLogic
{
   private List<IWinMoverStateObserver> _stateObservers = new();

   private static readonly HashSet<string> ClassNamesForWindowsThatShouldNotBeMovedOrResized = new()
   {
      "Shell_TrayWnd", // elements on ms windows task bar and in the tray
      "Shell_SecondaryTrayWnd", // task bar on other desktops (not in primary)
   };

   public void StartMonitoring()
   {
      WinApiHelper.StartMonitoringMouseMovement(OnMouseMovementChanged);
   }

   private void OnMouseMovementChanged(PositionOnDesktop pointToScreen)
   {
      var currentState = new WinDraggerState();
      currentState.IsKeyboardShortcutForResizePressed = IsKeyboardShortcutForResizePressed();
      currentState.IsKeyboardShortcutForMovementPressed = IsKeyboardShortcutForMovementPressed();
      currentState.MousePositionOnDesktop = pointToScreen;

      IntPtr windowUnderCursor = WinApiHelper.GetWindowFromPoint(pointToScreen);
      WindowUnderMouse windowToOperate = FindWindowToOperateOnFromWindowOnUnderCursor(windowUnderCursor);

      currentState.WindowUnderMouse = windowToOperate;

      foreach (var stateObserver in _stateObservers)
      {
         stateObserver.Notify(currentState);
      }
   }

   private WindowUnderMouse FindWindowToOperateOnFromWindowOnUnderCursor(IntPtr windowHandle)
   {
      if (windowHandle == IntPtr.Zero)
      {
         return WindowUnderMouse.Null;
      }

      if (WinApiHelper.IsDesktopWindow(windowHandle))
      {
         return WindowUnderMouse.Null;
      }

      // get root window (top) for current application
      while (ShouldContinueSearchingForParentWindow(windowHandle))
      {
         windowHandle = WinApiHelper.GetParent(windowHandle);
      }

      if (!IsWindowClassApplicableForResizeOrMove(windowHandle))
      {
         return WindowUnderMouse.Null;
      }

      var rect = WinApiHelper.GetWindowRectApi(windowHandle, out var windowRectGetResult);
      if (!windowRectGetResult)
      {
         return WindowUnderMouse.Null;
      }

      return new WindowUnderMouse()
      {
         Handle = windowHandle,
         Rect = rect
      };
   }

   private bool IsWindowClassApplicableForResizeOrMove(IntPtr windowHandle)
   {
      var className = WinApiHelper.GetClassName(windowHandle);
      return !ClassNamesForWindowsThatShouldNotBeMovedOrResized.Contains(className);
   }

   private bool ShouldContinueSearchingForParentWindow(IntPtr windowHandle)
   {
      var className = WinApiHelper.GetClassName(windowHandle);
      var classNamesForDialogs = new HashSet<string>()
      {
         "#32770", // '#32770' means - Ms Windows dialog window
         "SunAwtDialog", // AWT java dialog window (e.g. Jetbrains Rider find files window)
         "SunAwtWindow" // AWT java window (e.g. Jetbrains Rider project manager window)
      };
      if (classNamesForDialogs.Contains(className))
      {
         return false;
      }

      return WinApiHelper.GetParent(windowHandle) != IntPtr.Zero;
   }

   public void RegisterStateObserver(IWinMoverStateObserver stateObserver)
   {
      if (!_stateObservers.Contains(stateObserver))
      {
         _stateObservers.Add(stateObserver);
      }
   }

   public void StopMonitoring()
   {
      WinApiHelper.StopMonitoringMouseMovement();
   }

   private bool IsKeyboardShortcutForMovementPressed()
   {
      var isAltPressed = WinApiHelper.IsAltPressed();
      var isLeftMouseButtonPressed = WinApiHelper.IsLeftMouseButtonPressed();

      return isAltPressed
             && isLeftMouseButtonPressed;
   }

   private bool IsKeyboardShortcutForResizePressed()
   {
      return WinApiHelper.IsAltPressed() && WinApiHelper.IsRightMouseButtonPressed();
   }
}
