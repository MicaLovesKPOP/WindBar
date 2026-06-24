# WindBar module layout

WindBar now stores taskbar module placement as settings data instead of treating left, center and right placement as a permanent assumption.

## Layout model

Each module has:

- `Id`: stable module identifier.
- `Zone`: `Left`, `Center` or `Right`.
- `Order`: numeric ordering inside that zone.

Default module ids:

- `start`
- `search`
- `start.switcher`
- `taskbar.apps`
- `media`
- `settings`
- `theme`
- `placement`
- `autohide`
- `clock`

## Default placement

The default layout keeps the familiar Windows-like shape:

- Left: Start, Search, Start switcher.
- Center: pinned and running apps.
- Right: media, settings, theme, placement, auto-hide and clock.

## Loading behavior

Older settings files are repaired at load time. Missing module placement entries are added with the current default zone and order. This lets new modules be introduced without breaking existing users' saved settings.

## Renderer intent

The taskbar renderer should iterate over `WindBarSettings.GetOrderedModuleLayout()` and ask each known module id to build its UI element. The returned element should then be inserted into the saved zone. Hidden modules should return no element, while unknown module ids should be ignored.

The center app module should remain internally refreshable so the running-app timer can update pinned/running apps without rebuilding the entire taskbar surface.

## Current limitation

The settings/data model is implemented, and settings loading repairs old files. The main renderer still needs to be switched fully from hardcoded module placement to the saved layout model.
