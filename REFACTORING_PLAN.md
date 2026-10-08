# TextForge Refactoring Plan

## Executive Summary

The codebase has a **Module class that acts as a "God object"** mixing domain model, UI state, hierarchy management, cloning, and factory methods. The module editing system is tightly coupled with UI concerns, making it fragile and hard to maintain.

---

## Critical Issues Identified

### 1. Module.cs (262 lines) - God Class
**Location:** `src/TextForge.Core/Modules/Module.cs`

**Problems:**
- **Mixed concerns**: Domain data + UI state (`IsSelected`, `IsExpanded`, `INotifyPropertyChanged`) + hierarchy management + cloning + factory methods + truncation logic
- **Parent reference management**: `Parent` property with internal setter, but `Clone()` doesn't properly maintain parent-child relationships
- **Factory methods in domain model**: `CreateTitle`, `CreateHeading`, etc. belong in a factory/registry, not the entity
- **Truncation logic in entity**: `GetTruncatedPreview` is a presentation concern
- **ConnectionState** is a runtime UI state, not domain data

### 2. ModuleRegistry.cs - Static Registry
**Location:** `src/TextForge.Core/Modules/ModuleRegistry.cs`

**Problems:**
- Static class - cannot be mocked, extended, or replaced
- Hardcoded module definitions with lambdas
- No support for plugin/extensible module types

### 3. ModuleEditorView - Recursive XAML Template
**Location:** `src/TextForge.Desktop/Views/Components/ModuleEditorView.axaml` (188 lines)

**Problems:**
- Single massive recursive `DataTemplate` handling all depths
- Action buttons with `Tag="{Binding}"` pattern - fragile
- Scroll suppression hack in code-behind
- No separation between view and view-model

### 4. Document.cs - Overloaded Responsibilities
**Location:** `src/TextForge.Core/Documents/Document.cs` (300 lines)

**Problems:**
- Selection management (`SelectedModule`, `IsRootSelected`, `SelectRoot`, `SelectModule`)
- Mutation operations (`AddModule`, `RemoveModule`, `ClearModules`)
- Reordering (`MoveModuleUp`, `MoveModuleDown`, `DetachModule`)
- Event dispatching (`NotifyChanged`)
- Hierarchy traversal logic duplicated in multiple methods

### 5. Duplicate Evaluation Logic
**Locations:** 
- `DocumentEngine.EvaluateModule()` (lines 38-57)
- `PreviewRenderer.RenderModule()` (lines 24-36)

**Problem:** Nearly identical recursive tree traversal with template resolution

### 6. ModuleFeatures.MergeWith - Flawed Logic
**Location:** `src/TextForge.Core/Modules/ModuleFeatures.cs` (lines 23-41)

**Problem:**
```csharp
FontWeight = FontWeight != ModuleFontWeight.Normal ? FontWeight : fallback.FontWeight,
```
This checks *value* not *whether it was explicitly set*. A module with explicit `FontWeight.Normal` won't inherit fallback's Bold.

### 7. Confusing Identity Properties
**Module.cs lines 19, 21, 23, 25:**
- `Id` (Guid) - unique identity
- `Name` (string) - "default-text", "default-heading" - archetype identifier
- `StyleKey` (string?) - "Title", "Heading", "Body" - template style key
- `Type` (ModuleType) - Text, Section, Container, etc.

No clear distinction between these concepts.

---

## Refactoring Strategy: Incremental Steps

### Phase 1: Extract Pure Domain Models (No UI Dependencies)
**Goal:** Separate domain logic from UI concerns

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 1.1 | Create `ModuleData` record (immutable domain model) | New file, Module.cs | Low |
| 1.2 | Create `ModuleNode` for hierarchy (separate from data) | New file, Module.cs | Low |
| 1.3 | Move `GetTruncatedPreview` to a `ModulePreviewService` | Module.cs → new service | Low |
| 1.4 | Extract `ModuleFactory` from static methods | Module.cs, ModuleRegistry.cs | Medium |

### Phase 2: Decouple Module Registry
**Goal:** Make registry testable and extensible

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 2.1 | Convert `ModuleRegistry` to interface + implementation | ModuleRegistry.cs | Medium |
| 2.2 | Register as singleton in DI container | Program.cs, App.axaml.cs | Low |
| 2.3 | Add support for dynamic module registration | ModuleRegistry.cs | Low |

