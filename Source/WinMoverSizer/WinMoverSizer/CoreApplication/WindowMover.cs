using System;
using System.Collections.Generic;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public class WindowMover : IWinMoverStateObserver
{
   private MouseAndKeyboardState? _currentlyMovedWindowState;
   private static readonly HashSet<int> WindowMoveKeyCombination =  new() {Keys.LeftAlt, Keys.LeftMouseButton};


   public void Notify(MouseAndKeyboardState state)
   {
      ActOnState(state);

   }

   private void ActOnState(MouseAndKeyboardState state)
   {
      if (_currentlyMovedWindowState == null)
      {
         TryStartMoving(state);
         return;
      }

      ContinueMovingOrStop(state);
   }

   private void ContinueMovingOrStop(MouseAndKeyboardState nowState)
   {
      var isKeyboardForMovementPressed = nowState.KeyStates.AreKeysPressed(WindowMoveKeyCombination);
      if (!isKeyboardForMovementPressed) // we stopped moving
      {
         StopMoving();
         return;
      }

      // edge case,
      // keyboard shortcut still pressed but windows are different ?
      // this can happen when we move window between desktops and mouse cursor enters the taskbar

      var isTheSameWindowStillUnderTheCursor = IsTheSameWindowStillUnderTheCursor(nowState, _currentlyMovedWindowState);
      if (!isTheSameWindowStillUnderTheCursor)
      {
         // when we move window between desktops and mouse cursor enters the taskbar then the window handle will change to '0'
         // for this specific edge case we need to pretend that the mouse cursor is still over the window that we started to move originally
         if (nowState.CalculatedWindowToOperateOn.Handle == IntPtr.Zero)
         {
            nowState.CalculatedWindowToOperateOn = _currentlyMovedWindowState.CalculatedWindowToOperateOn;
         }
         else
         {
            // todo: we sometimes hit this logic branch/ how to handle it?
            StopMoving();
            return;
         }

      }

      MoveWindow(_currentlyMovedWindowState, nowState);

   }

   private void StopMoving()
   {
      _currentlyMovedWindowState = null;
   }

   private void TryStartMoving(MouseAndKeyboardState state)
   {
      var isKeyboardForMovementPressed = state.KeyStates.AreKeysPressed(WindowMoveKeyCombination);
      var isWindowUnderCursorMovable = IsWindowUnderCursorMoveable(state);

      if (isWindowUnderCursorMovable && isKeyboardForMovementPressed)
      {
         _currentlyMovedWindowState = state;
      }
   }

   private bool IsWindowUnderCursorMoveable(MouseAndKeyboardState state)
   {
      return state.CalculatedWindowToOperateOn != null;
   }



   private void MoveWindow(MouseAndKeyboardState? fromState, MouseAndKeyboardState toState)
   {
      var deltaX = toState.MousePositionOnDesktop.X - fromState.MousePositionOnDesktop.X;
      var deltaY = toState.MousePositionOnDesktop.Y - fromState.MousePositionOnDesktop.Y;


       var rect = fromState.CalculatedWindowToOperateOn.Rect;
       var handle = fromState.CalculatedWindowToOperateOn.Handle;

       var rectLeft = rect.Left + deltaX;
       var rectTop = rect.Top + deltaY;

      WinApiHelper.MoveWindow(handle, rectLeft, rectTop);
   }


   private bool IsTheSameWindowStillUnderTheCursor(MouseAndKeyboardState nowState, MouseAndKeyboardState? beforeState)
   {
      return beforeState?.CalculatedWindowToOperateOn?.Handle != IntPtr.Zero
             && beforeState?.CalculatedWindowToOperateOn.Handle == nowState.CalculatedWindowToOperateOn.Handle;
   }
}
