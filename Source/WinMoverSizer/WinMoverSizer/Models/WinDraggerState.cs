namespace WinMoverSizer.Models;

// todo: change to EnvironmentState
public class WinDraggerState // state of the buttons and mouse position
{
   public PositionOnDesktop MousePositionOnDesktop;
   public WindowUnderMouse? WindowUnderMouse = default;
   public bool IsKeyboardShortcutForMovementPressed;
   public bool IsKeyboardShortcutForResizePressed;

}
