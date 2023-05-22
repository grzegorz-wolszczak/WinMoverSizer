using System.Runtime.InteropServices;

namespace WinMoverSizer.WinApi;

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
   public int x;
   public int y;
}
