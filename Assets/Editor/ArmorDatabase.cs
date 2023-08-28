using UnityEngine;
using UnityEditor;

public class ArmorDatabase : EditorWindow
{
    private Vector2 scrollPosition;
    private Armor selectedItem;

    [MenuItem("Window/Item Manager/Armor Database")]
    public static void ShowWindow()
    {
        GetWindow<ArmorDatabase>("Armor Database");
    }

    private void OnGUI()
    {
        // Creation Section
        GUILayout.Label("Create New Armor:", EditorStyles.boldLabel);
        if (GUILayout.Button("Open Armor Creation Window"))
        {
            ArmorCreation.ShowWindow();
        }

        EditorGUILayout.Space();

        // Editing Section
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        string[] guids = AssetDatabase.FindAssets("t:Armor");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Armor armor = AssetDatabase.LoadAssetAtPath<Armor>(path);
            if (GUILayout.Button(armor.itemName))
            {
                selectedItem = armor;
                Selection.activeObject = armor;
            }
        }
        EditorGUILayout.EndScrollView();

        // Selected Item Details
        if (selectedItem != null)
        {
            DrawSelectedArmorProperties(selectedItem);
        }
    }

    private void DrawSelectedArmorProperties(Armor armor)
    {
        EditorGUI.BeginChangeCheck();
        armor.itemName = EditorGUILayout.TextField("Name:", armor.itemName);
        armor.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", armor.icon, typeof(Sprite), false);
        armor.description = EditorGUILayout.TextField("Description:", armor.description);
        armor.baseValue = EditorGUILayout.FloatField("Base Value:", armor.baseValue);
        armor.rarity = (Rarity)EditorGUILayout.EnumPopup("Rarity:", armor.rarity);
        armor.requiredLevel = EditorGUILayout.IntField("Required Level:", armor.requiredLevel);
        armor.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type:", armor.armorType);
        armor.defensePower = EditorGUILayout.FloatField("Defense Power:", armor.defensePower);
        armor.resistance = EditorGUILayout.FloatField("Resistance:", armor.resistance);
        armor.weight = EditorGUILayout.FloatField("Weight:", armor.weight);
        armor.movementSpeedModifier = EditorGUILayout.FloatField("Movement Speed Modifier:", armor.movementSpeedModifier);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(armor);
            AssetDatabase.SaveAssets();
        }
    }
}
