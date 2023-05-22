using System;
using System.Drawing;
using System.IO;
using WinMoverSizer.Utils;

namespace WinMoverSizer;

public static class Constants
{
  public static string ApplicationName = "WinMoverSizer";

  /* you need to add to csproj file in order to icon to be found
   <ItemGroup>
        <Resource Include="Images\application.ico" />
    </ItemGroup>
  */
  public static Uri IconUri = new Uri($@"/Images/application.ico", UriKind.Relative);
  public static Icon ApplicationIcon = GetAppIcon();
  //public static string ConfigFileName = $"{ApplicationName}.settings.json"; // todo remove that
    


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