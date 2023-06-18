using WindowSpy;

namespace WinMoverSizer.Models;


public record MouseAndKeyboardState
{
   public PositionOnDesktop? MousePositionOnDesktop { get; init; }
   public WindowData? CalculatedWindowToOperateOn { get; set; }
   public WindowData? OriginalWindowUnderCursor{ get; init; }
   public KeysPressed KeyStates { get; init; }

   public WindowList WindowList { get; init; }

}
