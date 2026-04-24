const vscode = require('vscode');

/**
 * ADVANCED: Extended MonoBehaviour Highlighter with custom rules
 * Copy this to extension.js to use these features
 */

// Color presets
const COLORS = {
  lifecycle: {
    color: '#569cd6',
    backgroundColor: 'rgba(86, 156, 214, 0.15)',
    border: '1px solid rgba(86, 156, 214, 0.3)'
  },
  coroutine: {
    color: '#4ec9b0',
    backgroundColor: 'rgba(78, 201, 176, 0.15)',
    border: '1px solid rgba(78, 201, 176, 0.3)'
  },
  callback: {
    color: '#ce9178',
    backgroundColor: 'rgba(206, 145, 120, 0.15)',
    border: '1px solid rgba(206, 145, 120, 0.3)'
  },
  custom: {
    color: '#dcdcaa',
    backgroundColor: 'rgba(220, 220, 170, 0.08)',
    border: '1px solid rgba(220, 220, 170, 0.2)'
  }
};

class AdvancedMonoBehaviourHighlighter {
  constructor(context) {
    this.context = context;
    this.decorationTypes = new Map();
    this.setupDecorations();
  }

  setupDecorations() {
    Object.entries(COLORS).forEach(([key, colorConfig]) => {
      this.decorationTypes.set(key, vscode.window.createTextEditorDecorationType({
        color: colorConfig.color,
        backgroundColor: colorConfig.backgroundColor,
        border: colorConfig.border,
        borderRadius: '3px',
        overviewRulerColor: colorConfig.color,
        overviewRulerLane: vscode.OverviewRulerLane.Right,
      }));
    });
  }

  categorizeMethod(methodName, methodBody) {
    // Lifecycle methods
    if (this.isLifecycleMethod(methodName)) {
      return 'lifecycle';
    }
    
    // Coroutine methods (return IEnumerator)
    if (methodBody.includes('IEnumerator')) {
      return 'coroutine';
    }
    
    // Event callbacks
    if (this.isEventCallback(methodName)) {
      return 'callback';
    }
    
    return 'custom';
  }

  isLifecycleMethod(name) {
    const lifecycleMethods = [
      'Awake', 'Start', 'Update', 'LateUpdate', 'FixedUpdate',
      'OnEnable', 'OnDisable', 'OnDestroy', 'OnGUI',
      'OnTriggerEnter', 'OnTriggerStay', 'OnTriggerExit',
      'OnTriggerEnter2D', 'OnTriggerStay2D', 'OnTriggerExit2D',
      'OnCollisionEnter', 'OnCollisionStay', 'OnCollisionExit',
      'OnCollisionEnter2D', 'OnCollisionStay2D', 'OnCollisionExit2D',
      'OnMouseDown', 'OnMouseUp', 'OnMouseOver', 'OnMouseEnter',
      'OnMouseExit', 'OnMouseDrag', 'OnPreRender', 'OnPostRender',
      'OnRenderObject', 'OnDrawGizmos', 'OnDrawGizmosSelected',
      'OnAnimatorIK', 'OnAnimatorMove', 'OnValidate',
      'OnApplicationPause', 'OnApplicationQuit', 'OnApplicationFocus',
      'OnBecameVisible', 'OnBecameInvisible', 'OnWillRenderObject',
      'OnTransformParentChanged', 'OnTransformChildScaleChanged'
    ];
    return lifecycleMethods.includes(name);
  }

  isEventCallback(name) {
    return name.startsWith('On') && !this.isLifecycleMethod(name);
  }

  highlightDocument(editor) {
    if (!editor || editor.document.languageId !== 'csharp') return;

    const document = editor.document;
    const decorationsByType = new Map();
    
    // Initialize arrays for each decoration type
    for (const key of this.decorationTypes.keys()) {
      decorationsByType.set(key, []);
    }

    const methodPattern = /\b(?:public|private|protected|internal|public\s+static|private\s+static|protected\s+static|internal\s+static)?\s*(?:void|bool|int|float|string|Transform|GameObject|Rigidbody|Collider|Vector\d|Quaternion|Color|IEnumerator|\.\.\.)\s+(\w+)\s*\(/g;

    let match;
    const text = document.getText();

    while ((match = methodPattern.exec(text)) !== null) {
      const methodName = match[1];
      const matchStart = match.index + (match[0].length - methodName.length - 1);
      const position = document.positionAt(matchStart);
      
      const category = this.categorizeMethod(methodName, text.substring(matchStart, Math.min(matchStart + 500, text.length)));
      
      decorationsByType.get(category).push(
        new vscode.Range(position, position.translate(0, methodName.length))
      );
    }

    // Apply all decorations
    for (const [type, ranges] of decorationsByType) {
      editor.setDecorations(this.decorationTypes.get(type), ranges);
    }
  }
}

exports.activate = function(context) {
  const highlighter = new AdvancedMonoBehaviourHighlighter(context);

  if (vscode.window.activeTextEditor) {
    highlighter.highlightDocument(vscode.window.activeTextEditor);
  }

  vscode.window.onDidChangeActiveTextEditor(editor => {
    highlighter.highlightDocument(editor);
  }, null, context.subscriptions);

  vscode.workspace.onDidChangeTextDocument(event => {
    const editor = vscode.window.visibleTextEditors.find(e => e.document === event.document);
    if (editor) {
      highlighter.highlightDocument(editor);
    }
  }, null, context.subscriptions);

  console.log('Advanced MonoBehaviour Highlighter activated');
};

exports.deactivate = function() {};
