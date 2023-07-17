using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using WinMoverSizer.Models;

namespace WinMoverSizer.WinApi;

public static class WinApiHelper
{
   private const int S_OK = 0;


   private static readonly IntPtr DesktopHandle = GetDesktop();

   [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
   public static extern IntPtr GetParent(IntPtr hWnd);

   [DllImport("user32.dll")]
   private static extern short GetAsyncKeyState(int vKey);

   [DllImport("user32.dll")]
   private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

   [DllImport("user32.dll")]
   private static extern IntPtr WindowFromPoint(POINT point);

   [DllImport("user32.dll")]
   private static extern IntPtr GetShellWindow();

   [DllImport("user32.dll", CharSet = CharSet.Unicode)]
   private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

   [DllImport("user32.dll")]
   private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

   [DllImport("user32.dll")]
   private static extern bool UnhookWindowsHookEx(IntPtr hhk);

   [DllImport("user32.dll")]
   private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

   [DllImport("kernel32.dll")]
   private static extern IntPtr GetModuleHandle(string lpModuleName);

   [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
   static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


   [DllImport("user32.dll", SetLastError = true)]
   static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

   private delegate IntPtr WinApiHookCallback(int nCode, IntPtr wParam, IntPtr lParam);

   [DllImport("user32.dll")]
   private static extern IntPtr SetWindowsHookEx(int hookType, WinApiHookCallback? callback, IntPtr hMod, uint dwThreadId);

   [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
   private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

   [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
   private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

   [DllImport("user32.dll")]
   private static extern bool GetCursorPos(out POINT lpPoint);

   private static IntPtr _mouseMonitoringHookHandle = IntPtr.Zero;
   private static IntPtr _keyboardMonitoringHookHandle = IntPtr.Zero;

   private static WinApiHookCallback? _mouseMonitorinHookCallback;
   private static WinApiHookCallback? _keyboardMonitorinHookCallback;

   private static readonly List<KeyMetadata> KeysToCheckIfPressed;

   static WinApiHelper()
   {
      KeysToCheckIfPressed = new List<KeyMetadata>()
      {
         Keys.LeftMouseButton,
         Keys.RightMouseButton,
         Keys.MiddleMouseButton,
         Keys.LeftAlt,
         Keys.RightAlt,
         Keys.LeftControl,
         Keys.RightControl,
         Keys.LeftShift,
         Keys.RightShift,
      };
   }

   // from https://learn.microsoft.com/en-us/windows/win32/inputdev/mouse-input-notifications
   // hooks overview: https://learn.microsoft.com/en-us/windows/win32/winmsg/about-hooks
   private const int WH_MOUSE_LL = 14; // https://learn.microsoft.com/en-us/windows/win32/winmsg/about-hooks#wh_mouse_ll
   private const int WH_KEYBOARD_LL = 13;
   private const int WM_MOUSEMOVE = 0x200;
   private const int WM_LBUTTONDOWN = 0x0201;
   private const int WM_RBUTTONDOWN = 0x0204;
   private const int WM_RBUTTONUP = 0x0205;
   private const int WM_LBUTTONUP = 0x0202;

   private const int WM_KEYDOWN = 0x0100;
   private const int WM_KEYUP = 0x0101;
   private const int WM_SYSKEYDOWN = 0x0104;
   private const int WM_SYSKEYUP = 0x0105;


   private static IntPtr GetDesktop()
   {
      // from here: https://stackoverflow.com/questions/1669111/how-do-i-get-the-window-handle-of-the-desktop
      var shellWindow = GetShellWindow();
      var hDefView = FindWindowEx(shellWindow, IntPtr.Zero, "SHELLDLL_DefView", null);
      var folderView = FindWindowEx(hDefView, IntPtr.Zero, "SysListView32", null);
      return folderView;
   }

   [DllImport("user32.dll")]
   private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

   public static string GetClassName(IntPtr hWnd)
   {
      StringBuilder className = new StringBuilder(100);
      if (GetClassName(hWnd, className, className.Capacity) > 0)
      {
         return className.ToString();
      }

      return String.Empty;
   }

   public static void SetWindowPosition(IntPtr handle, int x, int y, int width, int height)
   {
      const uint SWP_NOZORDER = 0x0004;
      const uint SWP_SHOWWINDOW = 0x0040;
      const uint SWP_NOSIZE = 0x0001;
      // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
      SetWindowPos(handle,
         IntPtr.Zero,
         x,
         y,
         width, height, SWP_NOZORDER | SWP_SHOWWINDOW | SWP_NOSIZE);
   }

   public static void ChangeWindowSize(IntPtr handle, WindowCoordinates coordinates)
   {
      const uint SWP_NOZORDER = 0x0004;
      const uint SWP_SHOWWINDOW = 0x0040;

      // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
      SetWindowPos(handle,
         IntPtr.Zero,
         coordinates.X,
         coordinates.Y,
         coordinates.Width, coordinates.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
   }

   public static void MoveWindow(IntPtr handle, int X, int Y)
   {
      const uint SWP_NOZORDER = 0x0004;
      const uint SWP_SHOWWINDOW = 0x0040;
      const uint SWP_NOSIZE = 0x0001;
      // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
      SetWindowPos(handle, IntPtr.Zero, X, Y,
         0, 0, SWP_NOZORDER | SWP_SHOWWINDOW | SWP_NOSIZE);
   }

   public static RECT GetWindowRectApi(IntPtr windowUnderCursor, out bool windowRectGetResult)
   {
      RECT rect = new RECT();
      windowRectGetResult = GetWindowRect(windowUnderCursor, ref rect);
      return rect;
   }

   private static bool IsKeyPressed(int keyCode)
   {
      const int KEY_PRESSED = 0x8000;
      var keyState = GetAsyncKeyState(keyCode);
      return (keyState & KEY_PRESSED) != 0;
   }

   public static POINT GetMousePoint()
   {
      var mousePosition = new POINT();
      GetCursorPos(out mousePosition);
      return mousePosition;
   }


   public static IntPtr GetWindowFromPoint(POINT position)
   {
      var windowHandle = WindowFromPoint(position);
      return windowHandle;
   }

   public static void StartMonitoringKeyboardKeys(Action onKeyboardKeyPressChangedCallback)
   {
      if (_keyboardMonitoringHookHandle != IntPtr.Zero)
      {
         // do not register twice
         return;
      }

      _keyboardMonitorinHookCallback = (nCode, wParam, lParam) =>
      {
         if (nCode >= 0 /*HC_ACTION*/)
         {
            if (
               wParam == (IntPtr) WM_KEYDOWN
               || wParam == (IntPtr) WM_KEYUP
               || wParam == (IntPtr) WM_SYSKEYDOWN
               || wParam == (IntPtr) WM_SYSKEYUP
            )
            {
               if (wParam == (IntPtr) WM_KEYDOWN)
               {
                  Debug.WriteLine("WM_KEYDOWN");
               }

               if (wParam == (IntPtr) WM_KEYUP)
               {
                  Debug.WriteLine("WM_KEYUP");
               }

               onKeyboardKeyPressChangedCallback();
            }
         }


         return CallNextHookEx(_keyboardMonitoringHookHandle, nCode, wParam, lParam);
      };
      _keyboardMonitoringHookHandle = SetKeyboardMovementHook(_keyboardMonitorinHookCallback);
   }

   public static void StartMonitoringMouseMovement(Action onMouseMoveUserProvidedCallback)
   {
      if (_mouseMonitoringHookHandle != IntPtr.Zero)
      {
         // do not register twice
         return;
      }

      _mouseMonitorinHookCallback = (nCode, wParam, lParam) =>
      {
         if (nCode >= 0 /*HC_ACTION*/) // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644988(v=vs.85)#parameters
         {
            if (
               wParam == (IntPtr) WM_MOUSEMOVE ||
               wParam == (IntPtr) WM_LBUTTONDOWN ||
               wParam == (IntPtr) WM_RBUTTONDOWN ||
               wParam == (IntPtr) WM_LBUTTONUP ||
               wParam == (IntPtr) WM_RBUTTONUP
            )
            {
               onMouseMoveUserProvidedCallback();
            }
         }


         return CallNextHookEx(_mouseMonitoringHookHandle, nCode, wParam, lParam);
      };
      _mouseMonitoringHookHandle = SetMouseMovementHook(_mouseMonitorinHookCallback);
   }

   public static void StopMonitoringMouseMovement()
   {
      if (_mouseMonitoringHookHandle == IntPtr.Zero)
      {
         return;
      }

      UnhookWindowsHookEx(_mouseMonitoringHookHandle);
      _mouseMonitoringHookHandle = IntPtr.Zero;
   }

   public static void StopMonitoringKeyboardKeys()
   {
      if (_keyboardMonitoringHookHandle == IntPtr.Zero)
      {
         return;
      }

      UnhookWindowsHookEx(_keyboardMonitoringHookHandle);
      _keyboardMonitoringHookHandle = IntPtr.Zero;
   }


   private static IntPtr SetMouseMovementHook(WinApiHookCallback? callback)
   {
      using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
      using (var curModule = curProcess.MainModule)
      {
         return SetWindowsHookEx(WH_MOUSE_LL, callback, GetModuleHandle(curModule.ModuleName), 0);
      }
   }

   private static IntPtr SetKeyboardMovementHook(WinApiHookCallback? callback)
   {
      using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
      using (var curModule = curProcess.MainModule)
      {
         return SetWindowsHookEx(WH_KEYBOARD_LL, callback, GetModuleHandle(curModule.ModuleName), 0);
      }
   }

   public static bool IsHandlePointingToWholeDesktop(IntPtr windowHandle)
   {
      return windowHandle == DesktopHandle;
   }


   public static KeysPressed GetKeyStates()
   {
      List<KeyMetadata> pressedKeys = new List<KeyMetadata>();
      foreach (var keyToCheck in KeysToCheckIfPressed)
      {
         if (IsKeyPressed(keyToCheck.Code))
         {
            pressedKeys.Add(keyToCheck);
         }
      }

      var keyPressed = new KeysPressed(pressedKeys.ToArray());

      return keyPressed;
   }


   public static WindowList GetWindowHierarchyFromPoint(POINT point)
   {
      var windowHandle = GetWindowFromPoint(point);

      return GetWindowHierarchyForWindow(windowHandle);
   }

   public static WindowList GetWindowHierarchyForWindow(IntPtr windowHandle)
   {
      var list = new List<WindowInfo>();

      while (GetParent(windowHandle) != IntPtr.Zero)
      {
         var windowInfo = BuildWindowInfo(windowHandle);
         list.Add(windowInfo);
         windowHandle = GetParent(windowHandle);
      }

      list.Add(BuildWindowInfo(windowHandle)); // top parent

      return new WindowList(list);
   }

   public static WindowInfo BuildWindowInfo(IntPtr handle)
   {
      var windowText = GetWindowText(handle);
      var className = GetClassName(handle);
      var applicationName = GetApplication(handle);
      WindowStyleInfo styleInfo = GetWindowStyleInfo(handle);
      return new WindowInfo()
      {
         Handle = handle,
         WindowText = windowText,
         ClassName = className,
         ApplicationName = applicationName,
         StyleInfo = styleInfo
      };
   }

   private static string GetApplication(IntPtr hWnd)
   {
      GetWindowThreadProcessId(hWnd, out var procId);
      Process proc = Process.GetProcessById(procId);
      return proc.MainModule.ModuleName;
   }


   private static string GetWindowText(IntPtr hWnd)
   {
      StringBuilder text = new StringBuilder(256);
      if (GetWindowText(hWnd, text, text.Capacity) > 0)
      {
         return text.ToString();
      }

      return String.Empty;
   }

   // This static method is required because Win32 does not support
// GetWindowLongPtr directly
   public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
   {
      if (IntPtr.Size == 8)
         return GetWindowLongPtr64(hWnd, nIndex);
      else
         return GetWindowLongPtr32(hWnd, nIndex);
   }

   public static WindowStyleInfo GetWindowStyleInfo(IntPtr windowHandle)
   {
      IntPtr result;
      result = GetWindowLongPtr(windowHandle, (int) GetWindowLongIndex.GWL_STYLE);

      var resultAsInt64 = result.ToInt64();
      return new WindowStyleInfo()
      {
         HasCaption = (resultAsInt64 & (long) WindowStyles.WS_CAPTION) != 0,
         HasDialogFrame = (resultAsInt64 & (long) WindowStyles.WS_DLGFRAME) != 0,
         IsChild = (resultAsInt64 & (long) WindowStyles.WS_CHILD) != 0,
         IsPopupWindow = (resultAsInt64 & (long) WindowStyles.WS_POPUP) != 0
      };
   }
}
