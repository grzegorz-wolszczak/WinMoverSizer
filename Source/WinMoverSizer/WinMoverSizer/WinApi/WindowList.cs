using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinMoverSizer.Models;


public sealed class WindowInfo
{
   public IntPtr Handle;
   public string? WindowText;
   public string? ClassName;
   public string? ApplicationName;
   public WindowStyleInfo StyleInfo;

}
public sealed class WindowList
{
   private List<WindowInfo> _windows = new();
   private string _asString;
   public string AsString() => _asString;

   public WindowList(List<WindowInfo> windows)
   {
      _windows = windows;
      _asString = ToWindowInfoString();
   }

   private string ToWindowInfoString()
   {
      List<WindowInfo> copy = _windows.ToList();
      copy.Reverse();

      var sb = new StringBuilder();
      const int indentStep = 2;
      int indent = 0;
      foreach (var windowInfo in copy)
      {
         var indentString = new string(' ', indent);
         var styleIndent = new string(' ', indent + 2);
         sb.AppendLine($"{indentString}hwnd        : {windowInfo.Handle}");
         sb.AppendLine($"{indentString}windowText  : {windowInfo.WindowText}");
         sb.AppendLine($"{indentString}class       : {windowInfo.ClassName}");
         sb.AppendLine($"{indentString}application : {windowInfo.ApplicationName}");
         sb.AppendLine($"{indentString}styles: ");
         var windowInfoStyleInfo = windowInfo.StyleInfo;
         sb.AppendLine($"{styleIndent}has caption     : {windowInfoStyleInfo.HasCaption}");
         sb.AppendLine($"{styleIndent}has dialog frame: {windowInfoStyleInfo.HasDialogFrame}");
         sb.AppendLine($"{styleIndent}is popup window : {windowInfoStyleInfo.IsPopupWindow}");
         //sb.AppendLine($"{styleIndent}is child        : {windowInfoStyleInfo.IsChild}");
         sb.AppendLine("");
         indent += indentStep;
      }

      return sb.ToString();
   }
}
