# 👻 PortGhost

**See what haunts your ports. Exorcise the demons.**

[![GitHub Release](https://img.shields.io/github/v/release/dev-alt/PortGhost?style=flat-square&logo=github)](https://github.com/dev-alt/PortGhost/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple?style=flat-square)](https://dotnet.microsoft.com/download)

PortGhost is a Windows desktop app that reveals which processes are haunting your network ports — perfect for developers tired of `"Port 3000 is already in use"` errors.

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

Download the latest self-contained executable from the [**Releases page**](https://github.com/dev-alt/PortGhost/releases/latest). No installation required — just run `PortGhost.exe`.

---

## 💻 How It Works

PortGhost uses native Windows APIs to map open TCP ports to the owning process, then lets you terminate it with one click.

---

## 📜 License

MIT — see [LICENSE](LICENSE) for details.

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
