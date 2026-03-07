using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PortGhost.Models;

namespace PortGhost.Services;

public enum PortProtocol { TCP, UDP, Both }

public static class PortScanner
{
    /// <summary>
    /// Gets the PID (and protocol) for a given port. Returns null if not in use.
    /// </summary>
    public static async Task<(int Pid, string Protocol)?> GetProcessIdFromPortAsync(int port, PortProtocol protocol = PortProtocol.TCP)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "netstat.exe",
            Arguments = "-ano",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                bool isTcp = trimmed.StartsWith("TCP", StringComparison.OrdinalIgnoreCase);
                bool isUdp = trimmed.StartsWith("UDP", StringComparison.OrdinalIgnoreCase);

                if (!isTcp && !isUdp) continue;

                // Filter by protocol preference
                if (protocol == PortProtocol.TCP && !isTcp) continue;
                if (protocol == PortProtocol.UDP && !isUdp) continue;

                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // TCP:  [proto] [local] [remote] [state] [pid]  → 5 parts, state == LISTENING
                // UDP:  [proto] [local] [*:*]    [pid]          → 4 parts (no state column)
                if (isTcp && parts.Length >= 5)
                {
                    var localAddress = parts[1];
                    var state = parts[3];
                    var pidStr = parts[4];

                    if (state.Equals("LISTENING", StringComparison.OrdinalIgnoreCase) &&
                        PortMatches(localAddress, port) &&
                        int.TryParse(pidStr, out int pid) && pid > 0)
                    {
                        return (pid, "TCP");
                    }
                }
                else if (isUdp && parts.Length >= 4)
                {
                    var localAddress = parts[1];
                    var pidStr = parts[3];

                    if (PortMatches(localAddress, port) &&
                        int.TryParse(pidStr, out int pid) && pid > 0)
                    {
                        return (pid, "UDP");
                    }
                }
            }
        }
        catch
        {
            // Ignore errors running netstat
        }

        return null;
    }

    /// <summary>
    /// Scans multiple ports concurrently. Returns all haunted manifestations.
    /// </summary>
    public static async Task<List<GhostManifestation>> SweepPortsAsync(IEnumerable<int> ports, PortProtocol protocol = PortProtocol.Both)
    {
        var tasks = new List<Task<GhostManifestation?>>();

        foreach (var port in ports)
        {
            tasks.Add(ScanSinglePortAsync(port, protocol));
        }

        var results = await Task.WhenAll(tasks);
        var haunted = new List<GhostManifestation>();
        foreach (var r in results)
        {
            if (r != null) haunted.Add(r);
        }
        return haunted;
    }

    private static async Task<GhostManifestation?> ScanSinglePortAsync(int port, PortProtocol protocol)
    {
        var result = await GetProcessIdFromPortAsync(port, protocol);
        if (!result.HasValue) return null;

        var ghost = ProcessExorcist.GetGhostDetails(port, result.Value.Pid);
        if (ghost != null) ghost.Protocol = result.Value.Protocol;
        return ghost;
    }

    private static bool PortMatches(string localAddress, int targetPort)
        => localAddress.EndsWith($":{targetPort}");
}
