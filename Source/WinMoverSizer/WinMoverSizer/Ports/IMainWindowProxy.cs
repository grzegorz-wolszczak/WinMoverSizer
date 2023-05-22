namespace WinMoverSizer.Ports;

public interface IMainWindowProxy
{
  void ShowWindow();
  bool IsWindowVisible();
  void HideWindow();
}