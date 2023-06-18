using System;
using System.Collections.Generic;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class WindowResizer : IWinMoverStateObserver
{
   private MouseAndKeyboardState? _originalResizedWindowState;
   private static readonly HashSet<int> WindowResizeKeyCombination = new() {Keys.LeftAlt, Keys.RightMouseButton};

   public void Notify(MouseAndKeyboardState state)
   {
      ActOnState(state);
   }

   private void ActOnState(MouseAndKeyboardState state)
   {
      if (_originalResizedWindowState == null)
      {
         TryStartResizing(state);
         return;
      }

      ContinueResizingOrStop(state);
   }

   private void ContinueResizingOrStop(MouseAndKeyboardState nowState)
   {
      var isKeyboardForResizePressed = nowState.KeyStates.AreKeysPressed(WindowResizeKeyCombination);
      if (!isKeyboardForResizePressed) // we stopped moving
      {
         StopResizing();
         return;
      }

      // edge case,
      // keyboard shortcut still pressed but windows are different ?
      // this happens when resizing window that spans across two different (or more) desktops and we move mouse cursor over  taskbar

      var isTheSameWindowStillUnderTheCursor = IsTheSameWindowStillUnderTheCursor(nowState, _originalResizedWindowState);
      if (!isTheSameWindowStillUnderTheCursor)
      {

         // this happens when resizing window that spans across two different (or more) desktops and we move mouse cursor over  taskbar
         // pretend that you still resizing the window that you started resizing in the first place
         if (nowState.CalculatedWindowToOperateOn.Handle == IntPtr.Zero)
         {
            nowState.CalculatedWindowToOperateOn = _originalResizedWindowState.CalculatedWindowToOperateOn;
         }
         else
         {
            StopResizing();
            return;
         }

      }
      // continue moving
      ResizeWindow( _originalResizedWindowState, nowState);
   }

   private void StopResizing()
   {
      _originalResizedWindowState = null;
   }

   private void TryStartResizing(MouseAndKeyboardState state)
   {
      var isKeyboardForResizePressed = state.KeyStates.AreKeysPressed(WindowResizeKeyCombination);
      var isWindowUnderCursorResizable = IsWindowUnderCursorResizable(state);

      if (isWindowUnderCursorResizable && isKeyboardForResizePressed)
      {
         _originalResizedWindowState = state;
      }
   }


   private bool IsWindowUnderCursorResizable(MouseAndKeyboardState state)
   {
      return state.CalculatedWindowToOperateOn != null;
   }


   private bool IsTheSameWindowStillUnderTheCursor(MouseAndKeyboardState nowState, MouseAndKeyboardState previousState)
   {
      return nowState?.CalculatedWindowToOperateOn?.Handle != IntPtr.Zero
             && nowState?.CalculatedWindowToOperateOn.Handle == previousState.CalculatedWindowToOperateOn.Handle;
   }

   private void ResizeWindow(MouseAndKeyboardState originalResizedWindowState, MouseAndKeyboardState currentState)
   {
      WindowCoordinates newWindowCoordinates = CooridantesCalculator.CalculateNewWindowCoordinates(
         originalResizedWindowState,
         currentState
      );
      WinApiHelper.ChangeWindowSize(currentState.CalculatedWindowToOperateOn.Handle, newWindowCoordinates);
   }
}
