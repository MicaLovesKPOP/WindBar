# WindBar

WindBar is an Explorer-safe Windows 11 taskbar replacement prototype written in C# and WPF. It keeps Explorer alive, draws its own topmost taskbar, and provides the groundwork for a modular taskbar with Start menu providers, widgets, themes, top and bottom placement, auto-hide, smart app discovery, persistent pinned apps, running-app visibility/activation/toggling, and future per-monitor layouts.

## Current prototype

The current build is a code-only WPF app targeting .NET 8. It avoids XAML build issues by disabling default XAML compilation and creating the UI from C#.

Implemented now:

- Top or bottom taskbar placement on the primary monitor.
- Auto-hide prototype.
- Light, dark, OLED and transparent theme modes.
- Left, center and right taskbar zones.
- Module visibility flags for Start, Search, Start switcher, pinned apps, running apps, theme, placement, auto-hide, settings and clock.
- Settings window for toggling modules, changing theme, switching top/bottom placement, enabling auto-hide and choosing Start style.
- Start provider switching buttons.
- Windows 11 style Start provider.
- Modern Start provider inspired by the Windows 10 layout.
- Full-screen Start provider inspired by Windows 8.1.
- Classic Start provider.
- Clock module.
- Persistent pinned app model, store and service.
- Center taskbar zone loads saved pins or creates first-run pins from smart discovery.
- Center taskbar zone can also show running apps and refreshes that list periodically.
- Running-app buttons can activate/restore their windows through Win32.
- Running-app buttons now show a different marker for the current foreground window.
- Clicking an already-foreground running app minimizes it, while clicking an inactive running app restores/activates it.
- Core settings model.
- Settings persistence in the user AppData folder.
- Widget contract and widget manager.
- App scanner for Start menu shortcuts, desktop shortcuts and selected portable app folders.
- Start surfaces that populate from discovered apps and group them by simple heuristics.
- GitHub Actions build workflow on Windows.

## Still needed for real taskbar parity

- Grouping by app identity.
- Task thumbnails and preview flyouts.
- Jump lists.
- Tray hosting.
- Appbar registration and work-area reservation.
- Multi-monitor support.

## Product direction

WindBar should eventually feel indistinguishable from the Windows 11 taskbar by default while giving power users much deeper control.

The final product should support:

- A native-feeling Windows 11 taskbar design.
- Optional modules for Start, search, pinned apps, running apps, tray, clock, action center, widgets and media.
- User-controlled left, center and right placement of modules.
- Fully functional Windows 11, Windows 10 and Windows 8.1 Start experiences using Windows 11 visual language.
- Smart app discovery and grouping for installed apps and portable apps.
- Top and bottom placement first, then left, right and per-monitor placement later.
- Light, dark, OLED and transparent themes.
- Appbar registration for proper work-area reservation.
- Safe fallback to the native Windows taskbar.

## Build

Open windbar.sln in Visual Studio 2022 or newer and build WindBar.App.

The next milestone is a safer app identity/grouping model, followed by preview thumbnails and tray/appbar hosting.
