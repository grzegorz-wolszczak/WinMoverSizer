using System;
using System.Drawing;
using System.IO;
using WinMoverSizer.Utils;

namespace WinMoverSizer;

public static class Constants
{
   public static readonly string ApplicationName = "WinMoverSizer";

   /* you need to add to csproj file in order to icon to be found
    <ItemGroup>
         <Resource Include="Images\application.ico" />
     </ItemGroup>
   */
   private static Uri IconUri = new($@"/Images/application.ico", UriKind.Relative);
   public static Icon ApplicationIcon = GetAppIcon();

   private static Icon GetAppIcon()
   {
      try
      {
         Stream iconStream = System.Windows.Application.GetResourceStream(IconUri).Stream;
         var bitmap = new Bitmap(iconStream);
         var iconHandle = bitmap.GetHicon();
         var icon = Icon.FromHandle(iconHandle);
         return icon;
      }
      catch
      {
         MessageWindow.Error("Could not find Application Icon");
         return SystemIcons.WinLogo;
      }
   }
}
