// using UnityEditor;
// using UnityEngine;
//
// public class StorySequenceEditor : EditorWindow
// {
//     private StorySequence storySequence;
//     private Vector2 scrollPosition;
//     
//     [MenuItem("Window/Twine/Story Sequence Editor")]
//     public static void ShowWindow()
//     {
//         GetWindow<StorySequenceEditor>("Story Sequence Editor");
//     }
//     
//     private void OnGUI()
//     {
//         if (storySequence == null)
//         {
//             EditorGUILayout.HelpBox("No StorySequence selected!", MessageType.Warning);
//         }
//         else
//         {
//             SerializedObject serializedObject = new SerializedObject(storySequence);
//             SerializedProperty property = serializedObject.FindProperty("storyModules");
//             
//             EditorGUILayout.BeginVertical();
//             scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
//             
//             EditorGUILayout.PropertyField(property, true);
//             
//             EditorGUILayout.EndScrollView();
//             EditorGUILayout.EndVertical();
//             
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
//     
//     public void SetStorySequence(StorySequence storySequence)
//     {
//         this.storySequence = storySequence;
//         Repaint();
//     }
// }