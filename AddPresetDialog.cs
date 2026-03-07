using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PortGhost;

/// <summary>
/// A minimal dialog for adding a custom preset port number.
/// </summary>
public partial class AddPresetDialog : Window
{
    public int? PortNumber { get; private set; }

    public AddPresetDialog()
    {
        // Build the window programmatically — no separate XAML needed
        Title = "Add Preset Port";
        Width = 280;
        Height = 150;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.ToolWindow;
        Background = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E293B"));

        var grid = new Grid { Margin = new Thickness(20) };
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var label = new TextBlock
        {
            Text = "Enter port number (1–65535):",
            Foreground = System.Windows.Media.Brushes.White,
            Margin = new Thickness(0, 0, 0, 8)
        };
        Grid.SetRow(label, 0);

        var input = new TextBox
        {
            Height = 32,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 12),
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F172A")),
            Foreground = System.Windows.Media.Brushes.White,
            BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#475569")),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8, 4, 8, 4),
            VerticalContentAlignment = VerticalAlignment.Center
        };
        input.KeyDown += (s, e) => { if (e.Key == Key.Enter) TryConfirm(input); };
        Grid.SetRow(input, 1);

        var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var addBtn = new Button
        {
            Content = "Add",
            Width = 60,
            Height = 28,
            Margin = new Thickness(0, 0, 8, 0),
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#38BDF8")),
            Foreground = System.Windows.Media.Brushes.White,
            BorderThickness = new Thickness(0)
        };
        addBtn.Click += (s, e) => TryConfirm(input);

        var cancelBtn = new Button
        {
            Content = "Cancel",
            Width = 60,
            Height = 28,
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#334155")),
            Foreground = System.Windows.Media.Brushes.White,
            BorderThickness = new Thickness(0)
        };
        cancelBtn.Click += (s, e) => { DialogResult = false; };

        btnPanel.Children.Add(addBtn);
        btnPanel.Children.Add(cancelBtn);
        Grid.SetRow(btnPanel, 2);

        grid.Children.Add(label);
        grid.Children.Add(input);
        grid.Children.Add(btnPanel);

        Content = grid;

        Loaded += (s, e) => input.Focus();
    }

    private void TryConfirm(TextBox input)
    {
        if (int.TryParse(input.Text, out int port) && port > 0 && port <= 65535)
        {
            PortNumber = port;
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("Enter a valid port (1–65535).", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
