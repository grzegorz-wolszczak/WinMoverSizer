namespace WinMoverSizer.Models;

public sealed record WindowStyleInfo
{
   public bool HasCaption { get; set; }

   public bool HasDialogFrame { get; set; }
   public bool IsChild { get; set; }
   public bool IsPopupWindow { get; set; }
}
