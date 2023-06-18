using System;
using WinMoverSizer.WinApi;

namespace WinMoverSizer.Models;

public record WindowData
{
   public IntPtr Handle {get; init;}
   public RECT Rect  {get; init;}

   public static readonly WindowData Null = new();
}
