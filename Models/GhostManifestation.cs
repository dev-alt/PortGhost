using System;

namespace PortGhost.Models;

public class GhostManifestation
{
    public int Port { get; set; }
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Protocol { get; set; } = "TCP";
}
