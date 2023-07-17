using System.ComponentModel;
using System.Windows;
using WinMoverSizer.Models;

namespace WinMoverSizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
   public MainWindow()
   {
      InitializeComponent();
      Closing += OnClosing;
   }

   public void UpdateControlsFromState(MouseAndKeyboardState state)
   {
      labelMousePosition.Content = $"Cursor: {state.MousePositionOnDesktop}" + $"\nWindow handle to move: {state.CalculatedWindowToOperateOn?.Handle}";
      //labelWindowUnderMouse.Content = $"Window handle to move: {state.CalculatedWindowToOperateOn?.Handle}";
      //var asString = state.KeyStates.AsString();
      //keyPressedLabel.Content = $"Key pressed? : {asString}";
      windowsHierarchyTextBlock.Text = state.WindowList.AsString();


   }
   private void OnClosing(object? sender, CancelEventArgs e)
   {
      MinimizeToTrayOnWindowClose(e);
   }

   private void MinimizeToTrayOnWindowClose(CancelEventArgs e)
   {
      e.Cancel = true;
      Hide();
   }

   private void ExitButtonClick(object sender, RoutedEventArgs e)
   {
      Application.Current.Shutdown();
   }
}
