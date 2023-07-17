using System;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.Models;

public sealed record WindowData
{
   public IntPtr Handle { get; init; }
   public RECT Rect { get; init; }
   public IntPtr ParentHandle { get; init; }
   public string ClassName { get; init; }
   public WindowStyleInfo WindowStyleInfo { get; set; }


   public static readonly WindowData Null = new();
}
