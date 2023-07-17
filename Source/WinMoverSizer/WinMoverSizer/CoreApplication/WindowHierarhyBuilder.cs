using System;
using System.Collections.Generic;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class WindowHierarchyBuilder
{
   private IntPtr _previousWindowHandle = IntPtr.Zero;
   private WindowList _previousWindowList = null;
   public WindowList GetWindowHierarchyFromPoint(POINT point)
   {
      var windowFromPoint = WinApiHelper.GetWindowFromPoint(point);
      return GetWindowHierarchyForWindow(windowFromPoint);
   }

   public WindowList GetWindowHierarchyForWindow(IntPtr windowFromPoint)
   {
      if (_previousWindowHandle == windowFromPoint)
      {
         return _previousWindowList;
      }

      var windowHandle = windowFromPoint;
      var windowList = new List<WindowInfo>();


      while (WinApiHelper.GetParent(windowHandle) != IntPtr.Zero)
      {
         var windowInfo = WinApiHelper.BuildWindowInfo(windowHandle);
         windowList.Add(windowInfo);
         windowHandle = WinApiHelper.GetParent(windowHandle);
      }

      windowList.Add(WinApiHelper.BuildWindowInfo(windowHandle)); // top parent

      var result = new WindowList(windowList);
      _previousWindowHandle = windowFromPoint;
      _previousWindowList = result;
      return result;
   }
}