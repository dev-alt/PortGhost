# 👻 PortGhost

**See what haunts your ports. Exorcise the demons.**

PortGhost is a Windows desktop application that reveals which 
processes are lurking on your network ports—perfect for developers 
tired of "Port 3000 is already in use" errors.

## 🎯 The Problem

You run `npm run dev` and see:
&gt; Error: listen EADDRINUSE: address already in use :::3000

You have no idea what ghost process from yesterday's coding session 
is still haunting that port.

## 👻 The Solution

PortGhost shines a light on port hauntings:

1. **Enter the port** (e.g., 3000)
2. **See the ghost** - Process name, PID, path, start time
3. **Bust the ghost** - One-click process termination
4. **Stay protected** - Optional tray monitoring

## 🛠️ Features

- **Instant Detection** - Sub-second port-to-process resolution
- **Ghost History** - Track repeat offenders
- **Auto-Bust** - Kill known ghosts automatically
- **Dev Port Presets** - Quick buttons for 3000, 8080, 5173, 4200
- **Safe Mode** - Warns before killing system processes

## 💻 Usage

```powershell
# PortGhost uses native Windows APIs:
Get-NetTCPConnection -LocalPort 3000
Get-Process -Id &lt;PID&gt;
