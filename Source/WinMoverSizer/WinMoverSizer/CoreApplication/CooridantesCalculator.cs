using System.Windows;
using WinMoverSizer.Models;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.CoreApplication;

public static class CooridantesCalculator
{
   public static WindowCoordinates CalculateNewWindowCoordinates(RECT rect, PositionOnDesktop currentMousePosition,
      PositionOnDesktop? nullablePreviousMousePosition)
   {
      var windowWidth = rect.Right - rect.Left;
      var windowHeight = rect.Bottom - rect.Top;

      var windowCoordinates = new WindowCoordinates()
      {
         Height = windowHeight,
         Width = windowWidth,
         X = rect.Left,
         Y = rect.Top
      };

      if (nullablePreviousMousePosition == null)
      {
         return windowCoordinates;
      }

      PositionOnDesktop previousMousePosition = nullablePreviousMousePosition;

      var horizontalMovementDelta = currentMousePosition.X - previousMousePosition.X;
      var verticalMovementDelta = currentMousePosition.Y - previousMousePosition.Y;

      var widthHalfPointCoordinate = (windowWidth / 2) + rect.Left;
      var heightHalfPointCoordinate = (windowHeight / 2) + rect.Top;

      if (horizontalMovementDelta != 0)
      {
         // if mouse cursor is on the 'right' half of the window
         if (currentMousePosition.X >= widthHalfPointCoordinate)
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
      if (currentMousePosition.Y >= heightHalfPointCoordinate)
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
