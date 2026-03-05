# 👻 Contributing to PortGhost

Thanks for wanting to help bust some port ghosts! Here's how to get started.

## 🛠️ Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — required to build
- Windows 10/11 — WPF requires Windows
- (Optional) Visual Studio 2022+ or VS Code with C# extension

## 🚀 Getting Started

```powershell
# 1. Fork & clone the repo
git clone https://github.com/your-username/PortGhost.git
cd PortGhost

# 2. Restore dependencies
dotnet restore

# 3. Build the project
dotnet build

# 4. Run the app
dotnet run
```

## 📦 Building a Release Executable

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./dist
```

Your standalone `PortGhost.exe` will be in the `./dist` folder.

## 🏪 Building an MSIX Package (Microsoft Store)

The `Package.appxmanifest` in the root contains the Store package metadata.

```powershell
dotnet publish -c Release -r win-x64 -p:GenerateMsixOnBuild=true -o ./msix_output
```

Before submitting to the Store, update the `Identity Name` and `Publisher` fields in `Package.appxmanifest` to match your [Partner Center](https://partner.microsoft.com) account.

## 🔀 Branching

- `main` — stable, production-ready code
- `dev` — active development; open PRs against this branch
- Feature branches: `feature/your-feature-name`

## 🐛 Bug Reports

Please open an [issue](https://github.com/your-username/PortGhost/issues) with:
1. Steps to reproduce
2. Expected vs. actual behaviour
3. Screenshots if applicable

## ✨ Feature Requests

Open a discussion before submitting a PR for large features, so we can align on the approach first.

## ✅ Pull Request Checklist

- [ ] Code builds without warnings
- [ ] Follows existing code style (C# naming conventions)
- [ ] PR description clearly explains the change
