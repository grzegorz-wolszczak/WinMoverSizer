using System.Windows;
using System.Windows.Forms;
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

  public static bool Question(string msg)
  {
    var result = FlexibleMessageBox.Show(msg, Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
    return result == DialogResult.Yes;
  }
}