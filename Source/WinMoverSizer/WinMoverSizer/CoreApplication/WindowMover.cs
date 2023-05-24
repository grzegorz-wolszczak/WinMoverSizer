using System;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class WindowMover : IWinMoverStateObserver
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
      var shouldMoveWindow = ShouldMoveWindow(currentState);
      if (shouldMoveWindow)
      {
         MoveWindow(currentState);
      }
   }

   private void MoveWindow(WinDraggerState currentState)
   {
      var deltaX = currentState.MousePositionOnDesktop.X - _previousState.MousePositionOnDesktop.X;
      var deltaY = currentState.MousePositionOnDesktop.Y - _previousState.MousePositionOnDesktop.Y;

      if (currentState.WindowUnderMouse == null)
      {
         return;
      }

      var rect = currentState.WindowUnderMouse.Rect;
      var handle = currentState.WindowUnderMouse.Handle;

      var rectLeft = rect.Left + deltaX;
      var rectTop = rect.Top + deltaY;

      int height = rect.Bottom - rect.Top;
      var width = rect.Right - rect.Left;

      WinApiHelper.SetWindowPosition(handle, rectLeft, rectTop, width, height);
   }

   private bool ShouldMoveWindow(WinDraggerState currentState)
   {
      var isTheSameWindowStillUnderTheCursor = IsTheSameWindowStillUnderTheCursor(currentState);
      var didCursorMoved = DidCursorMoved(currentState);
      var wasKeyboardShortcutForMovementStillPressed = WasKeyboardShortcutForMovementStillPressed(currentState);

      return isTheSameWindowStillUnderTheCursor
             && didCursorMoved &&
             wasKeyboardShortcutForMovementStillPressed;
   }


   private bool WasKeyboardShortcutForMovementStillPressed(WinDraggerState currentState)
   {
      var isShortcutPressed = currentState.IsKeyboardShortcutForMovementPressed;
      var wasShortcutPressed = _previousState.IsKeyboardShortcutForMovementPressed;

      return isShortcutPressed
             && wasShortcutPressed;
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
}
