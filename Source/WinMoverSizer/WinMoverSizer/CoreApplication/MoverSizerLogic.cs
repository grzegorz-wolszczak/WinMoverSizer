using System;
using System.Collections.Generic;
using System.Diagnostics;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public sealed class MoverSizerLogic
{
   private List<IWinMoverStateObserver> _stateObservers = new();
   private readonly WindowHierarchyBuilder _windowHierarchyBuilder;

   private static readonly HashSet<string> ClassNamesForWindowsThatShouldNotBeMovedOrResized = new()
   {
      "Shell_TrayWnd", // elements on ms windows task bar and in the tray
      "Shell_SecondaryTrayWnd", // task bar on other desktops (not in primary)

      // specific use cases
      "ZoomitClass", // window created by ZoomIt tool (https://learn.microsoft.com/en-us/sysinternals/downloads/zoomit) (it occupies entire deskttop)
   };

   private static readonly HashSet<string> ClassNamesForJavaDialogsWindows = new()
   {
      "SunAwtDialog", // AWT java dialog window (e.g. Jetbrains Rider find files window)
      "SunAwtWindow" // AWT java window (e.g. Jetbrains Rider project manager window)
   };

   public MoverSizerLogic()
   {
      _windowHierarchyBuilder = new WindowHierarchyBuilder();
   }

   public void Start()
   {
      WinApiHelper.StartMonitoringKeyboardKeys(OnMouseOrKeyboardStateChanged);
      WinApiHelper.StartMonitoringMouseMovement(OnMouseOrKeyboardStateChanged);
   }

   private void OnMouseOrKeyboardStateChanged()
   {
      try
      {
         var currentState = GetCurrentMouseAndKeyboardState();
         HandleStateChange(currentState);
      }
      catch (Exception e)
      {
         Debug.WriteLine(e);
         //UserInteractions.ShowExceptionDialog("Error While handling Mouse/Keyboard State changed event",e);
      }

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
      var mousePosition = WinApiHelper.GetMousePoint();
      IntPtr windowHandleUnderCursor = WinApiHelper.GetWindowFromPoint(mousePosition);
      var originalWindowUnderCursor = GetWindowFromHandle(windowHandleUnderCursor);
      var windowToOperate = FindWindowToOperateOnFromWindowOnUnderCursor(windowHandleUnderCursor);
      var keyStates = WinApiHelper.GetKeyStates();

      var windowList = _windowHierarchyBuilder.GetWindowHierarchyForWindow(windowHandleUnderCursor);

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

      if (WinApiHelper.IsHandlePointingToWholeDesktop(windowHandle))
      {
         return WindowData.Null;
      }

      var windowFromHandle = GetWindowFromHandle(windowHandle);
      // get root window (top) for current application
      while (ShouldContinueSearchingForParentWindow(windowFromHandle))
      {
         windowFromHandle = GetWindowFromHandle(windowFromHandle.ParentHandle);
      }

      if (!IsWindowClassApplicableForResizeOrMove(windowFromHandle.ClassName))
      {
         return WindowData.Null;
      }

      return windowFromHandle;
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
         Rect = rect,
         ParentHandle = WinApiHelper.GetParent(windowHandle),
         ClassName = WinApiHelper.GetClassName(windowHandle),
         WindowStyleInfo = WinApiHelper.GetWindowStyleInfo(windowHandle)
      };
   }

   private bool IsWindowClassApplicableForResizeOrMove(string className)
   {
      return !ClassNamesForWindowsThatShouldNotBeMovedOrResized.Contains(className);
   }

   private bool ShouldContinueSearchingForParentWindow(WindowData windowHandle)
   {
      // even though MS Windows Dialogs windows have
      // "#32770" class name, we detect it differently, by checking window style
      // this is why some windows with "#32770"  are actually 'inside' other type of windows and thus should not be considered for moving
      // eg. MS outlook message preview window has this classname but is 'embedded' inside main outlook window
      if (ClassNamesForJavaDialogsWindows.Contains(windowHandle.ClassName))
      {
         return false;
      }

      if (windowHandle.WindowStyleInfo.HasCaption
          || windowHandle.WindowStyleInfo.HasDialogFrame)
      {
         return false;
      }

      return windowHandle.ParentHandle != IntPtr.Zero;
   }

   public void RegisterStateObserver(IWinMoverStateObserver stateObserver)
   {
      if (!_stateObservers.Contains(stateObserver))
      {
         _stateObservers.Add(stateObserver);
      }
   }

   public void Stop()
   {
      WinApiHelper.StopMonitoringMouseMovement();
      WinApiHelper.StopMonitoringKeyboardKeys();
   }
}
