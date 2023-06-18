namespace WinMoverSizer.Models;

public record PositionOnDesktop
{
   public int X { get; init; }
   public int Y { get; init; }

   public override string ToString()
   {
      return $"{{{X},{Y}}}";
   }
}
