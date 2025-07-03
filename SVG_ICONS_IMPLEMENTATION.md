# SVG Icon Integration

## Overview
This document describes the implementation of inline SVG icons to replace Bootstrap Icons in the CycleRoutes application.

## Implementation

### 1. SvgIcon Component
Created `CycleRoutes.Shared/Components/SvgIcon.razor` - a reusable Blazor component that renders SVG icons inline.

**Features:**
- Supports multiple icon types through a string parameter
- Configurable width, height, CSS classes, and styles
- Uses `currentColor` for fill, allowing CSS color inheritance
- All icons are embedded as SVG path data

### 2. Icon Library
The following icons have been converted to SVG format:

| Icon Name | Usage | Description |
|-----------|-------|-------------|
| `chevron-left` | Navigation | Previous button, panel collapse |
| `chevron-right` | Navigation | Next button, panel expand |
| `play-fill` | Controls | Start navigation (filled) |
| `stop-fill` | Controls | Stop navigation (filled) |
| `rulers` | UI | Distance measurement control |
| `clock` | UI | Time/delay control |
| `play` | Controls | Resume auto-advance (outline) |
| `pause` | Controls | Pause auto-advance |
| `stop` | Controls | Stop auto-advance (outline) |
| `fast-forward` | Controls | Start auto-advance |
| `arrow-clockwise` | UI | Refresh/reload |

### 3. Usage

```razor
<!-- Basic usage -->
<SvgIcon Icon="chevron-left" />

<!-- With custom styling -->
<SvgIcon Icon="play-fill" CssClass="me-1" />

<!-- With custom size and style -->
<SvgIcon Icon="chevron-right" Style="transform: scale(1.2);" />
```

### 4. Benefits

1. **No External Dependencies**: Icons are embedded in the component, eliminating Bootstrap Icons dependency
2. **Performance**: No additional CSS/font files to load
3. **Scalability**: SVG icons scale perfectly at any size
4. **Customization**: Easy to modify colors via CSS `currentColor`
5. **Consistency**: All icons use the same 16x16 viewBox for consistent sizing
6. **Maintainability**: Centralized icon management in one component

### 5. File Changes

- **Created**: `CycleRoutes.Shared/Components/SvgIcon.razor`
- **Modified**: `CycleRoutes.Shared/_Imports.razor` (added component import)
- **Modified**: `CycleRoutes.Shared/Pages/Routes.razor` (replaced all Bootstrap icons)

### 6. Migration Pattern

All Bootstrap Icons were replaced following this pattern:
```razor
<!-- Before -->
<i class="bi bi-chevron-left"></i>

<!-- After -->
<SvgIcon Icon="chevron-left" />
```

CSS classes and styles were preserved by moving them to the `CssClass` and `Style` parameters respectively.

### 7. Future Extensions

To add new icons:
1. Add the icon name to the switch statement in `GetSvgPath()`
2. Add the corresponding SVG path data
3. Use the new icon via `<SvgIcon Icon="new-icon-name" />`

All SVG path data is sourced from Bootstrap Icons and maintains the same visual appearance while being embedded directly in the component.
