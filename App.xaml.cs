using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace PortGhost;

public partial class App : Application
{
    public static TaskbarIcon? TrayIcon { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Build tray icon programmatically
        TrayIcon = new TaskbarIcon
        {
            ToolTipText = "PortGhost — Port Haunting Detector",
            Icon = new System.Drawing.Icon("Assets/PortGhost.ico")
        };

        // Context menu
        var menu = new System.Windows.Controls.ContextMenu();

        var openItem = new System.Windows.Controls.MenuItem { Header = "👻 Open PortGhost" };
        openItem.Click += (s, e) => GetMainWindow()?.ShowFromTray();

        var exitItem = new System.Windows.Controls.MenuItem { Header = "✕ Exit" };
        exitItem.Click += (s, e) => GetMainWindow()?.ExitApp();

        menu.Items.Add(openItem);
        menu.Items.Add(new System.Windows.Controls.Separator());
        menu.Items.Add(exitItem);

        TrayIcon.ContextMenu = menu;

        // Double-click to restore
        TrayIcon.TrayMouseDoubleClick += (s, e) => GetMainWindow()?.ShowFromTray();
    }

    private static MainWindow? GetMainWindow() =>
        Application.Current.MainWindow as MainWindow;

    protected override void OnExit(ExitEventArgs e)
    {
        TrayIcon?.Dispose();
        base.OnExit(e);
    }
}
