# 👻 PortGhost

**See what haunts your ports. Exorcise the demons.**

[![GitHub Release](https://img.shields.io/github/v/release/your-username/PortGhost?style=flat-square&logo=github)](https://github.com/your-username/PortGhost/releases/latest)
[![Microsoft Store](https://img.shields.io/badge/Microsoft%20Store-Download-blue?style=flat-square&logo=microsoft)](https://apps.microsoft.com/store/detail/portghost/YOUR_APP_ID)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple?style=flat-square)](https://dotnet.microsoft.com/download)

PortGhost is a Windows desktop app that reveals which processes are haunting your network ports—perfect for developers tired of `"Port 3000 is already in use"` errors.

---

## 🎯 The Problem

You run `npm run dev` and see:
```
Error: listen EADDRINUSE: address already in use :::3000
```
Some ghost from yesterday's coding session is still haunting that port.

---

## 👻 The Solution

PortGhost shines a light on port hauntings:

1. **Enter the port** (e.g., `3000`)
2. **See the ghost** — Process name, PID, path, start time
3. **Bust the ghost** — One-click process termination
4. **Stay protected** — Optional tray monitoring

---

## 🛠️ Features

| Feature | Description |
|---|---|
| ⚡ **Instant Detection** | Sub-second port-to-process resolution |
| 📜 **Ghost History** | Track repeat offender processes |
| 💀 **Auto-Bust** | Kill known ghosts automatically |
| 🔢 **Dev Port Presets** | Quick buttons for `3000`, `8080`, `5173`, `4200` |
| 🛡️ **Safe Mode** | Warns before killing system processes |

---

## 💾 Download

### Option 1: GitHub Release (Standalone `.exe`)

Download the latest self-contained executable from the [**Releases page**](https://github.com/your-username/PortGhost/releases/latest). No installation required — just run `PortGhost.exe`.

### Option 2: Microsoft Store

*Coming soon — get it from the [Microsoft Store](https://apps.microsoft.com/store/detail/portghost/YOUR_APP_ID) for automatic updates.*

---

## 🔨 Building from Source

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Windows 10 or Windows 11

### Steps

```powershell
# Clone the repository
git clone https://github.com/your-username/PortGhost.git
cd PortGhost

# Restore & build
dotnet restore
dotnet build

# Run
dotnet run
```

### Build a Release Executable

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./dist
# Output: ./dist/PortGhost.exe
```

### Build MSIX (Microsoft Store Package)

```powershell
dotnet publish -c Release -r win-x64 -p:GenerateMsixOnBuild=true -o ./msix_output
```

> ⚠️ Before Store submission, update the `Identity Name` and `Publisher` DN in `Package.appxmanifest` to match your [Partner Center](https://partner.microsoft.com) account.

---

## 🏗️ Architecture

```
PortGhost/
├── Assets/                  # SVG icons and Store image assets
├── Models/                  # Data models (PortInfo, ProcessInfo, etc.)
├── Services/
│   └── PortScanner.cs       # Core port-to-process detection engine
├── MainWindow.xaml          # Main UI layout
├── MainWindow.xaml.cs       # Main UI logic
├── App.xaml                 # Application resources & theme
├── Package.appxmanifest     # MSIX / Microsoft Store manifest
└── .github/
    └── workflows/
        └── release.yml      # GitHub Actions release automation
```

---

## 💻 How It Works

PortGhost uses native Windows APIs via PowerShell/CIM:

```powershell
# Find the process occupying port 3000
Get-NetTCPConnection -LocalPort 3000
Get-Process -Id <PID>
```

---

## 📜 License

MIT — see [LICENSE](LICENSE) for details.

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for setup instructions and guidelines.
