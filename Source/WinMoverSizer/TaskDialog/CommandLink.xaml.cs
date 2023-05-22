using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TaskDialogInterop;

/// <summary>
/// Interaction logic for CommandLink.xaml
/// </summary>
public partial class CommandLink : UserControl
{
    /// <summary>
    /// Occurs when a <see cref="CommandLink"/> is clicked.
    /// </summary>
    [Category("Behavior")]
    public event RoutedEventHandler Click
    {
        add
        {
            CommandLinkButton.Click += value;
        }
        remove
        {
            CommandLinkButton.Click -= value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLink"/> class.
    /// </summary>
    public CommandLink()
    {
        InitializeComponent();

        Loaded += new RoutedEventHandler(CommandLink_Loaded);
    }

    /// <summary>
    /// Handles the Loaded event of the CommandLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void CommandLink_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext != null && DataContext is TaskDialogButtonData)
        {
            if ((DataContext as TaskDialogButtonData).IsDefault)
            {
                CommandLinkButton.Focus();
            }
        }
    }
}