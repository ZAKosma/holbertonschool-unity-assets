// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// public abstract class BaseItemCreation : EditorWindow
// {
//     protected string itemName;
//     protected Sprite icon; // Added icon field
//     protected string description; // Added description field
//     protected float baseValue;
//     protected int requiredLevel; // Added required level field
//     protected Rarity rarity;
//
//     protected void DrawCommonFields()
//     {
//         itemName = EditorGUILayout.TextField("Name:", itemName);
//         icon = (Sprite)EditorGUILayout.ObjectField("Icon:", icon, typeof(Sprite), false); // Added icon field
//         description = EditorGUILayout.TextField("Description:", description); // Added description field
//         baseValue = EditorGUILayout.FloatField("Base Value:", baseValue);
//         requiredLevel = EditorGUILayout.IntField("Required Level:", requiredLevel); // Added required level field
//         rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", rarity);
//     }
//
//     protected void CreateItem<T>() where T : BaseItem
//     {
//         T newItem = CreateInstance<T>();
//
//         // Determine the folder path based on the type of item
//         string folderPath = "Assets/Items/";
//         if (typeof(T) == typeof(Weapon))
//         {
//             folderPath += "Weapons/";
//         }
//         // Add more conditions for other types
//
//         // Create the directory if it doesn't exist
//         if (!AssetDatabase.IsValidFolder(folderPath))
//         {
//             System.IO.Directory.CreateDirectory(Application.dataPath + folderPath.Substring("Assets".Length));
//             AssetDatabase.Refresh();
//         }
//
//         // Define the full path for the asset
//         string fullPath = folderPath + itemName + ".asset";
//
//         AssetDatabase.CreateAsset(newItem, fullPath);
//         newItem.itemName = itemName;
//         newItem.baseValue = baseValue;
//         newItem.rarity = rarity;
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
//
// }
