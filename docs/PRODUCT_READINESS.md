# WindBar product readiness

This document tracks the requirements that must be met before WindBar can be honestly described as meeting the current product goals. It separates near-term requirements from clearly future-facing work.

## Current verdict

WindBar does **not** meet the current non-future requirements yet.

It is a useful prototype foundation, but it is not yet a robust release-ready Windows taskbar replacement. The most important missing pieces are reliable taskbar-level behavior, robust layout and settings, full app identity matching, tray/appbar support, and validation on real Windows installs.

## Non-future requirements before calling it ready

These are the requirements that should be considered part of the current product target, not far-future wishlist items.

### Core shell behavior

- [x] Keep Explorer alive instead of destructively replacing it.
- [x] Draw a custom topmost taskbar surface.
- [x] Support bottom placement.
- [x] Support top placement.
- [ ] Reserve screen work area using appbar registration.
- [ ] Avoid overlapping maximized windows.
- [ ] Provide a safe fallback/exit path back to the native Windows taskbar.
- [ ] Survive Explorer restarts and common shell state changes.

### Taskbar modules

- [x] Separate left, center and right zones.
- [x] Start button module.
- [x] Search placeholder module.
- [x] Pinned apps module.
- [x] Running apps module.
- [x] Clock module.
- [x] Settings module.
- [x] Universal media miniplayer module.
- [ ] System tray/notification-area module.
- [ ] Module ordering and zone assignment controlled by settings rather than hardcoded order.

### Running apps and pinned apps

- [x] Persistent pinned app storage.
- [x] First-run pin generation from app discovery.
- [x] Running app discovery.
- [x] Running app activation.
- [x] Minimize currently active running app on click.
- [x] Running apps grouped by executable identity.
- [x] Direct executable-path matching between pinned and running apps.
- [ ] Shortcut/AppUserModelID identity matching between pinned and running apps.
- [x] Group multiple windows from the same app.
- [ ] Stable app icons instead of text-only buttons.
- [x] Reasonable overflow handling when there are too many pinned/running apps.

### Start menu surfaces

- [x] Windows 11-style Start surface.
- [x] Modern Windows 10-inspired Start surface.
- [x] Full-screen Windows 8.1-inspired Start surface.
- [x] Classic Start surface.
- [x] Start provider switching.
- [x] Populate Start surfaces from discovered apps.
- [x] Launch apps directly from Start surfaces.
- [x] Search/filter apps from Start surfaces.
- [ ] Better app categorization and deduplication.

### Universal media miniplayer

- [x] Universal media provider contract.
- [x] Windows media-session provider.
- [x] Sample fallback provider.
- [x] Compact taskbar miniplayer.
- [x] Cassette/CD-style slideout panel.
- [x] Theme-aware miniplayer visuals.
- [x] Progress/time display when available.
- [x] Disable unsupported controls instead of breaking layout.
- [ ] Artwork display when available.
- [ ] Source preference handling when multiple sessions are present.
- [ ] No-media behavior setting: hide, collapse or muted last source.

### Themes and visual polish

- [x] Light theme.
- [x] Dark theme.
- [x] OLED theme.
- [x] Transparent theme.
- [x] Basic theme switching.
- [ ] Consistent Windows 11-style hover/pressed/focus states across all controls.
- [ ] Rounded surfaces and spacing polished across every module.
- [ ] Visual distinction between pinned, running and active apps.
- [ ] Keyboard accessibility and focus styling.

### Settings and persistence

- [x] Persist main settings to AppData.
- [x] Persist pinned apps to AppData.
- [x] Settings window for common module visibility and basic choices.
- [x] Settings reset/recovery path if the user hides important controls.
- [ ] Settings for module order and zone placement.
- [ ] Settings for media no-session behavior and preferred source.
- [ ] Settings import/export.

### Robustness and release readiness

- [x] GitHub Actions build workflow exists.
- [ ] Current CI build verified green after the latest media changes.
- [ ] Error logging that helps diagnose failures.
- [ ] No silent swallowing of critical startup failures.
- [ ] Tested on a clean Windows 11 install.
- [ ] Tested with top and bottom placement.
- [ ] Tested with multiple common media sources.
- [ ] Tested with many open windows.
- [ ] Packaged release artifact or installer.
- [ ] Clear user documentation for install, exit, recovery and known limitations.

## Future-facing requirements

These are important, but they are not blockers for the first meaningful release unless promoted into the current target.

- Left and right vertical taskbar placement.
- Per-monitor taskbars.
- Thumbnail previews.
- Jump lists.
- Rich widget marketplace/plugin loading.
- Advanced layout editor.
- Deep shell integration beyond appbar/tray basics.
- Rich animations everywhere.
- Full custom Start menu parity with Windows.

## Rule for readiness claims

Do not call WindBar ready, complete, release-ready or meeting the user's requirements until every item in the non-future requirements section is checked or deliberately moved to future-facing requirements with a clear reason.
