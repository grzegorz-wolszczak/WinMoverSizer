namespace WinMoverSizer.Models;


public class WinDraggerState
{
   public PositionOnDesktop MousePositionOnDesktop;
   public WindowUnderMouse? WindowUnderMouse = default;
   public bool IsKeyboardShortcutForMovementPressed;
   public bool IsKeyboardShortcutForResizePressed;

}
