using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowSpy;


public class WindowInfo
{
   public IntPtr Handle;
   public string? WindowText;
   public string? ClassName;
   public string? ApplicationName;

}
public class WindowList
{

   private List<WindowInfo> _windows = new();


   public void Add(WindowInfo? windowInfo)
   {
      if (windowInfo == null)
      {
         return;
      }
      _windows.Add(windowInfo);
   }

   public string ToWindowInfoString()
   {
      List<WindowInfo> copy = _windows.ToList();
      copy.Reverse();

      var sb = new StringBuilder();
      const int indentStep = 2;
      int indent = 0;
      foreach (var windowInfo in copy)
      {
         var indentString = new string(' ', indent);
         sb.AppendLine($"{indentString}hwnd        : {windowInfo.Handle}");
         sb.AppendLine($"{indentString}windowText  : {windowInfo.WindowText}");
         sb.AppendLine($"{indentString}class       : {windowInfo.ClassName}");
         sb.AppendLine($"{indentString}application : {windowInfo.ApplicationName}");
         sb.AppendLine("");
         indent += indentStep;
      }

      return sb.ToString();
   }
}
