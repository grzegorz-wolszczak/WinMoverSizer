using System;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class WindowResizer : IWinMoverStateObserver
{
   private WinDraggerState? _previousState;
   public void Notify(WinDraggerState state)
   {
      if (_previousState == null)
      {
         _previousState = state;
         return;
      }

      ActOnCurrentState(state);
      _previousState = state;
   }

   private void ActOnCurrentState(WinDraggerState currentState)
   {
      var shouldResizeWindow = ShouldResizeWindow(currentState);
      if (shouldResizeWindow)
      {
         ResizeWindow(currentState);
      }
   }

   private bool ShouldResizeWindow(WinDraggerState currentState)
   {
      var isTheSameWindowStillUnderTheCursor = IsTheSameWindowStillUnderTheCursor(currentState);
      var didCursorMoved = DidCursorMoved(currentState);
      var wasKeyboardShortcutForResizeStillPressed = WasKeyboardShortcutForResizeStillPressed(currentState);


      return isTheSameWindowStillUnderTheCursor
             && didCursorMoved &&
             wasKeyboardShortcutForResizeStillPressed;
   }

   private bool WasKeyboardShortcutForResizeStillPressed(WinDraggerState currentState)
   {
      return currentState.IsKeyboardShortcutForResizePressed && _previousState.IsKeyboardShortcutForResizePressed;
   }

   private bool DidCursorMoved(WinDraggerState currentState)
   {
      return _previousState?.MousePositionOnDesktop != currentState.MousePositionOnDesktop;
   }

   private bool IsTheSameWindowStillUnderTheCursor(WinDraggerState currentState)
   {
      return _previousState?.WindowUnderMouse?.Handle != IntPtr.Zero
             && _previousState?.WindowUnderMouse.Handle == currentState.WindowUnderMouse.Handle;
   }

   private void ResizeWindow(WinDraggerState currentState)
   {
      WindowCoordinates newWindowCoordinates = CooridantesCalculator.CalculateNewWindowCoordinates(
         currentState.WindowUnderMouse!.Rect,
         currentState.MousePositionOnDesktop,
         _previousState?.MousePositionOnDesktop);
      WinApiHelper.SetWindowPosition(currentState.WindowUnderMouse.Handle, newWindowCoordinates);
   }
}
