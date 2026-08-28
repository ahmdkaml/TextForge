<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="Assets/branding/logo-dark.svg">
    <source media="(prefers-color-scheme: light)" srcset="Assets/branding/logo-light.svg">
    <img alt="TextForge logo" src="Assets/branding/logo-light.svg" width="360">
  </picture>
</p>

<h1 align="center">TextForge</h1>

<p align="center">
  <strong>A modular, UI-agnostic rich document creation and PDF generation engine.</strong>
</p>

<p align="center">
  <a href="#overview">Overview</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#key-features">Key Features</a> •
  <a href="#repository-structure">Repository Structure</a> •
  <a href="#tech-stack">Tech Stack</a> •
  <a href="#installation">Installation</a> •
  <a href="#usage">Usage</a> •
  <a href="#license">License</a>
</p>

---

## Overview

**TextForge** is a modular, UI-agnostic rich document creation and PDF generation engine. It is built around a strict separation between a pure, deterministic document engine (`TextForge.Core`) and the client interfaces that consume it (`TextForge.Desktop`, with Web and Mobile clients planned).

This separation means the same document domain models, layout logic, and PDF compilation pipeline can power a desktop app today and a WASM or MAUI client tomorrow — without duplicating a single line of business logic.

---

## Architecture

TextForge enforces a one-directional dependency: client interfaces depend on the core engine, never the other way around. `TextForge.Core` has zero UI framework dependencies and is fully deterministic, stateless, and platform-agnostic.

```
                        ┌───────────────────────────────┐
                        │          TextForge.Core        │
                        │  (Pure Document Domain Engine)  │
                        │                                 │
                        │  • Document / Section / Module   │
                        │    domain models                │
                        │  • Layout & typography tokens     │
                        │  • Validation & schema rules       │
                        │  • Serialization routines           │
                        │  • QuestPDF compilation pipeline     │
                        │                                       │
                        │  Zero UI dependencies · deterministic │
                        │  stateless · platform-agnostic        │
                        └───────────────┬───────────────────────┘
                                        │
                     depended on by (never the reverse)
                                        │
        ┌───────────────────────────────┼────────────────────────────────┐
        │                               │                                │
        ▼                               ▼                                ▼
┌───────────────────┐        ┌────────────────────┐          ┌─────────────────────┐
│  TextForge.Desktop │        │   TextForge.Web      │          │  TextForge.Mobile     │
│  (Avalonia UI)      │        │   (planned · WASM)    │          │  (planned · MAUI)      │
│                     │        │                        │          │                        │
│ • Activity Bar       │        │  Browser-based client   │          │  Native mobile client   │
│ • Module Tree         │        │  reusing Core engine      │          │  reusing Core engine     │
│ • Active Module Editor │        │  via WebAssembly            │          │  via .NET MAUI             │
│ • Live Viewport         │        └────────────────────┘          └─────────────────────┘
│ • Property Inspector     │
└───────────────────────────┘
```

**Layer boundaries:**

| Layer | Responsibility |
|---|---|
| `TextForge.Core` | Pure document domain models, QuestPDF compilation engine, PDF pipeline, validation, schema definitions, serialization. No UI framework dependencies. |
| `TextForge.Desktop` | Cross-platform desktop interface (Avalonia UI + CommunityToolkit MVVM). Multi-zone workspace, system shell integrations. |

---

## Key Features

- **Modular Block Composition** — Build documents from composable, structured content modules.
- **Live Layout Preview** — Real-time visual feedback as document content and structure change.
- **Deterministic PDF Generation** — High-fidelity, reproducible document compilation powered by QuestPDF.
- **Cross-Platform** — Runs natively on Windows, macOS, and Linux via Avalonia UI.
- **UI-Agnostic Extensibility** — Core engine is ready to power future Web (WASM) and Mobile (MAUI) clients without logic duplication.

---

## Repository Structure

```
TextForge/
├── Assets/
│   ├── branding/            # SVG logos (logo-dark.svg, logo-light.svg), icon.png
│   └── icons/                # Multi-res app-icon.ico, .icns, favicon.ico
├── src/
│   ├── TextForge.Core/       # Domain models, QuestPDF compilation pipeline
│   └── TextForge.Desktop/    # Avalonia UI shell, Views, ViewModels
├── tests/                    # Automated test suites
└── README.md
```

---

## Tech Stack

| Component | Technology |
|---|---|
| Target Framework | .NET 10.0 |
| Desktop UI Toolkit | Avalonia UI (v12.x) — Fluent Theme, Inter font |
| MVVM Framework | CommunityToolkit.Mvvm (v8.x) |
| PDF Compilation Engine | QuestPDF |

---

## Installation

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- Git

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/ahmdkaml/TextForge.git
cd TextForge

# 2. Restore dependencies
dotnet restore

# 3. Build the solution
dotnet build
```

---

## Usage

Run the desktop client directly from the repository root:

```bash
dotnet run --project TextForge.Desktop
```

This launches the Avalonia-based desktop workspace, giving you access to the Activity Bar, Document Outline / Module Tree, Active Module Editor, Live Viewport, and Property Inspector.

---

## Desktop Workspace

`TextForge.Desktop` presents a multi-zone productivity workspace:

- **Activity Bar** — primary navigation between workspace modes.
- **Document Outline / Module Tree** — hierarchical view of document structure and modules.
- **Active Module Editor** — focused editing surface for the currently selected module.
- **Live Viewport / Rendered Preview Host** — real-time rendered preview of the compiled document.
- **Contextual Property Inspector** — inspect and edit properties of the active selection.

---

## Roadmap

- [ ] `TextForge.Web` — Blazor/WASM client reusing `TextForge.Core`
- [ ] `TextForge.Mobile` — .NET MAUI client reusing `TextForge.Core`
- [ ] Plugin/extension system for custom modules
- [ ] Template marketplace

---

## Contributing

Contributions are welcome. Please open an issue to discuss significant changes before submitting a pull request.

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

<p align="center">
  <sub>Built with .NET, Avalonia UI, and QuestPDF.</sub>
</p>
