using System;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.Models;

public record WindowUnderMouse
{
   public IntPtr Handle {get; init;}
   public RECT Rect  {get; init;}

   public static readonly WindowUnderMouse Null = new();
}
