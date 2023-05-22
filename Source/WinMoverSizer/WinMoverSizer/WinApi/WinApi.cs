using System;
using System.Runtime.InteropServices;
using System.Text;
using WinMoverSizer.Models;

namespace WinMoverSizer.WinApi;

public static class WinApiHelper
{
   private const int S_OK = 0;
   private const int KEY_PRESSED = 0x8000;


   private static readonly IntPtr DesktopHandle = GetDesktop();

   [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
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

   public delegate IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam);

   [DllImport("user32.dll")]
   private static extern IntPtr SetWindowsHookEx(int hookType, MouseHookCallback callback, IntPtr hMod, uint dwThreadId);

   [DllImport("user32.dll")]
   private static extern bool GetCursorPos(out POINT lpPoint);

   private static IntPtr _hookHandle = IntPtr.Zero;
   private static MouseHookCallback _hookCallback;
   // from https://learn.microsoft.com/en-us/windows/win32/inputdev/mouse-input-notifications
   private const int WH_MOUSE_LL = 14;
   private const int WM_MOUSEMOVE = 0x200;
   private const int WM_MBUTTONDOWN  = 0x0207;
   private const int WM_RBUTTONDOWN = 0x0204;
   private const int WM_LBUTTONDOWN = 0x0201;

   private static IntPtr GetDesktop()
   {
      // from here: https://stackoverflow.com/questions/1669111/how-do-i-get-the-window-handle-of-the-desktop
      //var bareDesktopWindow = GetDesktopWindow();
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
      SetWindowPos(handle,
         IntPtr.Zero,
         x,
         y,
         width, height, SWP_NOZORDER | SWP_SHOWWINDOW);
   }

   public static void SetWindowPosition(IntPtr handle, WindowCoordinates coordinates)
   {
      SetWindowPosition(handle, coordinates.X, coordinates.Y, coordinates.Width, coordinates.Height);
   }

   public static RECT GetWindowRectApi(IntPtr windowUnderCursor, out bool windowRectGetResult)
   {
      RECT rect = new RECT();
      windowRectGetResult = GetWindowRect(windowUnderCursor, ref rect);
      return rect;
   }

   public static bool IsAltPressed()
   {
      const int VK_MENU = 0x12; // Alt key
      int altKeyState = GetAsyncKeyState(VK_MENU);
      return (altKeyState & KEY_PRESSED) != 0;
   }

   public static bool IsLeftMouseButtonPressed()
   {
      const int VK_LBUTTON = 0x01; // right mouse button
      int rightMouseButtonState = GetAsyncKeyState(VK_LBUTTON);
      return (rightMouseButtonState & KEY_PRESSED) != 0;
   }

   public static bool IsRightMouseButtonPressed()
   {
      const int VK_RBUTTON = 0x02; // left mouse button
      int rightMouseButtonState = GetAsyncKeyState(VK_RBUTTON);
      return (rightMouseButtonState & KEY_PRESSED) != 0;
   }

   public static IntPtr GetWindowFromPoint(PositionOnDesktop position)
   {
      POINT mousePosition = new POINT();
      mousePosition.x = (int) position.X;
      mousePosition.y = (int) position.Y;
      var windowHandle = WindowFromPoint(mousePosition);
      return windowHandle;
      // if (windowHandle == IntPtr.Zero)
      // {
      //    return new WindowUnderMouse();
      // }
      //
      // var rect = GetWindowRectApi(windowHandle, out var windowRectGetResult);
      // if (!windowRectGetResult)
      // {
      //    return new WindowUnderMouse();
      // }
      //
      // return new WindowUnderMouse()
      // {
      //    Handle = windowHandle,
      //    Rect = rect
      // };
   }

   public static void StartMonitoringMouseMovement(Action<PositionOnDesktop> callback)
   {
      if (_hookHandle != IntPtr.Zero)
      {
         // do not register twice
         return;
      }

      _hookCallback = (nCode, wParam, lParam) =>
      {
         if (nCode >= 0)
         {
            if (wParam == (IntPtr) WM_MOUSEMOVE)
            {
               var mousePosition = new POINT();
               GetCursorPos(out mousePosition);
               callback(new PositionOnDesktop()
               {
                  X = mousePosition.x,
                  Y = mousePosition.y
               });
            }

            if (wParam == (IntPtr) WM_MBUTTONDOWN)
            {
               Console.WriteLine("Middle button down");
            }

            if (wParam == (IntPtr) WM_RBUTTONDOWN)
            {
               Console.WriteLine("Right button down");
            }

            if (wParam == (IntPtr) WM_LBUTTONDOWN)
            {
               Console.WriteLine("Left button down");
            }
         }


         return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
      };
      _hookHandle = SetHook(_hookCallback);
   }

   public static void StopMonitoringMouseMovement()
   {
      if (_hookHandle == IntPtr.Zero)
      {
         return;
      }
      UnhookWindowsHookEx(_hookHandle);
      _hookHandle = IntPtr.Zero;
   }


   private static IntPtr SetHook(MouseHookCallback callback)
   {
      using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
      using (var curModule = curProcess.MainModule)
      {
         return SetWindowsHookEx(WH_MOUSE_LL, callback, GetModuleHandle(curModule.ModuleName), 0);
      }
   }

   public static bool IsDesktopWindow(IntPtr windowHandle)
   {
      return windowHandle == DesktopHandle;
   }


}
