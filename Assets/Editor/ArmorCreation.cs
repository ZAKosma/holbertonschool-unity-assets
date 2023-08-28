using UnityEngine;
using UnityEditor;

public class ArmorCreation : BaseItemCreation
{
    // Armor specific properties
    private ArmorType armorType;
    private float defensePower;
    private float resistance; // Magical resistance
    private float weight; // Weight of the armor
    private float movementSpeedModifier; // Affects movement speed

    [MenuItem("Window/Item Manager/Armor Creation")]
    public static void ShowWindow()
    {
        GetWindow<ArmorCreation>("Armor Creation");
    }

    private void OnGUI()
    {
        DrawCommonFields();

        // Armor specific fields
        GUILayout.Label("Armor Properties:", EditorStyles.boldLabel);
        armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type:", armorType);
        defensePower = EditorGUILayout.FloatField("Defense Power:", defensePower);
        resistance = EditorGUILayout.FloatField("Resistance:", resistance);
        weight = EditorGUILayout.FloatField("Weight:", weight);
        movementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier:", movementSpeedModifier);

        if (GUILayout.Button("Create Armor")) CreateArmor();
    }

    private void CreateArmor()
    {
        // Similar to the weapon creation
        // Gather the common fields and check for missing ones

        // If confirmed
        Armor newItem = CreateInstance<Armor>();
        newItem.itemName = itemName;
        newItem.icon = icon;
        newItem.description = description;
        newItem.baseValue = baseValue;
        newItem.rarity = rarity;
        newItem.requiredLevel = requiredLevel;
        newItem.armorType = armorType;
        newItem.defensePower = defensePower;
        newItem.resistance = resistance;
        newItem.weight = weight;
        newItem.movementSpeedModifier = movementSpeedModifier;

        string folderPath = "Assets/Items/Armors/";

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
