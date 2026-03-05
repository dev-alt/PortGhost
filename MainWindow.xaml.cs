using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PortGhost.Models;
using PortGhost.Services;
using System.Windows.Controls;

namespace PortGhost;

public partial class MainWindow : Window
{
    private int _currentHauntedPid = 0;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        PortInput.Focus();
        PortInput.SelectAll();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content != null)
        {
            PortInput.Text = btn.Content.ToString();
            _ = ScanPortAsync();
        }
    }

    private async void ScanBtn_Click(object sender, RoutedEventArgs e)
    {
        await ScanPortAsync();
    }

    private async Task ScanPortAsync()
    {
        if (!int.TryParse(PortInput.Text, out int port) || port <= 0 || port > 65535)
        {
            MessageBox.Show("Please enter a valid port number (1-65535).", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Set scanning UI state
        ScanBtn.IsEnabled = false;
        ManifestationPanel.Visibility = Visibility.Collapsed;
        StatusText.Text = $"Scanning port {port}...";
        GhostIcon.Opacity = 1.0;
        GhostIcon.Text = "🔮";
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8")); // Scanning blue
        AnimateGhost(true);

        // Perform scan
        int? pid = await PortScanner.GetProcessIdFromPortAsync(port);
        AnimateGhost(false);

        if (pid.HasValue)
        {
            // Haunted!
            var ghost = ProcessExorcist.GetGhostDetails(port, pid.Value);
            if (ghost != null)
            {
                _currentHauntedPid = ghost.ProcessId;
                ShowHauntedState(ghost);
            }
            else
            {
                ShowClearState(); // Process exited during our check
            }
        }
        else
        {
            ShowClearState();
        }

        ScanBtn.IsEnabled = true;
    }

    private void ShowHauntedState(GhostManifestation ghost)
    {
        GhostIcon.Text = "🔴";
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")); // Haunted red
        StatusText.Text = $"Port {ghost.Port} is HAUNTED!";
        
        SpiritNameTxt.Text = ghost.ProcessName + ".exe";
        EntityIdTxt.Text = ghost.ProcessId.ToString();
        FromPathTxt.Text = ghost.ExecutablePath;
        
        var duration = DateTime.Now - ghost.StartTime;
        StartedTxt.Text = duration.TotalHours >= 1 
            ? $"{(int)duration.TotalHours}h {(int)duration.Minutes}m ago" 
            : $"{(int)duration.TotalMinutes}m ago";

        ManifestationPanel.Visibility = Visibility.Visible;
        
        var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(300));
        ManifestationPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }

    private void ShowClearState()
    {
        GhostIcon.Text = "🟢";
        GhostIcon.Opacity = 0.5;
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")); // Clear green
        StatusText.Text = "Port is clear.";
        ManifestationPanel.Visibility = Visibility.Collapsed;
    }

    private void IgnoreBtn_Click(object sender, RoutedEventArgs e)
    {
        ManifestationPanel.Visibility = Visibility.Collapsed;
        StatusText.Text = "Ghost ignored.";
        GhostIcon.Text = "👻";
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B")); 
    }

    private void ExorciseBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_currentHauntedPid == 0) return;

        ExorciseBtn.IsEnabled = false;
        GhostIcon.Text = "⚡";
        
        bool success = ProcessExorcist.Exorcise(_currentHauntedPid, out string error);
        
        if (success)
        {
            StatusText.Text = "Demon exorcised successfully!";
            ShowClearState();
        }
        else
        {
            MessageBox.Show($"Exorcism failed: {error}", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "Exorcism failed.";
        }
        
        ExorciseBtn.IsEnabled = true;
        _currentHauntedPid = 0;
    }

    private void AnimateGhost(bool isScanning)
    {
        if (isScanning)
        {
            var scale = new ScaleTransform(1.0, 1.0);
            GhostIcon.RenderTransform = scale;
            GhostIcon.RenderTransformOrigin = new Point(0.5, 0.5);
            
            var anim = new DoubleAnimation(1.0, 1.2, TimeSpan.FromMilliseconds(500))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
        else
        {
            GhostIcon.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            GhostIcon.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            GhostIcon.RenderTransform = Transform.Identity;
        }
    }
}