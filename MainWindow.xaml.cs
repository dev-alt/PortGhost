using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PortGhost.Models;
using PortGhost.Services;

namespace PortGhost;

public partial class MainWindow : Window
{
    private int _currentHauntedPid = 0;
    private bool _isTrayExit = false;
    private readonly ObservableCollection<GhostManifestation> _sweepResults = new();
    private AppSettings _settings = new();

    // Routed command for Ctrl+R re-scan
    private static readonly RoutedCommand _reScanCommand = new();

    public MainWindow()
    {
        InitializeComponent();
        SweepResultsList.ItemsSource = _sweepResults;

        // Keyboard shortcut: Ctrl+R = re-scan
        CommandBindings.Add(new CommandBinding(_reScanCommand,
            async (s, e) => await ScanPortAsync()));
        InputBindings.Add(new KeyBinding(_reScanCommand,
            new KeyGesture(Key.R, ModifierKeys.Control)));
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _settings = SettingsService.Load();
        RefreshPresetButtons();
        PortInput.Focus();
        PortInput.SelectAll();
    }

    // ─────────────────────────────────────────────
    // Window chrome
    // ─────────────────────────────────────────────

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        // Minimize to tray instead of closing
        HideToTray();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        HideToTray();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!_isTrayExit)
        {
            e.Cancel = true;
            HideToTray();
        }
    }

    private void HideToTray()
    {
        WindowState = WindowState.Minimized;
        Hide();
        App.TrayIcon?.ShowBalloonTip("PortGhost", "Running in the background.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
    }

    public void ShowFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void ExitApp()
    {
        _isTrayExit = true;
        Application.Current.Shutdown();
    }

    // ─────────────────────────────────────────────
    // Feature 1: Enter key + Ctrl+R
    // ─────────────────────────────────────────────

    private async void PortInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            await ScanPortAsync();
    }

    // ─────────────────────────────────────────────
    // Preset buttons (Feature 8)
    // ─────────────────────────────────────────────

    private void RefreshPresetButtons()
    {
        PresetButtonsPanel.Items.Clear();
        foreach (var port in _settings.CustomPresets)
        {
            var btn = new Button
            {
                Content = port.ToString(),
                Margin = new Thickness(0, 0, 5, 4),
                Padding = new Thickness(5, 2, 5, 2),
                FontSize = 11,
                Foreground = (Brush)FindResource("TextMutedBrush"),
                Tag = port
            };
            btn.Click += Preset_Click;

            // Right-click context menu to remove
            var cm = new ContextMenu();
            var removeItem = new MenuItem { Header = $"Remove {port}", Tag = port };
            removeItem.Click += RemovePreset_Click;
            cm.Items.Add(removeItem);
            btn.ContextMenu = cm;

            PresetButtonsPanel.Items.Add(btn);
        }
    }

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int port)
        {
            PortInput.Text = port.ToString();
            _ = ScanPortAsync();
        }
    }

    private void AddPreset_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddPresetDialog { Owner = this };
        if (dialog.ShowDialog() == true && dialog.PortNumber.HasValue)
        {
            int newPort = dialog.PortNumber.Value;
            if (!_settings.CustomPresets.Contains(newPort))
            {
                _settings.CustomPresets.Add(newPort);
                SettingsService.Save(_settings);
                RefreshPresetButtons();
            }
        }
    }

    private void RemovePreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is int port)
        {
            _settings.CustomPresets.Remove(port);
            SettingsService.Save(_settings);
            RefreshPresetButtons();
        }
    }

    // ─────────────────────────────────────────────
    // Single port scan
    // ─────────────────────────────────────────────

    private async void ScanBtn_Click(object sender, RoutedEventArgs e) => await ScanPortAsync();

    private async Task ScanPortAsync()
    {
        if (!int.TryParse(PortInput.Text, out int port) || port <= 0 || port > 65535)
        {
            MessageBox.Show("Please enter a valid port number (1-65535).", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ScanBtn.IsEnabled = false;
        SweepResultsPanel.Visibility = Visibility.Collapsed;
        ManifestationPanel.Visibility = Visibility.Collapsed;
        StatusText.Text = $"Scanning port {port}...";
        GhostIcon.Text = "🔮";
        GhostIcon.Opacity = 1.0;
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8"));
        AnimateGhost(true);

        var protocol = GetSelectedProtocol();
        var result = await PortScanner.GetProcessIdFromPortAsync(port, protocol);
        AnimateGhost(false);

        if (result.HasValue)
        {
            var ghost = ProcessExorcist.GetGhostDetails(port, result.Value.Pid);
            if (ghost != null)
            {
                ghost.Protocol = result.Value.Protocol;
                _currentHauntedPid = ghost.ProcessId;
                ShowHauntedState(ghost);
            }
            else ShowClearState();
        }
        else ShowClearState();

        ScanBtn.IsEnabled = true;
    }

    private PortProtocol GetSelectedProtocol() => ProtocolSelector.SelectedIndex switch
    {
        1 => PortProtocol.UDP,
        2 => PortProtocol.Both,
        _ => PortProtocol.TCP
    };

    private void ShowHauntedState(GhostManifestation ghost)
    {
        GhostIcon.Text = "🔴";
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
        StatusText.Text = $"Port {ghost.Port} is HAUNTED!";

        SpiritNameTxt.Text = ghost.ProcessName + ".exe";
        ProtocolTxt.Text = ghost.Protocol;
        EntityIdTxt.Text = ghost.ProcessId.ToString();
        FromPathTxt.Text = ghost.ExecutablePath;

        var duration = DateTime.Now - ghost.StartTime;
        StartedTxt.Text = duration.TotalHours >= 1
            ? $"{(int)duration.TotalHours}h {duration.Minutes}m ago"
            : $"{(int)duration.TotalMinutes}m ago";

        ManifestationPanel.Visibility = Visibility.Visible;
        ManifestationPanel.BeginAnimation(UIElement.OpacityProperty,
            new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(300)));
    }

    private void ShowClearState()
    {
        GhostIcon.Text = "🟢";
        GhostIcon.Opacity = 0.5;
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E"));
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

    // ─────────────────────────────────────────────
    // Feature 2: Copy to clipboard
    // ─────────────────────────────────────────────

    private void CopyPid_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(EntityIdTxt.Text);
        FlashButton(CopyPidBtn, "✓");
    }

    private void CopyPath_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(FromPathTxt.ToolTip?.ToString() ?? FromPathTxt.Text);
        FlashButton(CopyPathBtn, "✓");
    }

    private async void FlashButton(Button btn, string flashContent)
    {
        var original = btn.Content;
        btn.Content = flashContent;
        await Task.Delay(1200);
        btn.Content = original;
    }

    // ─────────────────────────────────────────────
    // Feature 3: Ghost Sweep
    // ─────────────────────────────────────────────

    private async void SweepBtn_Click(object sender, RoutedEventArgs e)
    {
        SweepBtn.IsEnabled = false;
        ScanBtn.IsEnabled = false;
        ManifestationPanel.Visibility = Visibility.Collapsed;
        SweepResultsPanel.Visibility = Visibility.Collapsed;
        _sweepResults.Clear();

        StatusText.Text = "👻 Sweeping all presets...";
        GhostIcon.Text = "🔮";
        GhostIcon.Opacity = 1.0;
        GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A78BFA"));
        AnimateGhost(true);

        var haunted = await PortScanner.SweepPortsAsync(_settings.CustomPresets, PortProtocol.Both);
        AnimateGhost(false);

        foreach (var ghost in haunted)
            _sweepResults.Add(ghost);

        SweepResultsPanel.Visibility = Visibility.Visible;
        SweepEmptyTxt.Visibility = haunted.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (haunted.Count > 0)
        {
            SweepHeaderTxt.Text = $"🧹 SWEEP RESULTS — {haunted.Count} ghost{(haunted.Count > 1 ? "s" : "")} found!";
            StatusText.Text = $"Found {haunted.Count} haunted port{(haunted.Count > 1 ? "s" : "")}.";
            GhostIcon.Text = "🔴";
            GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
        }
        else
        {
            SweepHeaderTxt.Text = "🧹 SWEEP RESULTS";
            StatusText.Text = "All preset ports are clear!";
            GhostIcon.Text = "🟢";
            GhostIcon.Opacity = 0.5;
            GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E"));
        }

        SweepBtn.IsEnabled = true;
        ScanBtn.IsEnabled = true;
    }

    private void SweepExorcise_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not int pid) return;

        bool success = ProcessExorcist.Exorcise(pid, out string error);
        if (success)
        {
            // Remove from list
            for (int i = _sweepResults.Count - 1; i >= 0; i--)
                if (_sweepResults[i].ProcessId == pid)
                    _sweepResults.RemoveAt(i);

            if (_sweepResults.Count == 0)
            {
                SweepEmptyTxt.Visibility = Visibility.Visible;
                StatusText.Text = "All ghosts exorcised!";
                GhostIcon.Text = "🟢";
                GhostIcon.Opacity = 0.5;
                GhostAvatar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E"));
            }
        }
        else
        {
            MessageBox.Show($"Exorcism failed: {error}", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─────────────────────────────────────────────
    // Ghost animation
    // ─────────────────────────────────────────────

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
            GhostIcon.RenderTransform?.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            GhostIcon.RenderTransform?.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            GhostIcon.RenderTransform = Transform.Identity;
        }
    }
}