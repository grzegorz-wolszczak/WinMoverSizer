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

   public void UpdateControlsFromState(WinDraggerState state)
   {
      Dispatcher.Invoke(() =>
      {
         labelMousePosition.Content = $"Mouse: {state.MousePositionOnDesktop}";
         labelWindowUnderMouse.Content = $"Window under cursor handle : {state.WindowUnderMouse?.Handle}";
         labelIsKeyShortcutForWindowResizePressed.Content = $"Resize Key pressed? : {state.IsKeyboardShortcutForResizePressed}";
         labelIsKeyShortuctForWindowMovementPressed.Content = $"Movement Key pressed? : {state.IsKeyboardShortcutForMovementPressed}";
      });
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
