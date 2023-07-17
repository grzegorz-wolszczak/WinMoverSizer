

using WinMoverSizer.WinApi;

namespace WinMoverSizer.Models;


public sealed record MouseAndKeyboardState
{
   public POINT MousePositionOnDesktop { get; init; }
   public WindowData? CalculatedWindowToOperateOn { get; set; }
   public WindowData? OriginalWindowUnderCursor{ get; init; }
   public KeysPressed KeyStates { get; init; }

   public WindowList WindowList { get; init; }

}
