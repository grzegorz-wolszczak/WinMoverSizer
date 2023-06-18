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

      // specific use cases
      "ZoomitClass", // window created by ZoomIt tool (https://learn.microsoft.com/en-us/sysinternals/downloads/zoomit) (it occupies entire deskttop)
   };

   public void StartMonitoring()
   {
      WinApiHelper.StartMonitoringKeyboardKeys(OnMouseOrKeyboardStateChanged);
      WinApiHelper.StartMonitoringMouseMovement(OnMouseOrKeyboardStateChanged);
   }

   private void OnMouseOrKeyboardStateChanged()
   {
      var currentState = GetCurrentMouseAndKeyboardState();
      HandleStateChange(currentState);
   }

   private void HandleStateChange(MouseAndKeyboardState currentState)
   {
      foreach (var stateObserver in _stateObservers)
      {
         stateObserver.Notify(currentState);
      }
   }


   private MouseAndKeyboardState GetCurrentMouseAndKeyboardState()
   {
      PositionOnDesktop mousePosition = WinApiHelper.GetMousePosition();
      IntPtr windowHandleUnderCursor = WinApiHelper.GetWindowFromPoint(mousePosition);
      var originalWindowUnderCursor = GetWindowFromHandle(windowHandleUnderCursor);
      var windowToOperate = FindWindowToOperateOnFromWindowOnUnderCursor(windowHandleUnderCursor);
      var keyStates = WinApiHelper.GetKeyStates();
      var windowList = WinApiHelper.GetWindowHierarchyFromPoint(mousePosition);
      var currentState = new MouseAndKeyboardState
      {
         MousePositionOnDesktop = mousePosition,
         CalculatedWindowToOperateOn = windowToOperate,
         OriginalWindowUnderCursor = originalWindowUnderCursor,
         KeyStates = keyStates,
         WindowList = windowList
      };

      return currentState;
   }

   private WindowData FindWindowToOperateOnFromWindowOnUnderCursor(IntPtr windowHandle)
   {
      if (windowHandle == IntPtr.Zero)
      {
         return WindowData.Null;
      }

      if (WinApiHelper.IsDesktopWindow(windowHandle))
      {
         return WindowData.Null;
      }

      // get root window (top) for current application
      while (ShouldContinueSearchingForParentWindow(windowHandle))
      {
         windowHandle = WinApiHelper.GetParent(windowHandle);
      }

      if (!IsWindowClassApplicableForResizeOrMove(windowHandle))
      {
         return WindowData.Null;
      }

      return GetWindowFromHandle(windowHandle);
   }

   private static WindowData GetWindowFromHandle(IntPtr windowHandle)
   {
      var rect = WinApiHelper.GetWindowRectApi(windowHandle, out var windowRectGetResult);
      if (!windowRectGetResult)
      {
         return WindowData.Null;
      }

      return new WindowData()
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
      WinApiHelper.StopMonitoringKeyboardKeys();
   }
}
