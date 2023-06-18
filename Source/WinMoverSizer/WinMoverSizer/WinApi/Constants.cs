namespace WinMoverSizer.WinApi;


public class KeyMetadata
{
   public readonly int Code;
   public readonly string LongName;

   public KeyMetadata(int code, string longName)
   {
      Code = code;
      LongName = longName;
   }

   public static implicit operator int(KeyMetadata d)
   {
      return d.Code;
   }
}

public static class Keys
{
   // key codes https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

   //public const int VK_MENU = 0x12; // Alt key (any left, and right)
   public static readonly KeyMetadata LeftMouseButton = new KeyMetadata(0x01, "LeftMouseButton");
   public static readonly KeyMetadata RightMouseButton =new KeyMetadata( 0x02,"RightMouseButton");
   public static readonly KeyMetadata MiddleMouseButton = new KeyMetadata(0x04,"MiddleMouseButton");
   //public const int VK_SHIFT = 0x10; // shift
   //public const int VK_CONTROL = 0x11; // control key

   public static readonly KeyMetadata LeftShift = new KeyMetadata(0xA0,"LeftShift");
   public static readonly KeyMetadata RightShift = new KeyMetadata(0xA1,"RightShift");
   public static readonly KeyMetadata LeftControl = new KeyMetadata(0xA2,"LeftControl");
   public static readonly KeyMetadata RightControl =new KeyMetadata( 0xA3,"RightControl");
   public static readonly KeyMetadata LeftAlt = new KeyMetadata(0xA4,"LeftAlt");
   public static readonly KeyMetadata RightAlt =new KeyMetadata( 0xA5,"RightAlt");

}
