using DialogueSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView _graphView;

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
    
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node", Vector2.zero); }) { text = "Create Node" };

            toolbar.Add(nodeCreateButton);

            rootVisualElement.Add(toolbar);
        }
    }
}