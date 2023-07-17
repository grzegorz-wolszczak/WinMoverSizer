using System.Runtime.InteropServices;

namespace WinMoverSizer.WinApi;

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
   public int X;
   public int Y;

   public override string ToString()
   {
      return $"{{{X},{Y}}}";
   }
}
