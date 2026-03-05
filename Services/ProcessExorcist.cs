using System;
using System.Collections.Generic;
using System.Diagnostics;
using PortGhost.Models;

namespace PortGhost.Services;

public static class ProcessExorcist
{
    private static readonly HashSet<string> ProtectedProcesses = new(StringComparer.OrdinalIgnoreCase)
    {
        "svchost", "csrss", "wininit", "smss", "services", "lsass", "winlogon", "explorer", "spoolsv",
        "System", "Idle"
    };

    public static GhostManifestation? GetGhostDetails(int port, int pid)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            
            string processName = process.ProcessName;
            string executablePath = "Unknown";
            
            try 
            {
                executablePath = process.MainModule?.FileName ?? "Unknown";
            }
            catch 
            {
                // Access denied or 32/64 bit mismatch
            }

            return new GhostManifestation
            {
                Port = port,
                ProcessId = pid,
                ProcessName = processName,
                ExecutablePath = executablePath,
                StartTime = process.StartTime
            };
        }
        catch
        {
            // Process might have exited already, or we lack access.
            return null;
        }
    }

    public static bool Exorcise(int pid, out string errorMessage)
    {
        errorMessage = string.Empty;
        try
        {
            using var process = Process.GetProcessById(pid);
            
            if (ProtectedProcesses.Contains(process.ProcessName))
            {
                errorMessage = $"Cannot exorcise protected system process: {process.ProcessName}";
                return false;
            }
            
            // Kill entire process tree
            process.Kill(entireProcessTree: true); 
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}
