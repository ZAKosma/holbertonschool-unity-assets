// using UnityEditor;
// using UnityEngine;
//
// public class DialogueGraphWindow : EditorWindow
// {
//     private DialogueTreeEditor treeEditor;
//
//     [MenuItem("Dialogue System/Dialogue Graph")]
//     public static void Open()
//     {
//         DialogueGraphWindow window = GetWindow<DialogueGraphWindow>();
//         window.titleContent = new GUIContent("Dialogue Graph");
//     }
//
//     private void OnGUI()
//     {
//         // Make sure the tree editor is open
//         if (!treeEditor)
//         {
//             treeEditor = GetWindow<DialogueTreeEditor>();
//         }
//
//         // Code for drawing the graph goes here
//         foreach (DialogueGraphWindowNode node in treeEditor.dialogueGraphWindowNodes)
//         {
//             node.Position = GUILayout.Window(node.NodeData.GetInstanceID(), node.Position, DrawNodeWindow, new GUIContent(node.NodeData.dialogueLine, "Click to edit node"));
//         }
//
//         // Draw lines between connected nodes
//         foreach (DialogueGraphWindowNode node in treeEditor.dialogueGraphWindowNodes)
//         {
//             foreach (DialogueGraphWindowNode connection in node.Connections)
//             {
//                 DrawNodeCurve(node.Position, connection.Position);
//             }
//         }
//     }
//
//     private void DrawNodeWindow(int id)
//     {    
//         DialogueGraphWindowNode node = treeEditor.dialogueGraphWindowNodes.Find(n => n.NodeData.GetInstanceID() == id);
//
//         if (node != null)
//         {
//             EditorGUILayout.LabelField("Speaker: " + node.NodeData.speaker);
//             EditorGUILayout.LabelField("Dialogue: " + node.NodeData.dialogueLine);
//             EditorGUILayout.LabelField("Options: " + node.NodeData.options.Count);
//
//             if (GUILayout.Button("Delete Node"))
//             {
//                 treeEditor.dialogueGraphWindowNodes.Remove(node);
//                 // You might also want to remove this node from all connections in other nodes
//                 foreach (DialogueGraphWindowNode otherNode in treeEditor.dialogueGraphWindowNodes)
//                 {
//                     otherNode.Connections.Remove(node);
//                 }
//             }
//
//             GUI.DragWindow();
//         }
//     }
//
//     private void DrawNodeCurve(Rect start, Rect end)
//     {
//         Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
//         Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
//         Vector3 tangency = Vector3.up * 50;
//
//         Color shadowCol = new Color(0, 0, 0, 0.06f);
//         for (int i = 0; i < 3; i++) // Draw a shadow with 3 lines offset
//         {
//             Handles.DrawBezier(startPos, endPos, startPos + tangency, endPos - tangency, shadowCol, null, (i + 1) * 5); // Thicker shadow
//         }
//
//         Handles.DrawBezier(startPos, endPos, startPos + tangency, endPos - tangency, Color.black, null, 1); // Line itself
//     }
// }