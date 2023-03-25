using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptGenerator {
[InitializeOnLoad]
public class ScriptGeneratorButton : Editor {
    public static GameObject SelectedGameObject;

    private const double UpdateInterval = 0.1f;
    private static double _lastUpdate;
    private static Button _button;

    static ScriptGeneratorButton() {
        EditorApplication.update += EditorUpdate;

        Selection.selectionChanged += () => {
            if (_button == null) return;
            _button.visible = Selection.activeGameObject != null;
        };
    }

    private static void EditorUpdate() {
        if (EditorApplication.timeSinceStartup - _lastUpdate < UpdateInterval) return;
        _lastUpdate = EditorApplication.timeSinceStartup;

        var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        foreach (var window in windows) {
            if (window.GetType().Name == "InspectorWindow") {
                var buttons = window.rootVisualElement.Q(className: "unity-inspector-add-component-button");
                if (buttons == null) continue;

                var container = new VisualElement {
                    style = { flexDirection = FlexDirection.Row, justifyContent = Justify.Center, marginTop = -10 }
                };
                buttons.parent.Add(container);
                var button = new Button(ClickEvent) {
                    text = "Generate Component", style = { width = 230, height = 25, marginLeft = 2 }
                };
                container.Add(button);

                if (_button != null) {
                    _button.clicked -= ClickEvent;
                    _button.RemoveFromHierarchy();
                }

                _button = button;

                EditorApplication.update -= EditorUpdate;
            }
        }
    }

    private static void ClickEvent() {
        var selection = Selection.activeGameObject;
        SelectedGameObject = selection;

        var window = EditorWindow.GetWindow<ScriptGenerator>(true, "Generate Component");
        window.Show();

        var inspector = Resources.FindObjectsOfTypeAll<EditorWindow>()
            .FirstOrDefault(w => w.GetType().Name == "InspectorWindow");
        if (inspector != null) {
            window.position = new Rect(inspector.position.x, _button.worldBound.yMax + 140, inspector.position.width,
                                       height: 120);
        }
    }
}
}