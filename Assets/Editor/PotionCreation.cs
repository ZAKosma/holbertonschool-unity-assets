using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PotionCreation : BaseItemCreation
{
    // Potion specific properties
    private PotionEffect potionEffect;
    private float effectPower;
    private float duration;
    private float cooldown;
    private bool isStackable;

    [MenuItem("Window/Item Manager/Potion Creation")]
    public static void ShowWindow()
    {
        GetWindow<PotionCreation>("Potion Creation");
    }

    private void OnGUI()
    {
        DrawCommonFields();

        // Potion specific fields
        GUILayout.Label("Potion Properties:", EditorStyles.boldLabel);
        potionEffect = (PotionEffect)EditorGUILayout.EnumPopup("Potion Effect:", potionEffect);
        effectPower = EditorGUILayout.FloatField("Effect Power:", effectPower);
        duration = EditorGUILayout.FloatField("Duration:", duration);
        cooldown = EditorGUILayout.FloatField("Cooldown:", cooldown);
        isStackable = EditorGUILayout.Toggle("Is Stackable:", isStackable);

        if (GUILayout.Button("Create Potion")) CreatePotion();
    }

    private void CreatePotion()
    {
        // Gather missing fields
        List<string> missingFields = new List<string>();
        if (string.IsNullOrEmpty(itemName)) missingFields.Add("Name");
        if (icon == null) missingFields.Add("Icon");
        if (string.IsNullOrEmpty(description)) missingFields.Add("Description");
        // Add checks for other fields if required

        string missingFieldsMessage = missingFields.Count > 0 ? 
            "\nMissing Properties: " + string.Join(", ", missingFields) + "\nAre you sure you want to proceed?" 
            : "";

        // Confirmation dialog
        if (EditorUtility.DisplayDialog(
            "Confirm Potion Creation",
            "Are you sure you want to create this potion with the following details?\n" +
            "Name: " + itemName + "\n" +
            "Description: " + description + "\n" +
            // Add other properties as desired
            missingFieldsMessage,
            "Yes", "No"))
        {
            Potion newItem = CreateInstance<Potion>();
            newItem.itemName = itemName;
            newItem.icon = icon; // Set icon
            newItem.description = description; // Set description
            newItem.baseValue = baseValue;
            newItem.rarity = rarity;
            newItem.requiredLevel = requiredLevel; // Set required level
            newItem.potionEffect = potionEffect;
            newItem.effectPower = effectPower;
            newItem.duration = duration;
            newItem.cooldown = cooldown;
            newItem.isStackable = isStackable;

            string folderPath = "Assets/Items/Potions/";

            // Create the directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + folderPath.Substring("Assets".Length));
                AssetDatabase.Refresh();
            }

            string fullPath = folderPath + itemName + ".asset";
            AssetDatabase.CreateAsset(newItem, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
