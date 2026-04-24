# MonoBehaviour Highlighter - Setup & Usage Guide

## Quick Start

The MonoBehaviour Highlighter extension has been created in `.vscode/extensions/monobehaviour-highlighter/`.

### Step 1: Enable the Extension

1. **Reload VS Code**: Press `Ctrl+Shift+P` and select "Developer: Reload Window"
2. Or simply **close and reopen VS Code**

### Step 2: Verify Installation

1. Open any C# file containing MonoBehaviour methods (like `PlayerInputHandler.cs`)
2. You should see:
   - **Blue bold text** on lifecycle methods (Awake, Start, Update, OnEnable, OnDisable, etc.)
   - **Yellow text** on other methods
   - **Colored borders** around highlighted methods

## Features

### Automatic Highlighting
Methods are highlighted automatically as you:
- Open C# files
- Switch between editors
- Type and save code

### Color Scheme
| Category | Color | Style |
|----------|-------|-------|
| Lifecycle Methods | Blue | Bold |
| Custom Methods | Yellow | Normal |
| - | Blue Border | Indicator |
| - | Yellow Border | Indicator |

### Supported Lifecycle Methods

**Core Lifecycle**
- Awake, Start, Update, LateUpdate, FixedUpdate, OnEnable, OnDisable, OnDestroy, OnGUI

**Collision & Trigger**
- OnTriggerEnter/Stay/Exit (3D)
- OnTriggerEnter/Stay/Exit2D (2D)
- OnCollisionEnter/Stay/Exit (3D)
- OnCollisionEnter/Stay/Exit2D (2D)

**Input & Mouse**
- OnMouseDown, OnMouseUp, OnMouseOver, OnMouseEnter, OnMouseExit, OnMouseDrag

**Rendering**
- OnPreRender, OnPostRender, OnRenderObject, OnDrawGizmos, OnDrawGizmosSelected, OnWillRenderObject

**Animation**
- OnAnimatorIK, OnAnimatorMove

**Other**
- OnValidate, OnApplicationPause, OnApplicationQuit, OnApplicationFocus, OnBecameVisible, OnBecameInvisible, OnTransformParentChanged, OnTransformChildScaleChanged

## Customization

### Change Colors

Edit `.vscode/extensions/monobehaviour-highlighter/extension.js` and modify the `setupDecorations()` method:

```javascript
this.decorationTypes.set('lifecycle', vscode.window.createTextEditorDecorationType({
  color: '#569cd6',                              // Hex color
  fontStyle: 'bold',                             // 'bold', 'italic', 'normal'
  backgroundColor: 'rgba(86, 156, 214, 0.15)',  // RGBA color with opacity
  border: '1px solid rgba(86, 156, 214, 0.3)',
  borderRadius: '3px',
  overviewRulerColor: '#569cd6',
  overviewRulerLane: vscode.OverviewRulerLane.Right,
}));
```

After editing, reload VS Code.

### Add New Lifecycle Methods

In `extension.js`, add method names to the `MONOBEHAVIOUR_METHODS` array:

```javascript
const MONOBEHAVIOUR_METHODS = [
  'Awake', 'Start', 'Update',
  'MyCustomLifecycleMethod',  // Add here
];
```

### Use Advanced Highlighting

For more categories (Coroutines, Event Callbacks, etc.):

1. Rename `extension.js` to `extension-basic.js`
2. Rename `extension-advanced.js` to `extension.js`
3. Reload VS Code

The advanced version highlights:
- **Lifecycle methods** (blue)
- **Coroutines** returning IEnumerator (cyan)
- **Event callbacks** starting with "On" (orange)
- **Custom methods** (yellow)

## Troubleshooting

### Highlighting Not Showing

1. **Check file type**: Only works on `.cs` files (C#)
2. **Reload VS Code**: `Ctrl+Shift+P` → "Developer: Reload Window"
3. **Check console**: `Ctrl+Shift+P` → "Developer: Toggle Developer Tools"

### Extension Not Activating

1. Verify the extension folder exists: `.vscode/extensions/monobehaviour-highlighter/`
2. Verify `package.json` exists in that folder
3. Check output console for errors

### Performance Issues

If highlighting is slow on large files:
1. Edit `extension.js`
2. Add a size check:
```javascript
highlightDocument(editor) {
  const lineCount = editor.document.lineCount;
  if (lineCount > 2000) return; // Skip very large files
  // ... rest of code
}
```

## File Structure

```
.vscode/extensions/monobehaviour-highlighter/
├── package.json              # Extension metadata
├── extension.js              # Main highlighter (basic version)
├── extension-advanced.js     # Advanced version with more categories
├── themes/
│   └── monobehaviour-dark.json  # Theme colors
├── README.md                 # Features overview
└── USAGE.md                  # This file
```

## Example

In your `PlayerInputHandler.cs`:

```csharp
public class PlayerInputHandler : MonoBehaviour
{
    private void Awake()           // ← Blue bold (Lifecycle)
    {
        // Setup
    }

    private void OnEnable()        // ← Blue bold (Lifecycle)
    {
        // Enable
    }

    private void Update()          // ← Blue bold (Lifecycle)
    {
        // Update
    }

    private void OnMovePerformed() // ← Yellow (Custom method)
    {
        // Custom
    }
}
```

## Support

For issues or feature requests, modify `extension.js` directly or create a custom theme file in the `themes/` folder.

---

**Happy coding! 🚀**
