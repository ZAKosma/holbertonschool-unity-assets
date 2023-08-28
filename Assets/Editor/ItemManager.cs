// using UnityEngine;
// using UnityEditor;
//
// public class ItemManager : EditorWindow
// {
//     // Selected item
//     private BaseItem selectedItem;
//
//     // Scroll position
//     private Vector2 scrollPosition;
//
//     [MenuItem("Window/Item Manager")]
//     public static void ShowWindow()
//     {
//         GetWindow<ItemManager>("Item Manager");
//     }
//
//     private void OnGUI()
//     {
//         EditorGUILayout.BeginVertical();
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
//
//         // List existing items
//         GUILayout.Label("Items:", EditorStyles.boldLabel);
//         string[] guids = AssetDatabase.FindAssets("t:BaseItem");
//         foreach (string guid in guids)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(guid);
//             BaseItem item = AssetDatabase.LoadAssetAtPath<BaseItem>(path);
//             if (GUILayout.Button(item.itemName))
//             {
//                 selectedItem = item;
//                 Selection.activeObject = item;
//             }
//         }
//
//         // Selected item details
//         if (selectedItem != null)
//         {
//             EditorGUILayout.Space();
//             GUILayout.Label("Selected Item Details:", EditorStyles.boldLabel);
//             EditorGUI.BeginChangeCheck();
//             selectedItem.itemName = EditorGUILayout.TextField("Name:", selectedItem.itemName);
//             selectedItem.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", selectedItem.icon, typeof(Sprite), false);
//             selectedItem.description = EditorGUILayout.TextField("Description:", selectedItem.description);
//             selectedItem.baseValue = EditorGUILayout.FloatField("Base Value:", selectedItem.baseValue);
//             selectedItem.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", selectedItem.rarity);
//
//             if (EditorGUI.EndChangeCheck())
//             {
//                 EditorUtility.SetDirty(selectedItem);
//                 AssetDatabase.SaveAssets();
//             }
//
//             // Delete button
//             if (GUILayout.Button("Delete Item"))
//             {
//                 AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem));
//                 selectedItem = null;
//             }
//         }
//
//         EditorGUILayout.EndScrollView();
//         EditorGUILayout.EndVertical();
//     }
// }
