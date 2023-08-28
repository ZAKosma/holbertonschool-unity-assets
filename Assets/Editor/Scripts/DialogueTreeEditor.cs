// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// public class DialogueTreeEditor : EditorWindow
// {
//     // Serialized fields
//     [SerializeField] private DialogueTree _currentDialogueTree;
//     
//     // Private variables
//     private Vector2 _scrollPosition;
//     private Rect _entryPointRect = new Rect(10, 10, 100, 50);
//     public List<DialogueGraphWindowNode> dialogueGraphWindowNodes = new List<DialogueGraphWindowNode>();
//     private DialogueGraphWindowNode selectedNode = null;
//     private bool isDragging;
//     private bool isDraggingEntryPoint;
//     private Vector2 startDragPosition;
//     
//     private Dictionary<string, int> nodeIdToIndexMap = new Dictionary<string, int>();
//
//     private float _shortDistance = 200f;
//
//
//     public DialogueTree CurrentDialogueTree
//     {
//         get { return _currentDialogueTree; }
//         set
//         {
//             _currentDialogueTree = value;
//             UpdateNodes();
//         }
//     }
//
//     [MenuItem("Dialogue System/Dialogue Tree Editor")]
//     public static void ShowWindow()
//     {
//         GetWindow<DialogueTreeEditor>();
//     }
//
//     // Method to update nodes in the Dialogue Tree Editor
//     private void UpdateNodes()
//     {
//         if (_currentDialogueTree == null || _currentDialogueTree.nodes == null)
//         {
//             Debug.LogError("CurrentDialogueTree or its nodes are null");
//             return;
//         }
//
//         dialogueGraphWindowNodes.Clear();
//         nodeIdToIndexMap.Clear();
//
//         int index = 0;
//         foreach (DialogueNode node in _currentDialogueTree.nodes)
//         {
//             Debug.Log($"Updating Node {index}, Node ID: {node.id}, Node Position: {node.position}");
//         
//             DialogueGraphWindowNode graphWindowNode = new DialogueGraphWindowNode
//             {
//                 NodeData = node,
//                 // Use the node's saved position instead of calculating a new one
//                 Position = new Rect(node.position.x, node.position.y, 200, 100)
//             };
//         
//             Debug.Log($"Created GraphWindowNode with Position: {graphWindowNode.Position.position}");
//
//             dialogueGraphWindowNodes.Add(graphWindowNode);
//             nodeIdToIndexMap[node.id] = index;
//             index++;
//         }
//     }
//
//
//
//     private void OnGUI()
//     {
//         _entryPointRect.position = new Vector2(position.width / 2, position.height / 2);
//
//         // Set the GUI up
//         SetUpGUI();
//
//         // Handle the Dialogue Tree data
//         HandleDialogueTree();
//
//         // Draw Node Editor
//         DrawNodeEditor();
//
//         // Draw a line from the Entry Point to the first node, if any
//         if (dialogueGraphWindowNodes.Any())
//         {
//             Vector3 startPoint = new Vector3(_entryPointRect.x + _entryPointRect.width / 2, _entryPointRect.y + _entryPointRect.height, 0);
//             Vector3 endPoint = new Vector3(dialogueGraphWindowNodes.First().Position.x, dialogueGraphWindowNodes.First().Position.y, 0);
//             Handles.DrawLine(startPoint, endPoint);
//         }
//
//         // Handle the Properties Editor
//         HandlePropertiesEditor();
//
//         // Handle Mouse Events
//         HandleMouseEvents();
//
//         // Call Repaint after selecting a node or moving a node
//         if (Event.current.type is EventType.MouseDown or EventType.MouseDrag)
//         {
//             Repaint();
//         }
//
//         if (GUI.changed)
//         {
//             // Save the changes if anything is modified
//             EditorUtility.SetDirty(CurrentDialogueTree);
//         }
//     }
//     
//     // Other methods to handle UI, mouse events, saving, sorting, etc...
//
//     // Method to handle GUI setup
//     private void SetUpGUI()
//     {
//         // Set minimum window size
//         minSize = new Vector2(600, 400);
//         
//         // Draw Entry Point
//         GUI.color = Color.green;
//         GUI.Box(_entryPointRect, "Entry");
//         GUI.color = Color.white;
//
//         // Added this line to allow dragging the entry point
//         _entryPointRect = GUI.Window(-1, _entryPointRect, DrawEntryPointWindow, GUIContent.none);
//     }
//
//     // Method to handle Dialogue Tree
//     private void HandleDialogueTree()
//     {
//         // Create Toolbar
//         GUILayout.BeginHorizontal("Toolbar");
//         {
//             GUILayout.Label("Dialogue Tree Editor", EditorStyles.largeLabel);
//             GUILayout.Space(10);
//             CurrentDialogueTree = (DialogueTree)EditorGUILayout.ObjectField(CurrentDialogueTree, typeof(DialogueTree), false);
//
//             if (CurrentDialogueTree == null)
//             {
//                 if (GUILayout.Button("Create New"))
//                 {
//                     CurrentDialogueTree = CreateInstance<DialogueTree>();
//                     CurrentDialogueTree.nodes = new List<DialogueNode>();
//                     AssetDatabase.CreateAsset(CurrentDialogueTree, "Assets/DialogueTrees/NewDialogueTree.asset");
//                     AssetDatabase.SaveAssets();
//                 }
//             }
//             else
//             {
//                 HandleDialogueTreeOptions();
//             }
//         }
//         GUILayout.EndHorizontal();
//     }
//
//     // Method to handle Dialogue Tree options
//     private void HandleDialogueTreeOptions()
//     {
//         if (GUILayout.Button("Add Node"))
//         {
//             AddNodeToCurrentDialogueTree();
//             UpdateNodes();
//         }
//         if (GUILayout.Button("Auto Sort"))
//         {
//             SortNodes();
//         }
//     }
//     
//     private void DrawNodeEditor()
//     {
//         foreach (var pair in nodeIdToIndexMap)
//         {
//             DialogueGraphWindowNode node = dialogueGraphWindowNodes[pair.Value];
//             node.Position = GUI.Window(pair.Value, node.Position, DrawNodeWindow, "");
//         }
//     }
//
//
//
//     private void HandlePropertiesEditor()
//     {
//         if (selectedNode != null)
//         {
//             EditorGUILayout.LabelField("Selected Node Properties", EditorStyles.boldLabel);
//             EditorGUILayout.ObjectField("Node Data", selectedNode.NodeData, typeof(DialogueNode), allowSceneObjects: false);
//             //... any other properties to edit
//         }
//     }
//
//
//     // Method to add node to the current Dialogue Tree
//     private void AddNodeToCurrentDialogueTree()
//     {
//         Undo.RecordObject(CurrentDialogueTree, "Added Node");
//
//         DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
//         newNode.id = System.Guid.NewGuid().ToString();
//
//         AssetDatabase.AddObjectToAsset(newNode, CurrentDialogueTree);
//         CurrentDialogueTree.nodes.Add(newNode);
//
//         DialogueGraphWindowNode newGraphNode = new DialogueGraphWindowNode();
//         newGraphNode.NodeData = newNode;
//
//         Vector2 nodePosition;
//
//         if (dialogueGraphWindowNodes.Count > 0)
//         {
//             var lastNodePosition = dialogueGraphWindowNodes.Last().Position.position;
//             // Set position of new node to be a short distance away from the last node
//             nodePosition = new Vector2(lastNodePosition.x + _shortDistance, lastNodePosition.y + _shortDistance);
//         }
//         else
//         {
//             nodePosition = new Vector2(position.width / 2, position.height / 2);
//         }
//
//         newGraphNode.Position = new Rect(nodePosition.x, nodePosition.y, 200, 100);
//
//         Debug.Log($"Added New Node with Position: {newGraphNode.Position.position}");
//
//         dialogueGraphWindowNodes.Add(newGraphNode);
//
//         if (dialogueGraphWindowNodes.Count == 1)
//         {
//             _entryPointRect.position = newGraphNode.Position.position;
//         }
//
//         AssetDatabase.SaveAssets();
//     }
//
//
//
//     private void DrawEntryPointWindow(int id)
//     {
//         GUI.DragWindow();
//     }
//
//     private void SaveNodePositions()
//     {
//         foreach (DialogueGraphWindowNode node in dialogueGraphWindowNodes)
//         {
//             node.NodeData.position = node.Position.position;
//         }
//     }
//     
//     private void SortNodes()
//     {
//         dialogueGraphWindowNodes.Sort((a, b) => a.NodeData.id.CompareTo(b.NodeData.id));
//     }
//     
//     private DialogueGraphWindowNode GetNodeAtPosition(Vector2 position)
//     {
//         for (int i = 0; i < dialogueGraphWindowNodes.Count; i++)
//         {
//             if (dialogueGraphWindowNodes[i].Position.Contains(position))
//                 return dialogueGraphWindowNodes[i];
//         }
//         return null;
//     }
//
//     private void HandleMouseEvents()
//     {
//         Event e = Event.current;
//
//         if (e.button != 0) return;
//
//         // Record the state of the objects before any changes are made, to support Undo operations
//         if (e.type == EventType.MouseDown)
//         {
//             Undo.RecordObject(this, "Drag");
//
//             startDragPosition = e.mousePosition;
//
//             if (_entryPointRect.Contains(startDragPosition))
//             {
//                 isDraggingEntryPoint = true;
//                 GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
//                 e.Use();
//             }
//             else
//             {
//                 selectedNode = GetNodeAtPosition(startDragPosition);
//                 if (selectedNode != null)
//                 {
//                     isDragging = true;
//                     GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
//                     e.Use();
//                 }
//             }
//         }
//         else if (e.type == EventType.MouseDrag)
//         {
//             Vector2 delta = e.mousePosition - startDragPosition;
//
//             if (isDraggingEntryPoint)
//             {
//                 _entryPointRect.position += delta;
//                 GUI.changed = true;
//                 e.Use();
//             }
//             else if (isDragging && selectedNode != null)
//             {
//                 selectedNode.Position.position += delta;
//                 GUI.changed = true;
//                 e.Use();
//             }
//
//             startDragPosition = e.mousePosition;
//         }
//         else if (e.type == EventType.MouseUp)
//         {
//             isDragging = false;
//             isDraggingEntryPoint = false;
//             GUIUtility.hotControl = 0;
//             e.Use();
//         }
//     }
//
//
//     private void DrawNodeWindow(int index)
//     {
//         if (index < 0 || index >= dialogueGraphWindowNodes.Count)
//         {
//             return;
//         }
//
//         DialogueGraphWindowNode node = dialogueGraphWindowNodes[index];
//
//         if (node != null)
//         {
//             Debug.Log($"Drawing Node {index} at Position: {node.Position.position}");
//
//             if (Event.current.type == EventType.MouseDown && node.Position.Contains(Event.current.mousePosition))
//             {
//                 selectedNode = node;
//                 Repaint();  // Force a repaint
//             }
//
//             GUILayout.Label("Speaker: " + (node.NodeData.speaker ?? "undefined"));
//             GUILayout.Label("Dialogue: " + (node.NodeData.dialogueLine ?? "undefined"));
//             GUILayout.Label("Options: " + node.NodeData.options.Count);
//         
//             // Reset color
//             GUI.color = Color.white;
//         }
//     }
//
//
//
//     public static void DeleteDialogueTree(DialogueTree dialogueTree)
//     {
//         string path = AssetDatabase.GetAssetPath(dialogueTree);
//         AssetDatabase.DeleteAsset(path);
//     }
//     
//     private void AutoLayoutNodes()
//     {
//         // Calculate node positions
//         var visitedNodes = new HashSet<DialogueNode>();
//         int level = 0;
//         int order = 0;
//         foreach (DialogueNode node in _currentDialogueTree.nodes)
//         {
//             if (!visitedNodes.Contains(node))
//             {
//                 AutoLayoutDFS(node, visitedNodes, level, order);
//                 order++;
//             }
//         }
//     }
//
//     private void AutoLayoutDFS(DialogueNode node, HashSet<DialogueNode> visitedNodes, int level, int order)
//     {
//         if (visitedNodes.Contains(node)) return;
//
//         visitedNodes.Add(node);
//
//         // Set node position
//         var windowNode = dialogueGraphWindowNodes.Find(n => n.NodeData == node);
//         if (windowNode != null)
//         {
//             windowNode.Position = new Rect(order * 210, level * 120, 200, 100);
//         }
//
//         foreach (DialogueOption option in node.options)
//         {
//             // Perform DFS on the target node of the option
//             AutoLayoutDFS(option.TargetNode, visitedNodes, level + 1, order);
//         }
//     }
//
// }
