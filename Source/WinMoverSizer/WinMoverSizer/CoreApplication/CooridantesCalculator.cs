using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public static class CooridantesCalculator
{
   public static WindowCoordinates CalculateNewWindowCoordinates(
      MouseAndKeyboardState originalResizedWindowState,
      MouseAndKeyboardState nowState
   )
   {

      RECT originalWindowRect = originalResizedWindowState.CalculatedWindowToOperateOn.Rect;
      var originalMousePosition = originalResizedWindowState.MousePositionOnDesktop;
      var currentMousePosition = nowState.MousePositionOnDesktop;

      var originalWindowWidth = originalWindowRect.Width;
      var originalWindowHeight = originalWindowRect.Height;

      var windowCoordinates = new WindowCoordinates()
      {
         Height = originalWindowHeight,
         Width = originalWindowWidth,
         X = originalWindowRect.Left,
         Y = originalWindowRect.Top
      };


      var horizontalMovementDelta = currentMousePosition.X - originalMousePosition.X;
      var verticalMovementDelta = currentMousePosition.Y - originalMousePosition.Y;

      var widthHalfPointCoordinate = (originalWindowWidth / 2) + originalWindowRect.Left;
      var heightHalfPointCoordinate = (originalWindowHeight / 2) + originalWindowRect.Top;

      if (horizontalMovementDelta != 0)
      {
         // if mouse cursor is on the 'right' half of the window
         if (originalMousePosition.X >= widthHalfPointCoordinate)
         {
            windowCoordinates.Width += horizontalMovementDelta;
         }
         else
         {
            windowCoordinates.Width -= horizontalMovementDelta;
            windowCoordinates.X += horizontalMovementDelta;
         }
      }


      if (verticalMovementDelta == 0) return windowCoordinates;

      // if mouse cursor is on the 'bottom' half of the window
      if (originalMousePosition.Y >= heightHalfPointCoordinate)
      {
         windowCoordinates.Height += verticalMovementDelta;
      }
      else
      {
         windowCoordinates.Height -= verticalMovementDelta;
         windowCoordinates.Y += verticalMovementDelta;
      }


      return windowCoordinates;
   }
}