### Phase 3: Fix ModuleFeatures Merge Logic
**Goal:** Correct cascading behavior

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 3.1 | Change ModuleFeatures to use `Option<T>` or nullable with explicit "unset" | ModuleFeatures.cs | Medium |
| 3.2 | Update `MergeWith` to check `HasValue` not value equality | ModuleFeatures.cs | Medium |
| 3.3 | Update DefaultTemplate to use new semantics | DefaultTemplate.cs | Medium |

### Phase 4: Split Document Responsibilities
**Goal:** Single responsibility per class

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 4.1 | Extract `DocumentSelectionService` | Document.cs | Medium |
| 4.2 | Extract `DocumentMutationService` (Add/Remove/Clear) | Document.cs | Medium |
| 4.3 | Extract `DocumentReorderService` (MoveUp/Down/Detach) | Document.cs | Medium |
| 4.4 | Keep `Document` as pure data aggregate | Document.cs | Low |

### Phase 5: Unify Evaluation Pipeline
**Goal:** Single source of truth for rendering

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 5.1 | Create `RenderTreeBuilder` service | DocumentEngine.cs, PreviewRenderer.cs | Medium |
| 5.2 | Make `DocumentEngine` use `RenderTreeBuilder` | DocumentEngine.cs | Low |
| 5.3 | Make `PreviewRenderer` use `RenderTreeBuilder` | PreviewRenderer.cs | Low |
| 5.4 | Remove duplicate logic | PreviewRenderer.cs | Low |

### Phase 6: Refactor ModuleEditorView (UI)
**Goal:** Maintainable, testable UI components

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 6.1 | Create `ModuleViewModel` wrapping `ModuleNode` | New ViewModels/ | Medium |
| 6.2 | Replace recursive XAML with `TreeView` or `ItemsRepeater` | ModuleEditorView.axaml | High |
| 6.3 | Move action handlers to ViewModel commands | ModuleEditorView.axaml.cs | Medium |
| 6.4 | Remove scroll suppression hack | ModuleEditorView.axaml.cs | Low |

### Phase 7: Clarify Identity Semantics
**Goal:** Clear, consistent naming

| Step | Task | Files Affected | Risk |
|------|------|----------------|------|
| 7.1 | Rename `Module.Name` → `ArchetypeId` | Module.cs, all consumers | High |
| 7.2 | Rename `Module.StyleKey` → `StyleName` | Module.cs, DefaultTemplate.cs | High |
| 7.3 | Document semantics of each property | All files | Low |

---

## Migration Approach

### For Each Phase:
1. **Write tests first** for new behavior (if not covered)
2. **Create new types** alongside old ones
3. **Migrate consumers one at a time** (Document → Engine → Preview → UI)
4. **Run full test suite** after each step
5. **Delete old code** only after zero references remain

### Safety Nets:
- All existing tests must pass at each step
- Add integration tests for critical paths (document → render → PDF)
- Use compiler errors to find all call sites

---

## Priority Order

**Start Here (Highest Impact, Lowest Risk):**
1. **Phase 3** - Fix ModuleFeatures merge logic (bug fix, isolated)
2. **Phase 1.1-1.3** - Extract domain models (foundational)
3. **Phase 5** - Unify evaluation (removes duplication)

**Then:**
4. **Phase 2** - Decouple registry (enables testing)
5. **Phase 4** - Split Document (improves maintainability)
6. **Phase 7** - Clarify identity (breaking but necessary)

**Last (Highest Risk):**
7. **Phase 6** - UI refactor (visual regression risk)

---

## Estimated Effort

| Phase | Estimated Steps | Risk Level |
|-------|-----------------|------------|
| 1 | 4 | Low |
| 2 | 3 | Medium |
| 3 | 3 | Medium |
| 4 | 4 | Medium |
| 5 | 4 | Medium |
| 6 | 4 | High |
| 7 | 3 | High |

**Total: ~25 small, verifiable steps**

---

## Next Action

Shall I start with **Phase 3** (fix ModuleFeatures.MergeWith) or **Phase 1.1** (extract ModuleData record)? Both are low-risk and high-value.