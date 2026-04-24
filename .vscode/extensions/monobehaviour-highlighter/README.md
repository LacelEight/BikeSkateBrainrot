# MonoBehaviour Highlighter

Highlights Unity MonoBehaviour lifecycle methods and other methods with custom syntax highlighting in VS Code, similar to how VS Code highlights language keywords.

## Features

- **Lifecycle Methods Highlighting** - Automatically highlights all Unity lifecycle methods (Awake, Start, Update, OnTriggerEnter, etc.) in blue with bold text
- **Custom Method Highlighting** - Highlights other methods in yellow for easy differentiation
- **Real-time Updates** - Highlights update as you type
- **Visual Indicators** - Includes colored borders and overview ruler marks for easy navigation

## Supported MonoBehaviour Methods

### Lifecycle Methods
- Awake, Start, Update, LateUpdate, FixedUpdate, OnEnable, OnDisable, OnDestroy, OnGUI

### Physics Callbacks
- OnTriggerEnter, OnTriggerStay, OnTriggerExit, OnTriggerEnter2D, OnTriggerStay2D, OnTriggerExit2D
- OnCollisionEnter, OnCollisionStay, OnCollisionExit, OnCollisionEnter2D, OnCollisionStay2D, OnCollisionExit2D

### Input Callbacks
- OnMouseDown, OnMouseUp, OnMouseOver, OnMouseEnter, OnMouseExit, OnMouseDrag

### Rendering Callbacks
- OnPreRender, OnPostRender, OnRenderObject, OnDrawGizmos, OnDrawGizmosSelected

### Animation Callbacks
- OnAnimatorIK, OnAnimatorMove

### Other Methods
- OnValidate, OnApplicationPause, OnApplicationQuit, OnApplicationFocus, OnBecameVisible, OnBecameInvisible, OnWillRenderObject, OnTransformParentChanged, OnTransformChildScaleChanged

## Installation

1. The extension files are already in `.vscode/extensions/monobehaviour-highlighter/`
2. Reload VS Code or restart the editor
3. Open any C# file in your Unity project

## Color Scheme

- **Blue (Bold)** - MonoBehaviour lifecycle methods
- **Yellow** - Custom public methods
- **Blue borders** - Visual indicator for lifecycle methods
- **Yellow borders** - Visual indicator for custom methods

## How It Works

The extension scans C# files for method declarations and:
1. Identifies methods that match MonoBehaviour lifecycle names
2. Applies bold blue highlighting with background color
3. Applies yellow highlighting to other methods
4. Updates decorations in real-time as you edit

## Customization

You can customize colors by editing `extension.js` in the `setupDecorations()` method:

```javascript
this.decorationTypes.set('lifecycle', vscode.window.createTextEditorDecorationType({
  color: '#569cd6',        // Change this hex color
  fontStyle: 'bold',       // Change to 'italic' or 'normal'
  backgroundColor: 'rgba(86, 156, 214, 0.15)',  // Change background
}));
```

## Requirements

- Visual Studio Code 1.70+
- C# extension
- Unity project with MonoBehaviour scripts

## Known Limitations

- May have performance impact on very large files (1000+ lines)
- Does not differentiate between MonoBehaviour and non-MonoBehaviour classes (highlights all methods)
- Requires manual reload if extension code is modified

## License

MIT
