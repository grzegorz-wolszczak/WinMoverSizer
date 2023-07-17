using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace WinMoverSizer.Utils;

public class MessageWindow
{
  public static void Error(string msg)
  {
    MessageBox.Show(
      msg,
      Constants.ApplicationName,
      MessageBoxButton.OK,
      MessageBoxImage.Error);
  }

}