## Phase 1: App Shell - MainWindow with Grid Layout

### Goal
Create the main application window with a proper grid-based layout structure.

### Tasks
- [ ] Create `MainWindow.axaml` with Grid layout (rows: Toolbar, Workspace, StatusBar)
- [ ] Create `MainWindowViewModel.cs` with basic properties
- [ ] Implement responsive grid columns: ActivityBar (auto), Sidebar (auto), Splitter, Editor (4*), Splitter, Preview (6*)
- [ ] Add basic styling: backgrounds, borders, spacing

### Learning Resources
- **Avalonia Grid Layout**: https://docs.avaloniaui.net/docs/reference/controls/grid
- **Responsive Layouts**: https://docs.avaloniaui.net/docs/guides/responsive-design
- **Window Chrome**: https://docs.avaloniaui.net/docs/reference/controls/window

### Acceptance Criteria
- [ ] MainWindow renders with correct grid structure
- [ ] Columns resize properly with GridSplitters
- [ ] No hardcoded widths/heights (use *, Auto, min/max)