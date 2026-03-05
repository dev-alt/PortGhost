using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PortGhost.Services;

public static class PortScanner
{
    public static async Task<int?> GetProcessIdFromPortAsync(int port)
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
            
            // Search lines like:
            // TCP    0.0.0.0:3000      0.0.0.0:0              LISTENING       12345
            // TCP    [::]:3000         [::]:0                 LISTENING       12345
            
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine) || !trimmedLine.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                    continue;

                var parts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 5)
                {
                    var localAddress = parts[1];
                    var state = parts[3];
                    var pidStr = parts[4];
                    
                    if (state.Equals("LISTENING", StringComparison.OrdinalIgnoreCase) &&
                        localAddress.EndsWith($":{port}") && 
                        int.TryParse(pidStr, out int pid) && 
                        pid > 0)
                    {
                        return pid;
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
}
