const vscode = require('vscode');

const MONOBEHAVIOUR_METHODS = [
  // Lifecycle Methods
  'Awake', 'Start', 'Update', 'LateUpdate', 'FixedUpdate',
  'OnEnable', 'OnDisable', 'OnDestroy', 'OnGUI',
  
  // Physics Callbacks
  'OnTriggerEnter', 'OnTriggerStay', 'OnTriggerExit',
  'OnTriggerEnter2D', 'OnTriggerStay2D', 'OnTriggerExit2D',
  'OnCollisionEnter', 'OnCollisionStay', 'OnCollisionExit',
  'OnCollisionEnter2D', 'OnCollisionStay2D', 'OnCollisionExit2D',
  
  // Input Callbacks
  'OnMouseDown', 'OnMouseUp', 'OnMouseOver', 'OnMouseEnter',
  'OnMouseExit', 'OnMouseDrag',
  
  // Rendering Callbacks
  'OnPreRender', 'OnPostRender', 'OnRenderObject',
  'OnDrawGizmos', 'OnDrawGizmosSelected',
  
  // Animation Callbacks
  'OnAnimatorIK', 'OnAnimatorMove',
  
  // Other Important Methods
  'OnValidate', 'OnApplicationPause', 'OnApplicationQuit',
  'OnApplicationFocus', 'OnBecameVisible', 'OnBecameInvisible',
  'OnWillRenderObject', 'OnTransformParentChanged',
  'OnTransformChildScaleChanged'
];

// Method pattern that includes access modifiers and return types
const METHOD_PATTERN = /\b(public|private|protected|internal|public\s+static|private\s+static|protected\s+static|internal\s+static)?\s*(void|bool|int|float|string|Transform|GameObject|Rigidbody|Collider|Vector\d|Quaternion|Color|IEnumerator|\.\.\.)\s+(\w+)\s*\(/g;

class MonoBehaviourHighlighter {
  constructor(context) {
    this.context = context;
    this.decorationTypes = new Map();
    this.setupDecorations();
  }

  setupDecorations() {
    // Lifecycle methods - blue highlight
    this.decorationTypes.set('lifecycle', vscode.window.createTextEditorDecorationType({
      color: '#569cd6',
      fontStyle: 'bold',
      backgroundColor: 'rgba(86, 156, 214, 0.15)',
      border: '1px solid rgba(86, 156, 214, 0.3)',
      borderRadius: '3px',
      overviewRulerColor: '#569cd6',
      overviewRulerLane: vscode.OverviewRulerLane.Right,
    }));

    // Custom methods - orange highlight
    this.decorationTypes.set('custom', vscode.window.createTextEditorDecorationType({
      color: '#dcdcaa',
      fontStyle: 'normal',
      backgroundColor: 'rgba(220, 220, 170, 0.08)',
      border: '1px solid rgba(220, 220, 170, 0.2)',
      borderRadius: '3px',
      overviewRulerColor: '#dcdcaa',
      overviewRulerLane: vscode.OverviewRulerLane.Right,
    }));
  }

  highlightDocument(editor) {
    if (!editor) return;
    
    const document = editor.document;
    if (document.languageId !== 'csharp') return;

    const lifecycleMethods = [];
    const customMethods = [];

    const text = document.getText();
    let match;

    // Find all method declarations
    while ((match = METHOD_PATTERN.exec(text)) !== null) {
      const methodName = match[3];
      const fullMatch = match[0];
      const startPos = match.index;
      const endPos = startPos + fullMatch.lastIndexOf(methodName) + methodName.length;

      // Convert offset to line and column
      const startOffset = startPos + (match[0].length - methodName.length - 1);
      const position = document.positionAt(startOffset);

      // Check if method name matches MonoBehaviour lifecycle
      if (MONOBEHAVIOUR_METHODS.includes(methodName)) {
        lifecycleMethods.push(
          new vscode.Range(
            position,
            position.translate(0, methodName.length)
          )
        );
      } else if (!this.isPrivateLocal(methodName)) {
        // Highlight other public methods
        customMethods.push(
          new vscode.Range(
            position,
            position.translate(0, methodName.length)
          )
        );
      }
    }

    // Apply decorations
    editor.setDecorations(this.decorationTypes.get('lifecycle'), lifecycleMethods);
    editor.setDecorations(this.decorationTypes.get('custom'), customMethods);
  }

  isPrivateLocal(methodName) {
    // Skip very short names that are likely local variables
    return methodName.length < 3 && methodName.toLowerCase() === methodName;
  }
}

exports.activate = function(context) {
  const highlighter = new MonoBehaviourHighlighter(context);

  // Highlight active editor
  if (vscode.window.activeTextEditor) {
    highlighter.highlightDocument(vscode.window.activeTextEditor);
  }

  // Listen to editor changes
  vscode.window.onDidChangeActiveTextEditor(editor => {
    if (editor) {
      highlighter.highlightDocument(editor);
    }
  }, null, context.subscriptions);

  // Listen to document changes
  vscode.workspace.onDidChangeTextDocument(event => {
    const editor = vscode.window.visibleTextEditors.find(
      e => e.document === event.document
    );
    if (editor) {
      highlighter.highlightDocument(editor);
    }
  }, null, context.subscriptions);

  // Re-highlight when configuration changes
  vscode.workspace.onDidChangeConfiguration(event => {
    if (event.affectsConfiguration('monobehaviourHighlighter')) {
      if (vscode.window.activeTextEditor) {
        highlighter.highlightDocument(vscode.window.activeTextEditor);
      }
    }
  }, null, context.subscriptions);

  console.log('MonoBehaviour Highlighter activated');
};

exports.deactivate = function() {};
